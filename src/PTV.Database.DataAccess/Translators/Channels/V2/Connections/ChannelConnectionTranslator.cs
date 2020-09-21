﻿/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using VmDigitalAuthorization = PTV.Domain.Model.Models.V2.Common.Connections.VmDigitalAuthorization;
using PTV.Domain.Model.Enums;
using System.Linq;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.Service;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(ITranslator<ServiceChannel, VmConnectionsInput>), RegisterType.Transient)]
    internal class ChannelConnectionsInputTranslator : Translator<ServiceChannel, VmConnectionsInput>
    {
        public ChannelConnectionsInputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager,
            translationPrimitives)
        {
        }

        public override VmConnectionsInput TranslateEntityToVm(ServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannel TranslateVmToEntity(VmConnectionsInput vModel)
        {
            var order = 1;
            vModel.SelectedConnections.ForEach(connection =>
            {
                connection.MainEntityId = connection.ConnectedEntityId;
                connection.ConnectedEntityId = vModel.UnificRootId;
                if (vModel.UseOrder)
                {
                    connection.ServiceOrderNumber = order++;
                }
                connection.MainEntityType = DomainEnum.Channels;
            });
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollectionWithRemove(
                    i => i.SelectedConnections,
                    o => o.ServiceServiceChannels,
                    r => vModel.IsAsti ? r.IsASTIConnection : !r.IsASTIConnection)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannel, VmChannelConnectionsOutput>), RegisterType.Transient)]
    internal class ChannelConnectionsOutputTranslator : Translator<ServiceChannel, VmChannelConnectionsOutput>
    {
        public ChannelConnectionsOutputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager,
            translationPrimitives)
        {
        }

        public override VmChannelConnectionsOutput TranslateEntityToVm(ServiceChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(input => input.ServiceServiceChannels.OrderBy(x => x.ServiceOrderNumber).ThenBy(x => x.Created), output => output.Connections)
                .GetFinal();
        }

        public override ServiceChannel TranslateVmToEntity(VmChannelConnectionsOutput vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmChannelConnectionOutput>), RegisterType.Transient)]
    internal class ChannelConnectionOutputTranslator : Translator<ServiceServiceChannel, VmChannelConnectionOutput>
    {
        public ChannelConnectionOutputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager,
            translationPrimitives)
        {
        }

        public override VmChannelConnectionOutput TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var connectionId = entity.ServiceId.ToString() + entity.ServiceChannelId.ToString();
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i=>i.ServiceOrderNumber, o=>o.ServiceOrderNumber)
                .AddPartial(i => i, o => o as VmConnectableService)
                .AddNavigation(i => connectionId, o => o.ConnectionId)
                .AddNavigation(i => i, o => o.BasicInformation)
                .AddNavigation(i => i, o => o.DigitalAuthorization)
                .AddNavigation(i => i, o => o.AstiDetails)
                .AddNavigation(i => i, o => o.ContactDetails)
                .AddNavigation(i => i, o => o.OpeningHours)
                .GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmChannelConnectionOutput vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmAstiDetails>), RegisterType.Transient)]
    internal class AstiDetailsTranslator : Translator<ServiceServiceChannel, VmAstiDetails>
    {
        public AstiDetailsTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        { }

        public override VmAstiDetails TranslateEntityToVm(ServiceServiceChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.ServiceServiceChannelExtraTypes, o => o.AstiTypeInfos)
                .AddSimple(i => i.IsASTIConnection, o => o.IsASTIConnection)
                .GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmAstiDetails vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmContactDetails>), RegisterType.Transient)]
    internal class ContactInformationTranslator : Translator<ServiceServiceChannel, VmContactDetails>
    {
        private readonly ITypesCache typesCache;
        private readonly EntityDefinitionHelper entityDefinitionHelper;

        public ContactInformationTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager,
            EntityDefinitionHelper entityDefinitionHelper
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            typesCache = cacheManager.TypesCache;
            this.entityDefinitionHelper = entityDefinitionHelper;
        }

        public override VmContactDetails TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var faxTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());
            var phoneTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
            var definition = CreateEntityViewModelDefinition(entity)
                .AddCollection(
                    input => input.ServiceServiceChannelAddresses,
                    output => output.PostalAddresses
                );

            entityDefinitionHelper
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceServiceChannelEmails.Select(x => x.Email),
                    o => o.Emails,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceServiceChannelWebPages,
                    o => o.WebPages,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceServiceChannelPhones
                        .Where(x => x.Phone.TypeId == phoneTypeId)
                        .Select(x => x.Phone),
                    o => o.PhoneNumbers,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceServiceChannelPhones
                        .Where(x => x.Phone.TypeId == faxTypeId)
                        .Select(x => x.Phone),
                    o => o.FaxNumbers,
                    k => languageCache.GetByValue(k.LocalizationId)
                );

            return definition.GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmContactDetails vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmOpeningHours>), RegisterType.Transient)]
    internal class ServiceHoursTranslator : Translator<ServiceServiceChannel, VmOpeningHours>
    {
        private readonly ITypesCache typesCache;
        public ServiceHoursTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpeningHours TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var serviceHours = entity.ServiceServiceChannelServiceHours.Select(x => x.ServiceHours).ToList();

            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Standard), o => o.StandardHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Exception), o => o.ExceptionHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Exception, true), o => o.HolidayHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Special), o => o.SpecialHours)
                .GetFinal();
        }
        private IEnumerable<ServiceHours> GetHoursByType(ICollection<ServiceHours> openingHours, ServiceHoursTypeEnum type, bool isHolidayHours = false)
        {
            if (isHolidayHours)
            {
                return openingHours
                    .Where(j => j.HolidayServiceHour != null && typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, type.ToString()))
                    .OrderBy(x => x.OrderNumber)
                    .ThenBy(x => x.Created);
            }

            return openingHours
                .Where(j => j.HolidayServiceHour == null && typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, type.ToString()))
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.Created);
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmOpeningHours vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceServiceChannelExtraType, VmAstiTypeInfo>), RegisterType.Transient)]
    internal class AstiTypeInfoTranslator : Translator<ServiceServiceChannelExtraType, VmAstiTypeInfo>
    {
        public AstiTypeInfoTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAstiTypeInfo TranslateEntityToVm(ServiceServiceChannelExtraType entity)

        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.ExtraSubTypeId, o => o.AstiTypeId)
                .AddDictionary
                (
                    i => i.ServiceServiceChannelExtraTypeDescriptions,
                    o => o.AstiDescription,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .GetFinal();
        }

        public override ServiceServiceChannelExtraType TranslateVmToEntity(VmAstiTypeInfo vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<ServiceServiceChannelExtraTypeDescription, string>), RegisterType.Transient)]
    internal class AstiDescriptionStringTranslator : Translator<ServiceServiceChannelExtraTypeDescription, string>
    {
        public AstiDescriptionStringTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        { }

        public override string TranslateEntityToVm(ServiceServiceChannelExtraTypeDescription entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Description, output => output)
                .GetFinal();
        }

        public override ServiceServiceChannelExtraTypeDescription TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<ExtraSubTypeName, string>), RegisterType.Transient)]
    internal class AstiNameStringTranslator : Translator<ExtraSubTypeName, string>
    {
        public AstiNameStringTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        { }

        public override string TranslateEntityToVm(ExtraSubTypeName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Name, output => output)
                .GetFinal();
        }

        public override ExtraSubTypeName TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmConnectableService>), RegisterType.Transient)]
    internal class ChannelConnectableServiceTranslator : Translator<ServiceServiceChannel, VmConnectableService>
    {
        public ChannelConnectableServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmConnectableService TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var service = VersioningManager.ApplyPublishingStatusFilterFallback(entity.Service.Versions);
            if (service == null) return new VmConnectableService();

            return CreateEntityViewModelDefinition(entity)
                .AddPartial(i => service, o => o)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddNavigation(i => i.ModifiedBy, o => o.ModifiedBy)
                .GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmConnectableService vModel)
        {
            throw new NotImplementedException("Translator VmConnectableService -> ServiceServiceChannel is not implemented");
        }
    }

    [RegisterService(typeof(ITranslator<ServiceVersioned, VmConnectableService>), RegisterType.Transient)]
    internal class ServiceVersionedConnectableServiceTranslator : Translator<ServiceVersioned, VmConnectableService>
    {
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly ITypesCache typesCache;

        public ServiceVersionedConnectableServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageOrderCache = cacheManager.LanguageOrderCache;
        }

        public override VmConnectableService TranslateEntityToVm(ServiceVersioned entity)
        {
            if (entity == null) return null;

            var serviceTypeId = entity.StatutoryServiceGeneralDescriptionId.IsAssigned()
                ? VersioningManager.ApplyPublishingStatusFilterFallback(entity.StatutoryServiceGeneralDescription.Versions)?.TypeId
                : entity.TypeId;

            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId)), o => o.LanguagesAvailabilities)
                .AddSimple(i => serviceTypeId, o => o.ServiceType)
                .AddSimple(i => i.OrganizationId, o => o.OrganizationId)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddNavigation(i => i.ModifiedBy, o => o.ModifiedBy)
                .AddDictionary(i => i.ServiceNames.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), o => o.Name, i => languageCache.GetByValue(i.LocalizationId))
                .GetFinal();
        }

        public override ServiceVersioned TranslateVmToEntity(VmConnectableService vModel)
        {
            throw new NotImplementedException("Translator VmConnectableService -> ServiceVersioned is not implemented");
        }
    }
}
