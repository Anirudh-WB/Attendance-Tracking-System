
CREATE PROCEDURE [dbo].[GetSumOfOutTimeDifferences]
    @startDate DATETIME,
    @endDate DATETIME,
    @userId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Results TABLE (
        UserId BIGINT,
        InTime DATETIME,
        OutTime DATETIME,
        TotalOutHours VARCHAR(8)
    );

    INSERT INTO @Results (UserId, InTime, OutTime, TotalOutHours)
    EXEC [dbo].[GetAttendanceOutTimeDifferences]
        @startDate = @startDate,
        @endDate = @endDate,
        @userId = @userId;

    SELECT
        r.UserId,
        e.ProfilePic,
        e.FirstName,
        e.LastName,
        CONVERT(
            VARCHAR, 
            DATEADD(SECOND, SUM(DATEDIFF(SECOND, r.InTime, r.OutTime)), 0), 
            108
        ) AS TotalHours
    FROM
        @Results r
    JOIN 
        EmployeeDetails e ON e.UserId = r.UserId
    GROUP BY
        r.UserId,
        e.ProfilePic,
        e.FirstName,
        e.LastName;

-- EXEC [dbo].[GetSumOfOutTimeDifferences] '2020-10-10', '2025-10-10', NULL
END
