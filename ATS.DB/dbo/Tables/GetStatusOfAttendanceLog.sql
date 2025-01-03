CREATE TABLE [dbo].[GetStatusOfAttendanceLog] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProfilePic] NVARCHAR (MAX) NOT NULL,
    [FirstName]  NVARCHAR (MAX) NOT NULL,
    [LastName]   NVARCHAR (MAX) NOT NULL,
    [Status]     NVARCHAR (MAX) NOT NULL,
    [InTime]     DATETIME2 (7)  NULL,
    [UserId]     BIGINT         NOT NULL,
    CONSTRAINT [PK_GetStatusOfAttendanceLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

