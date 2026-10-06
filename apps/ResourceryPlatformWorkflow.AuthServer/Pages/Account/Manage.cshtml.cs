using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace ResourceryPlatformWorkflow.AuthServer.Pages.Account
{
    [Authorize]
    [UnitOfWork]
    public class ManageModel : AbpPageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Tab { get; set; }

        public string SafeReturnUrl { get; private set; }

        [BindProperty]
        public ChangePasswordInputModel ChangePasswordInput { get; set; } = new ChangePasswordInputModel();

        [TempData]
        public string StatusMessage { get; set; }

        private readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<ManageModel> _logger;

        public ManageModel(
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository,
            ICurrentUser currentUser,
            ILogger<ManageModel> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public class InputModel
        {
            [Required]
            [StringLength(64)]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [StringLength(64)]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [StringLength(256)]
            public string Email { get; set; } = string.Empty;

            [Phone]
            [StringLength(16)]
            public string PhoneNumber { get; set; } = string.Empty;
        }

        public class ChangePasswordInputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; } = string.Empty;

            [Required]
            [StringLength(128, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(NewPassword))]
            public string ConfirmNewPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            SafeReturnUrl = NormalizeLocalReturnUrl(ReturnUrl);
            Tab = NormalizeLocalTab(Tab);

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Challenge();
            }

            Input = new InputModel
            {
                FirstName = user.Name ?? string.Empty,
                LastName = user.Surname ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            SafeReturnUrl = NormalizeLocalReturnUrl(ReturnUrl);
            Tab = "profile";

            ModelState.Clear();
            if (!TryValidateModel(Input, nameof(Input)))
            {
                var modelStateErrors = string.Join(" | ",
                    ModelState
                        .Where(x => x.Value?.Errors?.Count > 0)
                        .Select(x => $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}"));

                _logger.LogWarning(
                    "Manage profile post failed model validation for user {UserId}. Errors: {Errors}",
                    _currentUser.Id,
                    modelStateErrors);
                return Page();
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Challenge();
            }

            var firstName = (Input.FirstName ?? string.Empty).Trim();
            var lastName = (Input.LastName ?? string.Empty).Trim();
            var email = (Input.Email ?? string.Empty).Trim();
            var phoneNumber = string.IsNullOrWhiteSpace(Input.PhoneNumber)
                ? null
                : Input.PhoneNumber.Trim();

            var hasAnyChanges = false;

            if (!string.Equals(user.Name ?? string.Empty, firstName, StringComparison.Ordinal))
            {
                user.Name = firstName;
                hasAnyChanges = true;
            }

            if (!string.Equals(user.Surname ?? string.Empty, lastName, StringComparison.Ordinal))
            {
                user.Surname = lastName;
                hasAnyChanges = true;
            }

            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                var emailResult = await _userManager.SetEmailAsync(user, email);
                if (!emailResult.Succeeded)
                {
                    _logger.LogWarning("SetEmailAsync failed for user {UserId}. Errors: {Errors}",
                        _currentUser.Id,
                        string.Join("; ", emailResult.Errors));
                    foreach (var error in emailResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }

                hasAnyChanges = true;
            }

            if (!string.Equals(user.PhoneNumber ?? string.Empty, phoneNumber ?? string.Empty, StringComparison.Ordinal))
            {
                var phoneResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
                if (!phoneResult.Succeeded)
                {
                    _logger.LogWarning("SetPhoneNumberAsync failed for user {UserId}. Errors: {Errors}",
                        _currentUser.Id,
                        string.Join("; ", phoneResult.Errors));
                    foreach (var error in phoneResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }

                hasAnyChanges = true;
            }

            if (hasAnyChanges)
            {
                await _userRepository.UpdateAsync(user, autoSave: true);
            }

            StatusMessage = "Profile updated successfully.";
            return RedirectToPage(new { returnUrl = SafeReturnUrl, tab = Tab });
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            SafeReturnUrl = NormalizeLocalReturnUrl(ReturnUrl);
            Tab = "password";

            ModelState.Clear();
            if (!TryValidateModel(ChangePasswordInput, nameof(ChangePasswordInput)))
            {
                return Page();
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Challenge();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(
                user,
                ChangePasswordInput.CurrentPassword,
                ChangePasswordInput.NewPassword
            );

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            StatusMessage = "Password changed successfully.";
            return RedirectToPage(new { returnUrl = SafeReturnUrl, tab = Tab });
        }

        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            if (!_currentUser.Id.HasValue)
            {
                return null;
            }

            return await _userManager.GetByIdAsync(_currentUser.Id.Value);
        }

        private static string NormalizeLocalReturnUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return "/";
            }

            if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var absoluteUri)
                && (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps))
            {
                return absoluteUri.ToString();
            }

            if (Uri.TryCreate(returnUrl, UriKind.Relative, out var relativeUri))
            {
                var value = relativeUri.ToString();
                return value.StartsWith("/", StringComparison.Ordinal) ? value : "/" + value;
            }

            return "/";
        }

        private static string NormalizeLocalTab(string tab)
        {
            return string.Equals(tab, "password", StringComparison.OrdinalIgnoreCase)
                ? "password"
                : "profile";
        }

    }
}
