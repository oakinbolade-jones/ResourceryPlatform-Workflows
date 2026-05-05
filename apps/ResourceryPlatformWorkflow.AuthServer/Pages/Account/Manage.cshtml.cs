using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace ResourceryPlatformWorkflow.AuthServer.Pages.Account
{
    [Authorize]
    public class ManageModel : PageModel
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
        private readonly ICurrentUser _currentUser;

        public ManageModel(IdentityUserManager userManager, ICurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public class InputModel
        {
            [Required]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Phone]
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
            SafeReturnUrl = NormalizeReturnUrl(ReturnUrl);
            Tab = NormalizeTab(Tab);

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
            SafeReturnUrl = NormalizeReturnUrl(ReturnUrl);
            Tab = "profile";

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Challenge();
            }

            var hasProfileNameChanges = false;
            if (!string.Equals(user.Name ?? string.Empty, Input.FirstName ?? string.Empty, StringComparison.Ordinal))
            {
                user.Name = Input.FirstName;
                hasProfileNameChanges = true;
            }

            if (!string.Equals(user.Surname ?? string.Empty, Input.LastName ?? string.Empty, StringComparison.Ordinal))
            {
                user.Surname = Input.LastName;
                hasProfileNameChanges = true;
            }

            if (!string.Equals(user.Email, Input.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
            }

            if (!string.Equals(user.PhoneNumber ?? string.Empty, Input.PhoneNumber ?? string.Empty, StringComparison.Ordinal))
            {
                var phoneResult = await _userManager.SetPhoneNumberAsync(user, string.IsNullOrWhiteSpace(Input.PhoneNumber) ? null : Input.PhoneNumber);
                if (!phoneResult.Succeeded)
                {
                    foreach (var error in phoneResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
            }

            if (hasProfileNameChanges)
            {
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
            }

            StatusMessage = "Profile updated successfully.";
            return RedirectToPage(new { returnUrl = SafeReturnUrl, tab = Tab });
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            SafeReturnUrl = NormalizeReturnUrl(ReturnUrl);
            Tab = "password";

            if (!ModelState.IsValid)
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

        private static string NormalizeReturnUrl(string returnUrl)
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

        private static string NormalizeTab(string tab)
        {
            return string.Equals(tab, "password", StringComparison.OrdinalIgnoreCase)
                ? "password"
                : "profile";
        }
    }
}
