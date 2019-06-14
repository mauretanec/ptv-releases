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

using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class ExpiringTaskPageHandler<TEntity, TLanguageAvailability> : GuidPageWithNameHandlerBase<VmOpenApiExpiringTasks, VmOpenApiExpiringTask, TEntity>, ITaskHandlerBase<TEntity, TLanguageAvailability, VmOpenApiExpiringTask>
        where TEntity : class, IEntityIdentifier, IAuditing, IVersionedVolume
        where TLanguageAvailability : class, ILanguageAvailability
    {
        private List<Guid> entityIds;
        private List<Guid> publishingStatusIds;
        private DateTime lifeTime;
        private ITranslationEntity translationManagerToVm;
        private DateTime dateUtcNow;

        public Dictionary<Guid, List<TLanguageAvailability>> LanguageAvailabilities { get; set; }

        public ExpiringTaskPageHandler(
            List<Guid> entityIds,
            List<Guid> publishingStatusIds,
            ITranslationEntity translationManagerToVm,
            DateTime lifeTime,
            int pageNumber,
            int pageSize
            ) : base(pageNumber, pageSize)
        {
            this.entityIds = entityIds;
            this.publishingStatusIds = publishingStatusIds;
            this.lifeTime = lifeTime;
            this.translationManagerToVm = translationManagerToVm;
            dateUtcNow = DateTime.UtcNow;
        }

        protected override IList<Expression<Func<TEntity, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            return new List<Expression<Func<TEntity, bool>>>() { c => entityIds.Contains(c.UnificRootId) && publishingStatusIds.Contains(c.PublishingStatusId) };
        }

        protected override VmOpenApiExpiringTask GetItemData(TEntity entity)
        {
            return new VmOpenApiExpiringTask
            {
                Id = entity.UnificRootId,
                Name = Names.TryGetOrDefault(entity.Id, new Dictionary<Guid, string>())?.FirstOrDefault().Value,
                Statuses = translationManagerToVm.TranslateAll<TLanguageAvailability, VmOpenApiLanguageItem>(LanguageAvailabilities.TryGetOrDefault(entity.Id, new List<TLanguageAvailability>()))?.ToList(),
                ModifiedBy = entity.ModifiedBy,
                Expires = dateUtcNow.Add(entity.Modified - lifeTime)
            };
        }
    }
}
