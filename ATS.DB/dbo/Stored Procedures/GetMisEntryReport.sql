
CREATE   PROCEDURE [dbo].[GetMisEntryReport]
    @userId BIGINT = NULL,
    @date DATE
AS
BEGIN
    WITH GroupedLogs AS (
        SELECT
            Id,
            UserId,
            AttendanceLogTime,
            CheckType,
            CASE
                WHEN LAG(CheckType) OVER (PARTITION BY UserId, CAST(AttendanceLogTime AS DATE) ORDER BY UserId, AttendanceLogTime) = CheckType
                    THEN NULL
                ELSE CheckType
            END AS PeriodType
        FROM AttendanceLogs
        WHERE CAST(AttendanceLogTime AS DATE) = @date
          AND (@userId IS NULL OR UserId = @userId)
    ),

    PeriodGroups AS (
        SELECT
            Id,
            UserId,
            AttendanceLogTime,
            CheckType,
            SUM(CASE WHEN PeriodType IS NOT NULL THEN 1 ELSE 0 END) OVER (ORDER BY UserId, AttendanceLogTime) AS PeriodGroup
        FROM GroupedLogs
    ),

    MisEntries AS (
        SELECT
            Id,
            UserId,
            AttendanceLogTime,
            CheckType,
            CASE
                WHEN DATEDIFF(MINUTE, MIN(AttendanceLogTime) OVER (PARTITION BY PeriodGroup), MAX(AttendanceLogTime) OVER (PARTITION BY PeriodGroup)) >= 5 THEN 1
                ELSE 0
            END AS IsMisEntry
        FROM PeriodGroups
    )

    SELECT 
        m.Id, 
        m.UserId, 
        u.Email, 
        e.ProfilePic, 
        e.FirstName, 
        e.LastName, 
        m.AttendanceLogTime, 
        m.CheckType 
    FROM MisEntries m
    JOIN Users u ON m.UserId = u.Id
    JOIN EmployeeDetails e ON m.UserId = e.UserId
    WHERE m.IsMisEntry = 1;

-- EXEC GetMisEntryReport NULL, '2024-12-21';
END;