CREATE TABLE [dbo].[CustomerCompanies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[CustomerId] [int] NOT NULL
)
GO
ALTER TABLE [dbo].[CustomerCompanies] ADD  CONSTRAINT [PK_CustomerCompanies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CustomerCompanies_CompanyCode] ON [dbo].[CustomerCompanies]
(
	[Code] ASC
)
WHERE ([Code] IS NOT NULL)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CustomerCompanies_CustomerId] ON [dbo].[CustomerCompanies]
(
	[CustomerId] ASC
)
WHERE ([CustomerId] IS NOT NULL)
GO

ALTER TABLE [dbo].[CustomerCompanies] ADD CONSTRAINT [FK_CustomerCompanies_Customers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
ON DELETE CASCADE
GO

