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

using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Finto
{
    [RegisterService(typeof(ITranslator<OntologyTerm, VmOpenApiFintoItem>), RegisterType.Transient)]
    internal class OpenApiOntologyTermTranslator : Translator<OntologyTerm, VmOpenApiFintoItem>
    {
        public OpenApiOntologyTermTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiFintoItem TranslateEntityToVm(OntologyTerm entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Label, o => o.Name)
                .AddNavigation(i => i.Code, o => o.Code)
                .AddNavigation(i => i.OntologyType, o => o.OntologyType)
                .AddNavigation(i => i.Uri, o => o.Uri)
                .AddSimple(i => i.Parents.FirstOrDefault()?.ParentId, o => o.ParentId)
                .GetFinal();
        }

        public override OntologyTerm TranslateVmToEntity(VmOpenApiFintoItem vModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
