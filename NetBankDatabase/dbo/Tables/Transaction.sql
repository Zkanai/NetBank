CREATE TABLE [dbo].[Transaction] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [From]        INT           NOT NULL,
    [To]          INT           NOT NULL,
    [Amount]      INT           NOT NULL,
    [Date]        DATETIME      NOT NULL,
    [DueDate]     DATETIME      NOT NULL,
    [Description] VARCHAR (200) NULL,
    CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Transaction_From_CurrentAccount] FOREIGN KEY ([From]) REFERENCES [dbo].[CurrentAccount] ([Id]),
    CONSTRAINT [FK_Transaction_To_CurrentAccount] FOREIGN KEY ([To]) REFERENCES [dbo].[CurrentAccount] ([Id])
);







