/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
:r .\Script\Init\dbo.Company.Init.Table.sql
:r .\Script\Init\dbo.Person.Init.Table.sql
:r .\Script\Init\dbo.CurrentAccount.Init.Table.sql
:r .\Script\Init\dbo.TokenManager.Init.Table.sql
:r .\Script\Init\dbo.Transaction.Init.Table.sql
