var test1 = new TestRecord("Test1");
var test2 = test1 with { Name = "Test1" };

Console.WriteLine(test1 == test2);

public record TestRecord(string? Name);

