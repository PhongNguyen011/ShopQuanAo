-- Migration script to update ShippingFee table
-- If ShippingFees table exists with ProvinceId column, drop it and recreate with ProvinceName
-- If table doesn't exist, create it

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ShippingFees' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    -- Check if ProvinceId column exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.ShippingFees') AND name = 'ProvinceId')
    BEGIN
        -- Drop foreign key if exists
        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.ShippingFees') AND name LIKE 'FK_ShippingFees_Provinces%')
        BEGIN
            DECLARE @fkName NVARCHAR(255)
            SELECT @fkName = name FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.ShippingFees') AND name LIKE 'FK_ShippingFees_Provinces%'
            EXEC('ALTER TABLE dbo.ShippingFees DROP CONSTRAINT ' + @fkName)
        END
        
        -- Drop index if exists
        IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.ShippingFees') AND name = 'IX_ShippingFees_ProvinceId')
        BEGIN
            DROP INDEX IX_ShippingFees_ProvinceId ON dbo.ShippingFees
        END
        
        -- Drop table and recreate
        DROP TABLE dbo.ShippingFees
    END
END

-- Create ShippingFees table with ProvinceName
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShippingFees' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[ShippingFees] (
        [Id] int NOT NULL IDENTITY,
        [ProvinceName] nvarchar(100) NOT NULL,
        [Fee] decimal(18,2) NOT NULL,
        [Description] nvarchar(200) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ShippingFees] PRIMARY KEY ([Id])
    );
    
    CREATE INDEX [IX_ShippingFees_ProvinceName] ON [dbo].[ShippingFees] ([ProvinceName]);
END
GO

