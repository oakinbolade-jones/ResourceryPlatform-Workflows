using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Web;
using Volo.Abp.Identity;

namespace ResourceryPlatformWorkflow.AuthServer.Pages.Account
{
    public class LoginModel : Volo.Abp.Account.Web.Pages.Account.LoginModel
    {
        public LoginModel(
            IAuthenticationSchemeProvider schemeProvider,
            IOptions<AbpAccountOptions> accountOptions,
            IOptions<IdentityOptions> identityOptions,
            IdentityDynamicClaimsPrincipalContributorCache claimsPrincipalContributorCache,
            IWebHostEnvironment webHostEnvironment
        ) : base(
            schemeProvider,
            accountOptions,
            identityOptions,
            claimsPrincipalContributorCache,
            webHostEnvironment
        )
        {
        }
    }
}
