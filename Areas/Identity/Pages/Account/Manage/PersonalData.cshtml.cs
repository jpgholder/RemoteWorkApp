// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RemoteWork.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {

        public Task<IActionResult> OnGet()
        {
            return Task.FromResult<IActionResult>(RedirectToPage("/Error/NotFound"));
        }
    }
}
