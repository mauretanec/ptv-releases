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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;


namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<AddressAdditionalInformation, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiAddressDescriptionTranslator : Translator<AddressAdditionalInformation, VmOpenApiLanguageItem>
    {
        private readonly ILanguageCache languageCache;

        public OpenApiAddressDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(AddressAdditionalInformation entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Text, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .GetFinal();
        }

        public override AddressAdditionalInformation TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            var exists = vModel.OwnerReferenceId.IsAssigned();
            var langId = languageCache.Get(vModel.Language);

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i => exists, i => o => i.OwnerReferenceId.Value == o.AddressId && langId == o.LocalizationId, w => w.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Value, o => o.Text)
                .AddNavigation(i => i.Language, o => o.Localization)
                .GetFinal();
        }
    }
}
