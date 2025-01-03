CREATE     PROCEDURE [dbo].[GetSummationOfTotalHours]
    @userId BIGINT = NULL,
    @startDate DATETIME,
    @endDate DATETIME
AS
BEGIN
    -- Ensure the procedure is executed with correct settings
    SET NOCOUNT ON;

    -- Create a temporary table to store the results of the existing procedure
    DECLARE @Results TABLE (
        UserId BIGINT,
        ProfilePic VARCHAR(MAX),
        FirstName VARCHAR(50),
        LastName VARCHAR(50),
        PeriodStart DATETIME,
        PeriodEnd DATETIME,
        TotalTimeSpanFormatted VARCHAR(8)
    );

    -- Insert the results from the existing procedure into the temporary table
    INSERT INTO @Results (UserId, ProfilePic, FirstName, LastName, PeriodStart, PeriodEnd, TotalTimeSpanFormatted)
    EXEC [dbo].[GetTotalHour_Employee_Report]
        @startDate = @startDate,
        @endDate = @endDate,
        @userId = @userId;

    -- Calculate the total time differences for each UserId
    SELECT
        UserId,
        CAST(SUM(DATEDIFF(SECOND, PeriodStart, PeriodEnd)) / 3600 AS VARCHAR(3)) + ':' +
		RIGHT('0' + CAST((SUM(DATEDIFF(SECOND, PeriodStart, PeriodEnd)) % 3600) / 60 AS VARCHAR(2)), 2) + ':' +
		RIGHT('0' + CAST(SUM(DATEDIFF(SECOND, PeriodStart, PeriodEnd)) % 60 AS VARCHAR(2)), 2) AS TotalOutHours
    FROM
        @Results
    GROUP BY
        UserId;
END
