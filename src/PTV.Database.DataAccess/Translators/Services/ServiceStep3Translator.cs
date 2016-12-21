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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<Service, VmServiceStep3>), RegisterType.Transient)]
    internal class ServiceStep3Translator : Translator<Service, VmServiceStep3>
    {
        public ServiceStep3Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmServiceStep3 TranslateEntityToVm(Service entity)
        {
            return CreateEntityViewModelDefinition<VmServiceStep3>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimpleList(input => input.OrganizationServices.Where(x => x.RoleType.Code == RoleTypeEnum.Responsible.ToString() && x.OrganizationId.HasValue).Select(x => x.OrganizationId.Value), output => output.Organizers)
                .AddCollection(input => input.OrganizationServices.Where(x => x.RoleType.Code == RoleTypeEnum.Producer.ToString()).OrderBy(x => x.Created), output => output.ServiceProducers)
                .GetFinal();
        }

        public override Service TranslateVmToEntity(VmServiceStep3 vModel)
        {
            throw new NotSupportedException();
        }

    }
}
