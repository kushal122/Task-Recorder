CREATE TABLE [dbo].[TaskRequest]
(
	[RequestId] INT NOT NULL PRIMARY KEY Identity(1,1),
	[TaskId] NVARCHAR(100),
	[RequestDate] DATE,
	[DeadLine] DATE,
	[Status] NVARCHAR(max),
	[RequestConfirmed] bit,
	[AdminResponse] bit,
	[UserId] NVARCHAR(max),
	[Description] NVARCHAR(max)
)
