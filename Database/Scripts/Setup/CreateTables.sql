USE CareFusionWebDB;

-- Create AuditLogs table
CREATE TABLE [AuditLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [EntityName] nvarchar(100) NOT NULL,
    [EntityId] nvarchar(50) NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [Changes] nvarchar(max) NULL,
    [ChangedBy] nvarchar(100) NOT NULL,
    [ChangedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);

-- Create Users table  
CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Username] nvarchar(100) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

-- Create Patients table
CREATE TABLE [Patients] (
    [Id] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [MRN] nvarchar(20) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(10) NULL,
    [Phone] nvarchar(20) NULL,
    [Email] nvarchar(256) NULL,
    [Address] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([Id])
);

-- Create Exams table
CREATE TABLE [Exams] (
    [Id] uniqueidentifier NOT NULL,
    [PatientId] uniqueidentifier NOT NULL,
    [Modality] nvarchar(10) NOT NULL,
    [StudyType] nvarchar(100) NOT NULL,
    [StudyDateUtc] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Notes] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Exams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Exams_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([Id]) ON DELETE CASCADE
);

-- Create indexes
CREATE UNIQUE INDEX [IX_Patients_MRN] ON [Patients] ([MRN]);
CREATE INDEX [IX_Exams_PatientId] ON [Exams] ([PatientId]);

-- Insert default admin user
INSERT INTO [Users] ([Id], [Username], [Email], [PasswordHash], [IsActive], [CreatedAt])
VALUES (
    NEWID(),
    'admin@carefusion.com',
    'admin@carefusion.com',
    'AQAAAAIAAYagAAAAEHl2YRnBVjm3e7kKJVWvBOQ8RNz5X5x5W3k5W3k5W3k5W3k5',
    1,
    GETUTCDATE()
);

PRINT 'Database tables created successfully!';