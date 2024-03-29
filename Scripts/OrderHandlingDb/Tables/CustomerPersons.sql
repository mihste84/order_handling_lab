CREATE TABLE [dbo].[CustomerPersons](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[MiddleName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Ssn] [nvarchar](20) NOT NULL,
	[CustomerId] [int] NOT NULL,
)
GO
ALTER TABLE [dbo].[CustomerPersons] ADD  CONSTRAINT [PK_CustomerPersons] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CustomerPersons_Ssn] ON [dbo].[CustomerPersons]
(
	[Ssn] ASC
)
WHERE ([Ssn] IS NOT NULL)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CustomerPersons_CustomerId] ON [dbo].[CustomerPersons]
(
	[CustomerId] ASC
)
WHERE ([CustomerId] IS NOT NULL)
GO

ALTER TABLE [dbo].[CustomerPersons] ADD CONSTRAINT [FK_CustomerPersons_Customers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
ON DELETE CASCADE
GO
