-- Indexes to optimize data filtering, paging, and sorting queries

-- Index for FirstName which is used in filtering and sorting
CREATE INDEX IX_Products_FirstName ON Products(FirstName);

-- Index for LastName which is used in filtering and sorting
CREATE INDEX IX_Products_LastName ON Products(LastName);

-- Index for Email which is used in filtering and sorting
CREATE INDEX IX_Products_Email ON Products(Email);

-- Index for CreatedDate which is used in filtering by date and sorting
CREATE INDEX IX_Products_CreatedDate ON Products(CreatedDate);

-- Index for CategoryId which is used in joins and filtering
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);

-- Index for Categories table to optimize joins
CREATE INDEX IX_Categories_Name ON Categories(Name);

-- Composite index for FirstName, LastName to optimize searches by full name
CREATE INDEX IX_Products_FullName ON Products(FirstName, LastName);

-- Index optimization discussion:
/*
The indexes above have been chosen to improve the performance of the queries used in our stored procedures:

1. IX_Products_FirstName, IX_Products_LastName, IX_Products_Email:
   These indexes will significantly improve the performance of queries that filter or sort by these fields,
   which are common operations in the GetFilteredProducts procedure.

2. IX_Products_CreatedDate:
   This index is crucial for date-based filtering, which is one of the required filter parameters.
   The index will convert sequential access to the table rather than a full table scan.

3. IX_Products_CategoryId:
   This index will optimize the JOIN operation between Products and Categories tables,
   which is used in all our main queries.

4. IX_Categories_Name:
   This index will help when joining with Categories and sorting by category name.

5. IX_Products_FullName (composite index):
   This composite index will help optimize searches that involve both first and last names.

Query Execution Plan Analysis:
Without these indexes, the query execution plans would involve full table scans for filtering and
sorting operations, which become inefficient as the table size grows. The addition of these indexes
allows the SQL Server query optimizer to use index seeks and scans instead, greatly reducing the I/O
operations needed.

For example, when filtering by FirstName using a LIKE operator, an index on FirstName allows for
an index seek or scan operation, which is much more efficient than scanning the entire table.

Similarly, the CreatedDate index allows for efficient date range queries, which would otherwise require
full table scans.

The CategoryId index improves the performance of JOIN operations by reducing the lookup time for
matching records between the Products and Categories tables.

These indexes should be monitored in a production environment to ensure they are being used effectively
and are not causing excessive overhead during data modifications.
*/
