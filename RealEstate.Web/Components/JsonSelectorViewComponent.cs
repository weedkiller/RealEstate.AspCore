﻿using Microsoft.AspNetCore.Mvc;
using RealEstate.ViewModels;

namespace RealEstate.Web.Components
{
    public class JsonSelectorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(JsonSelectorViewModel model)
        {
            return View(model ?? new JsonSelectorViewModel());
        }
    }
}