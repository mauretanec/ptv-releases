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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class ServiceChannelsByMunicipalityPageHandler : V3GuidPageHandler<ServiceChannelVersioned, ServiceChannel>
    {
        private Guid municipalityId;
        private bool includeWholeCountry;
        private ITypesCache typesCache;

        public ServiceChannelsByMunicipalityPageHandler(
            Guid municipalityId,
            bool includeWholeCountry,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            ITypesCache typesCache,
            int pageNumber,
            int pageSize
            ) : base(EntityStatusExtendedEnum.Published, date, dateBefore, publishingStatusCache, pageNumber, pageSize)
        {
            this.municipalityId = municipalityId;
            this.includeWholeCountry = includeWholeCountry;
            this.typesCache = typesCache;
        }

        protected override IList<Expression<Func<ServiceChannelVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = base.GetFilters(unitOfWork);

            if (includeWholeCountry) // SFIPTV-806
            {
                // Areas related to defined municipality
                var areas = unitOfWork.CreateRepository<IAreaMunicipalityRepository>().All()
                .Where(a => a.MunicipalityId == municipalityId).Select(a => a.AreaId).ToList();

                // Get channels
                var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
                // is the municipality in 'Åland'? So do we need to include also AreaInformationType WholeCountryExceptAlandIslands?
                if (IsAreaInAland(unitOfWork, areas, typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))) // Åland
                {
                    filters.Add(c => (c.AreaInformationTypeId == wholeCountryId) ||
                    (c.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || c.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
                }
                else
                {
                    var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
                    filters.Add(c => (c.AreaInformationTypeId == wholeCountryId) || c.AreaInformationTypeId == wholeCountryExceptAlandId ||
                    (c.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || c.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId))));
                }
            }
            else
            {
                // Only return services that have the defined municipality attached
                filters.Add(c => c.AreaMunicipalities.Any(a => a.MunicipalityId == municipalityId) || c.Areas.Any(a => a.Area.AreaMunicipalities.Any(m => m.MunicipalityId == municipalityId)));
            }
            return filters;
        }
    }
}
