/**
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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationWebPage, V9VmOpenApiWebPage>), RegisterType.Transient)]
    internal class OpenApiOrganizationWebPageTranslator : Translator<OrganizationWebPage, V9VmOpenApiWebPage>
    {
        private ITypesCache typesCache;
        public OpenApiOrganizationWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override V9VmOpenApiWebPage TranslateEntityToVm(OrganizationWebPage entity)
        {
            return CreateEntityViewModelDefinition<V9VmOpenApiWebPage>(entity)
                .AddNavigation(i => i.WebPage, o => o.Url)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddNavigation(i => i.Name, o => o.Value)
                .AddSimple(i => i.OrderNumber ?? 0, o => o.OrderNumber)
                .GetFinal();
        }

        public override OrganizationWebPage TranslateVmToEntity(V9VmOpenApiWebPage vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var homePageType = typesCache.Get<WebPageType>(WebPageTypeEnum.HomePage.ToString());
            var exists = vModel.OwnerReferenceId.IsAssigned() && !vModel.Url.IsNullOrWhitespace();
            var languageId = languageCache.Get(vModel.Language);
            return CreateViewModelEntityDefinition<OrganizationWebPage>(vModel)
                .UseDataContextCreate(x => !exists)
                .UseDataContextLocalizedUpdate(x => exists, i => o => i.OwnerReferenceId == o.OrganizationVersionedId
                    && i.Url == o.WebPage.Url && languageId == o.LocalizationId, def => def.UseDataContextCreate(i => true))
                .AddSimple(i => languageId, o => o.LocalizationId)
                .AddNavigation(i => i.Url, o => o.WebPage)
                .AddNavigation(i => i.Value, o => o.Name)
                .AddSimple(i => homePageType, o => o.TypeId)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .GetFinal();
        }
    }
}
