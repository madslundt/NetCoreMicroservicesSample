SET NOCOUNT ON
SET XACT_ABORT ON

PRINT 'Installing Outbox...';

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT [schema_id] FROM [sys].[schemas] WHERE [name] = '$(OutboxSchema)')
BEGIN
    EXEC (N'CREATE SCHEMA [$(OutboxSchema)]');
    PRINT 'Created database schema [$(OutboxSchema)]';
END
ELSE
    PRINT 'Database schema [$(OutboxSchema)] already exists';
    

    CREATE TABLE [$(OutboxSchema)].[Jobs] (
        [Id] [uniqueidentifier] IDENTITY(1,1) NOT NULL,
		[Created] [datetime] NOT NULL,
		[Type] [nvarchar] NOT NULL,
		[Data] [nvarchar] NOT NULL

        CONSTRAINT [PK_Outbox_Jobs] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Created table [$(OutboxSchema)].[Jobs]';

COMMIT TRANSACTION;
PRINT 'Outbox installed';