using ATS.Data;
using ATS.DTO;
using ATS.Hubs;
using ATS.IRepository;
using ATS.IServices;
using ATS.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ATS.Services
{
    public class AttendanceLogServices : IAttendanceLogServices
    {
        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeDetailRepository _employeeDetailRepository;
        private readonly IHubContext<AtsHubs> _hubContext;

        public AttendanceLogServices(IAttendanceLogRepository attendanceLogRepository, IUserRepository userRepository, IEmployeeDetailRepository employeeDetailRepository, IHubContext<AtsHubs> hubContext)
        {
            _attendanceLogRepository = attendanceLogRepository;
            _userRepository = userRepository;
            _employeeDetailRepository = employeeDetailRepository;
            _hubContext = hubContext;
        }

        TimeSpan Correction(string str)
        {
            string[] values = str.Split(":");

            int Day = Convert.ToInt32(values[0]) / 24;

            return TimeSpan.Parse($"{Day}:{Convert.ToInt32(values[0]) - (Day * 24)}:{Convert.ToInt32(values[1])}:{Convert.ToInt32(values[2])}");
        }

        public string DateException(DateTime startDate, DateTime endDate) { 
            if(startDate > endDate)
            {
                throw new Exception("Start Date is should be less than End Date");
            }

            return string.Empty;
        }
        public async Task<GetAttendanceLogDto> CreateAttendanceLogAsync(CreateAttendanceLogDto attedanceLogDto)
        {
            try
            {
                var user = await _userRepository.GetAllAsync();
                if (user == null) 
                {
                    throw new Exception("No User Found!");
                }
                var attendanceLog = await _attendanceLogRepository.CreateAsync(new AttendanceLog()
                {
                    UserId = attedanceLogDto.UserId,
                    AttendanceLogTime = attedanceLogDto.AttendanceLogTime,
                    CheckType = attedanceLogDto.CheckType,
                });

                var res = new GetAttendanceLogDto(
                    attendanceLog.Id,
                    attendanceLog.UserId,            
                    attendanceLog.AttendanceLogTime,
                    attendanceLog.CheckType
                );

                await _hubContext.Clients.All.SendAsync("ReceiveItemUpdate", attendanceLog.UserId, attendanceLog.AttendanceLogTime, attendanceLog.CheckType);

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> CreateMultipleAttendanceLogsAsync(IEnumerable<CreateAttendanceLogDto> attendanceLogsDto)
        {
            try
            {
                var attendanceLogs = attendanceLogsDto.Select(dto => new AttendanceLog()
                {
                    UserId = dto.UserId,
                    AttendanceLogTime = dto.AttendanceLogTime,
                    CheckType = dto.CheckType
                }).ToList();

                var res = await _attendanceLogRepository.CreateMultipleAsync(attendanceLogs);

                var resDto = res.Select(log => {
                    var employee = _employeeDetailRepository.GetEmployeeDetailByUserId(log.UserId);
                    var user = _userRepository.GetAsync(log.UserId);

                    return new GetAttendanceLogsWithDetailsDto(
                        log.Id,
                        log.UserId,
                        employee.Result.First().User.Email,
                        employee.Result.First().ProfilePic,
                        employee.Result.First().FirstName,
                        employee.Result.First().LastName,
                        log.AttendanceLogTime,
                        log.CheckType
                    );
                }).ToList();

                await _hubContext.Clients.All.SendAsync("ReceiveItemUpdate", resDto);

                return resDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeleteAttendanceLogAsync(long id)
        {
            try
            {
                var attendanceLog = await _attendanceLogRepository.GetAsync(id);

                if (attendanceLog == null)
                {
                    throw new Exception($"No Log Found for id: {id}");
                }

                bool row = await _attendanceLogRepository.DeleteAsync(attendanceLog);

                return row;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetInActivityRecordDto>> GetInActivityRecord(long? userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var start = startDate == DateTime.MinValue || startDate == null ? DateTime.Now.Date : (DateTime)startDate;
                var end = endDate == DateTime.MinValue || endDate == null ? DateTime.Now.Date.AddDays(1) : (DateTime)endDate;

                DateException(start, end);

                var results = await _attendanceLogRepository.GetTotalInHours(userId, start, end);

                var dtoList = results.Select(li => new GetInActivityRecordDto(
                    Correction(li.InTime.ToString("HH:mm:ss")),
                    li.OutTime != null ? Correction(li.OutTime?.ToString("HH:mm:ss")) : null,
                    TimeSpan.Parse(li.TotalInHours)
                ));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetSumTotalHoursDto>> GetTotalInActivity(long? userId, DateTime? startDate, DateTime? endDate, string? reportType)
        {
            try
            {
                var start = startDate == DateTime.MinValue || startDate == null ? DateTime.Now.Date : (DateTime)startDate;
                var end = endDate == DateTime.MinValue || endDate == null ? DateTime.Now.Date.AddDays(1) : (DateTime)endDate;

                DateException(start, end);

                var report = string.IsNullOrEmpty(reportType) ? "Daily" : reportType;

                var results = await _attendanceLogRepository.GetSumTotalInHours(userId, start, end, report);

                var dtoList = results.Select(li => new GetSumTotalHoursDto(
                    li.UserId,
                    li.ProfilePic,
                    li.FirstName,
                    li.LastName,
                    Correction(li.TotalHours)
                ));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetSumTotalHoursDto>> GetTotalOutActivity(long? userId, DateTime? startDate, DateTime? endDate, string? reportType)
        {
            try
            {
                var start = startDate == DateTime.MinValue || startDate == null ? DateTime.Now.Date : (DateTime)startDate;
                var end = endDate == DateTime.MinValue || endDate == null ? DateTime.Now.Date.AddDays(1) : (DateTime)endDate;
                var report = string.IsNullOrEmpty(reportType) ? "Daily" : reportType;

                
                var results = await _attendanceLogRepository.GetSumTotalOutHours(userId, start, end, report);

                var dtoList = results.Select(li => new GetSumTotalHoursDto(
                    li.UserId,
                    li.ProfilePic,
                    li.FirstName,
                    li.LastName,
                    Correction(li.TotalHours)
                ));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> GetAllAttendanceLogsAsync(int? count, DateTime? startDate)
        {
            try
            {
                var start = startDate == DateTime.MinValue || startDate == null ? DateTime.Now.Date : (DateTime)startDate;
                int cnt = count ?? 100;

                var attendanceLogs = await _attendanceLogRepository.GetAllAttendanceLogs(cnt, start);

                var attendanceLogsDto = attendanceLogs.Select(attendanceLog => new GetAttendanceLogsWithDetailsDto(
                    attendanceLog.Id,
                    attendanceLog.UserId,
                    attendanceLog.User.Email,
                    attendanceLog.User?.EmployeeDetail?.ProfilePic,
                    attendanceLog.User?.EmployeeDetail?.FirstName,
                    attendanceLog.User?.EmployeeDetail?.LastName,
                    attendanceLog.AttendanceLogTime,
                    attendanceLog.CheckType
                ));

                return attendanceLogsDto.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<GetAttendanceLogDto> GetAttendanceLogAsync(long id)
        {
            try
            {
                var attendanceLog = await _attendanceLogRepository.GetAsync(id);

                if (attendanceLog == null)
                {
                    throw new Exception($"No Attendance Log Found with id : {id}");
                }

                var attendanceLogDto = new GetAttendanceLogDto(
                    attendanceLog.Id,
                    attendanceLog.UserId,
                    attendanceLog.AttendanceLogTime,
                    attendanceLog.CheckType
                );

                return attendanceLogDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> GetAttendanceLogByUserId(long userId, DateTime? date)
        {
            try
            {
                DateTime datefilter = date == DateTime.MinValue || date == null ? DateTime.Now.Date : (DateTime)date;

                var attendanceLog = await _attendanceLogRepository.GetAttendanceLogByUserId(userId, datefilter);
                if (attendanceLog == null)
                {
                    throw new Exception($"No Attendance Log Found with UserId : {userId}");
                }

                var attendanceLogDtos = attendanceLog.Select(attendanceLog => new GetAttendanceLogsWithDetailsDto(
                    attendanceLog.Id,
                    attendanceLog.UserId,
                    attendanceLog.User.Email,
                    attendanceLog.User.EmployeeDetail.ProfilePic,
                    attendanceLog.User.EmployeeDetail.FirstName,
                    attendanceLog.User.EmployeeDetail.LastName,
                    attendanceLog.AttendanceLogTime,
                    attendanceLog.CheckType
                ));

                return attendanceLogDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<GetAttendanceLogSummaryDto> GetAttendanceLogSummary(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var start = startDate == null ? DateTime.Now.Date : (DateTime)startDate;
                var end = endDate == null ? DateTime.Now.Date : (DateTime)endDate;

                DateException(start, end);

                IEnumerable<User> totalData = await _userRepository.GetAllAsync();

                var total = totalData.Count();

                IEnumerable<AttendanceLog> presentData = await _attendanceLogRepository.GetSummaryReport(start, end, "IN");

                var present = presentData.Count();

                IEnumerable<AttendanceLog> wfhData = await _attendanceLogRepository.GetSummaryReport(start, end, "WFH");

                var wfh = wfhData.Count();

                var absent = total - present;

                GetAttendanceLogSummaryDto summaryDto = new(total, present, wfh, absent);

                return summaryDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<GetAttendanceLogDto> UpdateAttendanceLogAsync(long id, UpdateAttendanceLogDto attendanceLogDto)
        {
            try
            {
                var oldAttendanceLog = await _attendanceLogRepository.GetAsync(id);

                if (oldAttendanceLog == null)
                {
                    throw new Exception($"No AttendanceLog Found for id : {id}");
                }

                oldAttendanceLog.UserId = attendanceLogDto.UserId;
                oldAttendanceLog.AttendanceLogTime = attendanceLogDto.AttendanceLogTime;
                oldAttendanceLog.CheckType = attendanceLogDto.CheckType;

                var attendanceLog = await _attendanceLogRepository.UpdateAsync(oldAttendanceLog);

                var newAttendanceLogDto = new GetAttendanceLogDto(
                    attendanceLog.Id,
                    attendanceLog.UserId,
                    attendanceLog.AttendanceLogTime,
                    attendanceLog.CheckType
                );

      
                return newAttendanceLogDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetTotalHoursDto>> GetTotalHoursOfEmployee(long? userId, DateTime? startDate, DateTime? endDate, string? reportType)
        {
            DateTime start = startDate == DateTime.MinValue || startDate == null ? DateTime.Now.Date : (DateTime)startDate;
            DateTime end = endDate == DateTime.MinValue || endDate == null ? DateTime.Now.Date.AddDays(1) : (DateTime)endDate;

            DateException(start, end);

            var report = reportType ?? "ALL-TIME";

            var results = await _attendanceLogRepository.GetTotalHoursAsync(userId, start, end, report);

            // Map the results to the DTO
            var dtoList = results.Select(model => new GetTotalHoursDto(
                model.UserId,
                model.ProfilePic,
                model.FirstName,
                model.LastName,
                DateOnly.FromDateTime((DateTime)model.PeriodEnd),
                Correction(model.PeriodStart.ToString("HH:mm:ss")),
                model.PeriodEnd != null ?  Correction(model.PeriodEnd?.ToString("HH:mm:ss")) : null,
                Correction(model.TotalTimeSpanFormatted)
            )).OrderByDescending(li => li.TotalHours);

            return dtoList;
        }
        public async Task<IEnumerable<GetOutActivityRecordDto>> GetOutActivityRecord(long? userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var start = startDate == DateTime.MinValue || startDate == null ? DateTime.Now.Date : (DateTime)startDate;
                var end = endDate == DateTime.MinValue || endDate == null ? DateTime.Now.Date.AddDays(1) : (DateTime)endDate;

                DateException(start, end);

                var results = await _attendanceLogRepository.GetTotalOutHours(userId, start, end);

                var dtoList = results.Select(model => new GetOutActivityRecordDto(
                    Correction(model.InTime.ToString("HH:mm:ss")),
                    Correction(model.OutTime.ToString("HH:mm:ss")),
                    TimeSpan.Parse(model.TotalOutHours)
                ));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetStatusOfAttendanceLogDto>> GetStatusOfAttendanceLog(string? FirstName, DateTime? Date)
        {
            try
            
            {
                IEnumerable<GetStatusOfAttendanceLog> data;

                var Name = FirstName ?? string.Empty;
                DateTime date = Date == DateTime.MinValue || Date == null ? DateTime.Now.Date : (DateTime)Date;

                if (string.IsNullOrEmpty(Name))
                {
                    data = await _attendanceLogRepository.GetAllStatusOfAttendanceLog(date);
                }
                else
                {
                    data = await _attendanceLogRepository.GetPacificStatusOfAttendanceLog(FirstName);
                }

                var sortedData = data
                    .OrderByDescending(model => model.Status)
                    .ThenBy(model => model.Status == "Absent" ? model.FirstName : string.Empty)
                    .ThenByDescending(model => model.Status == "Present" ? model.InTime : (DateTime?)null);

                var dtoList = sortedData.Select(model => new GetStatusOfAttendanceLogDto(
                    model.Id,
                    model.UserId,
                    model.ProfilePic,
                    model.FirstName,
                    model.LastName,
                    model.Status,
                    (model.InTime != null ? Correction(model.InTime?.ToString("HH:mm:ss")) : null)
                ));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> GetCurrentStatusOfAttendanceLog(string type, DateTime? date, int? count)

        {
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    throw new Exception("type not specified");
                }

                DateTime date1 = date == DateTime.MinValue || date == null ? DateTime.Now.Date : (DateTime) date;

                int cnt = count ?? 100;

                var attendanceLogs = await _attendanceLogRepository.GetCurrentStatusOfAttendanceLog(type, date1, cnt);

                var attendanceLogsDto = attendanceLogs.Select(attendanceLog => new GetAttendanceLogsWithDetailsDto(
                    attendanceLog.Id,
                    attendanceLog.UserId,
                    attendanceLog.Email,
                    attendanceLog.ProfilePic,
                    attendanceLog.FirstName,
                    attendanceLog.LastName,
                    attendanceLog.AttendanceLogTime,
                    attendanceLog.CheckType
                ));

                return attendanceLogsDto.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> GetMisEntryOfUsers(long userId, DateTime? date)
        {
            try
            {
                var user = await _userRepository.GetAsync(userId);
                if (user == null)
                {
                    throw new Exception($"No Attendance Log Found with UserId : {userId}");
                }
                DateTime Cdate = date == DateTime.MinValue || date == null ? DateTime.Now.Date : (DateTime)date;
                var data = await _attendanceLogRepository.GetMisEntry(userId, Cdate);

                var attendanceLogsDto = data.Select(attendanceLog => new GetAttendanceLogsWithDetailsDto(
                        attendanceLog.Id,
                        attendanceLog.UserId,
                        attendanceLog.Email,
                        attendanceLog.ProfilePic,
                        attendanceLog.FirstName,
                        attendanceLog.LastName,
                        attendanceLog.AttendanceLogTime,
                        attendanceLog.CheckType
                    ));

                return attendanceLogsDto.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<GetMisEntrySummaryDto>> GetMisEntrySummary(long? userId, DateTime? date)
        {
            try
            {
                DateTime Cdate = date == DateTime.MinValue || date == null ? DateTime.Now.Date : (DateTime)date;

                var data = await _attendanceLogRepository.GetMisEntrySummary(userId, Cdate);

                var attendanceLogsDto = data.Select(attendanceLog => new GetMisEntrySummaryDto(
                        attendanceLog.UserId,
                        attendanceLog.Email,
                        attendanceLog.ProfilePic,
                        attendanceLog.FirstName,
                        attendanceLog.LastName,
                        attendanceLog.TotalCount
                    ));

                return attendanceLogsDto.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }   
        public async Task<IEnumerable<GetAvgHoursDto>> GetAvgTotalHoursOfEmployee(long? userId, DateTime? date)
        {
            try
            {
                DateTime Cdate = date == DateTime.MinValue || date == null ? DateTime.Now.Date : (DateTime)date;
                var data = await _attendanceLogRepository.GetAverageHoursReport(userId, Cdate);

                var res = data.Select(li => new GetAvgHoursDto(
                    li.UserId,
                    li.TotalAvg,
                    li.TotalInAvg,
                    li.TotalOutAvg
                ));

                return res.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
