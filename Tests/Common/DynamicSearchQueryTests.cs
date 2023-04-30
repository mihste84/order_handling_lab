using Models.Values;

namespace Tests.Common;

public class DynamicSearchQueryTests
{
    [Theory]
    [InlineData("FirstName", "Value", SearchOperators.StartsWith, "FirstName LIKE 'Value%'")]
    [InlineData("FirstName", "Value", SearchOperators.Contains, "FirstName LIKE '%Value%'")]
    [InlineData("FirstName", "Value", SearchOperators.EndsWith, "FirstName LIKE '%Value'")]
    [InlineData("FirstName", "Value", SearchOperators.Equal, "FirstName = Value")]
    [InlineData("FirstName", "Value", SearchOperators.NotEqual, "FirstName <> Value")]
    [InlineData("FirstName", "Value", SearchOperators.GreaterThan, "FirstName > Value")]
    [InlineData("FirstName", "Value", SearchOperators.GreaterThanOrEqual, "FirstName >= Value")]
    [InlineData("FirstName", "Value", SearchOperators.LessThan, "FirstName < Value")]
    [InlineData("FirstName", "Value", SearchOperators.LessThanOrEqual, "FirstName <= Value")]
    [InlineData("FirstName", default, SearchOperators.IsNull, "FirstName IS NULL")]
    [InlineData("FirstName", default, SearchOperators.IsNotNull, "FirstName IS NOT NULL")]
    public void GetWhereFromSearchItem_Single_Value_Success(string name, string value, string op, string expectedQuery) {
        var item = new SearchItem(name, value, op);
        var query = DynamicSearchQuery.GetWhereFromSearchItem(item);
        Assert.Equal(expectedQuery, query.ToString());
    }

    [Fact]
    public void GetWhereFromSearchItem_Array_Value_Success() {
        var item = new SearchItem("FirstName", new[] { 1, 2 }, SearchOperators.In, false);
        var query = DynamicSearchQuery.GetWhereFromSearchItem(item);
        Assert.Equal("FirstName IN System.Int32[]", query.ToString());
        Assert.Equal("{0:raw} IN {1}", query.Format);
    }
}