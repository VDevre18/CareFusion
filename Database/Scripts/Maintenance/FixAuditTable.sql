USE CareFusionWebDB;

-- Drop and recreate AuditLogs table with correct column names
DROP TABLE IF EXISTS [AuditLogs];

CREATE TABLE [AuditLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [EntityName] nvarchar(100) NOT NULL,
    [EntityId] uniqueidentifier NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [TimestampUtc] datetime2 NOT NULL,
    [User] nvarchar(256) NULL,
    [ChangesJson] nvarchar(max) NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);

PRINT 'AuditLogs table recreated successfully!';