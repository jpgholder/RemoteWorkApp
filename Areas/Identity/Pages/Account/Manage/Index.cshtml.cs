// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RemoteWork.Models;

namespace RemoteWork.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Обязательное поле")]
            [DisplayName("Логин")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Обязательное поле")]
            [DisplayName("Имя")]
            public string FirstName { get; set; }
            
            [Required(ErrorMessage = "Обязательное поле")]
            [DisplayName("Фамилия")]
            public string LastName { get; set; }
        }

        private Task LoadAsync(ApplicationUser user)
        {
            Input = new InputModel()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
        
        private async Task<ValidationResult> ValidateUsername(string username)
        {   
            var duplicate = _userManager.FindByNameAsync(username).Result;
            var user = await _userManager.GetUserAsync(User); ;
            if ( duplicate == null || user == duplicate)
                return ValidationResult.Success;
            return new ValidationResult("Пользователь с таким логином уже существует");
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            
            var isValid = ValidateUsername(Input.UserName).Result;
            if (isValid == ValidationResult.Success)
            {
                user.UserName = Input.UserName;
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                var res = await _userManager.UpdateAsync(user);
                if (res != IdentityResult.Success)
                {
                    foreach (var err in res.Errors)
                        ModelState.AddModelError(string.Empty,
                            err.Code == "InvalidUserName"
                                ? "Логин может состоять только из русских и латинских букв и цифр"
                                : err.Description);
                    await LoadAsync(user);
                    return Page();
                }
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Информация обновлена";
                return RedirectToPage();
            }
            await LoadAsync(user);
            ModelState.AddModelError(string.Empty, isValid.ErrorMessage!);
            return Page();
        }
    }
}
