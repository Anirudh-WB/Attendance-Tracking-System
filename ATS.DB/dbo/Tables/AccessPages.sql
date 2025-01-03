CREATE TABLE [dbo].[AccessPages] (
    [Id]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [RoleId]    BIGINT        NOT NULL,
    [PageId]    BIGINT        NOT NULL,
    [CreatedAt] DATETIME2 (7) NOT NULL,
    [UpdatedAt] DATETIME2 (7) NOT NULL,
    [CreatedBy] BIGINT        NOT NULL,
    [UpdatedBy] BIGINT        NOT NULL,
    [IsActive]  BIT           DEFAULT (CONVERT([bit],(0))) NOT NULL,
    CONSTRAINT [PK_AccessPages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AccessPages_Pages_PageId] FOREIGN KEY ([PageId]) REFERENCES [dbo].[Pages] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AccessPages_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AccessPages_PageId]
    ON [dbo].[AccessPages]([PageId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AccessPages_RoleId]
    ON [dbo].[AccessPages]([RoleId] ASC);

