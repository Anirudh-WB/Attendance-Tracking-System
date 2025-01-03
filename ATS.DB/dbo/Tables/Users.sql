CREATE TABLE [dbo].[Users] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [Email]             NVARCHAR (50)  NOT NULL,
    [Password]          NVARCHAR (100) NOT NULL,
    [ContactNo]         NVARCHAR (10)  NOT NULL,
    [IsActive]          BIT            NOT NULL,
    [EmployeeDetailsId] BIGINT         NULL,
    [CreatedAt]         DATETIME2 (7)  NOT NULL,
    [UpdatedAt]         DATETIME2 (7)  NOT NULL,
    [CreatedBy]         BIGINT         NOT NULL,
    [UpdatedBy]         BIGINT         NOT NULL,
    [RoleId]            BIGINT         NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email]
    ON [dbo].[Users]([Email] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Users_RoleId]
    ON [dbo].[Users]([RoleId] ASC);

