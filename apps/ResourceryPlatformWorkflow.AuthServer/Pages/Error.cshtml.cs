using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ResourceryPlatformWorkflow.Administration.Localization;

namespace ResourceryPlatformWorkflow.AuthServer.Pages
{
    public class ErrorModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;
        private readonly IStringLocalizer<AdministrationResource> _localizer;

        public ErrorModel(ILogger<ErrorModel> logger, IStringLocalizer<AdministrationResource> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        [BindProperty(SupportsGet = true)]
        public string Title { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string Message { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Referrer { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                Title = _localizer["Common:ApiError:Title"];
            }

            if (string.IsNullOrWhiteSpace(Message))
            {
                Message = _localizer["Common:ApiError:NoCode"];
            }

            if (string.IsNullOrWhiteSpace(ReturnUrl) && !string.IsNullOrWhiteSpace(Request.Headers.Referer))
            {
                ReturnUrl = Request.Headers.Referer.ToString();
            }

            _logger.LogWarning(
                "Custom auth error page rendered. Title: {Title}. Message: {Message}. ReturnUrl: {ReturnUrl}. Referrer: {Referrer}",
                Title,
                Message,
                ReturnUrl,
                Referrer);
        }
    }
}
