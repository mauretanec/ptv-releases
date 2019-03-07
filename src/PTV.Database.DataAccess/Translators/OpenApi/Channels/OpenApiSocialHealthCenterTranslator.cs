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

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelSocialHealthCenter, string>), RegisterType.Transient)]
    internal class OpenApiSocialHealthCenterTranslator : Translator<ServiceChannelSocialHealthCenter, string>
    {
        public OpenApiSocialHealthCenterTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives) { }

        public override string TranslateEntityToVm(ServiceChannelSocialHealthCenter entity)
        {
            return base.CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Oid, o => o)
                .GetFinal();
        }

        public override ServiceChannelSocialHealthCenter TranslateVmToEntity(string vModel)
        {
            throw new System.NotImplementedException("No translation implemented in OpenApiSocialHealthCenterTranslator.");
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannelSocialHealthCenter, VmOpenApiStringItem>), RegisterType.Transient)]
    internal class OpenApiSocialHealthCenterInTranslator : Translator<ServiceChannelSocialHealthCenter, VmOpenApiStringItem>
    {
        public OpenApiSocialHealthCenterInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives) { }

        public override VmOpenApiStringItem TranslateEntityToVm(ServiceChannelSocialHealthCenter entity)
        {
            throw new System.NotImplementedException("No translation implemented in OpenApiSocialHealthCenterInTranslator.");
        }

        public override ServiceChannelSocialHealthCenter TranslateVmToEntity(VmOpenApiStringItem vModel)
        {
            if (vModel == null || vModel.Value == null) return null;

            var definitions = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(
                  i => i.OwnerReferenceId.IsAssigned(),
                  i => o => i.OwnerReferenceId.Value == o.ServiceChannelId,
                  def => def.UseDataContextCreate(i => true)
                )
                .AddNavigation(i => vModel.Value, o => o.Oid);

            if (vModel.OwnerReferenceId.IsAssigned())
            {
                definitions.AddSimple(i => i.OwnerReferenceId.Value, o => o.ServiceChannelId);
            }
            return definitions.GetFinal();
        }
    }
}
