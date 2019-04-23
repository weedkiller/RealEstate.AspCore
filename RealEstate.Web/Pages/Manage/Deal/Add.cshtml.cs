﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Deal
{
    public class AddModel : PageModel
    {
        private readonly IDealService _dealService;
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IDealService dealService,
            IItemService itemService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _dealService = dealService;
            _itemService = itemService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public DealInputViewModel NewDeal { get; set; }

        public ItemViewModel ItemInfo { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public string DealStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _dealService.DealInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(Deal.IndexModel).Page());

                var info = await _itemService.ItemAsync(model.ItemId, null).ConfigureAwait(false);
                if (info == null)
                    return RedirectToPage(typeof(Deal.IndexModel).Page());

                ItemInfo = info;
                NewDeal = model;
                PageTitle = _localizer["EditDeal"];
            }
            else
            {
                PageTitle = _localizer["NewDeal"];
            }

            DealStatus = !string.IsNullOrEmpty(status) ? status.To<StatusEnum>().GetDisplayName() : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var finalStatus = ModelState.IsValid
                ? (await _dealService.AddOrUpdateAsync(NewDeal, !NewDeal.IsNew, true).ConfigureAwait(false)).Item1
                : StatusEnum.RetryAfterReview;

            DealStatus = finalStatus.GetDisplayName();
            if (finalStatus != StatusEnum.Success || !NewDeal.IsNew)
                return RedirectToPage(typeof(Deal.AddModel).Page(), new
                {
                    id = NewDeal?.Id,
                    status = finalStatus
                });

            ModelState.Clear();
            NewDeal = default;

            return RedirectToPage(typeof(Deal.AddModel).Page(), new
            {
                id = NewDeal?.Id,
                status = StatusEnum.Success
            });
        }
    }
}