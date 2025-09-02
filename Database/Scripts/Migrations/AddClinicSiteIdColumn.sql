-- Add ClinicSiteId column to Patients table
-- Run this script if the EF migration fails

-- Check if column already exists
IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Patients]') 
    AND name = 'ClinicSiteId'
)
BEGIN
    -- Add the column
    ALTER TABLE [Patients] 
    ADD [ClinicSiteId] int NULL;
    
    -- Add foreign key constraint
    ALTER TABLE [Patients] 
    ADD CONSTRAINT [FK_Patients_ClinicSites_ClinicSiteId] 
    FOREIGN KEY ([ClinicSiteId]) REFERENCES [ClinicSites] ([Id]) 
    ON DELETE SET NULL;
    
    -- Add index
    CREATE NONCLUSTERED INDEX [IX_Patients_ClinicSiteId] 
    ON [Patients] ([ClinicSiteId]);
    
    PRINT 'ClinicSiteId column added successfully to Patients table';
END
ELSE
BEGIN
    PRINT 'ClinicSiteId column already exists in Patients table';
END