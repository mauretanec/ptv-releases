﻿/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PTV.Domain.Model.Models;
using System.Linq.Expressions;
using PTV.Domain.Logic.Channels;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Caches;
using Microsoft.AspNetCore.Http;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IOrganizationService), RegisterType.Transient)]
    [DataProviderServiceNameAttribute("Organizations")]
    internal class OrganizationService : ServiceBase, IOrganizationService, IDataProviderService
    {
        private readonly IContextManager contextManager;
        private ILogger logger;
        private DataUtils dataUtils;
        private ServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private OrganizationLogic organizationLogic;
        private ITypesCache typesCache;
        private IAddressService addressService;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;

        public OrganizationService(IContextManager contextManager,
                                   ITranslationEntity translationEntToVm,
                                   ITranslationViewModel translationVmtoEnt,
                                   ILogger<OrganizationService> logger,
                                   OrganizationLogic organizationLogic,
                                   ServiceUtilities utilities,
                                   DataUtils dataUtils,
                                   ICommonServiceInternal commonService,
                                   IAddressService addressService,
                                   IPublishingStatusCache publishingStatusCache,
                                   ILanguageCache languageCache,
                                   IVersioningManager versioningManager,
                                   IUserOrganizationChecker userOrganizationChecker,
                                   ITypesCache typesCache)
            : base(translationEntToVm, translationVmtoEnt, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.organizationLogic = organizationLogic;
            this.dataUtils = dataUtils;
            this.addressService = addressService;
            this.languageCache = languageCache;
            this.versioningManager = versioningManager;
            this.typesCache = typesCache;
            // ITypesCache cannot be injected in constructor because it uses internal access modifier
            // get the typescache from requestservices (IServiceProvider) basically using Locator pattern here
            //typesCache = ctxAccessor.HttpContext.RequestServices.GetService(typeof(ITypesCache)) as ITypesCache;
        }

        public IVmOpenApiGuidPageVersionBase GetOrganizations(DateTime? date, int openApiVersion, int pageNumber = 1, int pageSize = 1000, bool archived = false)
        {
            var vm = new VmOpenApiOrganizationGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<OrganizationVersioned> organizations = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                if (archived)
                {
                    organizations = GetArchivedEntities<OrganizationVersioned, Organization, OrganizationLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.OrganizationNames));
                }
                else
                {
                    organizations = GetPublishedEntities<OrganizationVersioned, Organization, OrganizationLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.OrganizationNames));                                
                }
                
            });
            return GetGuidPage(organizations, vm, openApiVersion);
        }

        public IVmOpenApiGuidPageVersionBase GetOrganizationsSaha(DateTime? date, int pageNumber, int pageSize)
        {
            var vm = new VmOpenApiOrganizationSahaGuidPage(pageNumber, pageSize);
            if (pageNumber <= 0) return vm;

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var deletedId = PublishingStatusCache.Get(PublishingStatus.Deleted);
            var oldPublishedId = PublishingStatusCache.Get(PublishingStatus.OldPublished);

            IList<Expression<Func<OrganizationVersioned, bool>>> filters = new List<Expression<Func<OrganizationVersioned, bool>>>();
            // published and archived versions
            filters.Add(o => (o.PublishingStatusId == publishedId && o.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) ||
                    ((o.PublishingStatusId == deletedId || o.PublishingStatusId == oldPublishedId) && (!o.UnificRoot.Versions.Any(x => x.PublishingStatusId == publishedId))));
            // Get only main and two sub organization levels.
            filters.Add(o => o.ParentId == null || // main level
                    (o.Parent != null && o.Parent.Versions.Any(p => (p.PublishingStatusId == publishedId || p.PublishingStatusId == deletedId) && (p.ParentId == null ||// first level child
                    p.Parent != null && p.Parent.Versions.Any(pp => (pp.PublishingStatusId == publishedId || pp.PublishingStatusId == deletedId) && pp.ParentId == null)))));// second level child
            // Date filter
            if (date.HasValue)
            {
                filters.Add(g => g.Modified > date.Value);
            }
            List<OrganizationVersioned> organizations = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                organizations = GetDistinctEntitiesForPage<OrganizationVersioned, Organization>(vm, unitOfWork, q => q.Include(i => i.OrganizationNames), filters);                
            });

            if (organizations?.Count > 0)
            {
                vm.ItemList = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationSaha>(organizations).ToList();
            }
            return vm;
        }

        public IVmOpenApiGuidPageVersionBase GetOrganizationsByMunicipality(Guid municipalityId, DateTime? date, int pageNumber, int pageSize)
        {
            var vm = new VmOpenApiOrganizationGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            List<OrganizationVersioned> organizations = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                // Areas related to defined municipality
                var areas = unitOfWork.CreateRepository<IAreaMunicipalityRepository>().All()
                    .Where(a => a.MunicipalityId == municipalityId).Select(a => a.AreaId).ToList();

                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                var municipalityTypeId = typesCache.Get<OrganizationType>(OrganizationTypeEnum.Municipality.ToString());
                IList<Expression<Func<OrganizationVersioned, bool>>> filters = new List<Expression<Func<OrganizationVersioned, bool>>>();
                // Is the municipality in 'Åland'? So do we need to include also AreaInformationType WholeCountryExceptAlandIslands?
                if (IsAreaInAland(unitOfWork, areas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    filters.Add(o => (o.TypeId == municipalityTypeId && o.MunicipalityId == municipalityId) || // For municipalilty organizations check attachec municipality (PTV-3423)
                        (o.TypeId != municipalityTypeId && // For all other organization types let's check areas (PTV-3423)
                        (o.AreaInformationTypeId == wholeCountryId || (o.OrganizationAreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || o.OrganizationAreas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))))));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    filters.Add(o => (o.TypeId == municipalityTypeId && o.MunicipalityId == municipalityId) || // For municipalilty organizations check attachec municipality (PTV-3423)
                        (o.TypeId != municipalityTypeId &&  // For all other organization types let's check areas (PTV-3423)
                        (o.AreaInformationTypeId == wholeCountryId || o.AreaInformationTypeId == wholeCountryExceptAlandId || 
                            (o.OrganizationAreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || o.OrganizationAreas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))))));                                   
                }
               
                organizations = GetPublishedEntities<OrganizationVersioned, Organization, OrganizationLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.OrganizationNames), filters);
            });

            return GetGuidPage(organizations, vm);
        }

        public IVmOpenApiOrganizationVersionBase GetOrganizationById(Guid id, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetOrganizationById(unitOfWork, id, openApiVersion, getOnlyPublished);
            });

            return result;
        }

        public IVmOpenApiOrganizationSaha GetOrganizationSahaById(Guid id)
        {
            IVmOpenApiOrganizationSaha result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                // Get the right version id
                Guid? entityId = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published);
                if (!entityId.IsAssigned())
                {
                    entityId = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Deleted);
                }
                if (entityId.IsAssigned())
                {
                    var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

                    var resultTemp = unitOfWork.ApplyIncludes(organizationRep.All().Where(o => o.Id.Equals(entityId.Value)), q =>
                        q.Include(i => i.OrganizationNames)
                        .Include(i => i.Parent).ThenInclude(i => i.Versions)
                        ).FirstOrDefault();

                    result = TranslationManagerToVm.Translate<OrganizationVersioned, VmOpenApiOrganizationSaha>(resultTemp);

                    if (result == null)
                    {
                        throw new Exception(CoreMessages.OpenApi.RecordNotFound);
                    }
                }
            });

            return result;
        }

        public IVmOpenApiGuidPageVersionBase GetGuidPage(IList<OrganizationVersioned> organizations, VmOpenApiOrganizationGuidPage vm, int openApiVersion = 7)
        {
            if (organizations != null && organizations.Count > 0)
            {
                vm.ItemList = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationItem>(organizations).ToList();
            }

            if (openApiVersion < 7)
            {
                return vm.ConvertToVersion6();
            }

            return vm;
        }

        private IVmOpenApiOrganizationVersionBase GetOrganizationById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            try
            {
                // Get the right version id
                Guid? entityId = null;
                if (getOnlyPublished)
                {
                    entityId = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published);
                }
                else
                {
                    entityId = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, id, null, false);
                }
                if (entityId.IsAssigned())
                {
                    result = GetOrganizationWithDetails(unitOfWork, entityId.Value, openApiVersion, getOnlyPublished);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            return result;
        }

        public IList<IVmOpenApiOrganizationVersionBase> GetOrganizationsByBusinessCode(string code, int openApiVersion)
        {
            try
            {
                IList<IVmOpenApiOrganizationVersionBase> results = new List<IVmOpenApiOrganizationVersionBase>();

                //Expression<Func<Organization, bool>> filter = o => o.Business.Code != null && o.Business.Code.Equals(code);
                //return GetOrganizationsWithDetails(filter);

                // Performance fix, replace above code. Locally the above code executed 1300ms and now in ~150ms (excluding the first call "warm up")
                // in training env it took 9 to 10 seconds
                // the query is very slow when using navigation property to filter
                // first get list of organization ids and use those to fetch the information
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var guidList = new List<Guid>();
                    var bidRepo = unitOfWork.CreateRepository<IBusinessRepository>();
                    var businessIds = bidRepo.All().Where(bid => bid.Code != null && bid.Code.Equals(code)).Select(b => b.Id).ToList();

                    if (businessIds.Count > 0)
                    {
                        var orgRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                        // Get organization versions that have the defined business code and which are published.
                        // the contains is evaluated in client because EF doesn't currently handle the nullable type
                        // https://github.com/aspnet/EntityFramework/issues/4114 (closed, we use this same solution .HasValue but still evaluated locally)
                        // https://github.com/aspnet/EntityFramework/issues/4247 relates to the previous and is currently labeled as enhancement
                        guidList = orgRepo.All().Where(o => o.BusinessId.HasValue && businessIds.Contains(o.BusinessId.Value)).Where(PublishedFilter<OrganizationVersioned>()).Where(ValidityFilter<OrganizationVersioned>()).Select(o => o.Id).ToList();
                    }
                    if (guidList.Count > 0)
                    {
                        results = GetOrganizationsWithDetails(unitOfWork, guidList, openApiVersion);
                    }
                });

                return results;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with code {0}. {1}", code, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        public IVmOpenApiOrganizationVersionBase GetOrganizationByOid(string oid, int openApiVersion)
        {
            try
            {
                IVmOpenApiOrganizationVersionBase result = null;

                contextManager.ExecuteReader(unitOfWork =>
                {
                    var orgRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var guidList = orgRepo.All().Where(o => o.Oid.Equals(oid)).Where(PublishedFilter<OrganizationVersioned>()).Where(ValidityFilter<OrganizationVersioned>()).Select(o => o.Id).ToList();
                    if (guidList.Count > 0)
                    {
                        result = GetOrganizationWithDetails(unitOfWork, guidList.FirstOrDefault(), openApiVersion);
                    }
                });
                return result;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting an organization with Oid {0}. {1}", oid, ex.Message);
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        public Guid GetOrganizationIdByOid(string oid)
        {
            var guid = Guid.Empty;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var org = orgRep.All().FirstOrDefault(o => o.Oid.Equals(oid));
                if (org != null)
                {
                    guid = org.UnificRootId;
                }
            });
            return guid;
        }

        public Guid? GetOrganizationIdByBusinessCode(string businessCode)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var orgId = orgRep.All().Where(o => o.Business.Code == businessCode).Select(i => i.UnificRootId).FirstOrDefault();
                if (!orgId.IsAssigned())
                {
                    return (Guid?)null;
                }
                return orgId;
            });
        }


        public Guid GetOrganizationIdBySource(string sourceId)
        {
            var guid = Guid.Empty;
            var userId = utilities.GetRelationIdForExternalSource();
            contextManager.ExecuteReader(unitOfWork =>
            {
                guid = GetPTVId<Organization>(sourceId, userId, unitOfWork);
            });
            return guid;
        }

        public IVmOpenApiOrganizationVersionBase GetOrganizationBySource(string sourceId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            var userId = utilities.GetRelationIdForExternalSource();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rootId = GetPTVId<Organization>(sourceId, userId, unitOfWork);
                result = GetOrganizationById(unitOfWork, rootId, openApiVersion, getOnlyPublished);
            });
            return result;
        }

        public IVmOpenApiOrganizationVersionBase AddOrganization(IVmOpenApiOrganizationInVersionBase vm, bool allowAnonymous, int openApiVersion)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var organization = new OrganizationVersioned();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists
                if (ExternalSourceExists<Organization>(vm.SourceId, userId, unitOfWork))
                {
                    throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
                }

                organization = TranslationManagerToEntity.Translate<IVmOpenApiOrganizationInVersionBase, OrganizationVersioned>(vm, unitOfWork);

                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                organizationRep.Add(organization);

                // Create the mapping between external source id and PTV id
                if (!string.IsNullOrEmpty(vm.SourceId))
                {
                    SetExternalSource(organization.UnificRoot, vm.SourceId, userId, unitOfWork);
                }

                unitOfWork.Save(saveMode);
            });

            // Update the map coordinates for addresses
            if (organization?.OrganizationAddresses?.Count > 0)
            {
                // only for visiting addresses which are of type street
                var visitingAddressId = typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString());
                var streetId = typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString());
                var addresses = organization.OrganizationAddresses.Where(a => a.CharacterId == visitingAddressId && a.Address.TypeId == streetId).Select(x => x.AddressId);
                addressService.UpdateAddress(addresses.ToList());
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<OrganizationVersioned, OrganizationLanguageAvailability>(organization.Id, i => i.OrganizationVersionedId == organization.Id);
            }

            return GetOrganizationWithDetails(organization.Id, openApiVersion, false);
        }

        public IVmListItemsData<IVmListItem> GetOrganizations(string searchText)
        {
            IReadOnlyList<VmListItem> result = new List<VmListItem>();

            contextManager.ExecuteReader(unitOfWork =>
            {
                result = commonService.GetOrganizationNames(unitOfWork, searchText, false);
            });

            return new VmListItemsData<IVmListItem>(result);

        }

        public IVmGetOrganizationSearch GetOrganizationSearch()
        {
            // get enum values to local variables before using them inside LINQ queries as otherwise the provider does the queries in-memory and not in DB
            // as it cannot translate the enum values to db queries and this way we get better performance as the queries are evaluated in DB
            string statusDeletedCode = PublishingStatus.Deleted.ToString();
            string statusOldPublishedCode = PublishingStatus.OldPublished.ToString();

            var result = new VmGetOrganizationSearch();
            contextManager.ExecuteReader(unitOfWork =>
            {
				// PTV-866 requested by customer, end user are confused when searching with preselected organization
                //var userOrganization = utilities.GetUserOrganization(unitOfWork);


				// PTV-866 requested by customer, end user are confused when searching with preselected organization
                //result.OrganizationId = userOrganization?.Id

                var publishingStatuses = commonService.GetPublishingStatuses();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("PublishingStatuses", publishingStatuses)
                );
                result.SelectedPublishingStatuses = publishingStatuses.Where(x => x.Code != statusDeletedCode && x.Code != statusOldPublishedCode).Select(x => x.Id).ToList();

            });

            return result;
        }

        public IVmOrganizationSearchResult SearchOrganizations(IVmOrganizationSearch vmOrganizationSearch)
        {
            vmOrganizationSearch.Name = vmOrganizationSearch.Name != null
                ? vmOrganizationSearch.Name.Trim()
                : vmOrganizationSearch.Name;

            IReadOnlyList<IVmOrganizationListItem> result = new List<VmOrganizationListItem>();
            int count = 0;
            bool moreAvailable = false;
            int safePageNumber = vmOrganizationSearch.PageNumber.PositiveOrZero();

            contextManager.ExecuteReader(unitOfWork =>
            {
                var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var resultTemp = orgRep.All();
                var languageCode = SetTranslatorLanguage(vmOrganizationSearch);
                var selectedLanguageId = languageCache.Get(languageCode.ToString());
                var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
                var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var aternateNameTypeId = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString());

                #region FilteringData
                var languagesIds = vmOrganizationSearch.Languages.Select(language => languageCache.Get(language.ToString()));
                if (vmOrganizationSearch.OrganizationId.HasValue)
                {
                    var allChildren = GetOrganizationsFlatten(LoadOrganizationTree(resultTemp, int.MaxValue, new List<Guid>() { vmOrganizationSearch.OrganizationId.Value }));
                    var allChildrenIds = allChildren.Select(i => i.Id).ToList();
                    resultTemp = resultTemp.Where(x => allChildrenIds.Contains(x.Id) || x.UnificRootId == vmOrganizationSearch.OrganizationId.Value);
                }

                if (!string.IsNullOrEmpty(vmOrganizationSearch.Name))
                {
                    var rootId = GetRootIdFromString(vmOrganizationSearch.Name);
                    if (!rootId.HasValue)
                    {
                        var searchText = vmOrganizationSearch.Name.ToLower();
                        resultTemp = resultTemp.Where(
                            x => x.OrganizationNames.Any(y =>
                                 (languagesIds.Contains(y.LocalizationId) || y.CreatedBy.ToLower().Contains(searchText) || y.ModifiedBy.ToLower().Contains(searchText)) 
                                 && y.Name.ToLower().Contains(searchText)));
                    }
                    else
                    {
                        resultTemp = resultTemp
                            .Where(organization =>
                                organization.UnificRootId == rootId
                            );
                    }
                }
                else
                {
                    resultTemp =
                        resultTemp.Where(
                            x =>
                                x.OrganizationNames.Any(
                                    y => languagesIds.Contains(y.LocalizationId) &&
                                         !string.IsNullOrEmpty(y.Name)));
                }
                if (vmOrganizationSearch.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesByEquivalents(vmOrganizationSearch.SelectedPublishingStatuses);
                    resultTemp = resultTemp.WherePublishingStatusIn(vmOrganizationSearch.SelectedPublishingStatuses);
                }

                #endregion FilteringData

                count = resultTemp.Count();
                moreAvailable = count.MoreResultsAvailable(safePageNumber);


                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var resultTempData = resultTemp.Select(i => new
                {
                    Id = i.Id,
                    PublishingStatusId = i.PublishingStatusId,
                    UnificRootId = i.UnificRootId,
                    Name = i.OrganizationNames.OrderBy(x=>x.Localization.OrderNumber).FirstOrDefault(name => languagesIds.Contains(name.LocalizationId) && name.TypeId == i.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId).DisplayNameTypeId).Name,
                    AllNames = i.OrganizationNames.Where(x => x.TypeId == nameType).Select(x => new { x.LocalizationId, x.Name }),
                    //ParentOrganizationNames = i.Parent.Versions.Where(x => x.PublishingStatusId == publishedStatusId).Select(parent => parent.OrganizationNames.OrderBy(z=>z.Localization.OrderNumber)).FirstOrDefault(),
                    ParentId = i.ParentId,
                    //ParentOrganizationDisplayNameTypes = i.Parent.Versions.FirstOrDefault(x => x.PublishingStatusId == publishedStatusId).OrganizationDisplayNameTypes,
//                    Children = i.UnificRoot.Children
//                                .Where(x => x.PublishingStatusId == publishedStatusId)
//                                .Select(child =>
//                                    child.OrganizationNames
//                                    .OrderBy(z => z.Localization.OrderNumber)
//                                    .FirstOrDefault(name => name.TypeId == i.OrganizationDisplayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId).DisplayNameTypeId
//                                ).Name ?? child.OrganizationNames.OrderBy(z => z.Localization.OrderNumber).First().Name),
//                    LanguageAvailabilities = i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber),
                    Versioning = i.Versioning,
                    VersionMajor = i.Versioning.VersionMajor,
                    VersionMinor = i.Versioning.VersionMinor,
                    Modified = i.Modified,
                    ModifiedBy = i.ModifiedBy
                })
                .ApplySortingByVersions(vmOrganizationSearch.SortData, new VmSortParam() { Column = "Modified", SortDirection = SortDirectionEnum.Desc })
                    .Select(i => new
                    {
                        Id = i.Id,
                        PublishingStatusId = i.PublishingStatusId,
                        UnificRootId = i.UnificRootId,
                        ParentId = i.ParentId,
                        Versioning = i.Versioning,
                        VersionMajor = i.Versioning.VersionMajor,
                        VersionMinor = i.Versioning.VersionMinor,
                        Modified = i.Modified,
                        ModifiedBy = i.ModifiedBy
                    })
                .ApplyPagination(safePageNumber)
                .ToList();
                var parentIds = resultTempData.Select(i => i.ParentId).ToList();
                var orgIds = resultTempData.Select(i => i.Id).ToList();
                var orgUnificRootIds = resultTempData.Select(i => i.UnificRootId).ToList();
                var orgNameRep = unitOfWork.CreateRepository<IOrganizationNameRepository>();
                var parentOrgNames = orgNameRep.All()
                .Where(i => i.OrganizationVersioned.PublishingStatusId == publishedStatusId && parentIds.Contains(i.OrganizationVersioned.UnificRootId) && ((i.OrganizationVersioned.OrganizationDisplayNameTypes.Select(m => m.DisplayNameTypeId).Contains(i.TypeId) && i.OrganizationVersioned.OrganizationDisplayNameTypes.Select(m => m.LocalizationId).Contains(i.LocalizationId)) || !(i.OrganizationVersioned.OrganizationDisplayNameTypes.Where(k => k.LocalizationId == i.LocalizationId).Any())))
                .OrderBy(z => z.Localization.OrderNumber)
                .Include(j => j.OrganizationVersioned).ThenInclude(j => j.OrganizationDisplayNameTypes).ToList()
                .GroupBy(i => i.OrganizationVersioned.UnificRootId).ToDictionary(i => i.Key, i => i.ToList());

                var mainOrgNames = orgNameRep.All()
                .Where(i => orgIds.Contains(i.OrganizationVersionedId) && ((i.OrganizationVersioned.OrganizationDisplayNameTypes.Select(m => m.DisplayNameTypeId).Contains(i.TypeId) && i.OrganizationVersioned.OrganizationDisplayNameTypes.Select(m => m.LocalizationId).Contains(i.LocalizationId)) || !(i.OrganizationVersioned.OrganizationDisplayNameTypes.Where(k => k.LocalizationId == i.LocalizationId).Any())))
                .OrderBy(z => z.Localization.OrderNumber)
                .Include(j => j.OrganizationVersioned).ToList()
                .GroupBy(i => i.OrganizationVersionedId)
                .ToDictionary(i => i.Key, i => i.GroupBy(j => j.LocalizationId).ToDictionary(x => languageCache.GetByValue(x.Key), x => x.FirstOrDefault(y=>y.TypeId == aternateNameTypeId)?.Name ?? x.First().Name));

                var childrenNames = orgNameRep.All()
                    .Where(i => 
                        i.OrganizationVersioned.PublishingStatusId == publishedStatusId &&
                        i.OrganizationVersioned.ParentId != null && 
                        orgUnificRootIds.Contains(i.OrganizationVersioned.ParentId.Value) &&
                         ((i.OrganizationVersioned.OrganizationDisplayNameTypes.Select(m => m.DisplayNameTypeId).Contains(i.TypeId) && i.OrganizationVersioned.OrganizationDisplayNameTypes.Select(m => m.LocalizationId).Contains(i.LocalizationId)) || !(i.OrganizationVersioned.OrganizationDisplayNameTypes.Where(k => k.LocalizationId == i.LocalizationId).Any()))
                        )
                .OrderBy(z => z.Localization.OrderNumber)
                .Include(j => j.OrganizationVersioned).ToList()
                .GroupBy(i => i.OrganizationVersioned.ParentId)
                .ToDictionary(i => i.Key, i => i.Select(k => k.Name).ToList());
                
                var orgLangAvailabilitiesRep = unitOfWork.CreateRepository<IOrganizationLanguageAvailabilityRepository>();
                var orgLangAvailabilities = orgLangAvailabilitiesRep.All().Where(x => orgIds.Contains(x.OrganizationVersionedId)).OrderBy(x => x.Language.OrderNumber).ToList()
                    .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
                result = resultTempData.Select(i =>
                    {
                        var parentName = i.ParentId != null ? parentOrgNames.TryGetOrDefault(i.ParentId.Value, new List<OrganizationName>()) : new List<OrganizationName>();
                        var childrenName = childrenNames.TryGetOrDefault(i.UnificRootId, new List<string>());
                        var listItem = new VmOrganizationListItem()
                        {
                            Id = i.Id,
                            PublishingStatusId = i.PublishingStatusId,
                            UnificRootId = i.UnificRootId,
                            Name = mainOrgNames.TryGetOrDefault(i.Id, new Dictionary<string, string>()),
                            MainOrganization = GetParentName(parentName, parentName.FirstOrDefault()?.OrganizationVersioned?.OrganizationDisplayNameTypes),
                            SubOrganizations = childrenName.Any() ? childrenName.Aggregate((o1, o2) => o1 + ", " + o2) : string.Empty,
                            LanguagesAvailabilities =
                                TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                                    orgLangAvailabilities.TryGetOrDefault(i.Id, new List<OrganizationLanguageAvailability>())),
                            Version = TranslationManagerToVm.Translate<Versioning, VmVersion>(i.Versioning),
                            Modified = i.Modified.ToEpochTime(),
                            ModifiedBy = i.ModifiedBy,
                        };
                        return listItem;
                    })
                .ToList();
            });

            return new VmOrganizationSearchResultResult()
            {
                Organizations = result,
                PageNumber = ++safePageNumber,
                Count = count,
                MoreAvailable = moreAvailable
            };
        }

        private string GetParentName(IEnumerable<OrganizationName> organizationNames, IEnumerable<OrganizationDisplayNameType> displayNameTypes)
        {
            if (organizationNames == null || displayNameTypes == null) return string.Empty;

            return organizationNames.FirstOrDefault(name => name.TypeId == displayNameTypes.FirstOrDefault(type => type.LocalizationId == name.LocalizationId)?.DisplayNameTypeId)?.Name
                                                ?? string.Empty;
        }

        public VmEntityNames GetOrganizationNames(VmEntityBase model)
        {
            var result = new VmEntityNames();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var organization = unitOfWork.ApplyIncludes(organizationRep.All(), q =>
                    q.Include(i => i.OrganizationNames)
                        .Include(i => i.LanguageAvailabilities)).Single(x => x.Id == model.Id.Value);

                result = TranslationManagerToVm.Translate<OrganizationVersioned, VmEntityNames>(organization);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
                );

            });
            return result;
        }

        public IVmOrganizationStep1 GetOrganizationStep1(IVmGetOrganizationStep model)
        {
            var result = new VmOrganizationStep1();
            contextManager.ExecuteReader(unitOfWork =>
            {
                TranslationManagerToVm.SetLanguage(model.Language);
                var organization = GetEntity<OrganizationVersioned>(model.OrganizationId, unitOfWork,
                    q => q
                        .Include(x => x.OrganizationNames)
                        .Include(x => x.OrganizationDescriptions)
                        .Include(x => x.PublishingStatus)
                        .Include(x => x.Business)
                        .Include(x => x.Municipality).ThenInclude(x => x.MunicipalityNames)
                        .Include(x => x.OrganizationEmails).ThenInclude(x => x.Email)
                        .Include(x => x.OrganizationPhones).ThenInclude(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.OrganizationWebAddress).ThenInclude(x => x.WebPage)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressStreets).ThenInclude(x => x.StreetNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(x => x.PostOfficeBoxNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressForeigns).ThenInclude(x => x.ForeignTextNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressStreets).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(x => x.PostalCode).ThenInclude(x => x.PostalCodeNames)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.AddressAdditionalInformations)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.Coordinates)
                        .Include(x => x.OrganizationAddresses).ThenInclude(x => x.Address).ThenInclude(x => x.Country).ThenInclude(x => x.CountryNames)
                        .Include(x => x.OrganizationAreas).ThenInclude(x => x.Area)
                        .Include(x => x.OrganizationAreaMunicipalities)
                        .Include(x => x.OrganizationDisplayNameTypes)
                        );

                result = GetModel<OrganizationVersioned, VmOrganizationStep1>(organization, unitOfWork);

                var organizationTypeRep = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                var orgTypes = CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<OrganizationType, OrganizationTypeName>(unitOfWork, organizationTypeRep.All())), x => x.Name);
                orgTypes.ForEach(x => x.IsDisabled = x.Children.Any());
                var areaInformationTypes = commonService.GetAreaInformationTypes();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("AvailableOrganizations", commonService.GetUserAvailableOrganizationNamesForOrganization(unitOfWork, result.UnificRootId, result.ParentId)),
                    () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork)),
                    () => GetEnumEntityCollectionModel("OrganizationTypes", orgTypes),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("WebPageTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("Municipalities", commonService.GetMunicipalities(unitOfWork)),
                    () => GetEnumEntityCollectionModel("AreaInformationTypes", areaInformationTypes),
                    () => GetEnumEntityCollectionModel("AreaTypes", commonService.GetAreaTypes()),
                    () => GetEnumEntityCollectionModel("BusinessRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.BusinessRegions)),
                    () => GetEnumEntityCollectionModel("HospitalRegions", commonService.GetAreas(unitOfWork, AreaTypeEnum.HospitalRegions)),
                    () => GetEnumEntityCollectionModel("Provinces", commonService.GetAreas(unitOfWork, AreaTypeEnum.Province)),
                    () => GetEnumEntityCollectionModel("DialCodes", commonService.GetDefaultDialCode(unitOfWork)),
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
                );

                if (!result.AreaInformationTypeId.IsAssigned())
                {
                    result.AreaInformationTypeId = areaInformationTypes.Single(x => x.Code == AreaInformationTypeEnum.WholeCountry.ToString()).Id;
                }
            });

            return result;
        }


        private bool IsCyclicDependency(IUnitOfWork unitOfWork, Guid unificRootId, Guid? parentId)
        {
            if (parentId == null) return false;
            if (!unificRootId.IsAssigned() || !parentId.IsAssigned()) return false;
            if (unificRootId == parentId) return true;
            var filteredOutStatuses = new List<Guid>()
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString())
            };
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var higherOrgs = orgRep.All().Where(i => !filteredOutStatuses.Contains(i.PublishingStatusId)).Where(i => i.UnificRootId == parentId.Value && i.ParentId != null).Select(i => i.ParentId.Value).Distinct().ToList();
            var allTree = higherOrgs.ToList();
            CyclicCheck(unitOfWork, higherOrgs, ref allTree, filteredOutStatuses);
            return allTree.Contains(unificRootId);
        }


        private void CyclicCheck(IUnitOfWork unitOfWork, List<Guid> orgs, ref List<Guid> allTree, List<Guid> filteredOutStatuses)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var higherOrgs = orgRep.All().Where(i => !filteredOutStatuses.Contains(i.PublishingStatusId)).Where(i => orgs.Contains(i.UnificRootId) && i.ParentId != null).Select(i => i.ParentId.Value).Distinct().ToList();
            var toCheck = higherOrgs.Except(allTree).ToList();
            allTree.AddRange(toCheck);
            if (toCheck.Any())
            {
                CyclicCheck(unitOfWork, toCheck, ref allTree, filteredOutStatuses);
            }
        }

        public IVmOrganizationStep1 SaveOrganizationStep1(VmOrganizationModel model)
        {
            Guid? organizationId = null;
            OrganizationVersioned organizationVersioned = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                if (IsCyclicDependency(unitOfWork, model.Step1Form.UnificRootId, model.Step1Form.ParentId))
                {
                    throw new OrganizationCyclicDependencyException();
                }
                organizationLogic.PrefilterViewModel(model.Step1Form);
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var typeNameRep = unitOfWork.CreateRepository<INameTypeRepository>();
                if (!string.IsNullOrEmpty(model.Step1Form.OrganizationId) && organizationRep.All().Any(x => (x.UnificRootId != model.Step1Form.UnificRootId) && (x.Oid == model.Step1Form.OrganizationId)))
                {
                    throw new PtvArgumentException("", model.Step1Form.OrganizationId);
                }

                if (typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT1.ToString()) == model.Step1Form.OrganizationTypeId ||
                    typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT2.ToString()) == model.Step1Form.OrganizationTypeId)
                {
                    if(organizationRep.All().Single(x => x.Id == model.Id).TypeId != model.Step1Form.OrganizationTypeId)
                    {
                        throw new PtvServiceArgumentException("Organization type is not allowed!", new List<string> { typesCache.GetByValue<OrganizationType>(model.Step1Form.OrganizationTypeId.Value) });
                    }

                }

                var nameCode = model.Step1Form.IsAlternateNameUsedAsDisplayName ? NameTypeEnum.AlternateName : NameTypeEnum.Name;
                model.Step1Form.DisplayNameId = typeNameRep.All().First(x => x.Code == nameCode.ToString()).Id;

                TranslationManagerToEntity.SetLanguage(model.Step1Form.Language);
                organizationVersioned = TranslationManagerToEntity.Translate<VmOrganizationModel, OrganizationVersioned>(model, unitOfWork);

                //Removing emails
                var emailRep = unitOfWork.CreateRepository<IOrganizationEmailRepository>();
                var emailIds = organizationVersioned.OrganizationEmails.Select(x => x.EmailId).ToList();
                var emailsToRemove = emailRep.All().Where(x => x.Email.Localization.Code == model.Step1Form.Language.ToString() && x.OrganizationVersionedId == organizationVersioned.Id && !emailIds.Contains(x.EmailId));
                emailsToRemove.ForEach(x => emailRep.Remove(x));

                //Removing phones
                var phoneRep = unitOfWork.CreateRepository<IOrganizationPhoneRepository>();
                var phoneIds = organizationVersioned.OrganizationPhones.Select(x => x.PhoneId).ToList();
                var phonesToRemove = phoneRep.All().Where(x => x.Phone.Localization.Code == model.Step1Form.Language.ToString() &&  x.OrganizationVersionedId == organizationVersioned.Id && !phoneIds.Contains(x.PhoneId));
                phonesToRemove.ForEach(x => phoneRep.Remove(x));

                //Removing webPages
                var webPageRep = unitOfWork.CreateRepository<IOrganizationWebPageRepository>();
                var wpIds = organizationVersioned.OrganizationWebAddress.Select(x => x.WebPageId).ToList();
                var webPagesToRemove = webPageRep.All().Where(x => x.WebPage.Localization.Code == model.Step1Form.Language.ToString() && x.OrganizationVersionedId == organizationVersioned.Id && !wpIds.Contains(x.WebPageId));
                webPagesToRemove.ForEach(x => webPageRep.Remove(x));

                //Removing Address
                var addressRep = unitOfWork.CreateRepository<IOrganizationAddressRepository>();
                var addressIds = organizationVersioned.OrganizationAddresses.Select(x => x.AddressId).ToList();
                var addressesToRemove = addressRep.All().Where(x => x.OrganizationVersionedId == organizationVersioned.Id && !addressIds.Contains(x.AddressId));
                addressesToRemove.ForEach(x => addressRep.Remove(x));

                organizationVersioned.OrganizationAreas = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, organizationVersioned.OrganizationAreas,
                   query => query.OrganizationVersionedId == organizationVersioned.Id,
                   area => area.AreaId);

                organizationVersioned.OrganizationAreaMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, organizationVersioned.OrganizationAreaMunicipalities,
                   query => query.OrganizationVersionedId == organizationVersioned.Id,
                   areaMunicipality => areaMunicipality.MunicipalityId);

                unitOfWork.Save(parentEntity: organizationVersioned);
                organizationId = organizationVersioned.Id;

            });
            var addresses = organizationVersioned.OrganizationAddresses.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses);
            return GetOrganizationStep1(new VmGetOrganizationStep { OrganizationId = organizationId, Language = model.Step1Form.Language });
        }

        public IVmEntityBase AddApiOrganization(VmOrganizationModel model)
        {
            OrganizationVersioned organizationVersioned = null;
            var result = new VmEnumEntityRootStatusBase();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                organizationVersioned = AddOrganization(unitOfWork, model);
                unitOfWork.Save();
                FillEnumEntities(result,
                  () => GetEnumEntityCollectionModel("Organizations", commonService.GetOrganizationNames(unitOfWork).ToList())
              );
            });

            var addresses = organizationVersioned.OrganizationAddresses.Select(x => x.AddressId);
            addressService.UpdateAddress(addresses);
            result.Id = organizationVersioned.Id;
            result.UnificRootId = organizationVersioned.UnificRootId;
            result.PublishingStatusId = commonService.GetDraftStatusId();
            return result;
        }

        private OrganizationVersioned AddOrganization(IUnitOfWorkWritable unitOfWork, VmOrganizationModel vm)
        {
            var typeNameRep = unitOfWork.CreateRepository<INameTypeRepository>();
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

            if (!string.IsNullOrEmpty(vm.Step1Form.OrganizationId) && organizationRep.All().Any(x => x.Oid == vm.Step1Form.OrganizationId))
            {
                throw new PtvArgumentException("", vm.Step1Form.OrganizationId);
            }
            if (typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT1.ToString()) == vm.Step1Form.OrganizationTypeId ||
                typesCache.Get<OrganizationType>(OrganizationTypeEnum.TT2.ToString()) == vm.Step1Form.OrganizationTypeId)
            {
                throw new PtvServiceArgumentException("Organization type is not allowed!", new List<string> { typesCache.GetByValue<OrganizationType>(vm.Step1Form.OrganizationTypeId.Value) });
            }

            vm.PublishingStatusId = commonService.GetDraftStatusId();
            var nameCode = vm.Step1Form.IsAlternateNameUsedAsDisplayName ? NameTypeEnum.AlternateName : NameTypeEnum.Name;
            vm.Step1Form.DisplayNameId = typeNameRep.All().First(x => x.Code == nameCode.ToString()).Id;
            organizationLogic.PrefilterViewModel(vm.Step1Form);
            TranslationManagerToEntity.SetLanguage(vm.Language);
            var organization = TranslationManagerToEntity.Translate<VmOrganizationModel, OrganizationVersioned>(vm, unitOfWork);
            organizationRep.Add(organization);
            return organization;
        }

        private IVmOpenApiOrganizationVersionBase GetOrganizationWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            //return GetOrganizationsWithDetails(unitOfWork, new List<Guid> { versionId }, openApiVersion, getOnlyPublished).FirstOrDefault();

            Guid publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var orgRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var orgQuery = orgRepo.All().Where(ov => ov.Id == versionId);

            if (getOnlyPublished)
            {
                orgQuery = orgQuery.Where(ov => ov.LanguageAvailabilities.Any(ola => ola.StatusId == publishedId));
            }

            var organization = unitOfWork.ApplyIncludes(orgQuery, GetOrganizationIncludeChain()).FirstOrDefault();

            if (organization == null)
            {
                // organization not found
                return null;
            }

            // Find all published sub organizations
            var allSubOrganizations = orgRepo.All().Where(ov => ov.ParentId == organization.UnificRootId && ov.PublishingStatusId == publishedId && ov.LanguageAvailabilities.Any(ola => ola.StatusId == publishedId)).ToList();

            // set sub-organization names if there were any
            if (allSubOrganizations?.Count > 0)
            {
                var subOrganizationIds = allSubOrganizations.Select(s => s.Id).ToList();
                var subOrganizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => subOrganizationIds.Contains(i.OrganizationVersionedId)).ToList()
                    .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
                // Set the names for sub organizations
                allSubOrganizations.ForEach(s =>
                {
                    s.OrganizationNames = subOrganizationNames.TryGet(s.Id);
                });
            }

            // sub organizations with names
            var subOrganizations = allSubOrganizations?.GroupBy(o => o.ParentId).ToDictionary(i => i.Key, i => i.ToList());

            // Filter out not published language versions
            if (getOnlyPublished)
            {
                var notPublishedLanguageVersions = organization.LanguageAvailabilities.Where(i => i.StatusId != publishedId).Select(i => i.LanguageId).ToList();
                if (notPublishedLanguageVersions.Count > 0)
                {
                    organization.OrganizationEmails = organization.OrganizationEmails.Where(i => !notPublishedLanguageVersions.Contains(i.Email.LocalizationId)).ToList();
                    organization.OrganizationNames = organization.OrganizationNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    organization.OrganizationDescriptions = organization.OrganizationDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    organization.OrganizationPhones = organization.OrganizationPhones.Where(i => !notPublishedLanguageVersions.Contains(i.Phone.LocalizationId)).ToList();
                    organization.OrganizationWebAddress = organization.OrganizationWebAddress.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();

                    organization.OrganizationEInvoicings.ForEach(invoicing =>
                    {
                        invoicing.EInvoicingAdditionalInformations = invoicing.EInvoicingAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });

                    organization.OrganizationAddresses.ForEach(address =>
                    {
                        address.Address.AddressStreets.ForEach(c =>
                        {
                            c.StreetNames = c.StreetNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressPostOfficeBoxes.ForEach(c =>
                        {
                            c.PostOfficeBoxNames = c.PostOfficeBoxNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressForeigns.ForEach(c =>
                        {
                            c.ForeignTextNames = c.ForeignTextNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressAdditionalInformations = address.Address.AddressAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });
                }
            }

            // Filter out not published parent organization
            if (organization.Parent?.Versions?.Count > 0)
            {
                if (!organization.Parent.Versions.Any(o => o.PublishingStatusId == publishedId))
                {
                    organization.ParentId = null;
                }
            }

            // Fill with sub organizations
            organization.UnificRoot.Children = subOrganizations.TryGet(organization.UnificRootId);

            // NOTE: Organization services are fetched after translation to improve performance with custom queries
            var tmpResult = TranslationManagerToVm.Translate<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(organization);

            // Get the sourceId if user is logged in
            var userId = utilities.GetRelationIdForExternalSource(false);
            if (!string.IsNullOrEmpty(userId))
            {
                tmpResult.SourceId = GetSourceId<Organization>(tmpResult.Id.Value, userId, unitOfWork);
            }

            // Get organizations services
            tmpResult.Services = GetOrganizationsServices(organization.UnificRootId, unitOfWork);

            return GetEntityByOpenApiVersion(tmpResult as IVmOpenApiOrganizationVersionBase, openApiVersion);
        }

        private List<Domain.Model.Models.OpenApi.V5.V5VmOpenApiOrganizationService> GetOrganizationsServices(Guid organizationUnificRootId, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            Guid publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // list for organizations services to return
            List<Domain.Model.Models.OpenApi.V5.V5VmOpenApiOrganizationService> services = new List<Domain.Model.Models.OpenApi.V5.V5VmOpenApiOrganizationService>(200);

            // Get organizations organizationservices (Default RoleType => OtherResponsible)
            var organizationServicesQuery = unitOfWork.CreateRepository<IOrganizationServiceRepository>().All()
                .Where(os => os.OrganizationId == organizationUnificRootId 
                && os.ServiceVersioned.PublishingStatusId == publishedId
                && os.ServiceVersioned.LanguageAvailabilities.Any(l => l.StatusId == publishedId));

            var organizationServices = organizationServicesQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                ServiceId = x.ServiceVersioned.UnificRootId,
                ServiceVersionedId = x.ServiceVersioned.Id
            }).ToList();

            // Get services where organization is main responsible (Default RoleType => Responsible)
            var responsibleServicesQuery = unitOfWork.CreateRepository<IServiceVersionedRepository>().All()
                .Where(sv => sv.OrganizationId == organizationUnificRootId 
                && sv.PublishingStatusId == publishedId);

            // ToDictionary used for performance as we need to lookup entries with ServiceVersionedId
            var responsibleServices = responsibleServicesQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                ServiceId = x.UnificRootId,
                ServiceVersionedId = x.Id
            }).ToDictionary(a => a.ServiceVersionedId, a => a);

            // Get services where organisation is the producer (Default RoleType => Producer)
            var producerServicesQuery = unitOfWork.CreateRepository<IServiceProducerOrganizationRepository>().All()
                .Where(spo => spo.OrganizationId == organizationUnificRootId 
                && spo.ServiceProducer.ServiceVersioned.PublishingStatusId == publishedId);

            var producerServices = producerServicesQuery.Select(x => new
            {
                OrganizationId = x.OrganizationId,
                ServiceId = x.ServiceProducer.ServiceVersioned.UnificRootId,
                ServiceVersionedId = x.ServiceProducer.ServiceVersioned.Id,
                ProvisionTypeId = x.ServiceProducer.ProvisionTypeId
            }).ToList();

            // Next fetch names for services (get all service ids and the use that list to fetch names in all languages)
            // (worst case is around 1500 IDs)
            var allServiceIds = organizationServices.Select(x => x.ServiceVersionedId).ToList();
            allServiceIds.AddWithDistinct(responsibleServices.Keys); // could be duplicates with organizationservices
            allServiceIds.AddRange(producerServices.Select(x => x.ServiceVersionedId));

            // Do we have any services
            if (allServiceIds.Count > 0)
            {
                var serviceNames = unitOfWork.CreateRepository<IServiceNameRepository>().All()
                    .Where(i => allServiceIds.Contains(i.ServiceVersionedId))
                    .Select(sn => new VmName()
                    {
                        LocalizationId = sn.LocalizationId,
                        Name = sn.Name,
                        TypeId = sn.TypeId,
                        OwnerReferenceId = sn.ServiceVersionedId
                    })
                    .ToList() // materialize
                    .GroupBy(i => i.OwnerReferenceId).ToDictionary(i => i.Key, i => i.ToList());

                // Add the services to the list (first organization services then responsible services and last the producer)
                // Order does matter so that we don't need to do many lookups
                // 
                // AdditionalInformation and WebPages are always empty lists these are remains from previous implementation (these fields might get removed in the future)

                // loop the organization services and create view model for, also check if the same service exists in responsibleServices
                // if so then change the RoleType and remove the matched service from responsibleServices
                organizationServices.ForEach(x =>
                {
                    var os = new Domain.Model.Models.OpenApi.V5.V5VmOpenApiOrganizationService()
                    {
                        AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        OrganizationId = x.OrganizationId.ToString(),
                        RoleType = "OtherResponsible",
                        Service = new VmOpenApiItem()
                        {
                            Id = x.ServiceId,
                            Name = GetServiceNameWithFallback(serviceNames.TryGet(x.ServiceVersionedId))
                        },
                        WebPages = new List<Domain.Model.Models.OpenApi.V4.V4VmOpenApiWebPage>()
                    };

                    // check if the same service is in responsibleServices
                    // responsibleServices is dictionary on purpose as the key lookup is much faster than using Where Linq
                    // as that would need to enumerate the whole collection every time
                    var match = responsibleServices.TryGet(x.ServiceVersionedId);

                    if (match != null)
                    {
                        os.RoleType = "Responsible";
                        // remove the responsible service entry so that we don't add the service two times
                        responsibleServices.Remove(x.ServiceVersionedId);
                    }

                    services.Add(os);
                });

                // Services where organization is responsible
                responsibleServices.Values.ForEach(x =>
                {
                    var rs = new Domain.Model.Models.OpenApi.V5.V5VmOpenApiOrganizationService()
                    {
                        AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        OrganizationId = x.OrganizationId.ToString(),
                        RoleType = "Responsible",
                        Service = new VmOpenApiItem()
                        {
                            Id = x.ServiceId,
                            Name = GetServiceNameWithFallback(serviceNames.TryGet(x.ServiceVersionedId))
                        },
                        WebPages = new List<Domain.Model.Models.OpenApi.V4.V4VmOpenApiWebPage>()
                    };

                    services.Add(rs);
                });

                // Services where organization is producer
                producerServices.ForEach(x =>
                {
                    var ps = new Domain.Model.Models.OpenApi.V5.V5VmOpenApiOrganizationService()
                    {
                        AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        OrganizationId = x.OrganizationId.ToString(),
                        ProvisionType = typesCache.GetByValue<ProvisionType>(x.ProvisionTypeId),
                        RoleType = "Producer",
                        Service = new VmOpenApiItem()
                        {
                            Id = x.ServiceId,
                            Name = GetServiceNameWithFallback(serviceNames.TryGet(x.ServiceVersionedId))
                        },
                        WebPages = new List<Domain.Model.Models.OpenApi.V4.V4VmOpenApiWebPage>()
                    };

                    services.Add(ps);
                });
            }

            return services;
        }

        /// <summary>
        /// Tries to get the service name in the following order: FI, SV and then EN
        /// </summary>
        /// <param name="serviceNames">List of service names</param>
        /// <returns>service name or null</returns>
        private string GetServiceNameWithFallback(ICollection<VmName> serviceNames)
        {
            if (serviceNames == null || serviceNames.Count == 0)
            {
                return null;
            }

            Guid nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            // first try to get finnish name
            string sname = GetServiceName(serviceNames, languageCache.Get(LanguageCode.fi.ToString()), nameTypeId);

            // did we find FI name
            if(!string.IsNullOrWhiteSpace(sname))
            {
                return sname;
            }

            sname = GetServiceName(serviceNames, languageCache.Get(LanguageCode.sv.ToString()), nameTypeId);

            // did we find SV name
            if (!string.IsNullOrWhiteSpace(sname))
            {
                return sname;
            }

            return GetServiceName(serviceNames, languageCache.Get(LanguageCode.en.ToString()), nameTypeId);
        }

        /// <summary>
        /// Get service name.
        /// </summary>
        /// <param name="serviceNames">List of service names</param>
        /// <param name="languageId">what language to get</param>
        /// <param name="nameTypeId">what type of name to get</param>
        /// <returns>service name or null</returns>
        private static string GetServiceName(ICollection<VmName> serviceNames, Guid languageId, Guid nameTypeId)
        {
            if (serviceNames == null || serviceNames.Count == 0)
            {
                return null;
            }

            return serviceNames.Where(sn => sn.LocalizationId == languageId && sn.TypeId == nameTypeId).FirstOrDefault()?.Name;
        }

        private IVmOpenApiOrganizationVersionBase GetOrganizationWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiOrganizationVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetOrganizationWithDetails(unitOfWork, versionId , openApiVersion, getOnlyPublished);
            });
            return result;
        }

        private IList<IVmOpenApiOrganizationVersionBase> GetOrganizationsWithDetails(IUnitOfWork unitOfWork, List<Guid> versionIdList, int openApiVersion, bool getOnlyPublished = true)
        {
            if (versionIdList.Count == 0) return new List<IVmOpenApiOrganizationVersionBase>();

            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var resultTemp = unitOfWork.ApplyIncludes(organizationRep.All().Where(o => versionIdList.Contains(o.Id)), q =>
                q.Include(i => i.Business)
                    .Include(i => i.Type)
                    .Include(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .Include(i => i.OrganizationEmails).ThenInclude(i => i.Email)
                    .Include(i => i.OrganizationNames)
                    .Include(i => i.OrganizationDisplayNameTypes)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.OrganizationServices).ThenInclude(i => i.ServiceVersioned).ThenInclude(i => i.LanguageAvailabilities)
                    //.Include(i => i.UnificRoot).ThenInclude(i => i.OrganizationServices).ThenInclude(i => i.ServiceVersioned)//.ThenInclude(i => i.ServiceNames)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.OrganizationServicesVersioned)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceProducerOrganizations).ThenInclude(i => i.ServiceProducer).ThenInclude(i => i.ServiceVersioned)
                    .Include(i => i.OrganizationDescriptions)
                    .Include(x => x.OrganizationPhones).ThenInclude(x => x.Phone).ThenInclude(i => i.PrefixNumber)
                    .Include(i => i.OrganizationWebAddress).ThenInclude(i => i.WebPage)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressStreets).ThenInclude(i => i.StreetNames)

                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)

                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(x => x.AddressForeigns).ThenInclude(i => i.ForeignTextNames)

                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                    .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)

                    .Include(i => i.OrganizationEInvoicings).ThenInclude(i => i.EInvoicingAdditionalInformations)

                    .Include(i => i.LanguageAvailabilities)
                    .Include(i => i.OrganizationAreas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                    .Include(i => i.OrganizationAreas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .Include(i => i.OrganizationAreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .Include(i => i.Parent).ThenInclude(i => i.Versions)
                ).ToList();

            // Filter out items that do not have language versions published!
            var organizations = getOnlyPublished ? resultTemp.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList() : resultTemp.ToList();

            // https://github.com/aspnet/EntityFrameworkCore/issues/7922
            // converting Guids to nullable so that the follwing Contains is executed on server and not on client
            var organizationRootIds = organizations.Select(o => o.UnificRootId).Cast<Guid?>();

#warning "Linq to entity, contains nullable guids fix"

            // Find all sub organizations and names for them
            var allSubOrganizations = organizationRep.All().Where(o => organizationRootIds.Contains(o.ParentId)
                && o.PublishingStatusId == publishedId && o.LanguageAvailabilities.Any(l => l.StatusId == publishedId)).ToList();

            var subOrganizationIds = allSubOrganizations.Select(s => s.Id).ToList();
            var subOrganizationNames = unitOfWork.CreateRepository<IOrganizationNameRepository>().All().Where(i => subOrganizationIds.Contains(i.OrganizationVersionedId)).ToList()
                .GroupBy(i => i.OrganizationVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            // Set the names for sub organizations
            allSubOrganizations.ForEach(s =>
            {
                s.OrganizationNames = subOrganizationNames.TryGet(s.Id);
            });

            // sub organizations with names
            var subOrganizations = allSubOrganizations.GroupBy(o => o.ParentId).ToDictionary(i => i.Key, i => i.ToList());

            organizations.ForEach(
                organization =>
                {
                    // Filter out not published services
                    organization.UnificRoot.OrganizationServices =
                        organization.UnificRoot.OrganizationServices.Where(i => i.ServiceVersioned.PublishingStatusId == publishedId &&
                        i.ServiceVersioned.LanguageAvailabilities.Any(l => l.StatusId == publishedId)) // filter out items if no published language versions are available
                        .ToList();

                    // Filter out not published services (main responsible)
                    organization.UnificRoot.OrganizationServicesVersioned = organization.UnificRoot.OrganizationServicesVersioned.Where(i => i.PublishingStatusId == publishedId).ToList();

                    // Filter out not published services (organization as producer)
                    organization.UnificRoot.ServiceProducerOrganizations = organization.UnificRoot.ServiceProducerOrganizations.Where(i => i.ServiceProducer.ServiceVersioned.PublishingStatusId == publishedId).ToList();

                    // Filter out not published language versions
                    if (getOnlyPublished)
                    {
                        var notPublishedLanguageVersions = organization.LanguageAvailabilities.Where(i => i.StatusId != publishedId).Select(i => i.LanguageId).ToList();
                        if (notPublishedLanguageVersions.Count > 0)
                        {
                            organization.OrganizationEmails = organization.OrganizationEmails.Where(i => !notPublishedLanguageVersions.Contains(i.Email.LocalizationId)).ToList();
                            organization.OrganizationNames = organization.OrganizationNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            organization.OrganizationDescriptions = organization.OrganizationDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            organization.OrganizationPhones = organization.OrganizationPhones.Where(i => !notPublishedLanguageVersions.Contains(i.Phone.LocalizationId)).ToList();
                            organization.OrganizationWebAddress = organization.OrganizationWebAddress.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();

                            organization.OrganizationEInvoicings.ForEach(invoicing =>
                            {
                                invoicing.EInvoicingAdditionalInformations = invoicing.EInvoicingAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            });

                            organization.OrganizationAddresses.ForEach(address =>
                            {
                                address.Address.AddressStreets.ForEach(c =>
                                {
                                    c.StreetNames = c.StreetNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                                });
                                address.Address.AddressPostOfficeBoxes.ForEach(c =>
                                {
                                    c.PostOfficeBoxNames = c.PostOfficeBoxNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                                });
                                address.Address.AddressForeigns.ForEach(c =>
                                {
                                    c.ForeignTextNames = c.ForeignTextNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                                });
                                address.Address.AddressAdditionalInformations = address.Address.AddressAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            });
                        }
                    }

                    // Filter out not published parent organization
                    if (organization.Parent?.Versions?.Count > 0)
                    {
                        if (!organization.Parent.Versions.Any(o => o.PublishingStatusId == publishedId))
                        {
                            organization.ParentId = null;
                        }
                    }

                    // Fill with sub organizations
                    organization.UnificRoot.Children = subOrganizations.TryGet(organization.UnificRootId);

                }
            );

            // Fill with service names
            var allServices = organizations.SelectMany(i => i.UnificRoot.OrganizationServices).Select(i => i.ServiceVersioned).ToList();
            allServices.AddRange(organizations.SelectMany(i => i.UnificRoot.OrganizationServicesVersioned).ToList());
            allServices.AddRange(organizations.SelectMany(i => i.UnificRoot.ServiceProducerOrganizations).Select(i => i.ServiceProducer.ServiceVersioned).ToList());
            var serviceIds = allServices.Select(i => i.Id).ToList();
            var serviceNames = unitOfWork.CreateRepository<IServiceNameRepository>().All().Where(i => serviceIds.Contains(i.ServiceVersionedId)).ToList()
                .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.ToList());
            allServices.ForEach(service =>
            {
                service.ServiceNames = serviceNames.TryGet(service.Id);
            });

            var result = TranslationManagerToVm.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(organizations).ToList();

            if (result == null)
            {
                throw new Exception(CoreMessages.OpenApi.RecordNotFound);
            }

            // Get the right open api view model version
            IList<IVmOpenApiOrganizationVersionBase> vmList = new List<IVmOpenApiOrganizationVersionBase>();
            result.ForEach(org =>
            {
                // Get the sourceId if user is logged in
                var userId = utilities.GetRelationIdForExternalSource(false);
                if (!string.IsNullOrEmpty(userId))
                {
                    org.SourceId = GetSourceId<Organization>(org.Id.Value, userId, unitOfWork);
                }                
                vmList.Add(GetEntityByOpenApiVersion(org as IVmOpenApiOrganizationVersionBase, openApiVersion));
            });

            return vmList;
        }

        public IVmEntityBase GetOrganizationStatus(Guid? organizationId)
        {
            VmPublishingStatus result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = GetOrganizationStatus(unitOfWork, organizationId);
            });
            return new VmEntityStatusBase { PublishingStatusId = result.Id };
        }

        private VmPublishingStatus GetOrganizationStatus(IUnitOfWorkWritable unitOfWork, Guid? organizationId)
        {
            var serviceRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var service = serviceRep.All()
                            .Include(x => x.PublishingStatus)
                            .Single(x => x.Id == organizationId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(service.PublishingStatus);
        }


        public VmPublishingResultModel PublishOrganization(VmPublishingModel model)
        {
            Guid channelId = model.Id;
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                if (IsParentOrgDeleted(unitOfWork, model.Id))
                {
                    throw new OrganizationCannotPublishDeletedRootException();
                }
                var affected = commonService.PublishEntity<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork, model);
                var result = new VmPublishingResultModel()
                {
                    Id = channelId,
                    PublishingStatusId = affected.AffectedEntities.First(i => i.Id == channelId).PublishingStatusNew,
                    LanguagesAvailabilities = model.LanguagesAvailabilities,
                    Version = affected.Version
                };
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Services",
                        affected.AffectedEntities.Select(i => new VmEntityStatusBase() {Id = i.Id, PublishingStatusId = i.PublishingStatusNew}).ToList<IVmBase>()));
                return result;
            });
        }
        
        public VmPublishingResultModel WithdrawOrganization(Guid organizationId)
        {
            VmArchiveResult result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = GetOrganizationConnectedEntities(unitOfWork, organizationId);
                if (result.AnyConnected)
                {
                    throw new WithdrawConnectedExistsException();
                }
            });
            return commonService.WithdrawEntity<OrganizationVersioned, OrganizationLanguageAvailability> (organizationId);
        }

        public VmPublishingResultModel RestoreOrganization(Guid organizationId)
        {
            return commonService.RestoreEntity<OrganizationVersioned, OrganizationLanguageAvailability>(organizationId, (unitOfWork, ov) =>
            {
                if (IsCyclicDependency(unitOfWork, ov.UnificRootId, ov.ParentId))
                {
                    throw new OrganizationCyclicDependencyException();
                }
                return true;
            });
        }


        private bool IsParentOrgDeleted(IUnitOfWork unitOfWork, Guid organizationId)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psDeletedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var psPublishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

            var parentOrgId = orgRep.All().Where(i => i.Id == organizationId).Select(i => i.ParentId).FirstOrDefault();
            if (!parentOrgId.IsAssigned()) return false;
            return IsParentOrgDeleted(orgRep, psDeletedId, psOldPublishedId, psPublishedId, parentOrgId.Value);
        }

        private bool IsParentOrgDeleted(IOrganizationVersionedRepository orgRep, Guid psDeletedId, Guid psOldPublishedId, Guid psPublishedId, Guid organizationRootId)
        {
            var parentOrgs = orgRep.All().Where(i => i.UnificRootId == organizationRootId).Where(i => i.PublishingStatusId != psDeletedId && i.PublishingStatusId != psOldPublishedId).Select(i => new { i.PublishingStatusId, i.ParentId }).ToList();
            if (parentOrgs.IsNullOrEmpty()) return true;
            var parentOrg = parentOrgs.FirstOrDefault(i => i.PublishingStatusId == psPublishedId);
            if (parentOrg == null) return true;
            if (!parentOrg.ParentId.IsAssigned()) return false;
            return IsParentOrgDeleted(orgRep, psDeletedId, psOldPublishedId, psPublishedId, parentOrg.ParentId.Value);
        }

        public IVmEntityBase DeleteOrganization(Guid? organizationId, bool CheckDelete = false)
        {
            VmArchiveResult result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = CascadeDeleteOrganization(unitOfWork, organizationId, CheckDelete);
                unitOfWork.Save();
            });
            return result;
        }

        private VmArchiveResult CascadeDeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid? id, bool checkDelete = false)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());            
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organizationUserMapRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
            var userMaps = organizationUserMapRep.All();
            var organizationIsUsed =
                organizationRep.All()
                    .Where(x => x.Id == id)
                    .Any(
                        i => userMaps.Any(k => k.OrganizationId == i.UnificRootId));
            if (organizationIsUsed)
            {
                throw new OrganizationNotDeleteInUserUseException();
            }

            var organization = organizationRep.All().SingleOrDefault(x => x.Id == id);

            if (checkDelete)
            {
                var result = GetOrganizationConnectedEntities(unitOfWork, organization.Id);
                if (result.AnyConnected)
                {
                    return result;
                }
            }
            else
            {
                organization.PublishingStatusId = psDeleted;
                ArchiveConnectedChannels(unitOfWork, organization.UnificRootId);
                ArchiveConnectedServices(unitOfWork, organization.UnificRootId);
                ArchiveSubOrganizations(unitOfWork, organization.UnificRootId);
            }
            return new VmArchiveResult { Id = organization.Id, PublishingStatusId = organization.PublishingStatusId };
        }

        private VmArchiveResult GetOrganizationConnectedEntities(IUnitOfWorkWritable unitOfWork, Guid? organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organization = organizationRep.All().SingleOrDefault(x => x.Id == organizationId);
            var organizations = organizationRep.All().Where(x => x.Id == organizationId);
            return new VmArchiveResult
            {
                Id = organization.Id,
                PublishingStatusId = organization.PublishingStatusId,
                ChannelsConnected = organizations.Any(i => i.UnificRoot.OrganizationServiceChannelsVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                SubOrganizationsConnected = organizations.Any(i => i.UnificRoot.Children.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished)),
                ServicesConnected = organizations.Any(i => i.UnificRoot.OrganizationServices.Any(j => j.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceVersioned.PublishingStatusId != psOldPublished))
                                    || organizations.Any(i => i.UnificRoot.ServiceProducerOrganizations.Any(j => j.ServiceProducer.ServiceVersioned.PublishingStatusId != psDeleted && j.ServiceProducer.ServiceVersioned.PublishingStatusId != psOldPublished))
                                    || organizations.Any(i => i.UnificRoot.OrganizationServicesVersioned.Any(j => j.PublishingStatusId != psDeleted && j.PublishingStatusId != psOldPublished))

            };
        }

        private void ArchiveConnectedChannels(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            channelRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                            .Where(x => x.OrganizationId == organizationId)
                            .ForEach(x => x.SafeCall(i => i.PublishingStatusId = psDeleted));
        }

        private void ArchiveConnectedServices(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var services = serviceRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                            .Where(x => x.OrganizationServices.Any(o=>o.OrganizationId == organizationId) ||
                                        x.ServiceProducers.SelectMany(sp => sp.Organizations).Any(o => o.OrganizationId == organizationId) ||
                                        x.OrganizationId == organizationId)
                            .Include(x => x.OrganizationServices).ThenInclude(x=>x.Organization).ThenInclude(x=>x.Versions)
                            .Include(x => x.ServiceProducers).ThenInclude(x => x.Organizations).ThenInclude(x => x.Organization).ThenInclude(x => x.Versions)
                            .ToList();

            foreach (var service in services)
            {
                var restOrganizations = service.OrganizationServices.Where(x => x.OrganizationId != organizationId && x.Organization.Versions.Any(y=>y.PublishingStatusId != psDeleted && y.PublishingStatusId != psOldPublished)).ToList();
                service.OrganizationServices = restOrganizations.ToList();

                var producersToDelete = HandleServiceProducers(service.ServiceProducers, organizationId, psDeleted, psOldPublished);
                if (producersToDelete.Any())
                {
                    producersToDelete.ForEach(p => service.ServiceProducers.Remove(p));

                    var orderNumber = 1;
                    service.ServiceProducers.OrderBy(p => p.OrderNumber).ForEach(p => p.OrderNumber = orderNumber++);
                }
                if (service.OrganizationId == organizationId)
                {
                    service.SafeCall(i => i.PublishingStatusId = psDeleted);
                }
            }
        }

        private List<ServiceProducer> HandleServiceProducers(ICollection<ServiceProducer> serviceProducers, Guid organizationId, Guid psDeleted, Guid psOldPublished)
        {
            var producersToDelete = new List<ServiceProducer>();
            if (serviceProducers == null) return producersToDelete;

            foreach (var producer in serviceProducers)
            {
                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()))
                {
                    var producerOrganizations = producer.Organizations.Where(spo => spo.OrganizationId != organizationId && spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished)).ToList();
                    if (producerOrganizations.Any())
                    {
                        producer.Organizations = producerOrganizations;
                    }
                    else
                    {
                        producersToDelete.Add(producer);
                    }
                }

                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.PurchaseServices.ToString()))
                {
                    var spo = producer.Organizations.FirstOrDefault();
                    if (spo == null)
                    {
                        producersToDelete.Add(producer);
                    }
                    else
                    {
                        if (spo.OrganizationId == organizationId && !spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished))
                        {
                            producersToDelete.Add(producer);
                        }
                    }
                }

                if (producer.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()))
                {
                    var spo = producer.Organizations.FirstOrDefault();
                    if (spo != null && spo.OrganizationId == organizationId && !spo.Organization.Versions.Any(ov => ov.PublishingStatusId != psDeleted && ov.PublishingStatusId != psOldPublished))
                    {
                        producersToDelete.Add(producer);
                    }
                }
            }

            return producersToDelete;
        }

        private void ArchiveSubOrganizations(IUnitOfWorkWritable unitOfWork, Guid organizationId)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var subOrgs = organizationRep.All().Where(x => x.PublishingStatusId != psDeleted && x.PublishingStatusId != psOldPublished)
                            .Where(x => x.ParentId == organizationId).ToList();

            foreach (var subOrg in subOrgs)
            {
                subOrg.PublishingStatusId = psDeleted;
                ArchiveConnectedChannels(unitOfWork, subOrg.UnificRootId);
                ArchiveConnectedServices(unitOfWork, subOrg.UnificRootId);
                ArchiveSubOrganizations(unitOfWork, subOrg.UnificRootId);
            }

        }

        private OrganizationVersioned DeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid? id)
        {
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organization = organizationRep.All().SingleOrDefault(x => x.Id == id);
            organization.SafeCall(i => i.PublishingStatusId = psDeleted);
            return organization;
        }

        public IVmOpenApiOrganizationVersionBase SaveOrganization(IVmOpenApiOrganizationInVersionBase vm, bool allowAnonymous, int openApiVersion)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            var organization = new OrganizationVersioned();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Get the root id according to source id (if defined)
                var rootId = vm.Id ?? GetPTVId<Organization>(vm.SourceId, userId, unitOfWork);

                // Get right version id
                vm.Id = versioningManager.GetVersionId<OrganizationVersioned>(unitOfWork, rootId, null, false);

                if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                {
                    try
                    {
                        var archiveResult = CascadeDeleteOrganization(unitOfWork, vm.Id, true);
                        if (!archiveResult.AnyConnected)
                        {
                            organization = DeleteOrganization(unitOfWork, vm.Id);                            
                        }
                        else
                        {
                            throw new Exception($"You cannot delete organization {rootId}. Services or service channels attached!");
                        }
                    }
                    catch (OrganizationNotDeleteInUserUseException)
                    {
                        throw new Exception($"You cannot delete organization {rootId}. Users are attached to the organization!");
                    }
                    catch(OrganizationNotDeleteInUseException)
                    {
                        throw new Exception($"You cannot delete organization {rootId}. Services or service channels attached!");
                    }
                }
                else
                {
                    var parentOrgId = vm.ParentOrganizationId.ParseToGuid();
                    if (IsCyclicDependency(unitOfWork, rootId, parentOrgId))
                    {
                        throw new Exception($"You cannot use {vm.ParentOrganizationId} as a parent organization. A cycling dependency is not allowed to be created between organizations!");
                    }
                    // Entity needs to be restored?
                    if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                        {
                            // We need to restore already archived item
                            var publishingResult = commonService.RestoreArchivedEntity<OrganizationVersioned>(unitOfWork, vm.Id.Value);
                        }
                    }

                    organization = TranslationManagerToEntity.Translate<IVmOpenApiOrganizationInVersionBase, OrganizationVersioned>(vm, unitOfWork);

                    if (!vm.OrganizationType.IsNullOrEmpty() && vm.OrganizationType != OrganizationTypeEnum.Municipality.ToString()) // Municipality info needs to be removed if organization type is not municipality!
                    {
                        organization.MunicipalityId = null;
                    }

                    if (vm.CurrentPublishingStatus == PublishingStatus.Draft.ToString())
                    {
                        // We need to manually remove items from collections!
                        if (!vm.SubAreaType.IsNullOrEmpty() && vm.Areas.Count > 0)
                        {
                            if (vm.SubAreaType == AreaTypeEnum.Municipality.ToString())
                            {
                                organization.OrganizationAreaMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, organization.OrganizationAreaMunicipalities,
                                query => query.OrganizationVersionedId == organization.Id, area => area.MunicipalityId);
                                // Remove all possible old areas
                                dataUtils.RemoveItemCollection<OrganizationArea>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                            }
                            else
                            {
                                organization.OrganizationAreas = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, organization.OrganizationAreas,
                                    query => query.OrganizationVersionedId == organization.Id, area => area.AreaId);
                                // Remove all possible old municipalities
                                dataUtils.RemoveItemCollection<OrganizationAreaMunicipality>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                            }
                        }
                        else if (!vm.AreaType.IsNullOrEmpty() && vm.AreaType != AreaInformationTypeEnum.AreaType.ToString())
                        {
                            // We need to remove possible old areas and municipalities
                            dataUtils.RemoveItemCollection<OrganizationArea>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                            dataUtils.RemoveItemCollection<OrganizationAreaMunicipality>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                        }

                        if (vm.DeleteAllPhones || (vm.PhoneNumbers != null && vm.PhoneNumbers.Count > 0))
                        {
                            // Remove the phones that were not included in vm
                            organization.OrganizationPhones = UpdateOrganizationCollectionWithRemove<OrganizationPhone, Phone>(unitOfWork, organization.Id,
                                organization.OrganizationPhones, e => e.PhoneId);
                        }
                        if (vm.DeleteAllEmails || (vm.EmailAddresses != null && vm.EmailAddresses.Count > 0))
                        {
                            // Remove all emails that were not included in vm
                            organization.OrganizationEmails = UpdateOrganizationCollectionWithRemove<OrganizationEmail, Email>(unitOfWork, organization.Id,
                                organization.OrganizationEmails, e => e.EmailId);
                        }
                        if (vm.DeleteAllWebPages || (vm.WebPages != null && vm.WebPages.Count > 0))
                        {
                            // Remove all web pages that were not included in vm
                            organization.OrganizationWebAddress = UpdateOrganizationCollectionWithRemove<OrganizationWebPage, WebPage>(unitOfWork, organization.Id,
                                organization.OrganizationWebAddress, e => e.WebPageId);
                        }
                        if (vm.DeleteAllAddresses || (vm.Addresses != null && vm.Addresses.Count > 0))
                        {
                            // Remove all adresses that were not included in vm
                            organization.OrganizationAddresses = UpdateOrganizationCollectionWithRemove<OrganizationAddress, Address>(unitOfWork, organization.Id,
                                organization.OrganizationAddresses, e => e.AddressId);
                        }
                        if (vm.DeleteAllElectronicInvoicings || (vm.ElectronicInvoicings != null && vm.ElectronicInvoicings.Count > 0))
                        {
                            // Remove all electronic invoicing addresses that were not included in vm
                            dataUtils.RemoveItemCollection<OrganizationEInvoicing>(unitOfWork, s => s.OrganizationVersionedId == organization.Id);
                        }
                    }

                    // Update the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        UpdateExternalSource<Organization>(organization.UnificRootId, vm.SourceId, userId, unitOfWork);
                    }
                }

                unitOfWork.Save(saveMode, organization);
            });

            // Update the map coordinates for addresses
            if (vm.PublishingStatus != PublishingStatus.Deleted.ToString())
            {
                if (organization?.OrganizationAddresses?.Count > 0)
                {
                    var addresses = organization.OrganizationAddresses.Select(x => x.AddressId);
                    addressService.UpdateAddress(addresses.ToList());
                }
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<OrganizationVersioned, OrganizationLanguageAvailability>(organization.Id, i => i.OrganizationVersionedId == organization.Id);
            }

            return GetOrganizationWithDetails(organization.Id, openApiVersion, false);
        }

        private ICollection<T> UpdateOrganizationCollectionWithRemove<T, TEntity>(IUnitOfWorkWritable unitOfWork, Guid organizationId, ICollection<T> collection, Func<T, Guid> getSelectedIdFunc)
            where T : IOrganization  where TEntity : IEntityIdentifier
        {
            // Remove all organization related entities that were not included in collection
            var updatedEntities = collection.Select(getSelectedIdFunc).ToList();
            var rep = unitOfWork.CreateRepository<IRepository<T>>();
            var currentOrganizationEntities = rep.All().Where(e => e.OrganizationVersionedId == organizationId).Select(getSelectedIdFunc).ToList();
            var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            currentOrganizationEntities.Where(e => !updatedEntities.Contains(e)).ForEach(e => {
                var entity = entityRep.All().FirstOrDefault(r => r.Id == e);
                if (entity != null)
                {
                    entityRep.Remove(entity);
                }
                });

            return collection;
        }

        public PublishingStatus? GetOrganizationStatusByRootId(Guid id)
        {
            PublishingStatus? result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = versioningManager.GetLatestVersionPublishingStatus<OrganizationVersioned>(unitOfWork, id);
            });

            return result;
        }

        public PublishingStatus? GetOrganizationStatusBySourceId(string sourceId)
        {
            PublishingStatus? result = null;
            bool externalSourceExists = false;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var id = GetPTVId<Organization>(sourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                if (id != Guid.Empty)
                {
                    externalSourceExists = true;
                    result = versioningManager.GetLatestVersionPublishingStatus<OrganizationVersioned>(unitOfWork, id);
                }
            });
            if (!externalSourceExists) { throw new Exception(string.Format(CoreMessages.OpenApi.ExternalSourceNotExists, sourceId)); }
            return result;
        }

        #region Lock
        public IVmEntityBase LockOrganization(Guid id)
        {
            return utilities.LockEntityVersioned<OrganizationVersioned, Organization>(id);
        }

        public IVmEntityBase UnLockOrganization(Guid id)
        {
            return utilities.UnLockEntityVersioned<OrganizationVersioned, Organization>(id);
        }

        public IReadOnlyList<IVmBase> Get(IUnitOfWork unitOfWork)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
            var organizations = unitOfWork
                .ApplyIncludes(organizationRep.All().Where(x => x.PublishingStatusId == psPublished), query => query.Include(organization => organization.OrganizationNames));
            return CreateTree<VmTreeItem>(LoadOrganizationTree(organizations));
        }
        #endregion Lock

        public IVmEntityBase IsOrganizationEditable(Guid id)
        {
            return utilities.CheckIsEntityEditable<OrganizationVersioned, Organization>(id);
        }

        private Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> GetOrganizationIncludeChain()
        {
            return q =>
                q.Include(i => i.Business)
                .Include(i => i.Type)
                .Include(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.OrganizationEmails).ThenInclude(i => i.Email)
                .Include(i => i.OrganizationNames)
                .Include(i => i.OrganizationDisplayNameTypes)
                .Include(i => i.OrganizationDescriptions)
                .Include(i => i.OrganizationPhones).ThenInclude(i => i.Phone).ThenInclude(i => i.PrefixNumber)
                .Include(i => i.OrganizationWebAddress).ThenInclude(i => i.WebPage)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressStreets).ThenInclude(i => i.StreetNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Country).ThenInclude(i => i.CountryNames)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.AddressAdditionalInformations)
                .Include(i => i.OrganizationAddresses).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.OrganizationEInvoicings).ThenInclude(i => i.EInvoicingAdditionalInformations)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.OrganizationAreas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.OrganizationAreas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.OrganizationAreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                .Include(i => i.Parent).ThenInclude(i => i.Versions)
                .Include(i => i.UnificRoot);
        }
    }
}

