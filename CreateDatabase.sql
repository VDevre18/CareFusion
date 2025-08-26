-- CareFusion Database Creation Script
-- Run this script in SQL Server Management Studio or similar tool

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CareFusionWebDB')
BEGIN
    CREATE DATABASE CareFusionWebDB;
END
GO

USE CareFusionWebDB;
GO

-- Create AuditLogs table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [EntityName] [nvarchar](100) NOT NULL,
        [EntityId] [uniqueidentifier] NOT NULL,
        [Action] [nvarchar](50) NOT NULL,
        [TimestampUtc] [datetime2](7) NOT NULL,
        [User] [nvarchar](256) NULL,
        [ChangesJson] [nvarchar](max) NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityName_EntityId_TimestampUtc] 
    ON [dbo].[AuditLogs] ([EntityName], [EntityId], [TimestampUtc]);
END
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users](
        [Id] [uniqueidentifier] NOT NULL,
        [CreatedAtUtc] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](max) NULL,
        [ModifiedAtUtc] [datetime2](7) NULL,
        [ModifiedBy] [nvarchar](max) NULL,
        [IsDeleted] [bit] NOT NULL,
        [RowVersion] [rowversion] NOT NULL,
        [Username] [nvarchar](100) NOT NULL,
        [Email] [nvarchar](256) NOT NULL,
        [PasswordHash] [nvarchar](256) NOT NULL,
        [Role] [nvarchar](50) NOT NULL,
        [IsActive] [bit] NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
END
GO

-- Create Patients table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Patients]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Patients](
        [Id] [uniqueidentifier] NOT NULL,
        [CreatedAtUtc] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](max) NULL,
        [ModifiedAtUtc] [datetime2](7) NULL,
        [ModifiedBy] [nvarchar](max) NULL,
        [IsDeleted] [bit] NOT NULL,
        [RowVersion] [rowversion] NOT NULL,
        [FirstName] [nvarchar](100) NOT NULL,
        [LastName] [nvarchar](100) NOT NULL,
        [MRN] [nvarchar](50) NULL,
        [DateOfBirth] [datetime2](7) NULL,
        [Gender] [nvarchar](25) NULL,
        CONSTRAINT [PK_Patients] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Patients_MRN] 
    ON [dbo].[Patients] ([MRN]) WHERE [MRN] IS NOT NULL;
END
GO

-- Create Exams table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exams]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Exams](
        [Id] [uniqueidentifier] NOT NULL,
        [CreatedAtUtc] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](max) NULL,
        [ModifiedAtUtc] [datetime2](7) NULL,
        [ModifiedBy] [nvarchar](max) NULL,
        [IsDeleted] [bit] NOT NULL,
        [RowVersion] [rowversion] NOT NULL,
        [PatientId] [uniqueidentifier] NOT NULL,
        [Modality] [nvarchar](100) NOT NULL,
        [StudyType] [nvarchar](100) NOT NULL,
        [StudyDateUtc] [datetime2](7) NOT NULL,
        [StorageUri] [nvarchar](500) NULL,
        [StorageKey] [nvarchar](256) NULL,
        [Status] [nvarchar](50) NOT NULL DEFAULT 'New',
        CONSTRAINT [PK_Exams] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Exams_Patients_PatientId] FOREIGN KEY([PatientId]) REFERENCES [dbo].[Patients] ([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Exams_PatientId_StudyDateUtc] 
    ON [dbo].[Exams] ([PatientId], [StudyDateUtc]);
END
GO

-- Insert sample admin user
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE Username = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] ([Id], [CreatedAtUtc], [Username], [Email], [PasswordHash], [Role], [IsActive], [IsDeleted])
    VALUES (
        NEWID(),
        GETUTCDATE(),
        'admin',
        'admin@carefusion.local',
        'AQAAAAIAAYagAAAAECcOpq6FGg7mNY7VWf0CGYJiWwTJdU0dA0l8OkzO4/R4YdFE7gXJqH7CiAkKnx7SdQ==', -- Password: admin
        'Admin',
        1,
        0
    );
END
GO

-- Insert sample patients for testing
IF NOT EXISTS (SELECT * FROM [dbo].[Patients])
BEGIN
    DECLARE @Patient1Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @Patient2Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @Patient3Id UNIQUEIDENTIFIER = NEWID();

    INSERT INTO [dbo].[Patients] ([Id], [CreatedAtUtc], [FirstName], [LastName], [MRN], [DateOfBirth], [Gender], [IsDeleted])
    VALUES 
        (@Patient1Id, GETUTCDATE(), 'John', 'Doe', 'MRN001', '1985-05-15', 'Male', 0),
        (@Patient2Id, GETUTCDATE(), 'Jane', 'Smith', 'MRN002', '1992-08-22', 'Female', 0),
        (@Patient3Id, GETUTCDATE(), 'Robert', 'Johnson', 'MRN003', '1978-12-03', 'Male', 0);

    -- Insert sample exams
    INSERT INTO [dbo].[Exams] ([Id], [CreatedAtUtc], [PatientId], [Modality], [StudyType], [StudyDateUtc], [Status], [IsDeleted])
    VALUES 
        (NEWID(), GETUTCDATE(), @Patient1Id, 'CT', 'Chest CT', DATEADD(day, -5, GETUTCDATE()), 'Completed', 0),
        (NEWID(), GETUTCDATE(), @Patient1Id, 'MRI', 'Brain MRI', DATEADD(day, -2, GETUTCDATE()), 'In Progress', 0),
        (NEWID(), GETUTCDATE(), @Patient2Id, 'X-Ray', 'Chest X-Ray', DATEADD(day, -1, GETUTCDATE()), 'New', 0),
        (NEWID(), GETUTCDATE(), @Patient3Id, 'Ultrasound', 'Abdominal US', GETUTCDATE(), 'New', 0);
END
GO

PRINT 'Database CareFusionWebDB created successfully with sample data!';