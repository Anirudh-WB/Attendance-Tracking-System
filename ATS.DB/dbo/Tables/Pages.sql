CREATE TABLE [dbo].[Pages] (
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [PageCode]  NVARCHAR (100) NOT NULL,
    [PageTitle] NVARCHAR (100) NOT NULL,
    [CreatedAt] DATETIME2 (7)  NOT NULL,
    [UpdatedAt] DATETIME2 (7)  NOT NULL,
    [CreatedBy] BIGINT         NOT NULL,
    [UpdatedBy] BIGINT         NOT NULL,
    CONSTRAINT [PK_Pages] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Pages_PageCode]
    ON [dbo].[Pages]([PageCode] ASC);

