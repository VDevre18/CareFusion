-- Create ClinicSites table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ClinicSites' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[ClinicSites] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL DEFAULT 0,
        [RowVersion] rowversion NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Address] nvarchar(200) NULL,
        [City] nvarchar(100) NULL,
        [State] nvarchar(100) NULL,
        [PostalCode] nvarchar(20) NULL,
        [Country] nvarchar(100) NULL,
        [Phone] nvarchar(20) NULL,
        [Email] nvarchar(256) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [Description] nvarchar(500) NULL,
        CONSTRAINT [PK_ClinicSites] PRIMARY KEY ([Id])
    );

    -- Create indexes
    CREATE UNIQUE INDEX [IX_ClinicSites_Code] ON [ClinicSites] ([Code]);
    CREATE INDEX [IX_ClinicSites_Name] ON [ClinicSites] ([Name]);

    PRINT 'ClinicSites table created successfully';
END
ELSE
BEGIN
    PRINT 'ClinicSites table already exists';
END

-- Insert the ClinicSites migration record
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250829000000_AddClinicSites')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250829000000_AddClinicSites', '9.0.8');
    PRINT 'Migration record added';
END