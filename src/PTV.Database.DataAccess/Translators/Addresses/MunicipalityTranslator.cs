/**
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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<Municipality, VmMunicipality>), RegisterType.Transient)]
    internal class MunicipalityTranslator: Translator<Municipality, VmMunicipality>
    {
        private readonly TranslatedItemDefinitionHelper definitionHelper;
        private readonly ICacheManager cacheManager;

        public MunicipalityTranslator(
            IResolveManager resolveManager, 
            ITranslationPrimitives translationPrimitives,
            TranslatedItemDefinitionHelper definitionHelper, 
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            this.cacheManager = cacheManager;
        }

        public override VmMunicipality TranslateEntityToVm(Municipality entity)
        {
            var finnish = cacheManager.LanguageCache.Get("fi");
            
            var translation = CreateEntityViewModelDefinition<VmMunicipality>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Description, o => o.Description)
                .AddNavigation(i => i.Code, o => o.MunicipalityCode);
            
            definitionHelper.AddTranslations(translation,
                entity.MunicipalityNames.FirstOrDefault(sn => sn.LocalizationId == finnish)?.Name);
            return translation.GetFinal();
        }

        public override Municipality TranslateVmToEntity(VmMunicipality vModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
