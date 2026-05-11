CREATE TABLE [dbo].[HHF_Holders]
(
	[HolderId]      VARCHAR(30) NOT NULL PRIMARY KEY,
    [CustomerName]  NVARCHAR(200) NOT NULL, 
    [AccountNumber] VARCHAR(50) NOT NULL, 
    [Address]       VARCHAR(200) NULL, 
    [City]          VARCHAR(100) NULL, 
    [State]         VARCHAR(50) NULL, 
    [Zip]           VARCHAR(20) NULL, 
    [CreatedAt]     DATETIME NOT NULL,
    [AccountClosed] DATETIME NULL,
    [UpdatedAt]     DATETIME NOT NULL
)
