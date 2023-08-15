namespace IntegrationTests.ApiTests;

[Order(1)]
public sealed class MasterDataTests : IClassFixture<TestBase>
{
    private readonly TestBase _testBase;

    public MasterDataTests(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public void App_Settings_Environment()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Assert.Equal("IntegrationTests", environment);
    }

    [Fact]
    public async Task Get_Master_Data()
    {
        await _testBase.ResetDbAsync();
        var response = await _testBase.HttpClient.GetFromJsonAsync<MasterDataDto>("/api/masterdata");
        Assert.NotNull(response);
    }
}