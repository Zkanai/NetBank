CREATE TABLE [dbo].[CurrentAccount] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [AccountNumber]        VARCHAR (50)  NOT NULL,
    [Balance]              INT           NOT NULL,
    [CompanyId]            INT           NULL,
    [PersonId]             INT           NULL,
    [AccountOpenedDate]    DATETIME      NULL,
    [EmailAddress]         VARCHAR (50)  NOT NULL,
    [TokenManagerTokenKey] VARCHAR (100) NOT NULL,
    [Password]             VARCHAR (100) NULL,
    [Salt]                 VARCHAR (100) NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CurrentAccount_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id]),
    CONSTRAINT [FK_CurrentAccount_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [IX_CurrentAccount] UNIQUE NONCLUSTERED ([TokenManagerTokenKey] ASC),
    CONSTRAINT [IX_EmailAddress] UNIQUE NONCLUSTERED ([EmailAddress] ASC)
);
























GO


