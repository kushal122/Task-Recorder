CREATE TABLE [dbo].[UserActivity]
(
	[Id] INT NOT NULL PRIMARY KEY IDentity(1,1),
	[Url] NVARCHAR(max),
	[Data] NVARCHAR(max),
	[UserName] NVARCHAR(max),
	[IpAddress] NVARCHAR(max),
	[ActivityDate] Date
)
