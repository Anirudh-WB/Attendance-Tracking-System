CREATE   PROCEDURE GetTotalHour_Employee_Report
    @userId BIGINT = NULL,
    @startDate DATE,
    @endDate DATE
AS
BEGIN
    WITH TotalHours AS (
        SELECT 
            UserId,
            AttendanceLogTime,
            CheckType,
            CASE 
                WHEN LAG(CheckType) OVER (
                        PARTITION BY UserId, CAST(AttendanceLogTime AS DATE) 
                        ORDER BY AttendanceLogTime
                    ) = CheckType 
                THEN 0 
                ELSE 1 
            END AS PeriodType
        FROM 
            AttendanceLogs
        WHERE 
            (@userId IS NULL OR UserId = @userId)
            AND AttendanceLogTime BETWEEN @startDate AND @endDate
    ),
    PeriodGroups AS (
        SELECT 
            UserId, 
            AttendanceLogTime, 
            CheckType,
            SUM(CASE WHEN PeriodType = 1 THEN 1 ELSE 0 END) 
                OVER (
                    PARTITION BY UserId, CAST(AttendanceLogTime AS DATE) 
                    ORDER BY AttendanceLogTime
                ) AS PeriodGroup
        FROM 
            TotalHours
    ),
    MinMaxTimes AS (
        SELECT 
            UserId,
            MAX(AttendanceLogTime) AS LastTime, 
            CheckType, 
            PeriodGroup
        FROM 
            PeriodGroups
        GROUP BY 
            PeriodGroup, CheckType, CAST(AttendanceLogTime AS DATE), UserId
    ),
    TimeDifferences AS (
        SELECT
            inPeriod.UserId,
            MIN(inPeriod.LastTime) AS InTime,
            MAX(outPeriod.LastTime) AS OutTime
        FROM 
            MinMaxTimes inPeriod 
        JOIN 
            MinMaxTimes outPeriod
        ON 
            inPeriod.UserId = outPeriod.UserId
            AND CAST(inPeriod.LastTime AS DATE) = CAST(outPeriod.LastTime AS DATE)
            AND inPeriod.PeriodGroup < outPeriod.PeriodGroup
        WHERE 
            inPeriod.CheckType = 'IN' 
            AND outPeriod.CheckType = 'OUT'
		GROUP BY
            inPeriod.UserId, CAST(inPeriod.LastTime AS DATE)
    )
    
    SELECT 
        t.UserId,
		e.ProfilePic,
		e.FirstName,
		e.LastName,
        t.InTime as PeriodStart,
        t.OutTime as PeriodEnd,
        CONVERT(VARCHAR(8) , DATEADD( SECOND, DATEDIFF(SECOND, t.InTime, t.OutTime), 0), 108) AS TotalTimeSpanFormatted
    FROM 
        TimeDifferences t
	JOIN
		EmployeeDetails e
	On
		t.UserId = e.UserId
    ORDER BY 
        t.UserId, t.InTime;
END;
