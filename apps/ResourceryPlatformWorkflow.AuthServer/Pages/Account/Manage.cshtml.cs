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
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Phone]
            public string PhoneNumber { get; set; }
        }

        public async Task OnGetAsync()
        {
            var userId = _currentUser.GetId();
            var user = await _userManager.GetByIdAsync(userId);
            Input = new InputModel
            {
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = _currentUser.GetId();
            var user = await _userManager.GetByIdAsync(userId);

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

            if (!string.Equals(user.PhoneNumber, Input.PhoneNumber))
            {
                var phoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!phoneResult.Succeeded)
                {
                    foreach (var error in phoneResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
            }

            return RedirectToPage();
        }
    }
}
