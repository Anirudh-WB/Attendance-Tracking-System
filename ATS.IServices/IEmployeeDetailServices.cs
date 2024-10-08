﻿using ATS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATS.IServices
{
    public interface IEmployeeDetailServices
    {
        Task<IEnumerable<GetEmployeeDetailDto>> GetEmployeeDetailsAsync();
        Task<GetSearchDto> GetEmployeeDetailsWithFilter(string? firstName, string? lastName, string? employeeCode, long? designationId, long? genderId, int? start, int? pageSize);
        Task<GetEmployeeDetailDto> GetEmployeeDetailAsync(long id);
        Task<GetEmployeeDetailDto> CreateEmployeeDetailAsync(CreateEmployeeDetailDto createEmployeeDetailDto);
        Task<GetEmployeeDetailDto> UpdateEmployeeDetailAsync(long id, UpdateEmployeeDetailDto updateEmployeeDetailDto);
        Task<bool> DeleteEmployeeDetailAsync(long id);
        Task<IEnumerable<GetEmployeeDetailDto>> GetEmployeeDetailByUserId(long userId);
        Task<IEnumerable<GetFaceEncodingDto>> GetFaceEncodingsAsync();

    }
}
