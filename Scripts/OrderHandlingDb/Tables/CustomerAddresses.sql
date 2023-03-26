CREATE TABLE [dbo].[CustomerAddresses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Address] [nvarchar](200) NOT NULL,
	[Primary] [bit] NOT NULL,
	[PostArea] [nvarchar](50) NOT NULL,
	[ZipCode] [nvarchar](10) NOT NULL,
	[CountryId] [int] NULL,
	[CustomerId] [int] NOT NULL,
	[CityId] [int] NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[UpdatedBy] [nvarchar](200) NOT NULL,
	[Updated] [datetime2](7) NOT NULL
)
GO

ALTER TABLE [dbo].[CustomerAddresses] ADD CONSTRAINT DF_CustomerAddresses_Created DEFAULT GETUTCDATE() FOR [Created];
GO
ALTER TABLE [dbo].[CustomerAddresses] ADD CONSTRAINT DF_CustomerAddresses_Updated DEFAULT GETUTCDATE() FOR [Updated];
GO

ALTER TABLE [dbo].[CustomerAddresses] ADD  CONSTRAINT [PK_CustomerAddresses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_CustomerAddresses_CityId] ON [dbo].[CustomerAddresses]
(
	[CityId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_CustomerAddresses_CountryId] ON [dbo].[CustomerAddresses]
(
	[CountryId] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CustomerAddresses_CustomerId_CountryId_CityId_PostArea_ZipCode_Address] ON [dbo].[CustomerAddresses]
(
	[CustomerId] ASC,
	[CountryId] ASC,
	[CityId] ASC,
	[PostArea] ASC,
	[ZipCode] ASC,
	[Address] ASC
)
WHERE ([CountryId] IS NOT NULL AND [CityId] IS NOT NULL)
GO

ALTER TABLE [dbo].[CustomerAddresses] ADD  CONSTRAINT [FK_CustomerAddresses_Cities_CityId] FOREIGN KEY([CityId])
REFERENCES [dbo].[Cities] ([Id])
GO

ALTER TABLE [dbo].[CustomerAddresses] CHECK CONSTRAINT [FK_CustomerAddresses_Cities_CityId]
GO

ALTER TABLE [dbo].[CustomerAddresses] ADD  CONSTRAINT [FK_CustomerAddresses_Countries_CountryId] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Countries] ([Id])
GO

ALTER TABLE [dbo].[CustomerAddresses] CHECK CONSTRAINT [FK_CustomerAddresses_Countries_CountryId]
GO

ALTER TABLE [dbo].[CustomerAddresses] ADD  CONSTRAINT [FK_CustomerAddresses_Customers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CustomerAddresses] CHECK CONSTRAINT [FK_CustomerAddresses_Customers_CustomerId]
GO