CREATE TABLE [dbo].[HHF_Holdings]
(
    [Id]            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [HolderId]      VARCHAR(30)       NOT NULL,
    [CusipOrSymbol] VARCHAR(50)       NOT NULL, 
    [Name]          VARCHAR(200)      NULL, 
    [Quantity]      DECIMAL(28, 14)   NOT NULL, 
    [AsOfDate]      DATETIME          NOT NULL DEFAULT (CAST(CAST(GETDATE() AS DATE) AS DATETIME)) 
)
