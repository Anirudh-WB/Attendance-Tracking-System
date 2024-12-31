using ATS.Data;
using ATS.IRepository;
using ATS.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ATS.Repository
{
    public class AttendanceLogRepository : Repository<AttendanceLog>, IAttendanceLogRepository
    {
        private readonly ATSDbContext _dbContext;
        public AttendanceLogRepository(ATSDbContext dbcontext) : base(dbcontext)
        {
            _dbContext = dbcontext;
        }
        public async Task<IEnumerable<GetStatusOfAttendanceLog>> GetPacificStatusOfAttendanceLog(string firstName)
        {
            try
            {
                var firstNameParameter = new SqlParameter("@UserIds", firstName);
                var data = await _dbContext.Set<GetStatusOfAttendanceLog>()
                    .FromSqlRaw("EXECUTE dbo.GetSpecificStatus @UserIds", firstNameParameter)
                    .ToListAsync();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<AttendanceLog>> GetAttendanceLogByUserId(long userId, DateTime date)
        {
            try
            {
                var attendanceLogs = await _dbContext.attendanceLogs
                    .Include(li => li.User)
                    .ThenInclude(li => li.EmployeeDetail)
                    .Where(li => li.UserId == userId)
                    .Where(li => li.AttendanceLogTime.Date == date)
                    .ToListAsync();

                return attendanceLogs;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AttendanceLog>> GetAllAttendanceLogs(int count, DateTime startDate)
        {
            try
            {
                var attendanceLogsQuery = _dbContext.attendanceLogs
                    .Include(log => log.User)
                    .ThenInclude(user => user.EmployeeDetail)
                    .Where(log => log.AttendanceLogTime.Date == startDate);

                var attendanceLogs = await attendanceLogsQuery.OrderByDescending(li => li.AttendanceLogTime).Take(count).ToListAsync();

                return attendanceLogs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<AttendanceLog>> GetAttendanceReport(DateTime date)
        {
            try
            {
                var res = await _dbContext.attendanceLogs
                    .Where(log => log.AttendanceLogTime.Date >= date)
                    .GroupBy(log => log.UserId)
                    .Select(group => group.OrderBy(log => log.AttendanceLogTime).FirstOrDefault())
                    .ToListAsync();

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AttendanceLog>> GetSummaryReport(DateTime startDate, DateTime endDate, string check)
        {
            try
            {
                var res = await _dbContext.attendanceLogs
                    .Where(log => log.AttendanceLogTime.Date >= startDate && log.AttendanceLogTime.Date <= endDate && log.CheckType == check)
                    .GroupBy(log => log.UserId)
                    .Select(group => group.OrderBy(log => log.AttendanceLogTime).FirstOrDefault())
                    .ToListAsync();

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<AttendanceLog>> GetActivityReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                var res = await _dbContext.attendanceLogs
                    .Where(log => log.AttendanceLogTime.Date >= startDate && log.AttendanceLogTime.Date <= endDate)
                    .OrderBy(log => log.AttendanceLogTime)
                    .ToListAsync();

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<GetTotalInHours>> GetTotalInHours(long? userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var startDateParameter = new SqlParameter("@StartDate", startDate);
                var endDateParameter = new SqlParameter("@EndDate", endDate);

                var userIdParameter = new SqlParameter("@UserId", SqlDbType.BigInt)
                {
                    Value = userId.HasValue ? (object)userId.Value : DBNull.Value
                };

                var results = await _dbContext.Set<GetTotalInHours>()
                            .FromSqlRaw("EXECUTE dbo.GetAttendanceInTimeDifferences @userId, @startDate, @endDate", userIdParameter, startDateParameter, endDateParameter)
                            .ToListAsync();
                return results;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<GetSumTotalHours>> GetSumTotalInHours(long? userId, DateTime startDate, DateTime endDate, string report)
        {
            try
            {
                var startDateParameter = new SqlParameter("@StartDate", startDate);
                var endDateParameter = new SqlParameter("@EndDate", endDate);
                var reportParameter = new SqlParameter("@PeriodType", report);

                var userIdParameter = new SqlParameter("@UserId", SqlDbType.BigInt)
                {
                    Value = userId.HasValue ? (object)userId.Value : DBNull.Value
                };

                var results = await _dbContext.Set<GetSumTotalHours>()
                            .FromSqlRaw("EXECUTE [dbo].[GetSumOfInTimeDifferences] @userId, @startDate, @endDate, @PeriodType", userIdParameter, startDateParameter, endDateParameter, reportParameter)
                            .ToListAsync();

                return results.OrderByDescending(li => li.TotalHours);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<GetTotalOutHours>> GetTotalOutHours(long? userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var startDateParameter = new SqlParameter("@StartDate", startDate);
                var endDateParameter = new SqlParameter("@EndDate", endDate);

                var userIdParameter = new SqlParameter("@UserId", SqlDbType.BigInt)
                {
                    Value = userId.HasValue ? (object)userId.Value : DBNull.Value
                };

                var results = await _dbContext.Set<GetTotalOutHours>()
                    .FromSqlRaw("EXECUTE dbo.GetAttendanceOutTimeDifferences @UserId, @StartDate, @EndDate", userIdParameter, startDateParameter, endDateParameter)
                    .ToListAsync();

                return results;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<GetSumTotalHours>> GetSumTotalOutHours(long? userId, DateTime startDate, DateTime endDate, string report)
        {
            try
            {
                var startDateParameter = new SqlParameter("@StartDate", startDate);
                var endDateParameter = new SqlParameter("@EndDate", endDate);
                var reportParameter = new SqlParameter("@PeriodType", report);

                var userIdParameter = new SqlParameter("@UserId", SqlDbType.BigInt)
                {
                    Value = userId.HasValue ? (object)userId.Value : DBNull.Value
                };

                var results = await _dbContext.Set<GetSumTotalHours>()
                    .FromSqlRaw("EXECUTE [dbo].[GetSumOfOutTimeDifferences] @UserId, @StartDate, @EndDate, @PeriodType", userIdParameter, startDateParameter, endDateParameter, reportParameter)
                    .ToListAsync();


                return results.OrderByDescending(li => li.TotalHours);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<AttendanceLogWithDetails>> GetCurrentStatusOfAttendanceLog(string type, DateTime date, int count)
        {
            try
            {
                var typeParameter = new SqlParameter("@type", type);
                var dateParameter = new SqlParameter("@date", date);

                var resultsQuery = await _dbContext.Set<AttendanceLogWithDetails>()
                    .FromSqlRaw("EXECUTE [dbo].[GetLastEntryOfAllUsers] @type, @date", typeParameter, dateParameter).ToListAsync();

                var results = resultsQuery.OrderByDescending(li => li.AttendanceLogTime).Take(count).ToList();

                return results;
            }
            catch (Exception)
            {

                throw;
            }
        }       
        public async Task<IEnumerable<GetStatusOfAttendanceLog>> GetAllStatusOfAttendanceLog(DateTime Date)
        {
            try
            {
                //var dateParameter = new SqlParameter("@Date", Date);
                var data = await _dbContext.Set<GetStatusOfAttendanceLog>()
                               .FromSqlRaw("EXECUTE dbo.GetEmployeeAttendanceSummary")
                               .ToListAsync();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<GetTotalHours>> GetTotalHoursAsync(long? userId, DateTime start,DateTime end, string report)
        {
          
            var startDateParameter = new SqlParameter("@StartDate", start);
            var endDateParameter = new SqlParameter("@EndDate", end);
            var periodTypeParameter = new SqlParameter("@PeriodType", report);

            var userIdParameter = new SqlParameter("@UserId", SqlDbType.BigInt)
            {
                Value = userId.HasValue ? (object)userId.Value : DBNull.Value
            };

            // Execute stored procedure and map results to the DTO
            var results = _dbContext.Set<ATS.Model.GetTotalHours>()
                .FromSqlRaw("EXECUTE dbo.GetTotalHour_Employee_Report @UserId, @StartDate, @EndDate", userIdParameter, startDateParameter, endDateParameter)
                .ToList();

            return results;
        }
        public async Task<IEnumerable<AttendanceLogWithDetails>> GetMisEntry(long userId, DateTime date)
        {
            try
            {
                var userIdParameter = new SqlParameter("@userId", userId); 
                var DateParameter = new SqlParameter("@date", date);

                var data = await _dbContext.Set<AttendanceLogWithDetails>()
                    .FromSqlRaw("EXECUTE dbo.GetMisEntryReport @userId, @date", userIdParameter, DateParameter)
                    .ToListAsync();

                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<MisEntrySummary>> GetMisEntrySummary(long? userId, DateTime date)
        {
            try
            {
                var userIdParameter = new SqlParameter("@UserId", SqlDbType.BigInt)
                {
                    Value = userId.HasValue ? (object)userId.Value : DBNull.Value
                };
                var DateParameter = new SqlParameter("@date", date);

                var data = await _dbContext.Set<MisEntrySummary>()
                    .FromSqlRaw("EXECUTE dbo.GetMisEntrySummary @userId, @date", userIdParameter, DateParameter)
                    .ToListAsync();

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<GetAverageHours>> GetAverageHoursReport(long? userId, DateTime date)
        {
            try
            {
                var userIdParameter = new SqlParameter("@UserId", SqlDbType.BigInt)
                {
                    Value = userId.HasValue ? (object)userId.Value : DBNull.Value
                };

                var DateParameter = new SqlParameter("@date", date);

                var data = await _dbContext.Set<GetAverageHours>()
                        .FromSqlRaw("EXECUTE dbo.GetAverageReport @userId @date", userIdParameter,DateParameter)
                        .ToListAsync();

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
