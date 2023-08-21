using Customers.BaseCustomers.Commands;
using Customers.CommonModels;
using DatabaseDapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Respawn;
using Respawn.Graph;

namespace IntegrationTests.ApiTests;

public class TestBase : IDisposable
{
    public readonly HttpClient HttpClient;
    private readonly string _connectionString;
    public readonly DapperUnitOfWork UnitOfWork;
    public TestBase()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        var configuration = webApplicationFactory.Services.GetService(typeof(IConfiguration)) as IConfiguration;
        _connectionString = configuration!.GetConnectionString("AppDbContext")!;
        HttpClient = webApplicationFactory.CreateDefaultClient();
        var loggerMock = new Mock<ILogger<DapperUnitOfWork>>();
        var connection = new SqlConnection(_connectionString);
        UnitOfWork = new DapperUnitOfWork(connection, loggerMock.Object);
    }

    private static async Task<Respawner> GetRespawnAsync(string connectionString)
    => await Respawner.CreateAsync(connectionString, new RespawnerOptions
    {
        TablesToIgnore = new Table[]
        {
            "Cities",
            "Countries"
        }
    });

    public async Task<SqlResult?> InsertCustomerAsync(InsertCustomerCommand? command = default)
    {
        var res = await HttpClient.PostAsJsonAsync(
            "/api/customer",
            command ?? GetBaseInsertCustomerPersonCommand()
        );
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<SqlResult>();
    }

    public static InsertCustomerCommand GetBaseInsertCustomerCompanyCommand()
    => new()
    {
        Name = "John",
        Code = "123456789",
        IsCompany = true,
        CustomerAddresses = new[] {
            new CustomerAddressModel {
                Address = "123 Main St",
                CityId = 1,
                CountryId = 1,
                IsPrimary = true,
                PostArea = "Stockholm",
                ZipCode = "12345"
            }
        },
        ContactInfo = new[] {
            new CustomerContactInfoModel {
                Type = ContactInfoType.Email,
                Value = "test@mail.com"
            }
        }
    };

    public static InsertCustomerCommand GetBaseInsertCustomerPersonCommand()
    => new()
    {
        Ssn = "12345678-1234",
        FirstName = "John",
        LastName = "Doe",
        IsCompany = false,
        CustomerAddresses = new[] {
            new CustomerAddressModel {
                Address = "123 Main St",
                CityId = 1,
                CountryId = 1,
                IsPrimary = true,
                PostArea = "Stockholm",
                ZipCode = "12345"
            }
        },
        ContactInfo = new[] {
            new CustomerContactInfoModel {
                Type = ContactInfoType.Email,
                Value = "test@mail.com"
            }
        }
    };

    public async Task ResetDbAsync()
    {
        var respawn = await GetRespawnAsync(_connectionString);
        await respawn.ResetAsync(_connectionString);
    }

    public void Dispose()
    {
        UnitOfWork.Dispose();
        GC.SuppressFinalize(this);
    }
}