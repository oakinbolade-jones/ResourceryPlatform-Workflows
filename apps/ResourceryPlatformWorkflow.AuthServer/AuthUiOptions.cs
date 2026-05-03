namespace ResourceryPlatformWorkflow;

public class AuthUiOptions
{
    public AuthUiPageOptions Login { get; set; } = new();

    public AuthUiPageOptions Register { get; set; } = new();
}

public class AuthUiPageOptions
{
    public string? TemplatePartial { get; set; }
}
