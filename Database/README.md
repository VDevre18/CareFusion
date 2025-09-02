# CareFusion Database Scripts

This directory contains all database-related scripts for the CareFusion application.

## Directory Structure

### `/Scripts/Setup`
Initial database setup scripts:
- `CreateDatabase.sql` - Creates the main CareFusion database
- `CreateClinicSitesTable.sql` - Creates the ClinicSites table
- `CreateTables.sql` - Creates all application tables

### `/Scripts/Migrations`
Database migration scripts (in chronological order):
- `AddClinicSiteIdColumn.sql` - Adds clinic site references to tables
- `AddExamStorageColumns.sql` - Adds storage-related columns to Exam table
- `AddDefaultClinicSiteId.sql` - Adds DefaultClinicSiteId to Users table

### `/Scripts/Maintenance`
Database maintenance and fix scripts:
- `DropRecreateDatabase.sql` - Completely recreates the database (USE WITH CAUTION)
- `FixAuditTable.sql` - Fixes issues with audit logging table
- `FixEntityColumns.sql` - Fixes column definitions in entities
- `AlterTables.sql` - General table alterations

### `/Schemas`
Database schema documentation and references.

## Usage Instructions

### For Initial Setup:
1. Run scripts in `/Scripts/Setup` in the following order:
   - CreateDatabase.sql
   - CreateTables.sql
   - CreateClinicSitesTable.sql

### For Migrations:
1. Run migration scripts in chronological order
2. Always backup your database before running migrations
3. Test migrations in a development environment first

### Important Notes:
- Always review scripts before execution
- Create database backups before running maintenance scripts
- Migration scripts include safety checks to prevent duplicate executions
- Scripts are designed to be idempotent where possible

## Entity Framework Migrations

This project also uses Entity Framework Core migrations in the `/CareFusion.Core/Migrations` directory. These should be kept in sync with manual SQL scripts when possible.