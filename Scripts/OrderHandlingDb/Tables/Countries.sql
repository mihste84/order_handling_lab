CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Abbreviation] [nvarchar](2) NOT NULL,
	[PhonePrefix] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[UpdatedBy] [nvarchar](200) NOT NULL,
	[Updated] [datetime2](7) NOT NULL,
	[RowVersion] [rowversion] NOT NULL
) 
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Countries_Name] ON [dbo].[Countries]
(
	[Name] ASC
)
GO

ALTER TABLE [dbo].[Countries] ADD CONSTRAINT DF_Countries_Created DEFAULT GETUTCDATE() FOR [Created];
GO
ALTER TABLE [dbo].[Countries] ADD CONSTRAINT DF_Countries_Updated DEFAULT GETUTCDATE() FOR [Updated];
GO

ALTER TABLE [dbo].[Countries] ADD  CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO