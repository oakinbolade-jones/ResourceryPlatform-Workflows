using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Localization;
using Volo.Abp.OpenIddict.Applications;

namespace ResourceryPlatformWorkflow.Pages;

public class IndexModel(
    IOpenIddictApplicationRepository openIdApplicationRepository,
    ILanguageProvider languageProvider,
    IConfiguration configuration
) : AbpPageModel
{
    public List<OpenIddictApplication> Applications { get; protected set; }

    public IReadOnlyList<LanguageInfo> Languages { get; protected set; }

    public string CurrentLanguage { get; protected set; }

    public string Environment { get; protected set; }

    protected IOpenIddictApplicationRepository OpenIdApplicationRepository { get; } =
        openIdApplicationRepository;

    protected ILanguageProvider LanguageProvider { get; } = languageProvider;

    protected IConfiguration Configuration { get; } = configuration;

    public async Task OnGetAsync()
    {
        Applications = await OpenIdApplicationRepository.GetListAsync();

        Languages = await LanguageProvider.GetLanguagesAsync();
        CurrentLanguage = CultureInfo.CurrentCulture.DisplayName;
        Environment = Configuration["App:Environment"] ?? "Unknown";
    }
}
