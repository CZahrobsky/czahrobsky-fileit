CREATE TABLE [dbo].[HHF_PresentValue]
(
    [Id]                  INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CusipOrSymbol]       VARCHAR(50)       NOT NULL, 
    [UnitPrice]           MONEY             NOT NULL, 
    [DividendMultiple]    DECIMAL(28, 14)   NOT NULL, 
    [SplitMultiple]       DECIMAL(28, 14)   NOT NULL, 
    [CumulativeSplits]    DECIMAL(28, 14)   NOT NULL, 
    [RiskScalar]          DECIMAL(28, 14)   NULL, 
    [AsOfDate]            DATETIME          NOT NULL DEFAULT (CAST(CAST(GETDATE() AS DATE) AS DATETIME)) 
)
