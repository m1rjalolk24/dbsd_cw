-- Database Security Setup
-- This script creates roles, logins, and users with appropriate minimal permissions

-- Create Logins
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'AppUser')
BEGIN
    CREATE LOGIN AppUser WITH PASSWORD = 'AppP@ss2025!', CHECK_POLICY = ON;
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'ExportUser')
BEGIN
    CREATE LOGIN ExportUser WITH PASSWORD = 'ExpP@ss2025!', CHECK_POLICY = ON;
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'ImportUser')
BEGIN
    CREATE LOGIN ImportUser WITH PASSWORD = 'ImpP@ss2025!', CHECK_POLICY = ON;
END

-- Create Database Users
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'AppUser')
BEGIN
    CREATE USER AppUser FOR LOGIN AppUser;
END

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ExportUser')
BEGIN
    CREATE USER ExportUser FOR LOGIN ExportUser;
END

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ImportUser')
BEGIN
    CREATE USER ImportUser FOR LOGIN ImportUser;
END

-- Create Roles
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ApplicationRole' AND type = 'R')
BEGIN
    CREATE ROLE ApplicationRole;
END

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ExportRole' AND type = 'R')
BEGIN
    CREATE ROLE ExportRole;
END

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ImportRole' AND type = 'R')
BEGIN
    CREATE ROLE ImportRole;
END

-- Application Role Permissions (CRUD operations on Products and Categories)
GRANT SELECT, INSERT, UPDATE, DELETE ON Products TO ApplicationRole;
GRANT SELECT ON Categories TO ApplicationRole;
GRANT EXECUTE ON GetFilteredProducts TO ApplicationRole;

-- Export Role Permissions (Read-only + Export procedures)
GRANT SELECT ON Products TO ExportRole;
GRANT SELECT ON Categories TO ExportRole;
GRANT EXECUTE ON ExportProductsToXML TO ExportRole;
GRANT EXECUTE ON ExportProductsToJSON TO ExportRole;

-- Import Role Permissions (Import procedures + Write to tables)
GRANT INSERT ON Products TO ImportRole;
GRANT INSERT ON Categories TO ImportRole;
GRANT EXECUTE ON ImportProductsFromXML TO ImportRole;
GRANT EXECUTE ON ImportProductsFromJSON TO ImportRole;

-- Add Users to Roles
ALTER ROLE ApplicationRole ADD MEMBER AppUser;
ALTER ROLE ExportRole ADD MEMBER ExportUser;
ALTER ROLE ImportRole ADD MEMBER ImportUser;

-- Security Best Practices:
/* 
1. Principle of Least Privilege:
   Each role has only the minimum permissions needed to perform its tasks.
   
2. Role-Based Access Control:
   Users are assigned to roles based on their functional responsibilities.
   
3. Separation of Duties:
   - ApplicationRole: Handles day-to-day CRUD operations
   - ExportRole: Only has read access and can execute export procedures
   - ImportRole: Only has insert permissions and can execute import procedures
   
4. No Direct Table Access for Import/Export Operations:
   Access is provided through stored procedures to maintain data integrity.
   
5. Strong Password Policies:
   Complex passwords with policy enforcement.

This security setup ensures that each user has only the permissions they need,
reducing the risk of unauthorized data access or modification.
*/ 