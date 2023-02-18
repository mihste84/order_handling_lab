namespace API.Models;

public record ScriptActivityMetadata
{
    public string Name { get; init; } = "Script";
    public string Description { get; init; } = "Custom script activity.";
    public ScriptTextInputMetadata TextInput { get; init; } = new();
    public ScriptLanguageInputMetadata LanguageInput { get; init; } = new();
}

public record ScriptTextInputMetadata {
    public string Name { get; init; } = "Script code";
    public string Type { get; init; } = "text";
    public ScriptTextInputValidators Validators { get; init; } = new();
}

public record ScriptLanguageInputMetadata {
    public string Name { get; init; } = "Script language";
    public string Type { get; init; } = "select";
    public string[] Languages { get; init; } = new [] { "Bash", "Powershell", "Python", "Javascript" };
    public ScriptLanguageInputValidators Validators { get; init; } = new();
}

public record ScriptTextInputValidators {
    public MetadataRequiredValidator Required { get; init; } = new("Script code is a required value.");
    public MetadataMinLengthValidator MinLength { get; init; } = new("Script code must be at least 1 char long.", 1);
};

public record ScriptLanguageInputValidators {
    public MetadataRequiredValidator Required { get; init; } = new("Script language is a required value.");
};