CREATE TABLE [dbo].[AttendanceLogs] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]            BIGINT         NOT NULL,
    [AttendanceLogTime] DATETIME2 (7)  NOT NULL,
    [CheckType]         NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_AttendanceLogs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AttendanceLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AttendanceLogs_UserId]
    ON [dbo].[AttendanceLogs]([UserId] ASC);

