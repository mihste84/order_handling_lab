CREATE TABLE [dbo].[CustomerContactInfo](
    [Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](200) NOT NULL,
    [CustomerId] [int] NOT NULL,
    [CreatedBy] [nvarchar](200) NOT NULL,
    [Created] [datetime2](7) NOT NULL,
    [UpdatedBy] [nvarchar](200) NOT NULL,
    [Updated] [datetime2](7) NOT NULL,
    [RowVersion] [rowversion] NOT NULL
)
GO
ALTER TABLE [dbo].[CustomerContactInfo] ADD CONSTRAINT DF_CustomerContactInfo_Created DEFAULT GETUTCDATE() FOR [Created];
GO
ALTER TABLE [dbo].[CustomerContactInfo] ADD CONSTRAINT DF_CustomerContactInfo_Updated DEFAULT GETUTCDATE() FOR [Updated];
GO

ALTER TABLE [dbo].[CustomerContactInfo] ADD  CONSTRAINT [PK_CustomerContactInfo] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)
GO 

CREATE UNIQUE NONCLUSTERED INDEX [IX_CustomerContactInfo_CustomerId_ContactType_ContactValue] ON [dbo].[CustomerContactInfo]
(
    [CustomerId] ASC,
    [Type] ASC,
    [Value] ASC
)
GO
CREATE NONCLUSTERED INDEX [IX_CustomerContactInfo_CustomerId_ContactType] ON [dbo].[CustomerContactInfo]
(
    [CustomerId] ASC,
    [Type] ASC
)
GO
ALTER TABLE [dbo].[CustomerContactInfo] ADD CONSTRAINT [FK_CustomerContactInfo_Customers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CustomerContactInfo] CHECK CONSTRAINT [FK_CustomerContactInfo_Customers_CustomerId]
GO
