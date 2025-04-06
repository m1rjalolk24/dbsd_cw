-- Create Database
CREATE DATABASE DBSD_CW2;
GO

USE DBSD_CW2;
GO

-- Create Products Table
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10,2) NOT NULL,
    StockQuantity INT NOT NULL,
    ImageData VARBINARY(MAX),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CategoryId INT,
    LastModifiedDate DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Create Categories Table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Add Foreign Key
ALTER TABLE Products
ADD CONSTRAINT FK_Products_Categories
FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId);

-- Create Stored Procedures for CRUD Operations

-- Create Product
CREATE PROCEDURE sp_CreateProduct
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(10,2),
    @StockQuantity INT,
    @ImageData VARBINARY(MAX),
    @CategoryId INT
AS
BEGIN
    INSERT INTO Products (Name, Description, Price, StockQuantity, ImageData, CategoryId)
    VALUES (@Name, @Description, @Price, @StockQuantity, @ImageData, @CategoryId);
    
    SELECT SCOPE_IDENTITY() AS ProductId;
END
GO

-- Read Product
CREATE PROCEDURE sp_GetProduct
    @ProductId INT
AS
BEGIN
    SELECT * FROM Products WHERE ProductId = @ProductId;
END
GO

-- Update Product
CREATE PROCEDURE sp_UpdateProduct
    @ProductId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(10,2),
    @StockQuantity INT,
    @ImageData VARBINARY(MAX),
    @CategoryId INT,
    @IsActive BIT
AS
BEGIN
    UPDATE Products
    SET Name = @Name,
        Description = @Description,
        Price = @Price,
        StockQuantity = @StockQuantity,
        ImageData = @ImageData,
        CategoryId = @CategoryId,
        IsActive = @IsActive,
        LastModifiedDate = GETDATE()
    WHERE ProductId = @ProductId;
END
GO

-- Delete Product
CREATE PROCEDURE sp_DeleteProduct
    @ProductId INT
AS
BEGIN
    DELETE FROM Products WHERE ProductId = @ProductId;
END
GO

-- Create Stored Procedure for Filtering, Paging, and Sorting
CREATE PROCEDURE sp_GetFilteredProducts
    @CategoryId INT = NULL,
    @MinPrice DECIMAL(10,2) = NULL,
    @MaxPrice DECIMAL(10,2) = NULL,
    @IsActive BIT = NULL,
    @SortBy NVARCHAR(50) = 'Name',
    @SortOrder NVARCHAR(4) = 'ASC',
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT p.*, c.Name AS CategoryName
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
        AND (@MinPrice IS NULL OR p.Price >= @MinPrice)
        AND (@MaxPrice IS NULL OR p.Price <= @MaxPrice)
        AND (@IsActive IS NULL OR p.IsActive = @IsActive)
    ORDER BY 
        CASE WHEN @SortOrder = 'ASC' THEN
            CASE @SortBy
                WHEN 'Name' THEN p.Name
                WHEN 'Price' THEN CAST(p.Price AS NVARCHAR(50))
                WHEN 'CreatedDate' THEN CAST(p.CreatedDate AS NVARCHAR(50))
                ELSE p.Name
            END
        END ASC,
        CASE WHEN @SortOrder = 'DESC' THEN
            CASE @SortBy
                WHEN 'Name' THEN p.Name
                WHEN 'Price' THEN CAST(p.Price AS NVARCHAR(50))
                WHEN 'CreatedDate' THEN CAST(p.CreatedDate AS NVARCHAR(50))
                ELSE p.Name
            END
        END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- Create Stored Procedure for XML Export
CREATE PROCEDURE sp_ExportProductsToXML
AS
BEGIN
    SELECT 
        p.ProductId AS '@ProductId',
        p.Name AS '@Name',
        p.Description AS 'Description',
        p.Price AS 'Price',
        p.StockQuantity AS 'StockQuantity',
        p.CreatedDate AS 'CreatedDate',
        p.IsActive AS 'IsActive',
        p.LastModifiedDate AS 'LastModifiedDate',
        c.CategoryId AS 'Category/@CategoryId',
        c.Name AS 'Category/Name',
        c.Description AS 'Category/Description'
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
    FOR XML PATH('Product'), ROOT('Products');
END
GO

-- Create Stored Procedure for JSON Export
CREATE PROCEDURE sp_ExportProductsToJSON
AS
BEGIN
    SELECT 
        p.ProductId,
        p.Name,
        p.Description,
        p.Price,
        p.StockQuantity,
        p.CreatedDate,
        p.IsActive,
        p.LastModifiedDate,
        (
            SELECT 
                c.CategoryId,
                c.Name,
                c.Description
            FROM Categories c
            WHERE c.CategoryId = p.CategoryId
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) AS Category
    FROM Products p
    FOR JSON PATH, ROOT('Products');
END
GO

-- Create Database Roles and Users
CREATE LOGIN AppUser WITH PASSWORD = 'AppUser123!';
CREATE LOGIN ExportUser WITH PASSWORD = 'ExportUser123!';
CREATE LOGIN ImportUser WITH PASSWORD = 'ImportUser123!';

CREATE USER AppUser FOR LOGIN AppUser;
CREATE USER ExportUser FOR LOGIN ExportUser;
CREATE USER ImportUser FOR LOGIN ImportUser;

-- Create Roles
CREATE ROLE AppRole;
CREATE ROLE ExportRole;
CREATE ROLE ImportRole;

-- Grant Permissions
GRANT EXECUTE ON sp_CreateProduct TO AppRole;
GRANT EXECUTE ON sp_GetProduct TO AppRole;
GRANT EXECUTE ON sp_UpdateProduct TO AppRole;
GRANT EXECUTE ON sp_DeleteProduct TO AppRole;

GRANT EXECUTE ON sp_ExportProductsToXML TO ExportRole;
GRANT EXECUTE ON sp_ExportProductsToJSON TO ExportRole;

GRANT EXECUTE ON sp_CreateProduct TO ImportRole;
GRANT EXECUTE ON sp_UpdateProduct TO ImportRole;

-- Add Users to Roles
ALTER ROLE AppRole ADD MEMBER AppUser;
ALTER ROLE ExportRole ADD MEMBER ExportUser;
ALTER ROLE ImportRole ADD MEMBER ImportUser; 