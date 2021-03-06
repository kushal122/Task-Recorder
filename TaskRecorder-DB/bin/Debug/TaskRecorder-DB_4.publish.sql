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
PRINT N'Altering [dbo].[Roles]...';


GO
ALTER TABLE [dbo].[Roles] ALTER COLUMN [RoleName] NVARCHAR (MAX) NULL;


GO
PRINT N'Altering [dbo].[TaskRequest]...';


GO
ALTER TABLE [dbo].[TaskRequest] ALTER COLUMN [Description] NVARCHAR (MAX) NULL;

ALTER TABLE [dbo].[TaskRequest] ALTER COLUMN [Status] NVARCHAR (MAX) NULL;

ALTER TABLE [dbo].[TaskRequest] ALTER COLUMN [TaskId] NVARCHAR (MAX) NULL;

ALTER TABLE [dbo].[TaskRequest] ALTER COLUMN [UserId] NVARCHAR (MAX) NULL;


GO
PRINT N'Starting rebuilding table [dbo].[Tasks]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Tasks] (
    [TaskId]       NVARCHAR (MAX) NOT NULL,
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

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Tasks])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Tasks] ([TaskId], [Title], [Description], [UploadedDate], [AssignedTo], [DeadLine], [Status], [IsConfirmed], [UserId])
        SELECT   [TaskId],
                 [Title],
                 [Description],
                 [UploadedDate],
                 [AssignedTo],
                 [DeadLine],
                 [Status],
                 [IsConfirmed],
                 [UserId]
        FROM     [dbo].[Tasks]
        ORDER BY [TaskId] ASC;
    END

DROP TABLE [dbo].[Tasks];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Tasks]', N'Tasks';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Altering [dbo].[UserRoles]...';


GO
ALTER TABLE [dbo].[UserRoles] ALTER COLUMN [UserId] NVARCHAR (MAX) NULL;


GO
PRINT N'Starting rebuilding table [dbo].[Users]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Users] (
    [UserId]      NVARCHAR (MAX) NOT NULL,
    [FirstName]   NVARCHAR (MAX) NULL,
    [LastName]    NVARCHAR (MAX) NULL,
    [Email]       NVARCHAR (MAX) NULL,
    [PhoneNumber] NVARCHAR (MAX) NULL,
    [Address]     NVARCHAR (MAX) NULL,
    [Password]    NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Users])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Users] ([UserId], [FirstName], [LastName], [Email], [PhoneNumber], [Address], [Password])
        SELECT   [UserId],
                 [FirstName],
                 [LastName],
                 [Email],
                 [PhoneNumber],
                 [Address],
                 [Password]
        FROM     [dbo].[Users]
        ORDER BY [UserId] ASC;
    END

DROP TABLE [dbo].[Users];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Users]', N'Users';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Update complete.';


GO
