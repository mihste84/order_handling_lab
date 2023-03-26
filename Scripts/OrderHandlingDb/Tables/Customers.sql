CREATE TABLE [dbo].[Customers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Active] [bit] NOT NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[UpdatedBy] [nvarchar](200) NOT NULL,
	[Updated] [datetime2](7) NOT NULL
)
GO
ALTER TABLE [dbo].[Customers] ADD CONSTRAINT DF_Customers_Created DEFAULT GETUTCDATE() FOR [Created];
GO
ALTER TABLE [dbo].[Customers] ADD CONSTRAINT DF_Customers_Updated DEFAULT GETUTCDATE() FOR [Updated];
GO
ALTER TABLE [dbo].[Customers] ADD CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
GO



