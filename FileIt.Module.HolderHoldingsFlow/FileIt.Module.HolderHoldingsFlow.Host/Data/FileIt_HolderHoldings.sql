/* FileIt + HolderHoldings DB User and Table creation script */

USE [master]
GO
/****** Object:  Database [FileIt]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'FileIt')
BEGIN
CREATE DATABASE [FileIt]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FileIt', FILENAME = N'D:\Source\SQL\MSSQL17.MSSQL.SERVER\MSSQL\DATA\FileIt.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'FileIt_log', FILENAME = N'D:\Source\SQL\MSSQL17.MSSQL.SERVER\MSSQL\DATA\FileIt_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
END
GO
ALTER DATABASE [FileIt] SET COMPATIBILITY_LEVEL = 170
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FileIt].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FileIt] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FileIt] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FileIt] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FileIt] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FileIt] SET ARITHABORT OFF 
GO
ALTER DATABASE [FileIt] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FileIt] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FileIt] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FileIt] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FileIt] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FileIt] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FileIt] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FileIt] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FileIt] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FileIt] SET  DISABLE_BROKER 
GO
ALTER DATABASE [FileIt] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FileIt] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FileIt] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FileIt] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FileIt] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FileIt] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FileIt] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FileIt] SET RECOVERY FULL 
GO
ALTER DATABASE [FileIt] SET  MULTI_USER 
GO
ALTER DATABASE [FileIt] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FileIt] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FileIt] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FileIt] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FileIt] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FileIt] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [FileIt] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [FileIt] SET QUERY_STORE = ON
GO
ALTER DATABASE [FileIt] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [FileIt]
GO
/****** Object:  User [FileItDev]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'FileItDev')
CREATE USER [FileItDev] FOR LOGIN [FileItDev] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [FileItDev]
GO
/****** Object:  Table [dbo].[ApiLog]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ApiLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ApiLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientRequestId] [nvarchar](100) NOT NULL,
	[RequestBody] [nvarchar](100) NULL,
	[ResponseBody] [nvarchar](100) NULL,
	[Status] [nvarchar](100) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[CommonLog]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommonLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CommonLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[MessageTemplate] [nvarchar](max) NULL,
	[Level] [nvarchar](100) NULL,
	[Exception] [nvarchar](max) NULL,
	[Properties] [nvarchar](max) NULL,
	[Environment] [nvarchar](100) NULL,
	[MachineName] [nvarchar](100) NULL,
	[Application] [nvarchar](100) NULL,
	[ApplicationVersion] [nvarchar](100) NULL,
	[InfrastructureVersion] [nvarchar](100) NULL,
	[SourceContext] [nvarchar](100) NULL,
	[CorrelationId] [nvarchar](100) NULL,
	[InvocationId] [nvarchar](100) NULL,
	[EventName] [nvarchar](100) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ComplexDocument]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ComplexDocument]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ComplexDocument](
	[DocumentId] [bigint] IDENTITY(1,1) NOT NULL,
	[PublicId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](260) NOT NULL,
	[ContentType] [nvarchar](128) NOT NULL,
	[SizeBytes] [bigint] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[CreatedUtc] [datetime2](7) NOT NULL,
	[ModifiedUtc] [datetime2](7) NOT NULL,
	[DeletedUtc] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[Version] [timestamp] NOT NULL,
 CONSTRAINT [PK_ComplexDocument] PRIMARY KEY CLUSTERED 
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ComplexDocument_PublicId] UNIQUE NONCLUSTERED 
(
	[PublicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ComplexIdempotency]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ComplexIdempotency]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ComplexIdempotency](
	[IdempotencyId] [bigint] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](128) NOT NULL,
	[RequestHash] [char](64) NOT NULL,
	[ResponseStatusCode] [int] NOT NULL,
	[ResponseBody] [nvarchar](max) NULL,
	[ResponseLocation] [nvarchar](2048) NULL,
	[CreatedUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ComplexIdempotency] PRIMARY KEY CLUSTERED 
(
	[IdempotencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ComplexIdempotency_Key] UNIQUE NONCLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DataFlowRequestLog]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataFlowRequestLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataFlowRequestLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Environment] [nvarchar](100) NOT NULL,
	[Host] [nvarchar](100) NOT NULL,
	[Agent] [nvarchar](100) NOT NULL,
	[BlobName] [nvarchar](500) NOT NULL,
	[ClientRequestId] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](500) NULL,
	[RowsIngested] [int] NOT NULL,
	[RowsTransformed] [int] NOT NULL,
	[ExportBlobName] [nvarchar](500) NULL,
	[Status] [nvarchar](100) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DeadLetterRecord]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DeadLetterRecord](
	[DeadLetterRecordId] [bigint] IDENTITY(1,1) NOT NULL,
	[MessageId] [nvarchar](128) NOT NULL,
	[CorrelationId] [nvarchar](128) NULL,
	[SessionId] [nvarchar](128) NULL,
	[SourceEntityType] [nvarchar](16) NOT NULL,
	[SourceEntityName] [nvarchar](260) NOT NULL,
	[SourceSubscriptionName] [nvarchar](260) NULL,
	[DeadLetterReason] [nvarchar](260) NULL,
	[DeadLetterErrorDescription] [nvarchar](max) NULL,
	[DeliveryCount] [int] NOT NULL,
	[EnqueuedTimeUtc] [datetime2](7) NOT NULL,
	[DeadLetteredTimeUtc] [datetime2](7) NOT NULL,
	[FailureCategory] [nvarchar](32) NOT NULL,
	[MessageBody] [nvarchar](max) NOT NULL,
	[MessageProperties] [nvarchar](max) NULL,
	[ContentType] [nvarchar](128) NULL,
	[Status] [nvarchar](32) NOT NULL,
	[StatusUpdatedUtc] [datetime2](7) NOT NULL,
	[StatusUpdatedBy] [nvarchar](128) NULL,
	[ReplayAttemptCount] [int] NOT NULL,
	[LastReplayAttemptUtc] [datetime2](7) NULL,
	[LastReplayMessageId] [nvarchar](128) NULL,
	[ResolutionNotes] [nvarchar](max) NULL,
	[CreatedUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DeadLetterRecord] PRIMARY KEY CLUSTERED 
(
	[DeadLetterRecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[HHF_Holders]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HHF_Holders]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HHF_Holders](
	[HolderId] [varchar](30) NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[AccountNumber] [varchar](50) NOT NULL,
	[Address] [varchar](200) NULL,
	[City] [varchar](100) NULL,
	[State] [varchar](50) NULL,
	[Zip] [varchar](20) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[AccountClosed] [datetime] NULL,
	[UpdatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[HolderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[HHF_Holdings]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HHF_Holdings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HHF_Holdings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HolderId] [varchar](30) NOT NULL,
	[CusipOrSymbol] [varchar](50) NOT NULL,
	[Name] [varchar](200) NULL,
	[Quantity] [decimal](28, 14) NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[HHF_PresentValue]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HHF_PresentValue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HHF_PresentValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CusipOrSymbol] [varchar](50) NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[DividendMultiple] [decimal](28, 14) NOT NULL,
	[SplitMultiple] [decimal](28, 14) NOT NULL,
	[CumulativeSplits] [decimal](28, 14) NOT NULL,
	[RiskScalar] [decimal](28, 14) NULL,
	[AsOfDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[HHF_Transactions]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HHF_Transactions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HHF_Transactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionType] [varchar](100) NOT NULL,
	[HolderId] [varchar](30) NOT NULL,
	[CusipOrSymbol] [varchar](50) NOT NULL,
	[Name] [varchar](200) NULL,
	[Quantity] [decimal](28, 14) NOT NULL,
	[AsOfDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[SimpleRequestLog]    Script Date: 6/4/2026 5:19:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SimpleRequestLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SimpleRequestLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Environment] [nvarchar](100) NOT NULL,
	[Host] [nvarchar](100) NOT NULL,
	[Agent] [nvarchar](100) NOT NULL,
	[BlobName] [nvarchar](100) NOT NULL,
	[ClientRequestId] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](100) NULL,
	[ApiId] [int] NOT NULL,
	[Status] [nvarchar](100) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CommonLog_EventName]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CommonLog]') AND name = N'IX_CommonLog_EventName')
CREATE NONCLUSTERED INDEX [IX_CommonLog_EventName] ON [dbo].[CommonLog]
(
	[EventName] ASC
)
WHERE ([EventName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ComplexDocument_DeletedUtc_ModifiedUtc]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ComplexDocument]') AND name = N'IX_ComplexDocument_DeletedUtc_ModifiedUtc')
CREATE NONCLUSTERED INDEX [IX_ComplexDocument_DeletedUtc_ModifiedUtc] ON [dbo].[ComplexDocument]
(
	[DeletedUtc] ASC,
	[ModifiedUtc] DESC
)
INCLUDE([Name],[ContentType],[SizeBytes]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ComplexDocument_Name]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ComplexDocument]') AND name = N'IX_ComplexDocument_Name')
CREATE NONCLUSTERED INDEX [IX_ComplexDocument_Name] ON [dbo].[ComplexDocument]
(
	[Name] ASC
)
INCLUDE([DeletedUtc],[ModifiedUtc]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ComplexIdempotency_CreatedUtc]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ComplexIdempotency]') AND name = N'IX_ComplexIdempotency_CreatedUtc')
CREATE NONCLUSTERED INDEX [IX_ComplexIdempotency_CreatedUtc] ON [dbo].[ComplexIdempotency]
(
	[CreatedUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DeadLetterRecord_CorrelationId]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]') AND name = N'IX_DeadLetterRecord_CorrelationId')
CREATE NONCLUSTERED INDEX [IX_DeadLetterRecord_CorrelationId] ON [dbo].[DeadLetterRecord]
(
	[CorrelationId] ASC
)
INCLUDE([SourceEntityName],[Status],[DeadLetteredTimeUtc]) 
WHERE ([CorrelationId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DeadLetterRecord_FailureCategory_DeadLetteredTimeUtc]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]') AND name = N'IX_DeadLetterRecord_FailureCategory_DeadLetteredTimeUtc')
CREATE NONCLUSTERED INDEX [IX_DeadLetterRecord_FailureCategory_DeadLetteredTimeUtc] ON [dbo].[DeadLetterRecord]
(
	[FailureCategory] ASC,
	[DeadLetteredTimeUtc] DESC
)
INCLUDE([SourceEntityName],[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DeadLetterRecord_MessageId_Source_DeadLetteredTime]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]') AND name = N'IX_DeadLetterRecord_MessageId_Source_DeadLetteredTime')
CREATE UNIQUE NONCLUSTERED INDEX [IX_DeadLetterRecord_MessageId_Source_DeadLetteredTime] ON [dbo].[DeadLetterRecord]
(
	[MessageId] ASC,
	[SourceEntityName] ASC,
	[DeadLetteredTimeUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DeadLetterRecord_SourceEntityName_DeadLetteredTimeUtc]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]') AND name = N'IX_DeadLetterRecord_SourceEntityName_DeadLetteredTimeUtc')
CREATE NONCLUSTERED INDEX [IX_DeadLetterRecord_SourceEntityName_DeadLetteredTimeUtc] ON [dbo].[DeadLetterRecord]
(
	[SourceEntityName] ASC,
	[DeadLetteredTimeUtc] DESC
)
INCLUDE([Status],[FailureCategory],[CorrelationId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_DeadLetterRecord_Status_StatusUpdatedUtc]    Script Date: 6/4/2026 5:19:58 AM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]') AND name = N'IX_DeadLetterRecord_Status_StatusUpdatedUtc')
CREATE NONCLUSTERED INDEX [IX_DeadLetterRecord_Status_StatusUpdatedUtc] ON [dbo].[DeadLetterRecord]
(
	[Status] ASC,
	[StatusUpdatedUtc] DESC
)
INCLUDE([SourceEntityName],[FailureCategory],[CorrelationId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ComplexDocument_PublicId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ComplexDocument] ADD  CONSTRAINT [DF_ComplexDocument_PublicId]  DEFAULT (newid()) FOR [PublicId]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ComplexDocument_ContentType]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ComplexDocument] ADD  CONSTRAINT [DF_ComplexDocument_ContentType]  DEFAULT ('text/plain') FOR [ContentType]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ComplexDocument_SizeBytes]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ComplexDocument] ADD  CONSTRAINT [DF_ComplexDocument_SizeBytes]  DEFAULT ((0)) FOR [SizeBytes]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ComplexDocument_CreatedUtc]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ComplexDocument] ADD  CONSTRAINT [DF_ComplexDocument_CreatedUtc]  DEFAULT (sysutcdatetime()) FOR [CreatedUtc]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ComplexDocument_ModifiedUtc]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ComplexDocument] ADD  CONSTRAINT [DF_ComplexDocument_ModifiedUtc]  DEFAULT (sysutcdatetime()) FOR [ModifiedUtc]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ComplexDocument_CreatedBy]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ComplexDocument] ADD  CONSTRAINT [DF_ComplexDocument_CreatedBy]  DEFAULT ('system') FOR [CreatedBy]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ComplexIdempotency_CreatedUtc]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ComplexIdempotency] ADD  CONSTRAINT [DF_ComplexIdempotency_CreatedUtc]  DEFAULT (sysutcdatetime()) FOR [CreatedUtc]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeadLetterRecord_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeadLetterRecord] ADD  CONSTRAINT [DF_DeadLetterRecord_Status]  DEFAULT ('New') FOR [Status]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeadLetterRecord_StatusUpdatedUtc]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeadLetterRecord] ADD  CONSTRAINT [DF_DeadLetterRecord_StatusUpdatedUtc]  DEFAULT (sysutcdatetime()) FOR [StatusUpdatedUtc]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeadLetterRecord_ReplayAttemptCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeadLetterRecord] ADD  CONSTRAINT [DF_DeadLetterRecord_ReplayAttemptCount]  DEFAULT ((0)) FOR [ReplayAttemptCount]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DeadLetterRecord_CreatedUtc]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DeadLetterRecord] ADD  CONSTRAINT [DF_DeadLetterRecord_CreatedUtc]  DEFAULT (sysutcdatetime()) FOR [CreatedUtc]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__HHF_Holdi__AsOfD__01142BA1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HHF_Holdings] ADD  DEFAULT (CONVERT([datetime],CONVERT([date],getdate()))) FOR [AsOfDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__HHF_Prese__AsOfD__03F0984C]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HHF_PresentValue] ADD  DEFAULT (CONVERT([datetime],CONVERT([date],getdate()))) FOR [AsOfDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__HHF_Trans__AsOfD__06CD04F7]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[HHF_Transactions] ADD  DEFAULT (CONVERT([datetime],CONVERT([date],getdate()))) FOR [AsOfDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_ComplexDocument_SizeBytes_NonNegative]') AND parent_object_id = OBJECT_ID(N'[dbo].[ComplexDocument]'))
ALTER TABLE [dbo].[ComplexDocument]  WITH CHECK ADD  CONSTRAINT [CK_ComplexDocument_SizeBytes_NonNegative] CHECK  (([SizeBytes]>=(0)))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_ComplexDocument_SizeBytes_NonNegative]') AND parent_object_id = OBJECT_ID(N'[dbo].[ComplexDocument]'))
ALTER TABLE [dbo].[ComplexDocument] CHECK CONSTRAINT [CK_ComplexDocument_SizeBytes_NonNegative]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_DeliveryCount_NonNegative]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord]  WITH CHECK ADD  CONSTRAINT [CK_DeadLetterRecord_DeliveryCount_NonNegative] CHECK  (([DeliveryCount]>=(0)))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_DeliveryCount_NonNegative]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord] CHECK CONSTRAINT [CK_DeadLetterRecord_DeliveryCount_NonNegative]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_FailureCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord]  WITH CHECK ADD  CONSTRAINT [CK_DeadLetterRecord_FailureCategory] CHECK  (([FailureCategory]='Unknown' OR [FailureCategory]='Poison' OR [FailureCategory]='SchemaViolation' OR [FailureCategory]='DownstreamUnavailable' OR [FailureCategory]='Transient'))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_FailureCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord] CHECK CONSTRAINT [CK_DeadLetterRecord_FailureCategory]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_ReplayAttemptCount_NonNegative]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord]  WITH CHECK ADD  CONSTRAINT [CK_DeadLetterRecord_ReplayAttemptCount_NonNegative] CHECK  (([ReplayAttemptCount]>=(0)))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_ReplayAttemptCount_NonNegative]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord] CHECK CONSTRAINT [CK_DeadLetterRecord_ReplayAttemptCount_NonNegative]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_SourceEntityType]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord]  WITH CHECK ADD  CONSTRAINT [CK_DeadLetterRecord_SourceEntityType] CHECK  (([SourceEntityType]='Topic' OR [SourceEntityType]='Queue'))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_SourceEntityType]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord] CHECK CONSTRAINT [CK_DeadLetterRecord_SourceEntityType]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_Status]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord]  WITH CHECK ADD  CONSTRAINT [CK_DeadLetterRecord_Status] CHECK  (([Status]='Discarded' OR [Status]='Resolved' OR [Status]='Replayed' OR [Status]='PendingReplay' OR [Status]='UnderReview' OR [Status]='New'))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_Status]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord] CHECK CONSTRAINT [CK_DeadLetterRecord_Status]
GO
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_SubscriptionPresence]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord]  WITH CHECK ADD  CONSTRAINT [CK_DeadLetterRecord_SubscriptionPresence] CHECK  (([SourceEntityType]=N'Queue' AND [SourceSubscriptionName] IS NULL OR [SourceEntityType]=N'Topic' AND [SourceSubscriptionName] IS NOT NULL))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_DeadLetterRecord_SubscriptionPresence]') AND parent_object_id = OBJECT_ID(N'[dbo].[DeadLetterRecord]'))
ALTER TABLE [dbo].[DeadLetterRecord] CHECK CONSTRAINT [CK_DeadLetterRecord_SubscriptionPresence]
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'ComplexDocument', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Persistent state for the Complex module simulated document API. See docs/complex-api.md.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ComplexDocument'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'ComplexIdempotency', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Idempotency-key cache for POST endpoints in the Complex module. See docs/complex-api.md.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ComplexIdempotency'
GO
IF NOT EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'DeadLetterRecord', NULL,NULL))
	EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Durable record of messages dead-lettered by Azure Service Bus on FileIt channels. One row per dead-letter receive; drives the operator-driven replay workflow. See docs/dead-letter-strategy.md.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeadLetterRecord'
GO
USE [master]
GO
ALTER DATABASE [FileIt] SET  READ_WRITE 
GO
