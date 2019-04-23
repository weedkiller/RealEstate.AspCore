﻿using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class UserPropertyCategoryViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly UserPropertyCategory _entity;

        public UserPropertyCategoryViewModel(UserPropertyCategory entity, bool includeDeleted, Action<UserPropertyCategoryViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public void GetCategory(bool includeDeleted = false, Action<CategoryViewModel> action = null)
        {
            Category = _entity?.Category.Into(includeDeleted, action);
        }

        public UserViewModel User { get; private set; }
        public CategoryViewModel Category { get; private set; }
    }
}