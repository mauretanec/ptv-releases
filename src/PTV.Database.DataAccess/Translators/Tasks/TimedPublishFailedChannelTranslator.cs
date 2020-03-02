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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Notifications;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Tasks
{
    [RegisterService(typeof(ITranslator<ServiceChannelLanguageAvailability, VmTimedPublishFailedTask>), RegisterType.Transient)]
    internal class TimedPublishFailedChannelTranslator : Translator<ServiceChannelLanguageAvailability, VmTimedPublishFailedTask>
    {
        private readonly ILanguageCache languageCache;
        
        public TimedPublishFailedChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) 
            : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmTimedPublishFailedTask TranslateEntityToVm(ServiceChannelLanguageAvailability entity)
        {
            var definition = CreateEntityViewModelDefinition<VmTimedPublishFailedTask>(entity)
                .AddSimple(i => i.ServiceChannelVersioned.UnificRootId, o => o.Id)
                .AddDictionary(i => i.ServiceChannelVersioned.ServiceChannelNames, o => o.Name, x => languageCache.GetByValue(x.LocalizationId))
                .AddSimple(i => i.StatusId, o => o.PublishingStatusId)
                .AddSimple(i => i.Created, o => o.Created)
                .AddNavigation(i => i.CreatedBy, o => o.CreatedBy)
                .AddSimple(i => EntityTypeEnum.Channel, o => o.EntityType)
                .AddNavigation(i => i.ServiceChannelVersioned.Type.Code, o => o.SubEntityType)
                .AddSimple(i => i.ServiceChannelVersionedId, o => o.VersionedId)
                .AddCollection(i => i.ServiceChannelVersioned.LanguageAvailabilities.Where(la => la.LastFailedPublishAt.HasValue).Select(la => la as ILanguageAvailability), o => o.LanguagesAvailabilities);

            return definition.GetFinal();
        }

        public override ServiceChannelLanguageAvailability TranslateVmToEntity(VmTimedPublishFailedTask vModel)
        {
            throw new System.NotImplementedException();
        }
    }
}