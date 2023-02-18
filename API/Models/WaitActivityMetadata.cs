namespace API.Models;

public record WaitActivityMetadata
{
    public string Name { get; init; } = "Wait";
    public string Description { get; init; } = "Wait number of seconds before proceeding to next activity.";
    public WaitInputMetadata Input { get; init; } = new();
}

public record WaitInputMetadata {
    public string Name { get; init; } = "Wait time";
    public string Type { get; init; } = "number";
    public WaitInputValidators Validators { get; init; } = new();
}

public record WaitInputValidators {
    public MetadataRequiredValidator Required { get; init; } = new("Wait time is a required value.");
    public MetadataMaxLengthValidator MaxLength { get; init; } = new("Wait time  cannot exceed 3600 seconds.", 3600);
    public MetadataMinLengthValidator MinLength { get; init; } = new("Wait time  must be at least 1 second.", 1);

};