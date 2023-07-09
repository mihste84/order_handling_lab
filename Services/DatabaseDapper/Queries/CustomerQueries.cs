namespace Services.DatabaseDapper.Queries;


public static class CustomerQueries
{
    public const string GetById = 
    """
        SELECT TOP 1 c.* FROM Customers c
        WHERE c.Id = @Id

        SELECT TOP 1 cp.* FROM Customers c
        INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
        WHERE c.Id = @Id

        SELECT TOP 1 cc.* FROM Customers c
        INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
        WHERE c.Id = @Id

        SELECT cc.* FROM Customers c
        INNER JOIN CustomerContactInfo cc ON c.Id = cc.CustomerId
        WHERE c.Id = @Id

        SELECT ca.* FROM Customers c
        INNER JOIN CustomerAddresses ca ON c.Id = ca.CustomerId
        WHERE c.Id = @Id
    """;

    public const string GetBySsn = 
    """
        SELECT TOP 1 c.* FROM Customers c
        INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
        WHERE cp.Ssn = @Ssn

        SELECT TOP 1 cp.* FROM Customers c
        INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
        WHERE cp.Ssn = @Ssn

        SELECT cc.* FROM Customers c
        INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
        INNER JOIN CustomerContactInfo cc ON c.Id = cc.CustomerId
        WHERE cp.Ssn = @Ssn

        SELECT ca.* FROM Customers c
        INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
        INNER JOIN CustomerAddresses ca ON c.Id = ca.CustomerId
        WHERE cp.Ssn = @Ssn
    """;

    public const string GetByCode = 
    """
        SELECT TOP 1 c.* FROM Customers c
        INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
        WHERE cc.Code = @Code

        SELECT TOP 1 cc.* FROM Customers c
        INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
        WHERE cc.Code = @Code

        SELECT cci.* FROM Customers c
        INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
        INNER JOIN CustomerContactInfo cci ON c.Id = cci.CustomerId
        WHERE cc.Code = @Code

        SELECT ca.* FROM Customers c
        INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
        INNER JOIN CustomerAddresses ca ON c.Id = ca.CustomerId
        WHERE cc.Code = @Code
    """;

    public const string Delete = "DELETE FROM Customers WHERE Id = @Id";
    
    public const string Update = 
    """
        UPDATE Customers 
        SET Active = @Active,
            UpdatedBy = @UpdatedBy,
            Updated = @Updated
        OUTPUT INSERTED.[Id], INSERTED.RowVersion 
        WHERE Id = @Id
    """;

    public const string Insert = 
    """
        INSERT INTO Customers (Active, CreatedBy, UpdatedBy) 
        OUTPUT INSERTED.[Id], INSERTED.RowVersion 
        VALUES (@Active, @CreatedBy, @UpdatedBy);
    """;

    public const string InsertNew = 
    """
        DECLARE @IdentityValue TABLE (ContactID int);

        INSERT INTO Customers (Active, CreatedBy, UpdatedBy) 
        OUTPUT INSERTED.[Id], INSERTED.RowVersion INTO @IdentityValue
        VALUES (@Active, @CreatedBy, @UpdatedBy);

        INSERT INTO CustomerPersons (CustomerId, FirstName, LastName, MiddleName, Ssn, CreatedBy, UpdatedBy)
        SELECT ContactID, @FirstName, @LastName, @MiddleName, @Ssn, @CreatedBy, @UpdatedBy FROM @IdentityValue;
    """;

    public static FormattableString GetSearchCustomersQuery(DynamicSearchQuery query) => 
    $@"
        SELECT * FROM (
            SELECT
                c.[Id] 
                ,cp.FirstName
                ,cp.LastName
                ,cp.MiddleName
                ,cp.Ssn
                ,co.Code
                ,co.Name
                ,[Active]
                ,c.[CreatedBy]
                ,c.[Created]
                ,c.[UpdatedBy]
                ,c.[Updated]
                ,CASE WHEN co.[Id] IS NOT NULL THEN 1 ELSE 0 END AS IsCompany
                ,CASE WHEN cp.[Id] IS NOT NULL THEN 1 ELSE 0 END AS IsPerson
            FROM [dbo].[Customers] c
            LEFT JOIN dbo.CustomerCompanies co ON c.[Id] = co.[CustomerId]
            LEFT JOIN dbo.CustomerPersons cp ON c.[Id] = cp.[CustomerId]
            /**where**/
        ) x
        ORDER BY {query.OrderBy:raw} {query.OrderByDirection:raw}
        OFFSET {query.StartRow:raw} ROWS FETCH NEXT {(query.EndRow - query.StartRow):raw} ROWS ONLY;

        SELECT count(*) FROM [dbo].[Customers] c
        LEFT JOIN dbo.CustomerCompanies co ON c.[Id] = co.[CustomerId]
        LEFT JOIN dbo.CustomerPersons cp ON c.[Id] = cp.[CustomerId]
        /**where**/    
    ";
}