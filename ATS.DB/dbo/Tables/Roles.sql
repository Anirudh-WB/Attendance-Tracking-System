CREATE TABLE [dbo].[Roles] (
    [Id]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [RoleName]  NVARCHAR (50) NOT NULL,
    [IsActive]  BIT           NOT NULL,
    [CreatedAt] DATETIME2 (7) NOT NULL,
    [UpdatedAt] DATETIME2 (7) NOT NULL,
    [CreatedBy] BIGINT        NOT NULL,
    [UpdatedBy] BIGINT        NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_RoleName]
    ON [dbo].[Roles]([RoleName] ASC);

