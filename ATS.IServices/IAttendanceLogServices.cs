using ATS.DTO;

namespace ATS.IServices
{
    public interface IAttendanceLogServices
    {
        Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> GetAllAttendanceLogsAsync(int? count, DateTime? startDate);

        Task<GetAttendanceLogDto> GetAttendanceLogAsync(long id);

        Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> GetAttendanceLogByUserId(long userId, DateTime? date);

        Task<GetAttendanceLogSummaryDto> GetAttendanceLogSummary(DateTime? startDate, DateTime? endDate);

        Task<IEnumerable<GetInActivityRecordDto>> GetInActivityRecord(long? userId, DateTime? startDate, DateTime? endDate);

        Task<IEnumerable<GetSumTotalHoursDto>> GetTotalInActivity(long? userId, DateTime? startDate, DateTime? endDate, string? reportType);

        Task<IEnumerable<GetOutActivityRecordDto>> GetOutActivityRecord(long? userId, DateTime? startDate, DateTime? endDate);

        Task<IEnumerable<GetSumTotalHoursDto>> GetTotalOutActivity(long? userId, DateTime? startDate, DateTime? endDate, string? reportType);

        Task<IEnumerable<GetStatusOfAttendanceLogDto>> GetStatusOfAttendanceLog(string? FirstName, DateTime? Date);

        Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> GetCurrentStatusOfAttendanceLog(string type, DateTime? date, int? count);

        Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> GetMisEntryOfUsers(long userId, DateTime? date);

        Task<IEnumerable<GetMisEntrySummaryDto>> GetMisEntrySummary(long? userId, DateTime? date);

        Task<GetAttendanceLogDto> CreateAttendanceLogAsync(CreateAttendanceLogDto attendanceLogDto);

        Task<GetAttendanceLogDto> UpdateAttendanceLogAsync(long id, UpdateAttendanceLogDto attendanceLogDto);

        Task<bool> DeleteAttendanceLogAsync(long id);

        Task<IEnumerable<GetTotalHoursDto>> GetTotalHoursOfEmployee(long? userId, DateTime? startDate, DateTime? endDate, string? reportType);

        Task<IEnumerable<GetAttendanceLogsWithDetailsDto>> CreateMultipleAttendanceLogsAsync(IEnumerable<CreateAttendanceLogDto> attendanceLogsDto);

        Task<IEnumerable<GetAvgHoursDto>> GetAvgTotalHoursOfEmployee(long? userId, DateTime? date);
    }
}
