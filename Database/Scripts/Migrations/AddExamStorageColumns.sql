USE CareFusionWebDB;

-- Add missing storage columns to Exams table
ALTER TABLE [Exams] ADD 
    [StorageUri] nvarchar(500) NULL,
    [StorageKey] nvarchar(256) NULL;

PRINT 'Exam storage columns added successfully!';