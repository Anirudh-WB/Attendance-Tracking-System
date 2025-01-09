using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATS.DTO
{
    public class AttendanceLogDto
    {
    }

    public record CreateAttendanceLogDto(
        [Required(ErrorMessage = "User Id is required")] long UserId,
        DateTime AttendanceLogTime,
        [Required(ErrorMessage = "Please Specify Check Type of Attendance Log")] 
        string CheckType
        );

    public record UpdateAttendanceLogDto(
        long UserId,
        DateTime AttendanceLogTime,
        string CheckType
        );

    public record GetAttendanceLogDto(
        long Id,
        long UserId,
        DateTime AttendanceLogTime,
        string CheckType
        );

    public record GetAttendanceLogsWithDetailsDto(
        long Id,
        long UserId,
        string Email,
        string ProfilePic,
        string FirstName,
        string LastName,
        DateTime AttendanceLogTime,
        string CheckType
        );

    public record GetAttendanceLogSummaryDto(
        int Total, 
        int Present, 
        int Wfh, 
        int Absent
        );

    public record GetAttendanceReportDto(
        string FirstName, 
        string LastName, 
        string Status, 
        DateTime InTime
        );

    public record GetInActivityRecordDto(
        TimeSpan InTime,
        TimeSpan? OutTime,
        TimeSpan InHours
    );

    public record GetOutActivityRecordDto(
        TimeSpan OutTime,
        TimeSpan? InTime,
        TimeSpan OutHours
    );

    public record GetTotalHoursDto(
        long UserId,
        string ProfilePic,
        string FirstName,
        string LastName,
        DateOnly CurrentDate,
        TimeSpan LastCheckInTime,
        TimeSpan? LastCheckOutTime,
        TimeSpan TotalHours
    );

    public record GetSumTotalHoursDto(
        long UserId,
        string ProfilePic,
        string FirstName,
        string LastName,
        TimeSpan TotalHours
    );

    public record GetStatusOfAttendanceLogDto(
        long Id,
        long UserId,
        string ProfilePic,
        string FirstName,
        string LastName,
        string Status,
        TimeSpan? InTime
    );


    public record GetMisEntrySummaryDto(
        long UserId,
        string Email,
        string ProfilePic,
        string FirstName,
        string LastName,
        int TotalCount
    );


    public record GetAvgHoursDto(
      long UserId,
      TimeSpan AvgTotalHours,
      TimeSpan AvgTotalInHours,
      TimeSpan AvgTotalOutHours
    );
}
