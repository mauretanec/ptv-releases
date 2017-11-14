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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IServiceService
    {
        IVmGetServiceSearch GetServiceSearch();
        IVmServiceSearchResult SearchServices(IVmServiceSearch vmServiceSearch);
        /// <summary>
        /// Get service for service id
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        IVmServiceSearchResult SearchRelationService(IVmServiceSearch model);
        /// <summary>
        /// Get services for channel id
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        IVmServiceSearchResult SearchRelationServices(IVmServiceSearch model);
        IVmServiceStep1 GetServiceStep1(IVmGetServiceStep vmGetServiceStep);
        IVmServiceStep2 GetServiceStep2(IVmGetServiceStep vmGetServiceStep);
        IVmServiceStep3 GetServiceStep3(IVmGetServiceStep vmGetServiceStep);
        IVmServiceStep4ChannelData GetServiceStep4Channeldata(IVmGetServiceStep vmGetServiceStep);
        IVmEntityBase AddService(VmService model);
        VmPublishingResultModel PublishService(VmPublishingModel model);

        VmPublishingResultModel WithdrawService(Guid serviceId);
        VmPublishingResultModel RestoreService(Guid serviceId);
        /// <summary>
        /// Lock service by Id
        /// </summary>
        /// <param name="id">Service Id</param>
        /// <returns></returns>
        IVmEntityBase LockService(Guid id);
        /// <summary>
        /// UnLock service by Id
        /// </summary>
        /// <param name="id">Service Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockService(Guid id);
        /// <summary>
        /// Gets the service names.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        VmEntityNames GetServiceNames(VmEntityBase model);
//        /// <summary>
//        /// Publish all services
//        /// </summary>
//        /// <param name="serviceIds">List of services ids</param>
//        /// <returns></returns>
//        IVmListItemsData<VmEntityStatusBase> PublishServices(List<Guid> serviceIds);
        IVmEntityBase DeleteService(Guid serviceId);
        IVmOpenApiGuidPageVersionBase GetServices(DateTime? date, int pageNumber, int pageSize, bool archived, bool active);
        IVmOpenApiGuidPageVersionBase GetServicesByServiceChannel(Guid channelId, DateTime? date, int pageNumber = 1, int pageSize = 1000);
        IVmOpenApiGuidPageVersionBase GetServicesByServiceClass(Guid serviceClassId, DateTime? date, int pageNumber = 1, int pageSize = 1000);
        IVmOpenApiGuidPageVersionBase GetServicesByMunicipality(Guid municipalityId, DateTime? date, int pageNumber, int pageSize);
        IVmOpenApiServiceVersionBase GetServiceById(Guid id, int openApiVersion, VersionStatusEnum status);
        IVmOpenApiServiceVersionBase GetServiceByIdSimple(Guid id, bool getOnlyPublished = true);
        IList<IVmOpenApiServiceVersionBase> GetServicesByServiceChannel(Guid id, DateTime? date, int openApiVersion);
        IVmOpenApiServiceVersionBase GetServiceBySource(string sourceId);
        IVmOpenApiServiceBase AddService(IVmOpenApiServiceInVersionBase vm, bool allowAnonymous, int openApiVersion, bool attachProposedChannels);
        IVmServiceStep1 SaveStep1Changes(Guid serviceId, VmServiceStep1 model);
        IVmServiceStep2 SaveStep2Changes(Guid serviceId, VmServiceStep2 model);
        IVmServiceStep3 SaveStep3Changes(Guid serviceId, VmServiceStep3 model);
        IVmServiceStep4ChannelData SaveStep4Changes(Guid serviceId, List<Guid> model);
        IVmEntityBase GetServiceStatus(Guid serviceId);
       IVmOpenApiServiceBase SaveService(IVmOpenApiServiceInVersionBase vm, bool allowAnonymous, int openApiVersion, bool attachProposedChannels, string sourceId = null);
        bool ServiceExists(Guid serviceId);

        /// <summary>
        /// Get information if service is editable
        /// </summary>
        /// <param name="id">Service Id</param>
        /// <returns></returns>
        IVmEntityBase IsServiceEditable(Guid id);

        /// <summary>
        /// Searching services for service search in relation.
        /// </summary>
        /// <param name="vmServiceSearch"></param>
        /// <returns></returns>
        VmServiceSearchResultResult RelationSearchServices(IVmServiceSearch vmServiceSearch);
    }
}
