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
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V10;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IServiceCollectionService
    {
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceCollections(DateTime? date, int pageNumber, int pageSize, bool archived, DateTime? dateBefore = null);
        IVmOpenApiServiceCollectionVersionBase GetServiceCollectionById(Guid id, int openApiVersion, bool getOnlyPublished = true);
        IVmOpenApiServiceCollectionVersionBase GetServiceCollectionBySource(string sourceId, int openApiVersion, bool getOnlyPublished = true, string userName = null);
        IVmOpenApiModelWithPagingBase<V10VmOpenApiServiceCollectionItem> GetServiceCollectionsByOrganization(IList<Guid> organizationIds, int pageNumber);
        IVmOpenApiServiceCollectionBase AddServiceCollection(IVmOpenApiServiceCollectionInVersionBase vm, int openApiVersion, string userName = null);
        IVmOpenApiServiceCollectionBase SaveServiceCollection(IVmOpenApiServiceCollectionInVersionBase vm, int openApiVersion, string sourceId = null, string userName = null);
    }
}
