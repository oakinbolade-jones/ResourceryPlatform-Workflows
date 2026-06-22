namespace ResourceryPlatformWorkflow.Auth;

public class AuthServerOptions
{
    public string Authority { get; set; }        // external (public)
    public string MetaAuthority { get; set; }    // internal (docker)
}