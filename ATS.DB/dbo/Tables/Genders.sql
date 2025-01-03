CREATE TABLE [dbo].[Genders] (
    [Id]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [GenderName] NVARCHAR (50) NOT NULL,
    [GenderCode] NVARCHAR (10) NOT NULL,
    [IsActive]   BIT           NOT NULL,
    [CreatedAt]  DATETIME2 (7) NOT NULL,
    [UpdatedAt]  DATETIME2 (7) NOT NULL,
    [CreatedBy]  BIGINT        NOT NULL,
    [UpdatedBy]  BIGINT        NOT NULL,
    CONSTRAINT [PK_Genders] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Genders_GenderCode]
    ON [dbo].[Genders]([GenderCode] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Genders_GenderName]
    ON [dbo].[Genders]([GenderName] ASC);

