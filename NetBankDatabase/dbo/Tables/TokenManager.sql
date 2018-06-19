CREATE TABLE [dbo].[TokenManager] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [TokenKey]       VARCHAR (100) NOT NULL,
    [TokenExpiry]    DATETIME      NULL,
    [TokenIssued]    DATETIME      NULL,
    [AccountCreated] DATETIME      NULL,
    [IsActive]       BIT           NULL,
    CONSTRAINT [PK_Registration] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TokenManager_CurrentAccount] FOREIGN KEY ([TokenKey]) REFERENCES [dbo].[CurrentAccount] ([TokenManagerTokenKey])
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TokenManager]
    ON [dbo].[TokenManager]([TokenKey] ASC);

