CREATE TABLE [dbo].[Tasks]
(
	[TaskId] NVARCHAR(100) NOT NULL PRIMARY KEY,
	[Title] NVARCHAR(max),
	[Description] NVARCHAR(max),
	[UploadedDate] DATE,
	[AssignedTo] NVARCHAR(max),
	[DeadLine] DATE,
	[Status] NVARCHAR(max),
	[IsConfirmed] bit,
	[UserId] NVARCHAR(max)
)
