-- Manual SQL script to add DefaultClinicSiteId column to Users table
-- This fixes the "Invalid column name 'DefaultClinicSiteId'" error

USE [CareFusionWebDB]
GO

-- Check if column already exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'DefaultClinicSiteId')
BEGIN
    -- Add the DefaultClinicSiteId column
    ALTER TABLE dbo.Users 
    ADD DefaultClinicSiteId int NULL;
    
    PRINT 'Added DefaultClinicSiteId column to Users table';
END
ELSE
BEGIN
    PRINT 'DefaultClinicSiteId column already exists in Users table';
END

-- Check if FirstName column exists and add if missing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'FirstName')
BEGIN
    ALTER TABLE dbo.Users 
    ADD FirstName nvarchar(100) NULL;
    
    PRINT 'Added FirstName column to Users table';
END

-- Check if LastName column exists and add if missing  
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'LastName')
BEGIN
    ALTER TABLE dbo.Users 
    ADD LastName nvarchar(100) NULL;
    
    PRINT 'Added LastName column to Users table';
END

-- Create foreign key constraint if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_ClinicSites_DefaultClinicSiteId')
BEGIN
    -- Only create foreign key if ClinicSites table exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ClinicSites')
    BEGIN
        ALTER TABLE dbo.Users
        ADD CONSTRAINT FK_Users_ClinicSites_DefaultClinicSiteId 
        FOREIGN KEY (DefaultClinicSiteId) 
        REFERENCES dbo.ClinicSites(Id);
        
        PRINT 'Added foreign key constraint FK_Users_ClinicSites_DefaultClinicSiteId';
    END
    ELSE
    BEGIN
        PRINT 'ClinicSites table does not exist, skipping foreign key creation';
    END
END
ELSE
BEGIN
    PRINT 'Foreign key constraint FK_Users_ClinicSites_DefaultClinicSiteId already exists';
END

-- Create index on DefaultClinicSiteId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'IX_Users_DefaultClinicSiteId')
BEGIN
    CREATE INDEX IX_Users_DefaultClinicSiteId ON dbo.Users(DefaultClinicSiteId);
    PRINT 'Created index IX_Users_DefaultClinicSiteId';
END
ELSE
BEGIN
    PRINT 'Index IX_Users_DefaultClinicSiteId already exists';
END

-- Update the __EFMigrationsHistory table to reflect the migration
IF NOT EXISTS (SELECT * FROM dbo.__EFMigrationsHistory WHERE MigrationId = '20250902000000_AddDefaultClinicSiteIdColumn')
BEGIN
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) 
    VALUES ('20250902000000_AddDefaultClinicSiteIdColumn', '9.0.8');
    
    PRINT 'Added migration record to __EFMigrationsHistory';
END
ELSE
BEGIN
    PRINT 'Migration record already exists in __EFMigrationsHistory';
END

PRINT 'Database schema update completed successfully!';
GO