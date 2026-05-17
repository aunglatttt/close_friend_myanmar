IF COL_LENGTH('dbo.Township', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.Township
    ADD IsDeleted bit NOT NULL
        CONSTRAINT DF_Township_IsDeleted DEFAULT (0);
END
GO

IF COL_LENGTH('dbo.Township', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Township
    ADD DeletedAt datetime2 NULL;
END
GO
