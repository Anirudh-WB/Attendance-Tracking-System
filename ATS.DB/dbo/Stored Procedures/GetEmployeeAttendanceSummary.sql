CREATE   PROCEDURE GetEmployeeAttendanceSummary
AS
BEGIN

    CREATE TABLE #AttendanceSummary (
        UserId BIGINT,
        EntryTime DATETIME2
    );

    INSERT INTO #AttendanceSummary (UserId, EntryTime)
    SELECT
        UserId,
        MIN(AttendanceLogTime) AS EntryTime
    FROM AttendanceLogs
    WHERE CheckType = 'IN'
      AND CAST(AttendanceLogTime AS DATE) = CAST(GETDATE() AS DATE)
    GROUP BY UserId;

    SELECT 
		e.Id,
		e.UserId,
		e.ProfilePic,
        e.FirstName,
        e.LastName,
        a.EntryTime AS Intime,
        CASE 
            WHEN a.EntryTime IS NOT NULL THEN 'Present'
            ELSE 'Absent'
        END AS Status
    FROM EmployeeDetails e
    INNER JOIN Users u ON e.Id = u.EmployeeDetailsId
    LEFT JOIN #AttendanceSummary a ON u.Id = a.UserId
    ORDER BY e.FirstName, e.LastName, e.Id;

    DROP TABLE #AttendanceSummary;
END;


