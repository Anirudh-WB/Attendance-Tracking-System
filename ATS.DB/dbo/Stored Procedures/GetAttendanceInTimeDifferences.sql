
CREATE PROCEDURE [dbo].[GetAttendanceInTimeDifferences]
    @userId BIGINT = NULL,
    @startDate DATE,
    @endDate DATE
AS
BEGIN
    WITH GroupedLogs AS (
        SELECT 
            UserId, 
            AttendanceLogTime, 
            CheckType,
            CASE 
                WHEN LAG(CheckType) OVER (PARTITION BY UserId, CAST(AttendanceLogTime AS DATE) 
                                          ORDER BY UserId, AttendanceLogTime) = CheckType 
                    THEN NULL 
                ELSE CheckType 
            END AS PeriodType
        FROM AttendanceLogs
        WHERE 
            (@userId IS NULL OR UserId = @userId)
            AND AttendanceLogTime BETWEEN @startDate AND @endDate
    ),
    PeriodGroups AS (
        SELECT 
            UserId, 
            AttendanceLogTime, 
            CheckType,
            SUM(CASE WHEN PeriodType IS NOT NULL THEN 1 ELSE 0 END) 
                OVER (ORDER BY UserId, AttendanceLogTime) AS PeriodGroup
        FROM GroupedLogs
    ),
    MaxTimes AS (
        SELECT 
            UserId,
            MAX(AttendanceLogTime) AS LastTime, 
            CheckType, 
            PeriodGroup 
        FROM PeriodGroups 
        GROUP BY 
            UserId,
            PeriodGroup, 
            CheckType
    ), 
    TimeDifferences AS (
        SELECT 
            inPeriod.UserId,
            inPeriod.LastTime AS InTime, 
            outPeriod.LastTime AS OutTime, 
            CONVERT(VARCHAR(10), DATEADD(SECOND, DATEDIFF(SECOND, inPeriod.LastTime, outPeriod.LastTime), 0), 108) AS TotalInHours 
        FROM MaxTimes AS inPeriod 
        JOIN MaxTimes AS outPeriod 
            ON inPeriod.PeriodGroup = outPeriod.PeriodGroup - 1
        WHERE 
            inPeriod.CheckType = 'IN' 
            AND outPeriod.CheckType = 'OUT'
            AND CAST(inPeriod.LastTime AS DATE) = CAST(outPeriod.LastTime AS DATE)
    )

    SELECT * 
    FROM TimeDifferences

-- EXEC [dbo].[GetAttendanceInTimeDifferences] NULL, '2020-10-10', '2025-10-10'
END
