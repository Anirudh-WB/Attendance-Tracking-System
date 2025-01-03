CREATE TABLE [dbo].[EmployeeDetails] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [UserId]        BIGINT          NOT NULL,
    [EmployeeCode]  NVARCHAR (MAX)  NULL,
    [FirstName]     NVARCHAR (50)   NOT NULL,
    [LastName]      NVARCHAR (50)   NOT NULL,
    [DesignationId] BIGINT          NOT NULL,
    [GenderId]      BIGINT          NOT NULL,
    [ProfilePic]    NVARCHAR (MAX)  NOT NULL,
    [FaceEncoding]  VARBINARY (MAX) NOT NULL,
    [CreatedAt]     DATETIME2 (7)   NOT NULL,
    [UpdatedAt]     DATETIME2 (7)   NOT NULL,
    [CreatedBy]     BIGINT          NOT NULL,
    [UpdatedBy]     BIGINT          NOT NULL,
    CONSTRAINT [PK_EmployeeDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeDetails_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [dbo].[Designations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeDetails_Genders_GenderId] FOREIGN KEY ([GenderId]) REFERENCES [dbo].[Genders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeDetails_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_EmployeeDetails_DesignationId]
    ON [dbo].[EmployeeDetails]([DesignationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EmployeeDetails_GenderId]
    ON [dbo].[EmployeeDetails]([GenderId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EmployeeDetails_UserId]
    ON [dbo].[EmployeeDetails]([UserId] ASC);

