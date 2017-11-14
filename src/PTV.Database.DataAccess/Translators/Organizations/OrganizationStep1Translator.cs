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
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOrganizationStep1>), RegisterType.Transient)]
    internal class OrganizationStep1Translator : Translator<OrganizationVersioned, VmOrganizationStep1>
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;

        public OrganizationStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmOrganizationStep1 TranslateEntityToVm(OrganizationVersioned entity)
        {
            var step = CreateEntityViewModelDefinition<VmOrganizationStep1>(entity)

                    .AddSimple(input => input.Id, output => output.Id)
                    .AddSimple(input => input.UnificRootId, output => output.UnificRootId)
                    .AddSimple(input => input.TypeId, output => output.OrganizationTypeId)
                    .AddSimple(input => input.AreaInformationTypeId, output => output.AreaInformationTypeId)
                    .AddSimple(input => input.ParentId, output => output.ParentId)
                    .AddLocalizable(input => GetName(input, NameTypeEnum.Name), output => output.OrganizationName)
                    .AddLocalizable(input => GetName(input, NameTypeEnum.AlternateName), output => output.OrganizationAlternateName)
                    .AddNavigation(input => input.Oid, output => output.OrganizationId)
                    .AddSimple(input => typesCache.Compare<NameType>(input.OrganizationDisplayNameTypes.FirstOrDefault(type=>type.LocalizationId == RequestLanguageId)?.DisplayNameTypeId, NameTypeEnum.AlternateName.ToString()), output => output.IsAlternateNameUsedAsDisplayName)
                    .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description)
                    .AddNavigation(input => input.Municipality, output => output.Municipality)
                    .AddNavigation(input => input.Business, output => output.Business)
                    .AddSimple(input => input.PublishingStatusId, output => output.PublishingStatusId)
                    .AddCollection(input => languageCache.FilterCollection(input.OrganizationEmails.Select(x => x.Email), RequestLanguageCode).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.Emails)
                    .AddCollection(input => languageCache.FilterCollection(input.OrganizationPhones.Select(x => x.Phone), RequestLanguageCode).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PhoneNumbers)
                    .AddCollection(input => languageCache.FilterCollection(input.OrganizationWebAddress.OrderBy(x => x.WebPage.OrderNumber).Select(x=>x.WebPage), RequestLanguageCode), output => output.WebPages)
                    .AddCollection(input => input.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString())).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.VisitingAddresses)
                    .AddCollection(input => input.OrganizationAddresses.Where(x => x.CharacterId == typesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString())).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PostalAddresses)                    
                    .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())).Select(x => x.AreaId), output => output.BusinessRegions)
                    .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())).Select(x => x.AreaId), output => output.HospitalRegions)
                    .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString())).Select(x => x.AreaId), output => output.Provinces)
                    .AddSimpleList(input => input.OrganizationAreaMunicipalities.Select(x => x.MunicipalityId), output => output.Municipalities)
                    .GetFinal();

            step.ShowContacts = step.Emails.Count > 0 || step.PhoneNumbers.Count > 0 || step.WebPages.Count > 0 || step.PostalAddresses.Count > 0 || step.VisitingAddresses.Count > 0;
            step.ShowPostalAddress = step.PostalAddresses.Count > 0;
            step.ShowVisitingAddress = step.VisitingAddresses.Count > 0;


            return step;
        }

        private IEnumerable<IName> GetName(OrganizationVersioned organizationVersioned, NameTypeEnum type)
        {
            return organizationVersioned.OrganizationNames.Where(x => x.TypeId == typesCache.Get<NameType>(type.ToString()));
        }

        private IEnumerable<IDescription> GetDescription(OrganizationVersioned organizationVersioned, DescriptionTypeEnum type)
        {
            return organizationVersioned.OrganizationDescriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(type.ToString()));

        }

        public override OrganizationVersioned TranslateVmToEntity(VmOrganizationStep1 vModel)
        {
            throw new NotImplementedException();
        }
    }
}
