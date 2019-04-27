﻿using Microsoft.EntityFrameworkCore;
using MoreLinq;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IItemService
    {
        Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel);

        Task<StatusEnum> RequestRejectAsync(string itemId, bool save);

        Task<StatusEnum> ItemRemoveAsync(string id);

        Task<List<ItemOutJsonViewModel>> ItemListAsync();

        Task<PaginationViewModel<ItemViewModel>> RequestListAsync(DealRequestSearchViewModel model);

        Task<(StatusEnum, Item)> ItemAddOrUpdateAsync(ItemInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Item)> RequestAsync(DealRequestInputViewModel model, bool save);

        Task<Item> ItemEntityAsync(string id);

        Task<ItemInputViewModel> ItemInputAsync(string id);

        Task<ItemViewModel> ItemAsync(string id, DealStatusEnum? specificStatus);

        Task<(StatusEnum, Item)> ItemAddAsync(ItemInputViewModel model, bool save);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly ICustomerService _customerService;
        private readonly IFeatureService _featureService;
        private readonly IPropertyService _propertyService;
        private readonly DbSet<Deal> _itemRequests;
        private readonly DbSet<Item> _items;
        private readonly DbSet<Ownership> _ownerships;
        private readonly DbSet<Customer> _customers;
        private readonly DbSet<DealRequest> _dealRequests;
        private readonly DbSet<Applicant> _applicants;

        public ItemService(
            IBaseService baseService,
            IUnitOfWork unitOfWork,
            IFeatureService featureService,
            IPropertyService propertyService,
            ICustomerService customerService
            )
        {
            _baseService = baseService;
            _unitOfWork = unitOfWork;
            _featureService = featureService;
            _customerService = customerService;
            _propertyService = propertyService;
            _itemRequests = _unitOfWork.Set<Deal>();
            _applicants = _unitOfWork.Set<Applicant>();
            _items = _unitOfWork.Set<Item>();
            _ownerships = _unitOfWork.Set<Ownership>();
            _customers = _unitOfWork.Set<Customer>();
            _dealRequests = _unitOfWork.Set<DealRequest>();
        }

        public async Task<StatusEnum> RequestRejectAsync(string itemId, bool save)
        {
            if (string.IsNullOrEmpty(itemId))
                return StatusEnum.IdIsNull;

            var item = await _items.FirstOrDefaultAsync(x => x.Id == itemId).ConfigureAwait(false);
            if (item == null)
                return StatusEnum.ItemIsNull;

            var (requestStatus, newState) = await RequestAddStateAsync(item.Id, DealStatusEnum.Rejected).ConfigureAwait(false);
            return requestStatus;
        }

        private async Task<(StatusEnum, DealRequest)> RequestAddStateAsync(string itemId, DealStatusEnum newState)
        {
            if (string.IsNullOrEmpty(itemId))
                return new ValueTuple<StatusEnum, DealRequest>(StatusEnum.ItemIsNull, null);

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null)
                return new ValueTuple<StatusEnum, DealRequest>(StatusEnum.UserIsNull, null);

            var lastRequest = await _dealRequests.OrderDescendingByCreationDateTime().Where(x => x.ItemId == itemId).FirstOrDefaultAsync().ConfigureAwait(false);
            if (lastRequest?.Status == newState)
                return new ValueTuple<StatusEnum, DealRequest>(StatusEnum.AlreadyExists, null);

            var addState = await _baseService.AddAsync(new DealRequest
            {
                ItemId = itemId,
                Status = newState
            }, null, true).ConfigureAwait(false);
            return addState;
        }

        public async Task<StatusEnum> SyncApplicantsAsync(Item item, DealRequestInputViewModel model, bool save)
        {
            var allowedCustomers = await _customerService.ListJsonAsync(item.Id).ConfigureAwait(false);
            if (allowedCustomers?.Any() != true)
                return StatusEnum.CustomerIsNull;

            var currentUser = _baseService.CurrentUser();
            if (currentUser == null) return StatusEnum.UserIsNull;

            var mustBeLeft = item.Applicants.Where(ent => model.Customers.Any(mdl => ent.CustomerId == mdl.CustomerId)).ToList();
            var mustBeRemoved = item.Applicants.Where(x => !mustBeLeft.Contains(x)).ToList();
            if (mustBeRemoved.Count > 0)
            {
                foreach (var redundant in mustBeRemoved)
                {
                    await _baseService.UpdateAsync(redundant,
                        _ => redundant.ItemId = null, null, false, StatusEnum.ApplicantIsNull).ConfigureAwait(false);
                }
            }

            if (model.Customers?.Any() != true)
                return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);

            foreach (var customer in model.Customers)
            {
                var source = item.Applicants.FirstOrDefault(ent => ent.CustomerId == customer.CustomerId);
                if (source == null)
                {
                    var appli = await _applicants.FirstOrDefaultAsync(x => x.CustomerId == customer.CustomerId && x.UserId == currentUser.Id).ConfigureAwait(false);
                    if (appli != null)
                    {
                        await _baseService.UpdateAsync(appli,
                            _ => appli.ItemId = item.Id, null, false, StatusEnum.ApplicantIsNull).ConfigureAwait(false);
                    }
                    else
                    {
                        var cnt = await _customers.FirstOrDefaultAsync(x => x.Id == customer.CustomerId).ConfigureAwait(false);
                        if (cnt == null)
                            continue;

                        var (addStatus, newApplicant) = await _baseService.AddAsync(_ => new Applicant
                        {
                            CustomerId = customer.CustomerId,
                            UserId = currentUser.Id,
                            Type = ApplicantTypeEnum.Applicant,
                            ItemId = item.Id
                        },
                            null,
                            false).ConfigureAwait(false);
                    }
                }
                else
                {
                    var applicant = await _customerService.ApplicantEntityAsync(customer.ApplicantId).ConfigureAwait(false);
                    await _baseService.UpdateAsync(applicant,
                        _ => applicant.ItemId = item.Id, null, false, StatusEnum.ApplicantIsNull).ConfigureAwait(false);
                }
            }

            return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);
        }

        public async Task<List<ItemOutJsonViewModel>> ItemListAsync()
        {
            var query = from item in _items
                        let requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
                        let lastRequest = requests.FirstOrDefault()
                        where !requests.Any() || lastRequest.Status == DealStatusEnum.Rejected
                        select item;
            var models = await query.ToListAsync().ConfigureAwait(false);
            if (models?.Any() != true)
                return default;

            var result = new List<ItemOutJsonViewModel>();
            foreach (var model in models)
            {
                var converted = model.Into<Item, ItemViewModel>(false, act =>
                {
                    act.GetProperty(false, act2 =>
                    {
                        act2.GetCategory();
                        act2.GetDistrict();
                        act2.GetPropertyFacilities(false, act3 => act3.GetFacility());
                        act2.GetPropertyFeatures(false, act3 => act3.GetFeature());
                        act2.GetPropertyOwnerships(false, act3 => act3.GetOwnerships(false, act4 => act4.GetCustomer()));
                    });
                    act.GetCategory();
                    act.GetItemFeatures(false, act2 => act2.GetFeature());
                });
                if (converted == null)
                    continue;

                var item = new ItemOutJsonViewModel
                {
                    Property = new PropertyOutJsonViewModel
                    {
                        Address = converted.Property?.Address,
                        Category = converted.Property?.Category?.Name,
                        District = converted.Property?.District?.Name,
                        Description = converted.Description,
                        Facilities = converted.Property?.PropertyFacilities?.Select(x => x.Facility?.Name).ToList(),
                        Ownerships = converted.Property?.PropertyOwnerships?.SelectMany(x => x.Ownerships?.Select(c => c.Customer?.Name)).ToList(),
                        Features = converted.Property?.PropertyFeatures?.Select(x => new ValueTuple<string, string>(x.Feature?.Name, x.Value)).ToList(),
                    },
                    Category = converted.Category?.Name,
                    ItemFeatures = converted.ItemFeatures?.Select(x => new ValueTuple<string, string>(x.Feature.Name, x.Value)).ToList()
                };
                result.Add(item);
            }
            return result;
        }

        public async Task<PaginationViewModel<ItemViewModel>> ItemListAsync(ItemSearchViewModel searchModel)
        {
            var query = from item in _items
                        let requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
                        let lastRequest = requests.FirstOrDefault()
                        where !requests.Any() || lastRequest.Status == DealStatusEnum.Rejected
                        select item;

            if (searchModel != null)
            {
                query = query.SearchBy(searchModel.CategoryId, x => x.CategoryId);
                query = query.SearchBy(searchModel.Address, x => x.Property.Address);
                query = query.SearchBy(searchModel.ItemId, x => x.Id);

                if (!string.IsNullOrEmpty(searchModel.CustomerId))
                {
                    query = query.Where(x => x.Applicants.Any(c => c.CustomerId == searchModel.CustomerId)
                                             || x.Property.PropertyOwnerships.Any(v => v.Ownerships.Any(b => b.CustomerId == searchModel.CustomerId)));
                }

                if (!string.IsNullOrEmpty(searchModel.FeatureName))
                {
                    if (searchModel.FromValue != null && searchModel.FromValue > 0)
                        query = query.Where(x =>
                            x.Property.PropertyFeatures.Any(c => c.Feature.Name == searchModel.FeatureName && double.Parse(c.Value) >= searchModel.FromValue));

                    if (searchModel.ToValue != null && searchModel.ToValue > 0)
                        query = query.Where(x =>
                            x.Property.PropertyFeatures.Any(c => c.Feature.Name == searchModel.FeatureName && double.Parse(c.Value) <= searchModel.FromValue));
                }
            }

            var result = await _baseService.PaginateAsync(query, searchModel?.PageNo ?? 1,
                item => item.Into<Item, ItemViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetCategory();
                    act.GetItemFeatures(false, act2 => act2.GetFeature());
                    act.GetProperty(false, act3 =>
                    {
                        act3.GetPropertyOwnerships(true, act4 => act4.GetOwnerships(false, act5 => act5.GetCustomer()));
                        act3.GetPropertyFacilities(false, act4 => act4.GetFacility());
                        act3.GetPropertyFeatures(false, act4 => act4.GetFeature());
                        act3.GetCategory();
                        act3.GetDistrict();
                    });
                })).ConfigureAwait(false);
            return result;
        }

        public async Task<PaginationViewModel<ItemViewModel>> RequestListAsync(DealRequestSearchViewModel model)
        {
            var query = from item in _items
                        let requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
                        let lastRequest = requests.FirstOrDefault()
                        where requests.Any() && lastRequest.Status == DealStatusEnum.Requested
                        select item;

            var result = await _baseService.PaginateAsync(query, model?.PageNo ?? 1,
                item => item.Into<Item, ItemViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetProperty(false, act3 => act3.GetPropertyOwnerships(false, act4 => act4.GetOwnerships(false, act5 => act5.GetCustomer())));
                    act.GetItemFeatures(false, act3 => act3.GetFeature());
                    act.GetApplicants(false, act6 => act6.GetCustomer());
                })).ConfigureAwait(false);
            return result;
        }

        public async Task<(StatusEnum, Item)> RequestAsync(DealRequestInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ModelIsNull, null);

            if (model?.Customers?.Any() != true)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ApplicantsEmpty, null);

            var item = await _items.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            if (item == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ItemIsNull, null);

            var (requestAddStatus, newState) = await RequestAddStateAsync(item.Id, DealStatusEnum.Requested).ConfigureAwait(false);
            if (requestAddStatus != StatusEnum.Success)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.DealRequestIsNull, null);

            await SyncApplicantsAsync(item, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(item, save).ConfigureAwait(false);
        }

        public async Task<ItemInputViewModel> ItemInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var entity = await _items.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = entity?.Into<Item, ItemViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
            {
                act.GetCategory();
                act.GetItemFeatures(false, act2 => act2.GetFeature());
                act.GetProperty(false, act3 => act3.GetPropertyOwnerships(false, act4 => act4.GetOwnerships(false, act5 => act5.GetCustomer())));
            });

            if (viewModel == null)
                return default;

            var result = new ItemInputViewModel
            {
                Id = viewModel.Id,
                Description = viewModel.Description,
                CategoryId = viewModel.Category?.Id,
                ItemFeatures = viewModel.ItemFeatures?.Select(x => new FeatureJsonValueViewModel
                {
                    Id = x.Feature?.Id,
                    Name = x.Feature?.Name,
                    Value = x.Value
                }).ToList(),
                PropertyId = viewModel.Property?.Id
            };
            return result;
        }

        public async Task<ItemViewModel> ItemAsync(string id, DealStatusEnum? specificStatus)
        {
            var query = _items.Select(item => new
            {
                item,
                requests = item.DealRequests.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime)
            })
                .Select(processed => new
                {
                    processedItem = processed,
                    lastRequest = processed.requests.FirstOrDefault()
                });

            if (specificStatus != null)
            {
                switch (specificStatus)
                {
                    case DealStatusEnum.Rejected:
                        query = query.Where(editedItem => !editedItem.processedItem.requests.Any() || editedItem.lastRequest.Status == DealStatusEnum.Rejected);
                        break;

                    case DealStatusEnum.Requested:
                    case DealStatusEnum.Finished:
                    default:
                        query = query.Where(editedItem => editedItem.processedItem.requests.Any() && editedItem.lastRequest.Status == specificStatus);
                        break;
                }
            }

            var que = query.Select(editedItem => editedItem.processedItem.item);
            var entity = await que.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (entity == null)
                return default;

            var viewModel = entity?.Into<Item, ItemViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
            {
                act.GetCategory();
                act.GetItemFeatures(false, act2 => act2.GetFeature());
                act.GetProperty(false, act3 => act3.GetPropertyOwnerships(false, act4 => act4.GetOwnerships(false, act5 => act5.GetCustomer())));
                act.GetApplicants(false, act2 => act2.GetCustomer());
            });
            return viewModel;
        }

        public async Task<Item> ItemEntityAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var entity = await _items.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return entity;
        }

        public async Task<StatusEnum> ItemRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var property = await ItemEntityAsync(id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(property,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public Task<(StatusEnum, Item)> ItemAddOrUpdateAsync(ItemInputViewModel model, bool update, bool save)
        {
            return update
                ? ItemUpdateAsync(model, save)
                : ItemAddAsync(model, save);
        }

        private async Task<(StatusEnum, Item)> ItemUpdateAsync(ItemInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.IdIsNull, null);

            var entity = await ItemEntityAsync(model.Id).ConfigureAwait(false);
            var (updateStatus, updatedItem) = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.CategoryId = model.CategoryId;
                    entity.Description = model.Description;
                    entity.PropertyId = model.PropertyId;
                }, null, false, StatusEnum.PropertyIsNull).ConfigureAwait(false);

            if (updatedItem == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ItemIsNull, null);

            await ItemSyncAsync(updatedItem, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedItem, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Item)> ItemAddAsync(ItemInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Item>(StatusEnum.ModelIsNull, null);

            var (itemAddStatus, newItem) = await _baseService.AddAsync(new Item
            {
                CategoryId = model.CategoryId,
                Description = model.Description,
                PropertyId = model.PropertyId,
            }, null, false).ConfigureAwait(false);

            await ItemSyncAsync(newItem, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newItem, save).ConfigureAwait(false);
        }

        private async Task<StatusEnum> ItemSyncAsync(Item newItem, ItemInputViewModel model, bool save)
        {
            var syncFeatures = await _baseService.SyncAsync(
                newItem.ItemFeatures,
                model.ItemFeatures,
                (feature, currentUser) => new ItemFeature
                {
                    FeatureId = feature.Id,
                    Value = feature.Value,
                    ItemId = newItem.Id
                }, (currentFeature, newFeature) => currentFeature.FeatureId == newFeature.Id,
                null,
                save).ConfigureAwait(false);
            return syncFeatures;
        }
    }
}