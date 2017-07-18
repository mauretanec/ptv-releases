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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceRequirement, VmServiceRequirement>), RegisterType.Transient)]
    internal class ServiceRequirementTranslator : Translator<StatutoryServiceRequirement, VmServiceRequirement>
    {
        public ServiceRequirementTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
        public override VmServiceRequirement TranslateEntityToVm(StatutoryServiceRequirement entity)
        {
            throw new NotImplementedException();
        }

        public override StatutoryServiceRequirement TranslateVmToEntity(VmServiceRequirement vModel)
        {
            return CreateViewModelEntityDefinition<StatutoryServiceRequirement>(vModel)
                .UseDataContextLocalizedUpdate(input => input.Id.HasValue, input => output => input.Id == output.StatutoryServiceGeneralDescriptionVersionedId, def => def.UseDataContextCreate(x => true))
                .AddNavigation(input => input.Requirement, output => output.Requirement)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<StatutoryServiceRequirement, JsonLanguageLabel>), RegisterType.Transient)]
    internal class StatutoryServiceRequirementTranslator : Translator<StatutoryServiceRequirement, JsonLanguageLabel>
    {
        private ILanguageCache languageCache;
        public StatutoryServiceRequirementTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }
        public override JsonLanguageLabel TranslateEntityToVm(StatutoryServiceRequirement entity)
        {
            throw new NotSupportedException();
//            return CreateEntityViewModelDefinition(entity).AddNavigation(i => i.Requirement, o => o).GetFinal();
        }

        public override StatutoryServiceRequirement TranslateVmToEntity(JsonLanguageLabel vModel)
        {
            var languageId = languageCache.Get(vModel.Lang);
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(),
                    i => o => i.OwnerReferenceId == o.StatutoryServiceGeneralDescriptionVersionedId && languageId == o.LocalizationId, def => def.UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid()))
                .AddNavigation(i => i.Label, o => o.Requirement)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .GetFinal();
        }
    }
}
