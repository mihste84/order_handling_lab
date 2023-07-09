namespace Services.DatabaseDapper.Queries;


public static class CustomerAddressQueries
{
    public const string GetById = "SELECT * FROM CustomerAddresses WHERE Id = @Id";

    public const string GetAllReferenceDataAsync = "SELECT * FROM Cities; SELECT * FROM Countries;";

    public const string Delete = "DELETE FROM CustomerAddresses WHERE Id = @Id";

    public const string RemoveAllPrimary = "UPDATE CustomerAddresses SET IsPrimary = 0 WHERE CustomerId = @CustomerId";
    
    public const string Update = 
    """
        UPDATE CustomerAddresses 
        SET Address = @Address,
            Primary = @Primary,
            PostArea = @PostArea,
            ZipCode = @ZipCode,
            CityId = @CityId,
            CountryId = @CountryId,
            UpdatedBy = @UpdatedBy,
            Updated = @Updated
        OUTPUT INSERTED.[Id], INSERTED.RowVersion
        WHERE Id = @Id
    """;

    public const string InsertMultiple = """
        INSERT INTO CustomerAddresses (Address, IsPrimary, PostArea, ZipCode, CountryId, CustomerId, CityId, CreatedBy, UpdatedBy) 
        VALUES (@Address, @IsPrimary, @PostArea, @ZipCode, @CountryId, @CustomerId, @CityId, @CreatedBy, @UpdatedBy);
    """;
    public const string Insert = 
    """
        INSERT INTO CustomerAddresses (Address, IsPrimary, PostArea, ZipCode, CountryId, CustomerId, CityId, CreatedBy, UpdatedBy)
        OUTPUT INSERTED.[Id], INSERTED.RowVersion
        VALUES (@Address, @IsPrimary, @PostArea, @ZipCode, @CountryId, @CustomerId, @CityId, @CreatedBy, @UpdatedBy);
    """;

}