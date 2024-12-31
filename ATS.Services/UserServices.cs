using ATS.Data;
using ATS.DTO;
using ATS.Hubs;
using ATS.IRepository;
using ATS.IServices;
using ATS.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ATS.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeDetailServices _employeeDetailServices;
        private readonly IEmployeeDetailRepository _employeeDetailRepository;
        private readonly IAccessPageRepository _accessPageRepository;
        private readonly IHubContext<AtsHubs> _hubContext;

        public UserServices(IUserRepository userRepository, IEmployeeDetailServices employeeDetailServices, IHubContext<AtsHubs> hubContext, IAccessPageRepository accessPageRepository, IEmployeeDetailRepository employeeDetailRepository)
        {
            _userRepository = userRepository;
            _employeeDetailServices = employeeDetailServices;
            _hubContext = hubContext;
            _accessPageRepository = accessPageRepository;
            _employeeDetailRepository = employeeDetailRepository;
        }

        public async Task<GetUserDto> CreateUserAsync(CreateUserDto UserDto)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(UserDto.Password);
                var users = await _userRepository.CreateAsync(new User
                {
                    Email = UserDto.Email,
                    Password = hashedPassword,
                    ContactNo = UserDto.ContactNo,
                    CreatedBy = UserDto.CreatedBy,
                    UpdatedBy = UserDto.CreatedBy,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

                var createdUser = new GetUserDto(
                    users.Id,
                    users.Email,
                    users.Password,
                    users.ContactNo,
                    users.IsActive,
                    users.RoleId
                );

                await _hubContext.Clients.All.SendAsync("ReceiveUserUpdate", users.Email, users.Password, users.ContactNo);

                return createdUser;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                {
                    throw new Exception("This User already exists.");
                }
                throw;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<bool> DeleteUserAsync(long id)
        {
            try
            {
                var gender = await _userRepository.GetAsync(id);

                if (gender == null)
                {
                    throw new Exception($"No User Found for id: {id}");
                }

                bool row = await _userRepository.DeleteAsync(gender);

                if (row)
                {
                    await _hubContext.Clients.All.SendAsync("DeleteUserUpdate", id);
                }

                return row;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetUserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                var usersDto = users.Select(users => new GetUserDto(
                    users.Id,
                    users.Email,
                    users.Password,
                    users.ContactNo,
                    users.IsActive,
                    users.RoleId
                ));

                return usersDto.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<GetUserDto> GetUserAsync(long id)
        {
            try
            {
                var user = await _userRepository.GetAsync(id);

                if (user == null)
                {
                    throw new Exception($"No User Found with id : {id}");
                }

                var userDto = new GetUserDto(
                    user.Id,
                    user.Email,
                    user.Password,
                    user.ContactNo,
                    user.IsActive,
                    user.RoleId
                );

                return userDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<GetAuthDto> SignUpUserAsync(SignUpDto signUpDto)
        {
            await _userRepository.BeginTransactionAsync();

            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(signUpDto.Password);
                var user = await _userRepository.CreateAsync(new User
                {
                    Email = signUpDto.Email,
                    Password = hashedPassword,
                    ContactNo = signUpDto.ContactNo,
                    RoleId = signUpDto.RoleId ?? 3,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

                var employeeInfo = await _employeeDetailServices.CreateEmployeeDetailAsync(
                    new CreateEmployeeDetailDto(
                        UserId: user.Id,
                        EmployeeCode: signUpDto?.EmployeeCode,
                        FirstName: signUpDto.FirstName,
                        LastName: signUpDto.LastName,
                        ProfilePic: signUpDto.ProfilePic,
                        CreatedBy: user.Id
                    )
                );

                await _userRepository.CommitTransactionAsync();

                var createdUser = new GetUserDto(
                    user.Id,
                    user.Email,
                    user.Password,
                    user.ContactNo,
                    user.IsActive,
                    user.RoleId
                );

                IEnumerable<GetAccessPageDto> accessPageDtos = new List<GetAccessPageDto>();

                var createEmployee = new GetAuthDto(
                    createdUser.Id,
                    employeeInfo.Id,
                    employeeInfo.ProfilePic,
                    employeeInfo.FirstName,
                    employeeInfo.LastName,
                    createdUser.Email,
                    createdUser.ContactNo,
                    createdUser.RoleId,
                    accessPageDtos
                );

                await _hubContext.Clients.All.SendAsync("ReceiveSignUpUpdate", createEmployee.FirstName, createEmployee.LastName, createEmployee.Email, createEmployee.ContactNo, createEmployee.ProfilePic);

                return createEmployee;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                {
                    throw new Exception("This User already exists.");
                }
                throw;
            }
            catch (Exception)
            {
                await _userRepository.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<GetAuthDto> LogInUserAsync(LogInDto logInDto)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync( logInDto.Email );

                if (user == null || !BCrypt.Net.BCrypt.Verify(logInDto.Password, user.Password))
                {
                    throw new Exception("Invalid Credentials");
                }

                var userDto = new GetUserDto(
                    user.Id,
                    user.Email,
                    user.Password,
                    user.ContactNo,
                    user.IsActive,
                    user.RoleId
                );

                var employee = await _employeeDetailRepository.GetEmployeeDetailByUserId(user.Id);

                var data = await _accessPageRepository.GetAccessByRoleId(userDto.RoleId);

                var accessPageDtos = data.Select(accessPage => new GetAccessPageDto(
                    accessPage.Id,
                    accessPage.RoleId,
                    accessPage.Role?.RoleName,
                    accessPage.PageId,
                    accessPage.Page?.PageTitle,
                    accessPage.IsActive
                )).Where(ac => ac.IsActive==true);

                var res = new GetAuthDto(
                  userDto.Id,
                  employee.First().Id,
                  employee.First().ProfilePic,
                  employee.First().FirstName,
                  employee.First().LastName,
                  userDto.Email, 
                  userDto.ContactNo,
                  userDto.RoleId,
                  accessPageDtos
                );
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<GetUserDto> UpdateUserAsync(long id, UpdateUserDto UserDto)
        {
            try
            {
                var oldUser = await _userRepository.GetAsync(id);

                if (oldUser == null)
                {
                    throw new Exception($"No User Found for id : {id}");
                }

                if(!BCrypt.Net.BCrypt.Verify(UserDto.OldPassword, oldUser.Password))
                {
                    throw new Exception("Bad Credentials");
                }

                if (!string.IsNullOrEmpty(UserDto.NewPassword))
                {
                    oldUser.Password = BCrypt.Net.BCrypt.HashPassword(UserDto.NewPassword);
                }

                oldUser.Email = UserDto.Email;
                //oldUser.Password = UserDto.Password;
                oldUser.ContactNo = string.IsNullOrEmpty(UserDto.ContactNo) ? oldUser.ContactNo : UserDto.ContactNo;
                oldUser.IsActive = UserDto.IsActive ?? true;
                oldUser.UpdatedBy = UserDto.UpdatedBy ?? 0;
                oldUser.UpdatedAt = DateTime.Now;

                var user = await _userRepository.UpdateAsync(oldUser);

                var newUserDto = new GetUserDto(
                    user.Id,
                    user.Email,
                    user.Password,
                    user.ContactNo,
                    user.IsActive,
                    user.RoleId
                );

                return newUserDto;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true ||
                    ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                {
                    throw new Exception("This User already exists.");
                }
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
