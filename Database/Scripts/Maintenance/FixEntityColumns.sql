USE CareFusionWebDB;

-- Drop old columns that conflict with Entity Framework columns
ALTER TABLE [Users] DROP COLUMN [CreatedAt];
ALTER TABLE [Users] DROP COLUMN [UpdatedAt];
ALTER TABLE [Patients] DROP COLUMN [CreatedAt];
ALTER TABLE [Patients] DROP COLUMN [UpdatedAt]; 
ALTER TABLE [Exams] DROP COLUMN [CreatedAt];
ALTER TABLE [Exams] DROP COLUMN [UpdatedAt];

PRINT 'Old timestamp columns removed successfully!';