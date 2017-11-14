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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.Organizations
{

    [RegisterService(typeof(ITranslator<ServiceServiceChannelDescription, VmDescription>), RegisterType.Transient)]
    internal class ServiceServiceChannelDescriptionTranslator : Translator<ServiceServiceChannelDescription, VmDescription>
    {
        public ServiceServiceChannelDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmDescription TranslateEntityToVm(ServiceServiceChannelDescription entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannelDescription TranslateVmToEntity(VmDescription vModel)
        {
            if (vModel.LocalizationId.HasValue && vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }

            return CreateViewModelEntityDefinition<ServiceServiceChannelDescription>(vModel)
                .UseDataContextLocalizedUpdate(i => true, i => o => (i.OwnerReferenceId == o.ServiceId) && (i.OwnerReferenceId2 == o.ServiceChannelId) && (i.TypeId == o.TypeId), def => def.UseDataContextCreate(i => true))
                .AddNavigation(input => input.Description, output => output.Description)
                .AddRequestLanguage(output => output)
                .AddSimple(input => input.TypeId, output => output.TypeId)
                .GetFinal();
        }
    }


    [RegisterService(typeof(ITranslator<ServiceServiceChannelDescription, VmLocalizedDescription>), RegisterType.Transient)]
    internal class ServiceServiceChannelLocalizedDescriptionTranslator : Translator<ServiceServiceChannelDescription, VmLocalizedDescription>
    {
        private ILanguageCache languageCache;
        public ServiceServiceChannelLocalizedDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmLocalizedDescription TranslateEntityToVm(ServiceServiceChannelDescription entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannelDescription TranslateVmToEntity(VmLocalizedDescription vModel)
        {
            Guid languageId = languageCache.Get(vModel.Language.ToString());
            return CreateViewModelEntityDefinition<ServiceServiceChannelDescription>(vModel)
                .UseDataContextUpdate(i => true, i => o => (i.OwnerReferenceId == o.ServiceId) && (i.OwnerReferenceId2 == o.ServiceChannelId) && (i.TypeId == o.TypeId) && (languageId == o.LocalizationId)
                ,def => def.UseDataContextCreate(i => true))
                .AddNavigation(input => input.Description, output => output.Description)
                .AddSimple(input => input.TypeId, output => output.TypeId)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .GetFinal();
        }
    }


    [RegisterService(typeof(ITranslator<ServiceServiceChannelDescription, string>), RegisterType.Transient)]
    internal class ServiceServiceChannelDescriptionStringTranslator : Translator<ServiceServiceChannelDescription, string>
    {
        public ServiceServiceChannelDescriptionStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(ServiceServiceChannelDescription entity)
        {
            return CreateEntityViewModelDefinition(entity)
              .AddNavigation(i => i.Description, o => o)
              .GetFinal();
        }

        public override ServiceServiceChannelDescription TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
}
