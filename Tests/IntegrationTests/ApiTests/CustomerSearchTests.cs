using Customers.BaseCustomers.Commands;
using Customers.BaseCustomers.Queries;
using Customers.CommonModels;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace IntegrationTests.ApiTests;

[Order(3)]
public sealed class CustomerSearchTests : IClassFixture<TestBase>
{
    private readonly TestBase _testBase;

    public CustomerSearchTests(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Search_Customers_Pagination()
    {
        await _testBase.ResetDbAsync();
        await InsertSearchTestCustomersAsync();

        var query = new SearchCustomersQuery
        {
            StartRow = 0,
            EndRow = 2,
            OrderBy = "Id",
            OrderByDirection = "ASC"
        };

        for (var i = 0; i < 3; i++)
        {
            query.StartRow = i * 2;
            query.EndRow = query.StartRow + 2;
            var queryString = GetQueryStringFromObject("/api/customer/search", query);
            var response = await _testBase.HttpClient.GetAsync(queryString);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<SearchResult<CustomerDto>>();
            Assert.NotNull(result);
            Assert.IsType<SearchResult<CustomerDto>>(result);
            Assert.Equal(8, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }
    }

    [Fact]
    public async Task Search_Customers_Not_Found()
    {
        await _testBase.ResetDbAsync();
        await InsertSearchTestCustomersAsync();

        var query = new SearchCustomersQuery
        {
            StartRow = 0,
            EndRow = 2,
            OrderBy = "Id",
            OrderByDirection = "ASC",
            SearchItems = new[] {
                new SearchItem("FirstName", "NotExisting", SearchOperators.Equal)
            }
        };

        var queryString = GetQueryStringFromObject("/api/customer/search", query);
        var response = await _testBase.HttpClient.GetAsync(queryString);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(SearchTestDataTheory), MemberType = typeof(CustomerSearchTests))]
    public async Task Search_Customers(SearchTestData testData)
    {
        await _testBase.ResetDbAsync();
        await InsertSearchTestCustomersAsync();

        var query = new SearchCustomersQuery
        {
            StartRow = 0,
            EndRow = 15,
            OrderBy = "Id",
            OrderByDirection = "ASC",
            SearchItems = testData.SearchItems
        };

        var queryString = GetQueryStringFromObject("/api/customer/search", query);
        var response = await _testBase.HttpClient.GetAsync(queryString);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<SearchResult<CustomerDto>>();
        Assert.NotNull(result);
        Assert.IsType<SearchResult<CustomerDto>>(result);
        Assert.Equal(testData.ExpectedCount, result.TotalCount);
    }

    public static TheoryData<SearchTestData> SearchTestDataTheory()
    => new() {
        new SearchTestData(new[] { new SearchItem("Name", "Company", SearchOperators.StartsWith) }, 2),
        new SearchTestData(new[] {
            new SearchItem("FirstName", "Stefan", SearchOperators.Equal),
            new SearchItem("LastName", "M", SearchOperators.StartsWith)
        }, 1),
        new SearchTestData(new[] {
            new SearchItem("FirstName", "Stefan", SearchOperators.NotEqual)
        }, 3),
        new SearchTestData(new[] {
            new SearchItem("FirstName", "Stefan", SearchOperators.Equal),
            new SearchItem("Phone", "+46704551122", SearchOperators.Equal, false)
        }, 1),
        new SearchTestData(new[] {
            new SearchItem("Email", "company1", SearchOperators.StartsWith, false)
        }, 1 ),
        new SearchTestData(new[] {
            new SearchItem("FirstName", "j", SearchOperators.Contains)
        }, 1),
        new SearchTestData(new[] {
            new SearchItem("FirstName", "an", SearchOperators.EndsWith)
        }, 3),
        new SearchTestData(new[] {
            new SearchItem("CityId", "3", SearchOperators.Equal, false)
        }, 2),
        new SearchTestData( new[] {
            new SearchItem("CityId", "3", SearchOperators.GreaterThanOrEqual, false)
        }, 3),
        new SearchTestData(new[] {
            new SearchItem("CityId", "3", SearchOperators.LessThan, false)
        }, 6),
        new SearchTestData(new[] {
            new SearchItem("FirstName", "Stefan", SearchOperators.Equal),
            new SearchItem("LastName", "Mih", SearchOperators.StartsWith),
            new SearchItem("MiddleName", "", SearchOperators.IsNull),
            new SearchItem("CityId", "1", SearchOperators.Equal, false)
        }, 1),
    };

    public class SearchTestData
    {
        public SearchItem[] SearchItems { get; set; }
        public int ExpectedCount { get; set; }

        public SearchTestData(SearchItem[] searchItems, int expectedCount)
        {
            SearchItems = searchItems;
            ExpectedCount = expectedCount;
        }
    }

    private async Task<SqlResult?> InsertCustomerAsync(InsertCustomerCommand model)
    {
        var res = await _testBase.HttpClient.PostAsJsonAsync("/api/customer", model);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<SqlResult>();
    }

    private async Task InsertSearchTestCustomersAsync()
    {
        var customers = new[] {
            new InsertCustomerCommand()
            {
                Name = "Company 1",
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
                        Value = "company1@mail.com"
                    },
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Phone,
                        Value = "+46701234567"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                Name = "Company 2",
                Code = "987654321",
                IsCompany = true,
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "987 Main St",
                        CityId = 2,
                        CountryId = 2,
                        IsPrimary = true,
                        PostArea = "Solna",
                        ZipCode = "54321"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "company2@mail.com"
                    },
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Phone,
                        Value = "+46987654321"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                Name = "Danish Company",
                Code = "45654841321",
                IsCompany = true,
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "658 Main St",
                        CityId = 4,
                        CountryId = 4,
                        IsPrimary = true,
                        PostArea = "Kopenhagen",
                        ZipCode = "44444"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "danish_company@mail.com"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                FirstName = "Stefan",
                LastName = "Mihailovic",
                Ssn = "19840324-1234",
                IsCompany = false,
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "Regementsgatan 1",
                        CityId = 1,
                        CountryId = 1,
                        IsPrimary = true,
                        PostArea = "Solna",
                        ZipCode = "54321"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "stefan.mihailovic@mail.com"
                    },
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Phone,
                        Value = "+46704551122"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                FirstName = "Stefan",
                LastName = "Larsson",
                Ssn = "12345678-1234",
                IsCompany = false,
                MiddleName = "Lars",
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "654 Main St",
                        CityId = 1,
                        CountryId = 1,
                        IsPrimary = true,
                        PostArea = "Solna",
                        ZipCode = "54321"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "stefan.larsson@mail.com"
                    }
                }
            },
             new InsertCustomerCommand()
            {
                FirstName = "Johan",
                LastName = "Larsson",
                Ssn = "89784561-1234",
                IsCompany = false,
                MiddleName = "Lars",
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "111 Main St",
                        CityId = 1,
                        CountryId = 1,
                        IsPrimary = true,
                        PostArea = "Solna",
                        ZipCode = "54321"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "johan.larsson@mail.com"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                FirstName = "Lars",
                LastName = "Nilsson",
                Ssn = "87654321-1234",
                IsCompany = false,
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "XYZ St",
                        CityId = 3,
                        CountryId = 3,
                        IsPrimary = true,
                        PostArea = "ABC",
                        ZipCode = "78945"
                    },
                    new CustomerAddressModel {
                        Address = "ABC St",
                        CityId = 1,
                        CountryId = 1,
                        IsPrimary = false,
                        PostArea = "CBA",
                        ZipCode = "56431"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "Lars.Nilsson@mail.com"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                FirstName = "Lars",
                LastName = "Nilsson",
                Ssn = "45874612-1234",
                IsCompany = false,
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "ASDF St",
                        CityId = 3,
                        CountryId = 3,
                        IsPrimary = true,
                        PostArea = "ABC",
                        ZipCode = "78945"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "Lars.Nilsson11@mail.com"
                    }
                }
            },
        };

        foreach (var customer in customers)
        {
            await InsertCustomerAsync(customer);
        }
    }

    private static string GetQueryStringFromObject(string basePath, SearchCustomersQuery model)
    {
        var queryParams = new List<KeyValuePair<string, StringValues>>() {
            new KeyValuePair<string, StringValues>(nameof(model.EndRow), model.EndRow.ToString()),
            new KeyValuePair<string, StringValues>(nameof(model.StartRow), model.StartRow.ToString()),
            new KeyValuePair<string, StringValues>(nameof(model.OrderBy), model.OrderBy),
            new KeyValuePair<string, StringValues>(nameof(model.OrderByDirection), model.OrderByDirection)
        };

        foreach (var item in model.SearchItems.Select((value, index) => new { value, index }))
        {
            queryParams.Add(new KeyValuePair<string, StringValues>($"searchItems[{item.index}].Name", item.value.Name));
            if (!string.IsNullOrWhiteSpace(item.value.Value))
                queryParams.Add(new KeyValuePair<string, StringValues>($"searchItems[{item.index}].Value", item.value.Value));
            queryParams.Add(new KeyValuePair<string, StringValues>($"searchItems[{item.index}].Operator", item.value.Operator));
            queryParams.Add(new KeyValuePair<string, StringValues>($"searchItems[{item.index}].HandleAutomatically", item.value.HandleAutomatically.ToString()));
        }

        return QueryHelpers.AddQueryString(basePath, queryParams);
    }
}