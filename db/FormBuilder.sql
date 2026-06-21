IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [DatabaseConnections] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [ConnectionString] nvarchar(1000) NOT NULL,
        [Provider] nvarchar(50) NOT NULL,
        [IsDefault] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_DatabaseConnections] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [FieldMappings] (
        [Id] uniqueidentifier NOT NULL,
        [FormId] uniqueidentifier NOT NULL,
        [FormFieldId] uniqueidentifier NOT NULL,
        [TableName] nvarchar(256) NOT NULL,
        [ColumnName] nvarchar(256) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FieldMappings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [Forms] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [Slug] nvarchar(250) NOT NULL,
        [Status] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Forms] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [FormSubmissions] (
        [Id] uniqueidentifier NOT NULL,
        [FormId] uniqueidentifier NOT NULL,
        [SubmittedAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FormSubmissions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [FormPages] (
        [Id] uniqueidentifier NOT NULL,
        [FormId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Order] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FormPages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FormPages_Forms_FormId] FOREIGN KEY ([FormId]) REFERENCES [Forms] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [FormSubmissionValues] (
        [Id] uniqueidentifier NOT NULL,
        [SubmissionId] uniqueidentifier NOT NULL,
        [FormFieldId] uniqueidentifier NOT NULL,
        [Value] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FormSubmissionValues] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FormSubmissionValues_FormSubmissions_SubmissionId] FOREIGN KEY ([SubmissionId]) REFERENCES [FormSubmissions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [FormRows] (
        [Id] uniqueidentifier NOT NULL,
        [PageId] uniqueidentifier NOT NULL,
        [Order] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FormRows] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FormRows_FormPages_PageId] FOREIGN KEY ([PageId]) REFERENCES [FormPages] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [FormColumns] (
        [Id] uniqueidentifier NOT NULL,
        [RowId] uniqueidentifier NOT NULL,
        [Width] int NOT NULL,
        [Order] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FormColumns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FormColumns_FormRows_RowId] FOREIGN KEY ([RowId]) REFERENCES [FormRows] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE TABLE [FormFields] (
        [Id] uniqueidentifier NOT NULL,
        [ColumnId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Label] nvarchar(250) NOT NULL,
        [Type] int NOT NULL,
        [Required] bit NOT NULL,
        [Placeholder] nvarchar(500) NULL,
        [HelpText] nvarchar(1000) NULL,
        [DefaultValue] nvarchar(1000) NULL,
        [Order] int NOT NULL,
        [ValidationJson] nvarchar(max) NULL,
        [SettingsJson] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FormFields] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FormFields_FormColumns_ColumnId] FOREIGN KEY ([ColumnId]) REFERENCES [FormColumns] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DatabaseConnections_Name] ON [DatabaseConnections] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FieldMappings_FormId] ON [FieldMappings] ([FormId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FieldMappings_FormId_FormFieldId] ON [FieldMappings] ([FormId], [FormFieldId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FormColumns_RowId_Order] ON [FormColumns] ([RowId], [Order]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FormFields_ColumnId_Order] ON [FormFields] ([ColumnId], [Order]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FormPages_FormId_Order] ON [FormPages] ([FormId], [Order]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FormRows_PageId_Order] ON [FormRows] ([PageId], [Order]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Forms_Slug] ON [Forms] ([Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Forms_Status] ON [Forms] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FormSubmissions_FormId] ON [FormSubmissions] ([FormId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FormSubmissions_SubmittedAt] ON [FormSubmissions] ([SubmittedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FormSubmissionValues_FormFieldId] ON [FormSubmissionValues] ([FormFieldId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FormSubmissionValues_SubmissionId] ON [FormSubmissionValues] ([SubmissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620180735_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620180735_InitialCreate', N'9.0.0');
END;

COMMIT;
GO

