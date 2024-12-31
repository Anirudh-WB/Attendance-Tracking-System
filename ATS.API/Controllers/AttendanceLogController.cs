using ATS.DTO;
using ATS.IServices;
using ATS.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ATS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceLogController : ControllerBase
    {
        private readonly IAttendanceLogServices _attendanceLogServices;

        public AttendanceLogController(IAttendanceLogServices attendanceLogServices)
        {
            _attendanceLogServices = attendanceLogServices;
        }

        // GET: api/<AttendanceLogController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAttendanceLogDto>>> Get([FromQuery] int? count, DateTime? startDate)
        {
            try
            {
                var res = await _attendanceLogServices.GetAllAttendanceLogsAsync(count,startDate);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<AttendanceLogController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetAttendanceLogDto>> Get(int id)
        {
            try
            {
                var res = await _attendanceLogServices.GetAttendanceLogAsync(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/<AttendanceLogController>/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<GetAttendanceLogsWithDetailsDto>> GetByUserId(long userId, [FromQuery] DateTime? date)
        {
            try
            {
                var res = await _attendanceLogServices.GetAttendanceLogByUserId(userId, date);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/<AttendanceLogController>/summary
        [HttpGet("summary")]
        public async Task<ActionResult<GetAttendanceLogSummaryDto>> GetSummary([FromQuery] DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var res = await _attendanceLogServices.GetAttendanceLogSummary(startDate, endDate);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/<AttendanceLogController>/totalhours
        [HttpGet("totalhours")]
        public  async Task<ActionResult<GetTotalHoursDto>> GetUserTotalHour([FromQuery] long? userId, DateTime? startDate, DateTime? endDate, string? reportType)
        {
            try
            {
                var res =  await _attendanceLogServices.GetTotalHoursOfEmployee(userId, startDate, endDate, reportType);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        // GET api/<AttendanceLogController>/activity-record/in
        [HttpGet("activity-record/in")]
        public async Task<ActionResult<GetInActivityRecordDto>> GetInActivityRecord([FromQuery] long? userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var res = await _attendanceLogServices.GetInActivityRecord(userId, startDate, endDate);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        // GET api/<AttendanceLogController>/total-hours/in
        [HttpGet("total-hours/in")]
        public async Task<ActionResult<GetInActivityRecordDto>> GetTotalInActivity([FromQuery] long? userId, DateTime? startDate, DateTime? endDate, string? reportType)
        {
            try
            {
                var res = await _attendanceLogServices.GetTotalInActivity(userId, startDate, endDate, reportType);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        // GET api/<AttendanceLogController>/total-hours/out
        [HttpGet("total-hours/out")]
        public async Task<ActionResult<GetInActivityRecordDto>> GetTotalOutActivity([FromQuery] long? userId, DateTime? startDate, DateTime? endDate, string? reportType)
        {
            try
            {
                var res = await _attendanceLogServices.GetTotalOutActivity(userId, startDate, endDate, reportType);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/<AttendanceLogController>/activity-record/out
        [HttpGet("activity-record/out")]
        public async Task<ActionResult<GetOutActivityRecordDto>> GetActivityRecordOutHour([FromQuery] long? userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var res = await _attendanceLogServices.GetOutActivityRecord(userId, startDate, endDate);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/<AttendanceLogController>/status
        [HttpGet("status")]
        public async Task<ActionResult<GetOutActivityRecordDto>> GetStatusOfEmployee([FromQuery] string? FirstName, DateTime? Date)
        {
            try
            {
                var res = await _attendanceLogServices.GetStatusOfAttendanceLog(FirstName, Date);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/<AttendanceLogController>/current-status?type=
        [HttpGet("current-status")]
        public async Task<ActionResult<GetOutActivityRecordDto>> GetCurrentStatusOfEmployee([FromQuery] string? type, DateTime? date, int? count)
        {
            try
            {
                var res = await _attendanceLogServices.GetCurrentStatusOfAttendanceLog(type, date, count);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST api/<AttendanceLogController>
        [HttpPost]
        public async Task<ActionResult<GetAttendanceLogDto>> Post([FromBody] CreateAttendanceLogDto attendanceLogDto)
        {
            try
            {
                var res = await _attendanceLogServices.CreateAttendanceLogAsync(attendanceLogDto);
                return CreatedAtAction(nameof(Get), new { id = res.Id }, res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<AttendanceLogController>/multiple
        [HttpPost("multiple")]
        public async Task<ActionResult<IEnumerable<GetAttendanceLogsWithDetailsDto>>> Post([FromBody] IEnumerable<CreateAttendanceLogDto> attendanceLogsDto)
        {
            try
            {
                var res = await _attendanceLogServices.CreateMultipleAttendanceLogsAsync(attendanceLogsDto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<AttendanceLogController>/misentry
        [HttpGet("misentry")]
        public async Task<ActionResult<IEnumerable<GetAttendanceLogsWithDetailsDto>>> Get([FromQuery] long userId, DateTime? date)
        {
            try
            {
                var res = await _attendanceLogServices.GetMisEntryOfUsers(userId, date);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<AttendanceLogController>/misentry
        [HttpGet("misentry/summary")]
        public async Task<ActionResult<IEnumerable<GetMisEntrySummaryDto>>> MisEntrySummary([FromQuery] long? userId, DateTime? date)
        {
            try
            {
                var res = await _attendanceLogServices.GetMisEntrySummary(userId, date);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<AttendanceLogController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<GetAttendanceLogDto>> Put(int id, [FromBody] UpdateAttendanceLogDto attendanceLogDto)
        {
            try
            {
                var res = await _attendanceLogServices.UpdateAttendanceLogAsync(id, attendanceLogDto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE api/<AttendanceLogController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var res = await _attendanceLogServices.DeleteAttendanceLogAsync(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("avgtotalhours")]
        public async Task<ActionResult<GetAvgHoursDto>> GetAverageTotalHour([FromQuery] long? userId, DateTime? date)
        {
            try
            {
                var res = await _attendanceLogServices.GetAvgTotalHoursOfEmployee(userId, date);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
