-- Example migrations for FileIt Database
-- Reference these patterns when adding new changesets

-- ============================================================================
-- EXAMPLE 1: Add a new table with constraints
-- ============================================================================
--liquibase formatted sql

--changeset fileit:2-add-foreign-keys runOnChange:false
-- Add Foreign Key from HHF_Holdings to HHF_Holders
ALTER TABLE [dbo].[HHF_Holdings] 
ADD CONSTRAINT FK_HHF_Holdings_HolderId 
FOREIGN KEY ([HolderId]) 
REFERENCES [dbo].[HHF_Holders]([HolderId]);
--rollback ALTER TABLE [dbo].[HHF_Holdings] DROP CONSTRAINT FK_HHF_Holdings_HolderId;

-- ============================================================================
-- EXAMPLE 2: Add indexes for performance
-- ============================================================================
--changeset fileit:3-add-indexes runOnChange:false
-- Index on HHF_Holdings for query performance
CREATE INDEX IX_HHF_Holdings_HolderId 
ON [dbo].[HHF_Holdings]([HolderId]);
--rollback DROP INDEX IX_HHF_Holdings_HolderId ON [dbo].[HHF_Holdings];

-- Index on HHF_PresentValue for symbol lookups
CREATE INDEX IX_HHF_PresentValue_CusipOrSymbol 
ON [dbo].[HHF_PresentValue]([CusipOrSymbol]);
--rollback DROP INDEX IX_HHF_PresentValue_CusipOrSymbol ON [dbo].[HHF_PresentValue];

-- ============================================================================
-- EXAMPLE 3: Add a new column
-- ============================================================================
--changeset fileit:4-add-audit-columns runOnChange:false
-- Add audit columns to HHF_Holdings
ALTER TABLE [dbo].[HHF_Holdings]
ADD [CreatedBy] VARCHAR(100) NULL,
    [UpdatedBy] VARCHAR(100) NULL,
    [CreatedDate] DATETIME2 NULL DEFAULT GETUTCDATE(),
    [UpdatedDate] DATETIME2 NULL DEFAULT GETUTCDATE();
--rollback ALTER TABLE [dbo].[HHF_Holdings] 
--rollback DROP COLUMN [CreatedBy], [UpdatedBy], [CreatedDate], [UpdatedDate];

-- ============================================================================
-- EXAMPLE 4: Modify column data type (with safety precautions)
-- ============================================================================
--changeset fileit:5-increase-precision runOnChange:false
-- Increase precision of Quantity column for larger holdings
-- Note: This modifies an existing column - test thoroughly!
ALTER TABLE [dbo].[HHF_Holdings]
ALTER COLUMN [Quantity] DECIMAL(38, 10) NOT NULL;
--rollback ALTER TABLE [dbo].[HHF_Holdings]
--rollback ALTER COLUMN [Quantity] DECIMAL(38, 32) NOT NULL;

-- ============================================================================
-- EXAMPLE 5: Add a new table with seed data
-- ============================================================================
--changeset fileit:6-add-transaction-log-table runOnChange:false
CREATE TABLE [dbo].[HHF_TransactionLog]
(
    [TransactionId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [HolderId] VARCHAR(30) NOT NULL,
    [TransactionType] VARCHAR(50) NOT NULL, -- 'BUY', 'SELL', 'DIVIDEND', etc.
    [SecurityId] VARCHAR(50) NOT NULL,
    [Quantity] DECIMAL(38, 32) NOT NULL,
    [Price] MONEY NOT NULL,
    [TransactionDate] DATETIME NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_HHF_TransactionLog_HolderId 
        FOREIGN KEY ([HolderId]) 
        REFERENCES [dbo].[HHF_Holders]([HolderId])
);
--rollback DROP TABLE [dbo].[HHF_TransactionLog];

-- ============================================================================
-- EXAMPLE 6: Data migration (transform existing data)
-- ============================================================================
--changeset fileit:7-normalize-phone-numbers runOnChange:false context:data-cleanup
-- Normalize phone numbers (remove formatting, keep only digits)
UPDATE [dbo].[HHF_Holders]
SET [Zip] = REPLACE(REPLACE(REPLACE([Zip], '-', ''), ' ', ''), '.', '')
WHERE [Zip] IS NOT NULL;
--rollback -- No rollback for data normalization, manual review required

-- ============================================================================
-- EXAMPLE 7: Create a view
-- ============================================================================
--changeset fileit:8-add-holdings-summary-view runOnChange:true
CREATE OR ALTER VIEW [dbo].[vw_HoldingsSummary] AS
SELECT 
    h.[HolderId],
    ho.[CustomerName],
    COUNT(DISTINCT h.[CusipOrSymbol]) AS [SecurityCount],
    SUM(h.[Quantity]) AS [TotalQuantity],
    MAX(h.[AsOfDate]) AS [LastUpdated]
FROM [dbo].[HHF_Holdings] h
INNER JOIN [dbo].[HHF_Holders] ho ON h.[HolderId] = ho.[HolderId]
GROUP BY h.[HolderId], ho.[CustomerName];
--rollback DROP VIEW [dbo].[vw_HoldingsSummary];

-- ============================================================================
-- EXAMPLE 8: Conditional changeset (runs only in specific context)
-- ============================================================================
--changeset fileit:9-seed-test-data context:test-data runOnChange:false
-- Only runs when liquibase is executed with context=test-data
-- Usage: mvn liquibase:update -Dliquibase.contexts=test-data

INSERT INTO [dbo].[HHF_Holders] 
([HolderId], [CustomerName], [AccountNumber], [CreatedAt], [UpdatedAt])
VALUES 
('TEST001', 'Test Account 1', 'ACC001', GETDATE(), GETDATE()),
('TEST002', 'Test Account 2', 'ACC002', GETDATE(), GETDATE());

INSERT INTO [dbo].[HHF_Holdings]
([HolderId], [CusipOrSymbol], [Name], [Quantity], [AsOfDate])
VALUES
('TEST001', 'AAPL', 'Apple Inc.', 100.000000000000000000000000000000, GETDATE()),
('TEST001', 'MSFT', 'Microsoft Corp.', 50.000000000000000000000000000000, GETDATE());
--rollback DELETE FROM [dbo].[HHF_Holdings] WHERE [HolderId] LIKE 'TEST%';
--rollback DELETE FROM [dbo].[HHF_Holders] WHERE [HolderId] LIKE 'TEST%';

-- ============================================================================
-- GUIDELINES FOR WRITING CHANGESETS
-- ============================================================================
-- 1. Use meaningful changeset IDs: author:sequence-description
-- 2. Always include rollback statements (-- rollback)
-- 3. One logical change per changeset
-- 4. Test rollback before committing
-- 5. Use runOnChange:false for schema changes, true for views/functions
-- 6. Keep changesets idempotent when possible
-- 7. Use uppercase for SQL keywords for readability
-- 8. Include descriptive comments
-- 9. Use proper schema qualifiers ([dbo].[TableName])
-- 10. Test in development before production deployment
