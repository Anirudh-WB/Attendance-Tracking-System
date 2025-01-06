
CREATE   PROCEDURE [dbo].[GetSumOfInTimeDifferences]
    @userId BIGINT = NULL,
    @startDate DATETIME,
    @endDate DATETIME,
    @periodType VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- Temporary table to store daily totals
    DECLARE @DailyTotals TABLE (
        UserId BIGINT,
        LogDate DATE,
        TotalTimeSpanSeconds INT
    );

    -- Populate @DailyTotals by calculating daily differences
    INSERT INTO @DailyTotals (UserId, LogDate, TotalTimeSpanSeconds)
    SELECT
        al.UserId,
        CAST(al.AttendanceLogTime AS DATE) AS LogDate,
        DATEDIFF(SECOND, 
            (SELECT MAX(AttendanceLogTime) 
             FROM AttendanceLogs 
             WHERE UserId = al.UserId 
               AND CheckType = 'IN' 
               AND AttendanceLogTime < al.AttendanceLogTime 
               AND CAST(AttendanceLogTime AS DATE) = CAST(al.AttendanceLogTime AS DATE)),
            al.AttendanceLogTime
        ) AS TotalTimeSpanSeconds
    FROM AttendanceLogs al
    WHERE al.CheckType = 'OUT'
      AND CAST(al.AttendanceLogTime AS DATE) BETWEEN @startDate AND @endDate
      AND (@userId IS NULL OR al.UserId = @userId);

    -- Temporary table to store aggregated results
    DECLARE @AggregatedTotals TABLE (
        UserId BIGINT,
        PeriodStart DATE,
        PeriodEnd DATE,
        TotalTimeSpanFormatted VARCHAR(8)
    );

    -- Variables for period calculation
    DECLARE @CurrentStartDate DATE = @startDate;
    DECLARE @PeriodStart DATE;
    DECLARE @PeriodEnd DATE;

    WHILE @CurrentStartDate <= @endDate
    BEGIN
        -- Determine the period based on @periodType
        IF @periodType = 'daily'
        BEGIN
            SET @PeriodStart = @CurrentStartDate;
            SET @PeriodEnd = @CurrentStartDate;
        END
        ELSE IF @periodType = 'weekly'
        BEGIN
            SET @PeriodStart = DATEADD(DAY, 1 - DATEPART(WEEKDAY, @CurrentStartDate), @CurrentStartDate);
            SET @PeriodEnd = DATEADD(DAY, 7 - DATEPART(WEEKDAY, @CurrentStartDate), @CurrentStartDate);
        END
        ELSE IF @periodType = 'monthly'
        BEGIN
            SET @PeriodStart = DATEADD(MONTH, DATEDIFF(MONTH, 0, @CurrentStartDate), 0);
            SET @PeriodEnd = DATEADD(DAY, -1, DATEADD(MONTH, 1, @PeriodStart));
        END
        ELSE IF @periodType = 'quarterly'
        BEGIN
            SET @PeriodStart = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @CurrentStartDate), 0);
            SET @PeriodEnd = DATEADD(DAY, -1, DATEADD(QUARTER, 1, @PeriodStart));
        END
        ELSE IF @periodType = 'yearly'
        BEGIN
            SET @PeriodStart = DATEADD(YEAR, DATEDIFF(YEAR, 0, @CurrentStartDate), 0);
            SET @PeriodEnd = DATEADD(DAY, -1, DATEADD(YEAR, 1, @PeriodStart));
        END

        -- Aggregate totals for the current period
        INSERT INTO @AggregatedTotals (UserId, PeriodStart, PeriodEnd, TotalTimeSpanFormatted)
        SELECT
            dt.UserId,
            @PeriodStart,
            @PeriodEnd,
            RIGHT('0' + CAST(SUM(dt.TotalTimeSpanSeconds) / 3600 AS VARCHAR), 2) + ':' +
            RIGHT('0' + CAST((SUM(dt.TotalTimeSpanSeconds) % 3600) / 60 AS VARCHAR), 2) + ':' +
            RIGHT('0' + CAST(SUM(dt.TotalTimeSpanSeconds) % 60 AS VARCHAR), 2)
        FROM @DailyTotals dt
        WHERE dt.LogDate BETWEEN @PeriodStart AND @PeriodEnd
        GROUP BY dt.UserId;

        -- Move to the next period
        IF @periodType = 'daily'
        BEGIN
            SET @CurrentStartDate = DATEADD(DAY, 1, @CurrentStartDate);
        END
        ELSE IF @periodType = 'weekly'
        BEGIN
            SET @CurrentStartDate = DATEADD(DAY, 7, @CurrentStartDate);
        END
        ELSE IF @periodType = 'monthly'
        BEGIN
            SET @CurrentStartDate = DATEADD(MONTH, 1, @CurrentStartDate);
        END
        ELSE IF @periodType = 'quarterly'
        BEGIN
            SET @CurrentStartDate = DATEADD(QUARTER, 1, @CurrentStartDate);
        END
        ELSE IF @periodType = 'yearly'
        BEGIN
            SET @CurrentStartDate = DATEADD(YEAR, 1, @CurrentStartDate);
        END
    END

    
    SELECT
        at.UserId,
        e.ProfilePic,
        e.FirstName,
        e.LastName,
        at.TotalTimeSpanFormatted AS TotalHours
    FROM @AggregatedTotals at
    JOIN EmployeeDetails e ON e.UserId = at.UserId;

END