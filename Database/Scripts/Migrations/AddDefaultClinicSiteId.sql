-- Migration: Add DefaultClinicSiteId column to Users table
-- Date: 2025-01-02
-- Description: Adds the DefaultClinicSiteId column to Users table and creates foreign key relationship

USE CareFusionWebDB;
GO

-- Add DefaultClinicSiteId column to Users table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'DefaultClinicSiteId')
BEGIN
    ALTER TABLE dbo.Users 
    ADD DefaultClinicSiteId int NULL;
    
    PRINT 'Added DefaultClinicSiteId column to Users table';
END
ELSE
BEGIN
    PRINT 'DefaultClinicSiteId column already exists in Users table';
END
GO

-- Create foreign key constraint if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_ClinicSites_DefaultClinicSiteId')
BEGIN
    ALTER TABLE dbo.Users
    ADD CONSTRAINT FK_Users_ClinicSites_DefaultClinicSiteId 
    FOREIGN KEY (DefaultClinicSiteId) 
    REFERENCES dbo.ClinicSites(Id);
    
    PRINT 'Added foreign key constraint FK_Users_ClinicSites_DefaultClinicSiteId';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint FK_Users_ClinicSites_DefaultClinicSiteId already exists';
END
GO

-- Update existing non-admin users to have a default clinic site (optional)
-- This assigns all non-admin users to the first available clinic site
UPDATE dbo.Users 
SET DefaultClinicSiteId = (SELECT TOP 1 Id FROM dbo.ClinicSites WHERE IsActive = 1)
WHERE Role != 'Admin' 
  AND DefaultClinicSiteId IS NULL
  AND EXISTS (SELECT 1 FROM dbo.ClinicSites WHERE IsActive = 1);

PRINT 'Migration completed successfully';
GO