namespace API.Models;

public record MetadataMaxLengthValidator(string Message, int MaxLength);
public record MetadataMinLengthValidator(string Message, int MinLength);
public record MetadataRequiredValidator(string Message);