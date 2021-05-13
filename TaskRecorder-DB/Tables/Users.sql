CREATE TABLE [dbo].[Users]
(
	[UserId] NVARCHAR(100) NOT NULL PRIMARY KEY,
	[FirstName] NVARCHAR(max),
	[LastName] NVARCHAR(max),
	[Email] NVARCHAR(max),
	[PhoneNumber] NVARCHAR(max),
	[Address] NVARCHAR(max),
	[Password] NVARCHAR(max)
)
