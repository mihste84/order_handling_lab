SET IDENTITY_INSERT [dbo].Countries ON;

IF NOT EXISTS (SELECT * FROM [dbo].Countries)
    INSERT INTO [dbo].Countries ([Id], [Abbreviation], [Created], [CreatedBy], [Name], [PhonePrefix], [Updated], [UpdatedBy])
    VALUES (1, N'SE', '2023-03-19T20:31:29.6461210Z', N'Seed', N'Sweden', NULL, '2023-03-19T20:31:29.6461210Z', N'Seed'),
    (2, N'FI', '2023-03-19T20:31:29.6461210Z', N'Seed', N'Finland', NULL, '2023-03-19T20:31:29.6461210Z', N'Seed'),
    (3, N'NO', '2023-03-19T20:31:29.6461210Z', N'Seed', N'Norway', NULL, '2023-03-19T20:31:29.6461210Z', N'Seed'),
    (4, N'DK', '2023-03-19T20:31:29.6461210Z', N'Seed', N'Denmark', NULL, '2023-03-19T20:31:29.6461210Z', N'Seed');
ELSE 
    print 'Countries already seeded';

SET IDENTITY_INSERT [dbo].Countries OFF;    

GO
    
SET IDENTITY_INSERT [dbo].[Cities] ON;
IF NOT EXISTS (SELECT * FROM [dbo].[Cities])
    INSERT INTO [dbo].[Cities] ([Id], [CountryId], [Created], [CreatedBy], [Name], [Updated], [UpdatedBy])
    VALUES (1, 1, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Stockholm', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (2, 2, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Helsinki', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (3, 3, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Oslo', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (4, 4, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Copenhagen', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (5, 1, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Gothenburg', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (6, 1, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Malmö', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (7, 2, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Tampere', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (8, 2, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Turku', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (9, 3, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Bergen', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (10, 3, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Trondheim', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (11, 4, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Aarhus', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (12, 4, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Odense', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (13, 1, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Uppsala', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (14, 1, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Västerås', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (15, 2, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Espoo', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (16, 2, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Vantaa', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (17, 3, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Stavanger', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (18, 3, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Bærum', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (19, 4, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Aalborg', '2023-03-19T20:31:29.6457690Z', N'Seed'),
    (20, 4, '2023-03-19T20:31:29.6457690Z', N'Seed', N'Frederiksberg', '2023-03-19T20:31:29.6457690Z', N'Seed');
ELSE 
    print 'Cities already seeded';

SET IDENTITY_INSERT [dbo].[Cities] OFF;
GO