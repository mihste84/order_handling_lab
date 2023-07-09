namespace Services.DatabaseDapper.Queries;


public static class CustomerCompanyQueries
{
    public const string GetById = "SELECT * FROM CustomerCompanies WHERE Id = @Id";

    public const string Delete = "DELETE FROM CustomerCompanies WHERE Id = @Id";
   
    public const string Update = 
    """
        UPDATE CustomerCompanies 
        SET Code = @Code,
            Name = @Name,
            CustomerId = @CustomerId,
            UpdatedBy = @UpdatedBy,
            Updated = @Updated
        OUTPUT INSERTED.[Id], INSERTED.RowVersion 
        WHERE Id = @Id
    """;

    public const string Insert = 
    """
        INSERT INTO CustomerCompanies (Code, Name, CustomerId, CreatedBy, UpdatedBy) 
        OUTPUT INSERTED.[Id], INSERTED.RowVersion 
        VALUES (@Code, @Name, @CustomerId, @CreatedBy, @UpdatedBy);
    """;

}