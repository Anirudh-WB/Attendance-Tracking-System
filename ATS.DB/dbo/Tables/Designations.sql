CREATE TABLE [dbo].[Designations] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [DesignationName] NVARCHAR (100) NOT NULL,
    [DesignationCode] NVARCHAR (450) NOT NULL,
    [IsActive]        BIT            NOT NULL,
    [CreatedAt]       DATETIME2 (7)  NOT NULL,
    [UpdatedAt]       DATETIME2 (7)  NOT NULL,
    [CreatedBy]       BIGINT         NOT NULL,
    [UpdatedBy]       BIGINT         NOT NULL,
    CONSTRAINT [PK_Designations] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Designations_DesignationCode]
    ON [dbo].[Designations]([DesignationCode] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Designations_DesignationName]
    ON [dbo].[Designations]([DesignationName] ASC);

