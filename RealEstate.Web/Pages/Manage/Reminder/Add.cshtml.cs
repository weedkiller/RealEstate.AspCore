﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Reminder
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : PageModel
    {
        private readonly IReminderService _reminderService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IReminderService reminderService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _reminderService = reminderService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ReminderInputViewModel NewReminder { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _reminderService.ReminderInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(IndexModel).Page());

                NewReminder = model;
                PageTitle = _localizer["EditReminder"];
            }
            else
            {
                NewReminder = new ReminderInputViewModel
                {
                    Date = DateTime.Now.GregorianToPersian(true)
                };
                PageTitle = _localizer["NewReminder"];
            }

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _reminderService.ReminderAddOrUpdateAsync(NewReminder, !NewReminder.IsNew, true)
            ).ConfigureAwait(false);

            return RedirectToPage(status != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
                {
                    status = message,
                    id = status != StatusEnum.Success ? NewReminder?.Id : null
                });
        }
    }
}