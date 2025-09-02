USE CareFusionWebDB;

-- Add missing columns to Users table
ALTER TABLE [Users] ADD 
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [CreatedAtUtc] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] nvarchar(100) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(100) NULL,
    [RowVersion] rowversion NOT NULL;

-- Add missing columns to Patients table  
ALTER TABLE [Patients] ADD
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [CreatedAtUtc] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] nvarchar(100) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(100) NULL,
    [RowVersion] rowversion NOT NULL;

-- Add missing columns to Exams table
ALTER TABLE [Exams] ADD
    [IsDeleted] bit NOT NULL DEFAULT 0,
    [CreatedAtUtc] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] nvarchar(100) NULL,
    [ModifiedAtUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(100) NULL,
    [RowVersion] rowversion NOT NULL;

PRINT 'Database tables updated successfully!';