CREATE PROCEDURE [dbo].[GetSpecificStatus]
    @Names NVARCHAR(MAX) -- Parameter for comma-separated names
AS
BEGIN
    -- Convert @Names into a table variable
    DECLARE @NamesTable TABLE (Name NVARCHAR(50));
    DECLARE @Delimiter CHAR(1) = ',';

    -- Split @Names and insert into the table variable
    INSERT INTO @NamesTable (Name)
    SELECT LTRIM(RTRIM(Value)) -- Trim spaces around each name
    FROM STRING_SPLIT(@Names, @Delimiter);

    -- Create a temporary table to store IN logs with datetime2 type
    CREATE TABLE #AttendanceSummary (
        UserId BIGINT,
        EntryTime DATETIME2
    );

    -- Insert IN logs into the temporary table
    INSERT INTO #AttendanceSummary (UserId, EntryTime)
    SELECT
        UserId,
        MIN(CASE WHEN CheckType = 'IN' THEN AttendanceLogTime END) AS EntryTime
    FROM AttendanceLogs
    WHERE CAST(AttendanceLogTime AS DATE) = CAST(GETDATE() AS DATE) -- Filter for today's date
    GROUP BY UserId
    HAVING MIN(CASE WHEN CheckType = 'IN' THEN AttendanceLogTime END) IS NOT NULL;

    -- Retrieve user details and attendance summary based on the provided names
    SELECT 
        e.ProfilePic,
        e.FirstName,
        e.LastName,
        CASE 
            WHEN MIN(a.EntryTime) IS NOT NULL THEN 'Present'
            ELSE 'Absent'
        END AS Status,
        MIN(a.EntryTime) AS Intime
    FROM EmployeeDetails e
    INNER JOIN Users u ON e.UserId = u.Id
    LEFT JOIN #AttendanceSummary a ON e.UserId = a.UserId
    WHERE EXISTS (
        SELECT 1 
        FROM @NamesTable n
        WHERE e.FirstName LIKE '%' + n.Name + '%'
    ) -- Filter using the LIKE operator with the @NamesTable
    GROUP BY e.FirstName, e.LastName, e.ProfilePic;

    -- Drop the temporary table
    DROP TABLE #AttendanceSummary;
END;
