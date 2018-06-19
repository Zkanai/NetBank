CREATE TABLE [dbo].[Person] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [FullName]        VARCHAR (100) NOT NULL,
    [IdNumber]        VARCHAR (8)   NOT NULL,
    [MotherName]      VARCHAR (50)  NOT NULL,
    [TelephoneNumber] VARCHAR (16)  NOT NULL,
    [Address]         VARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([Id] ASC)
);











