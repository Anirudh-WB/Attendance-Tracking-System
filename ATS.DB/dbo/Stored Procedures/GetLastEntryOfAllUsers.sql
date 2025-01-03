CREATE   PROCEDURE [dbo].[GetLastEntryOfAllUsers]
    @type NVARCHAR(50),
    @date DATE
AS
BEGIN
    WITH LatestEntries AS (
        SELECT
            a.Id,
            a.UserId,
            a.CheckType,
            a.AttendanceLogTime,
            ROW_NUMBER() OVER (PARTITION BY a.UserId ORDER BY a.AttendanceLogTime DESC) AS rn
        FROM AttendanceLogs a
        WHERE CAST(a.AttendanceLogTime AS DATE) = @date
    )
    
    SELECT
        le.Id,
        le.UserId,
        u.Email,
        ed.ProfilePic,
        ed.FirstName,
        ed.LastName,
        le.AttendanceLogTime,
        le.CheckType
    FROM LatestEntries le
    INNER JOIN [Users] u ON le.UserId = u.Id
    INNER JOIN EmployeeDetails ed ON u.EmployeeDetailsId = ed.Id
    WHERE le.rn = 1 AND le.CheckType = @type;
END;
