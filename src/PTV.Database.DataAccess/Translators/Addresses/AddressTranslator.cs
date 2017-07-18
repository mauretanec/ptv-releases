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
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<Address, VmAddressSimple>), RegisterType.Transient)]
    internal class AddressTranslator : Translator<Address, VmAddressSimple>
    {

        private readonly ITypesCache typesCache;

        public AddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmAddressSimple TranslateEntityToVm(Address entity)
        {
            StreetAddressTypeEnum type = GetStreetAddressType(entity);

            var definition = CreateEntityViewModelDefinition<VmAddressSimple>(entity)
                .AddPartial(i => i, o => o as VmAddressSimpleBase)
                .AddSimple(input => input.MunicipalityId, output => output.MunicipalityId)
                .AddLocalizable(input => input.PostOfficeBoxNames, output => output.PoBox)
                .AddNavigation(input => input.PostalCode != null && input.PostalCode.Code != "Undefined" ? input.PostalCode : null, output => output.PostalCode)
                .AddNavigation(input => input.StreetNumber, output => output.StreetNumber)
                .AddLocalizable(input => input.StreetNames, output => output.Street)
                .AddNavigation(input => type.ToString(), output => output.StreetType)
                .AddLocalizable<IText, string>(input => input.AddressAdditionalInformations, output => output.AdditionalInformation);

            return definition.GetFinal();
        }

        public override Address TranslateVmToEntity(VmAddressSimple vModel)
        {
            if (vModel == null) return null;

            bool exists = vModel.Id.IsAssigned();
            StreetAddressTypeEnum type;
            if (!Enum.TryParse(vModel.StreetType, true, out type))
            {
                type = StreetAddressTypeEnum.Street;
            }
            if (vModel.PostalCode == null)
            {
                throw new Exception("AddressTranslator, PostalCodeId must be filled in!");
            }

            var transaltionDefinition = CreateViewModelEntityDefinition<Address>(vModel)
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
                .AddSimple(input => input.MunicipalityId, output => output.MunicipalityId)
                .AddNavigation(input => CountryCode.FI.ToString(), output => output.Country)
                .AddNavigation(input => input.StreetNumber, output => output.StreetNumber);

            if (vModel.PostalCode.Id.IsAssigned())
            {
                transaltionDefinition.AddSimple(input => vModel.PostalCode.Id, o => o.PostalCodeId);
            }
            else
            {
                transaltionDefinition.AddNavigation(input => input.PostalCode, output => output.PostalCode);
            }

            switch (type)
            {
                case StreetAddressTypeEnum.PostBox:
                    SetPoBoxTranslation(transaltionDefinition);
                    break;
                case StreetAddressTypeEnum.Street:
                    SetStreetAddtressTranslation(transaltionDefinition, vModel.Id);
                    break;
                default:
                    break;
            }

            if (vModel.AdditionalInformation != null)
            {
                transaltionDefinition.AddCollection(input => new List<VmStringText> { new VmStringText { Text = input.AdditionalInformation, Id = vModel.Id ?? Guid.Empty } }, output => output.AddressAdditionalInformations);
            }

            var entity = transaltionDefinition.GetFinal();

            vModel.Coordinates?.ForEach(x => x.OwnerReferenceId = entity.Id);

            // TODO: apply the remove flag on coordinates
            // save main coordinate with "Loading" state
            transaltionDefinition.AddCollectionWithRemove(
                i => vModel.Coordinates ?? new List<VmCoordinate>
                {
                    new VmCoordinate
                    {
                        OwnerReferenceId = entity.Id,
                        TypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString()),
                        CoordinateState = CoordinateStates.Loading.ToString(),
                        Latitude = 0,
                        Longtitude = 0
                    }
                }, o => o.Coordinates, TranslationPolicy.FetchData, r => true);


            return entity;
        }

        private static void SetPoBoxTranslation(ITranslationDefinitions<VmAddressSimple, Address> transaltionDefinition)
        {
            transaltionDefinition.AddLocalizable(input => input, output => output.PostOfficeBoxNames);
            transaltionDefinition.AddLocalizable(input => new VmAddressSimple() {Street = string.Empty, Id = input.Id}, output => output.StreetNames);
        }

        private StreetAddressTypeEnum GetStreetAddressType(Address address)
        {
            var poBox = address?.PostOfficeBoxNames.Any(x => !string.IsNullOrEmpty(x.Text));
            if (poBox.HasValue && poBox.Value)
            {
                return StreetAddressTypeEnum.PostBox;
            }

            return StreetAddressTypeEnum.Street;
        }

        private static void SetStreetAddtressTranslation(ITranslationDefinitions<VmAddressSimple, Address> transaltionDefinition, Guid? id)
        {
            transaltionDefinition.AddLocalizable(input => input, output => output.StreetNames);
            transaltionDefinition.AddLocalizable(input => new VmAddressSimple() { PoBox = string.Empty, Id = input.Id }, output => output.PostOfficeBoxNames);
        }

    }

    [RegisterService(typeof(ITranslator<Address, VmAddressSimpleBase>), RegisterType.Transient)]
    internal class BaseAddressTranslator : Translator<Address, VmAddressSimpleBase>
    {
        private readonly ITypesCache typesCache;

        public BaseAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmAddressSimpleBase TranslateEntityToVm(Address entity)
        {
            var definition = CreateEntityViewModelDefinition<VmAddressSimpleBase>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddCollection(input => GetCoordinates(input.Coordinates), output => output.Coordinates);

            return definition.GetFinal();
        }

        private IList<Coordinate> GetCoordinates(ICollection<Coordinate> coordinates)
        {
            var mainCoordinateType = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());
            var mainCoordinates = coordinates.Where(x => x.TypeId == mainCoordinateType).ToList();
            var mainCoordinate = mainCoordinates.Count > 1
                ? mainCoordinates.FirstOrDefault(x => x.CoordinateState == CoordinateStates.Ok.ToString()) ??
                  mainCoordinates.FirstOrDefault()
                : mainCoordinates.FirstOrDefault();
            var filteredCoordinates = coordinates.Except(mainCoordinates).ToList();
            if (mainCoordinate != null)
            {
                filteredCoordinates.Add(mainCoordinate);
            }

            return filteredCoordinates;
        }
        public override Address TranslateVmToEntity(VmAddressSimpleBase vModel)
        {
            throw new NotImplementedException();
        }
    }
}
