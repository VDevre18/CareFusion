-- Drop existing database and recreate with correct int-based schema
USE master;
GO

-- Drop the database if it exists
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'CareFusionWebDB')
BEGIN
    ALTER DATABASE CareFusionWebDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CareFusionWebDB;
END
GO

-- Create new database
CREATE DATABASE CareFusionWebDB;
GO

USE CareFusionWebDB;
GO

-- Create the migration history table
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

-- Insert the migration record to mark it as applied
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250828000000_InitialCreate', N'9.0.8');
GO

-- Create AuditLogs table
CREATE TABLE [AuditLogs] (
    [Id] bigint NOT NULL IDENTITY(1,1),
    [EntityName] nvarchar(100) NOT NULL,
    [EntityId] nvarchar(max) NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [TimestampUtc] datetime2 NOT NULL,
    [User] nvarchar(256) NULL,
    [ChangesJson] nvarchar(max) NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);
GO

-- Create Users table with INT identity
CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [Username] nvarchar(100) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [Role] nvarchar(50) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

-- Create Patients table with INT identity
CREATE TABLE [Patients] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [MRN] nvarchar(50) NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(25) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([Id])
);
GO

-- Create Exams table with INT identity
CREATE TABLE [Exams] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [PatientId] int NOT NULL,
    [Modality] nvarchar(100) NOT NULL,
    [StudyType] nvarchar(100) NOT NULL,
    [StudyDateUtc] datetime2 NOT NULL,
    [StorageUri] nvarchar(500) NULL,
    [StorageKey] nvarchar(256) NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT 'New',
    CONSTRAINT [PK_Exams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Exams_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([Id]) ON DELETE CASCADE
);
GO

-- Create PatientNotes table with INT identity
CREATE TABLE [PatientNotes] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [PatientId] int NOT NULL,
    [NoteType] nvarchar(50) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [AuthorName] nvarchar(100) NOT NULL,
    [NoteDate] datetime2 NOT NULL,
    CONSTRAINT [PK_PatientNotes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientNotes_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([Id]) ON DELETE CASCADE
);
GO

-- Create PatientReports table with INT identity
CREATE TABLE [PatientReports] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [PatientId] int NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [ReportType] nvarchar(50) NOT NULL,
    [FileSizeBytes] bigint NOT NULL,
    [ContentType] nvarchar(100) NULL,
    [FilePath] nvarchar(500) NULL,
    CONSTRAINT [PK_PatientReports] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PatientReports_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([Id]) ON DELETE CASCADE
);
GO

-- Create ExamImages table with INT identity
CREATE TABLE [ExamImages] (
    [Id] int NOT NULL IDENTITY(1,1),
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [ExamId] int NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [FileSizeBytes] bigint NOT NULL,
    [Width] int NULL,
    [Height] int NULL,
    [FilePath] nvarchar(500) NULL,
    [ThumbnailPath] nvarchar(500) NULL,
    CONSTRAINT [PK_ExamImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamImages_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE CASCADE
);
GO

-- Create indexes
CREATE INDEX [IX_AuditLogs_EntityName_EntityId_TimestampUtc] ON [AuditLogs] ([EntityName], [EntityId], [TimestampUtc]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO

CREATE UNIQUE INDEX [IX_Patients_MRN] ON [Patients] ([MRN]) WHERE [MRN] IS NOT NULL;
GO

CREATE INDEX [IX_Exams_PatientId_StudyDateUtc] ON [Exams] ([PatientId], [StudyDateUtc]);
GO

CREATE INDEX [IX_PatientNotes_PatientId_NoteDate] ON [PatientNotes] ([PatientId], [NoteDate]);
GO

CREATE INDEX [IX_PatientReports_PatientId_ReportType] ON [PatientReports] ([PatientId], [ReportType]);
GO

CREATE INDEX [IX_ExamImages_ExamId] ON [ExamImages] ([ExamId]);
GO

PRINT 'Database recreated successfully with INT-based schema';