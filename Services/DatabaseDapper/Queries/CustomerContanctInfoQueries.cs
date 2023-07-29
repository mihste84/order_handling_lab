namespace DatabaseDapper.Queries;

public static class CustomerContactInfoQueries
{
    public const string GetById = "SELECT * FROM CustomerContactInfo WHERE Id = @Id";
    public const string Delete = "DELETE FROM CustomerContactInfo WHERE Id = @Id";
    public const string Update =
    """
        UPDATE CustomerContactInfo 
        SET Type = @Type,
            Value = @Value,
            CustomerId = @CustomerId,
            UpdatedBy = @UpdatedBy,
            Updated = @Updated
        OUTPUT INSERTED.[Id], INSERTED.RowVersion 
        WHERE Id = @Id
    """;
    public const string Insert =
    """
        INSERT INTO CustomerContactInfo (Type, Value, CustomerId, CreatedBy, UpdatedBy) 
        OUTPUT INSERTED.[Id], INSERTED.RowVersion
        VALUES (@Type, @Value, @CustomerId, @CreatedBy, @UpdatedBy);
    """;
    public const string InsertMultiple =
    """
        INSERT INTO CustomerContactInfo (Type, Value, CustomerId, CreatedBy, UpdatedBy)
        VALUES (@Type, @Value, @CustomerId, @CreatedBy, @UpdatedBy);
    """;
}