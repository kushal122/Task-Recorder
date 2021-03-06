/*
Deployment script for TaskRecorder-DB

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "TaskRecorder-DB"
:setvar DefaultFilePrefix "TaskRecorder-DB"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Starting rebuilding table [dbo].[TaskRequest]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_TaskRequest] (
    [RequestId]   INT            IDENTITY (1, 1) NOT NULL,
    [TaskId]      NVARCHAR (100) NULL,
    [RequestDate] DATE           NULL,
    [DeadLine]    DATE           NULL,
    [Status]      NVARCHAR (MAX) NULL,
    [UserId]      NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([RequestId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[TaskRequest])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_TaskRequest] ON;
        INSERT INTO [dbo].[tmp_ms_xx_TaskRequest] ([RequestId], [TaskId], [RequestDate], [DeadLine], [Status], [UserId], [Description])
        SELECT   [RequestId],
                 [TaskId],
                 [RequestDate],
                 [DeadLine],
                 [Status],
                 [UserId],
                 [Description]
        FROM     [dbo].[TaskRequest]
        ORDER BY [RequestId] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_TaskRequest] OFF;
    END

DROP TABLE [dbo].[TaskRequest];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_TaskRequest]', N'TaskRequest';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[Tasks]...';


GO
CREATE TABLE [dbo].[Tasks] (
    [TaskId]       NVARCHAR (100) NOT NULL,
    [Title]        NVARCHAR (MAX) NULL,
    [Description]  NVARCHAR (MAX) NULL,
    [UploadedDate] DATE           NULL,
    [AssignedTo]   NVARCHAR (MAX) NULL,
    [DeadLine]     DATE           NULL,
    [Status]       NVARCHAR (MAX) NULL,
    [IsConfirmed]  BIT            NULL,
    [UserId]       NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([TaskId] ASC)
);


GO
PRINT N'Creating [dbo].[UserActivity]...';


GO
CREATE TABLE [dbo].[UserActivity] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Url]          NVARCHAR (MAX) NULL,
    [Data]         NVARCHAR (MAX) NULL,
    [UserName]     NVARCHAR (MAX) NULL,
    [IpAddress]    NVARCHAR (MAX) NULL,
    [ActivityDate] DATE           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[UserRoles]...';


GO
CREATE TABLE [dbo].[UserRoles] (
    [Id]     INT            NOT NULL,
    [RoleId] INT            NULL,
    [UserId] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Users]...';


GO
CREATE TABLE [dbo].[Users] (
    [UserId]      NVARCHAR (MAX) NOT NULL,
    [FirstName]   NVARCHAR (MAX) NULL,
    [LastName]    NVARCHAR (MAX) NULL,
    [Email]       NVARCHAR (MAX) NULL,
    [PhoneNumber] NVARCHAR (MAX) NULL,
    [Address]     NVARCHAR (MAX) NULL,
    [Password]    NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC)
);


GO
PRINT N'Update complete.';


GO
