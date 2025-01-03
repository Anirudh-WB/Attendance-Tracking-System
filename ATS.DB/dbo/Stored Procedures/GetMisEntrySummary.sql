CREATE   PROCEDURE [dbo].[GetMisEntrySummary]
    @userId BIGINT = NULL,
    @date DATE
AS
BEGIN
    -- Step 1: Group attendance logs into logical periods
    WITH PeriodGroups AS (
        SELECT
            a.Id,
            a.UserId,
            a.CheckType,
            a.AttendanceLogTime,
            ROW_NUMBER() OVER (PARTITION BY a.UserId ORDER BY a.AttendanceLogTime) -
            ROW_NUMBER() OVER (PARTITION BY a.UserId, a.CheckType ORDER BY a.AttendanceLogTime) AS PeriodGroup
        FROM AttendanceLogs a
        WHERE CAST(a.AttendanceLogTime AS DATE) = @date
          AND (@userId IS NULL OR a.UserId = @userId)
    ),
    
    -- Step 2: Calculate period boundaries (start and end times)
    PeriodBoundaries AS (
        SELECT
            UserId,
            CheckType,
            MIN(AttendanceLogTime) AS PeriodStart,
            MAX(AttendanceLogTime) AS PeriodEnd,
            COUNT(*) AS EntryCount,
            PeriodGroup
        FROM PeriodGroups
        GROUP BY UserId, CheckType, PeriodGroup
    ),
    
    -- Step 3: Identify misentries based on duration and count
    MisEntries AS (
        SELECT
            UserId,
            CheckType,
            PeriodStart,
            PeriodEnd,
            EntryCount,
            CASE
                WHEN DATEDIFF(MINUTE, PeriodStart, PeriodEnd) >= 5 THEN 1 ELSE 0
            END AS IsMisEntry
        FROM PeriodBoundaries
    ),
    
    -- Step 4: Aggregate misentry counts and entry counts for misentry groups
    MisEntryCounts AS (
        SELECT
            UserId,
            SUM(IsMisEntry) AS MisEntryCount,
            SUM(CASE WHEN IsMisEntry = 1 THEN EntryCount ELSE 0 END) AS TotalEntryCount
        FROM MisEntries
        GROUP BY UserId
    )

    -- Step 5: Retrieve user data and combine with misentry counts
    SELECT 
        u.Id AS UserId,
        u.Email,
        e.ProfilePic,
        e.FirstName,
        e.LastName,
        ISNULL(m.TotalEntryCount, 0) AS TotalCount
    FROM Users u
    INNER JOIN EmployeeDetails e
        ON u.Id = e.UserId
    LEFT JOIN MisEntryCounts m
        ON u.Id = m.UserId
    WHERE (@userId IS NULL OR u.Id = @userId)
    ORDER BY u.Id;

	--EXEC GetMisEntrySummary 5, '2024-12-21'
END;
