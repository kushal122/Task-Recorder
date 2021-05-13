CREATE TABLE [dbo].[NonUserActivity]
(
	[Id] INT NOT NULL PRIMARY KEY IDentity(1,1),
	[Url] NVARCHAR(max),
	[Data] NVARCHAR(max),
	[IpAddress] NVARCHAR(max),
	[ActivityDate] Date
)
