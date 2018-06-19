CREATE TABLE [dbo].[Company] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50)  NOT NULL,
    [TaxNumber]       VARCHAR (13)  NOT NULL,
    [ContactName]     VARCHAR (100) NOT NULL,
    [RegistryNumber]  VARCHAR (13)  NOT NULL,
    [TelephoneNumber] VARCHAR (16)  NOT NULL,
    [Address]         VARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([Id] ASC)
);



















