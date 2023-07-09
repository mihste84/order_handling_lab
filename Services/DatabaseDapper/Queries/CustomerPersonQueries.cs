namespace Services.DatabaseDapper.Queries;


public static class CustomerPersonQueries 
{
    public const string GetById = "SELECT * FROM CustomerPersons WHERE Id = @Id";

    public const string Delete = "DELETE FROM CustomerPersons WHERE Id = @Id";

    public const string Update = 
    """
        UPDATE CustomerPersons 
        SET FirstName = @FirstName,
            MiddleName = @MiddleName,
            LastName = @LastName,
            Ssn = @Ssn,
            CustomerId = @CustomerId,
            UpdatedBy = @UpdatedBy,
            Updated = @Updated
        OUTPUT INSERTED.[Id], INSERTED.RowVersion 
        WHERE Id = @Id
    """;

    public const string Insert = 
    """
        INSERT INTO CustomerPersons (FirstName, MiddleName, LastName, Ssn, CustomerId, CreatedBy, UpdatedBy) 
        OUTPUT INSERTED.[Id], INSERTED.RowVersion 
        VALUES (@FirstName, @MiddleName, @LastName, @Ssn, @CustomerId, @CreatedBy, @UpdatedBy);
    """;
}