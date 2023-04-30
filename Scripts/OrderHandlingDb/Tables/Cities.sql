CREATE TABLE [dbo].[Cities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CountryId] [int] NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[UpdatedBy] [nvarchar](200) NOT NULL,
	[Updated] [datetime2](7) NOT NULL,
	[RowVersion] [rowversion] NOT NULL
)
GO

ALTER TABLE [dbo].[Cities] ADD CONSTRAINT DF_Cities_Created DEFAULT GETUTCDATE() FOR [Created];
GO
ALTER TABLE [dbo].[Cities] ADD CONSTRAINT DF_Cities_Updated DEFAULT GETUTCDATE() FOR [Updated];
GO


ALTER TABLE [dbo].[Cities] ADD  CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) 
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Cities_CountryId_Name] ON [dbo].[Cities]
(
	[CountryId] ASC,
	[Name] ASC
)
WHERE ([CountryId] IS NOT NULL)
GO

ALTER TABLE [dbo].[Cities] ADD  CONSTRAINT [FK_Cities_Countries_CountryId] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Countries] ([Id])
GO

ALTER TABLE [dbo].[Cities] CHECK CONSTRAINT [FK_Cities_Countries_CountryId]
GO

