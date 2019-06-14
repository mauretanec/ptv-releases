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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class OrganizationGuidPageHandler : GuidPageHandler<V8VmOpenApiOrganizationGuidPage, V8VmOpenApiOrganizationItem, OrganizationVersioned, Organization>
    {
        public OrganizationGuidPageHandler(
            EntityStatusExtendedEnum entityStatus,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            int pageNumber,
            int pageSize
            ) : base(entityStatus, date, dateBefore, publishingStatusCache, pageNumber, pageSize)
        { }

        protected override IList<Expression<Func<OrganizationVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var filters = base.GetFilters(unitOfWork);

            // Get only organizations that have services or channels attached (PTV-1619)
            if (EntityStatus == EntityStatusExtendedEnum.Published)
            {
                filters.Add(o => o.UnificRoot.OrganizationServiceChannelsVersioned.Any(c => c.PublishingStatusId == PublishedId) || // Published channels attached
                o.UnificRoot.OrganizationServicesVersioned.Any(s => s.PublishingStatusId == PublishedId) || // Organization as main responsible for a service
                o.UnificRoot.OrganizationServices.Any(s => s.ServiceVersioned.PublishingStatusId == PublishedId) || // Organization as other responsible for a service
                o.UnificRoot.ServiceProducerOrganizations.Any(s => s.ServiceProducer.ServiceVersioned.PublishingStatusId == PublishedId)); // Organization as service producer
            }
            return filters;
        }

        protected override V8VmOpenApiOrganizationItem GetItemData(OrganizationVersioned entity)
        {
            return new V8VmOpenApiOrganizationItem
            {
                Id = entity.UnificRootId,
                Name = Names.TryGetOrDefault(entity.Id, new Dictionary<Guid, string>())?.FirstOrDefault().Value,
                ParentOrganizationId = entity.ParentId
            };
        }
    }
}
