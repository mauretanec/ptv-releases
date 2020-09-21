﻿/**
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


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.DataValidators;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Service and channel base controller
    /// </summary>
    public class ServiceAndChannelBaseController : BaseController
    {
        private IServiceAndChannelService serviceAndChannelService;
        private IServiceService serviceService;
        private IChannelService channelService;
        private IUserOrganizationService userOrganizationService;
        private ICodeService codeService;

        private int versionNumber;

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceAndChannelService">The service and channel service.</param>
        /// <param name="serviceService">The service service.</param>
        /// <param name="channelService">The channel service.</param>
        /// <param name="userOrganizationService">The user organization service</param>
        /// <param name="codeService"></param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">Open api version.</param>
        public ServiceAndChannelBaseController(
            IServiceAndChannelService serviceAndChannelService,
            IServiceService serviceService,
            IChannelService channelService,
            IUserOrganizationService userOrganizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            ILogger logger,
            int versionNumber)
            : base(userOrganizationService, settings, logger)
        {
            this.serviceAndChannelService = serviceAndChannelService;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.userOrganizationService = userOrganizationService;
            this.codeService = codeService;

            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PostServiceAndChannel<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>(List<TModelRelation> request)
            where TModelRelation : IOpenApiConnectionPost<TModelContact, TModelServiceHours, TModelOpeningTime>
            where TModelContact : IVmOpenApiContactDetailsInVersionBase
            where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var connections = request.Select(x => x.ConvertToLatestVersion()).ToList();
            // validate services access rights
            var servicesValidator = new ServiceIdListValidator(connections.Select(x=>Guid.Parse(x.ServiceId)).ToList(), serviceService, "Services", true);
            servicesValidator.Validate(ModelState);

            // validate channels access rights
            var serviceChannelValidator = new ServiceChannelIdListValidator(connections.Select(x=>Guid.Parse(x.ServiceChannelId)).ToList(), channelService, userOrganizationService.UserHighestRole());
            serviceChannelValidator.Validate(ModelState);

            // Validate service hours and contact details
            var connectionValidator = new ConnectionBaseValidator<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>(request, codeService, "[]");
            connectionValidator.Validate(ModelState);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var list = new List<V11VmOpenApiServiceServiceChannelAstiInBase>();
            request.ForEach(i => list.Add(i.ConvertToLatestVersion()));

            var msgList = serviceAndChannelService.SaveServicesAndChannels(list);

            return Ok(msgList);
        }

        /// <summary>
        /// Update service and channel relationships. This is for ASTI connections.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutAstiServiceConnections<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>
            (string serviceId, IOpenApiServiceRelation<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime> request)
            where TModelRelation : IOpenApiAstiConnectionForService<TModelContact, TModelServiceHours, TModelOpeningTime>
            where TModelContact : IVmOpenApiContactDetailsInVersionBase
            where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            return PutServiceConnections(serviceId, request, true);
        }

        /// <summary>
        /// Post service and channel relationship.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PostServiceAndChannelBySource<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>
            (List<TModelRelation> request)
            where TModelRelation : IOpenApiConnectionBySourcePost<TModelContact, TModelServiceHours, TModelOpeningTime>
            where TModelContact : IVmOpenApiContactDetailsInVersionBase
            where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var list = new List<V11VmOpenApiServiceAndChannelRelationBySource>();
            request.ForEach(r =>
            {
                list.Add(r.ConvertToLatestVersion());
            });

            var connectionAdditionalInfoValidator = new ConnectionBaseValidator<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>(request, codeService, "[]");
            connectionAdditionalInfoValidator.Validate(ModelState);

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return Ok(serviceAndChannelService.SaveServicesAndChannelsBySource(list));
        }

        /// <summary>
        /// Post service and channel relationship. This is for both regular and Asti connections.
        /// </summary>
        /// <param name="serviceSourceId">External source identifier for service</param>
        /// <param name="request"></param>
        /// <param name="isASTI"></param>
        /// <returns></returns>
        protected virtual IActionResult PutServiceConnectionsBySource<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>
            (string serviceSourceId, IOpenApiServiceRelationBySource<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime> request, bool isASTI = false)
            where TModelRelation : IOpenApiConnectionBySource<TModelContact, TModelServiceHours, TModelOpeningTime>
            where TModelContact : IVmOpenApiContactDetailsInVersionBase
            where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // Do we have dublicates (PTV-3812)?
            var duplicates = request.ChannelRelations.GroupBy(i => i.ServiceChannelSourceId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicates.Count > 0)
            {
                ModelState.AddModelError("ChannelRelations", string.Format(CoreMessages.OpenApi.DublicateItemsNotAllowed, string.Join(", ", duplicates)));
                return new BadRequestObjectResult(ModelState);
            }

            // Validate service hours and contact details
            var connectionAdditionalInfoValidator = new ConnectionBaseValidator<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>(request.ChannelRelations, codeService);
            connectionAdditionalInfoValidator.Validate(ModelState);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var srv = serviceAndChannelService.SaveServiceConnectionsBySource(serviceSourceId, request.ConvertToLatestVersion(), versionNumber, isASTI);
            if (srv == null)
            {
                return NotFound(new VmError { ErrorMessage = $"Service with source id '{serviceSourceId}' not found." });
            }

            return Ok(srv);
        }

        /// <summary>
        /// Post service and channel relationship. This is for ASTI connections.
        /// </summary>
        /// <param name="serviceSourceId">External source identifier for service</param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutAstiServiceConnectionsBySource<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>
            (string serviceSourceId, IOpenApiServiceRelationBySource<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime> request)
            where TModelRelation : IOpenApiAstiConnectionBySource<TModelContact, TModelServiceHours, TModelOpeningTime>
            where TModelContact : IVmOpenApiContactDetailsInVersionBase
            where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            request?.ChannelRelations?.ForEach(r => r.IsASTIConnection = true);
            return PutServiceConnectionsBySource(serviceSourceId, request, true);
        }

        /// <summary>
        /// Handels PUT service and channel connection request.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="request"></param>
        /// <param name="isASTI"></param>
        /// <returns></returns>
        protected IActionResult PutServiceConnections<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>(string serviceId, IOpenApiServiceRelation<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime> request, bool isASTI = false)
            where TModelRelation : IOpenApiConnectionForService<TModelContact, TModelServiceHours, TModelOpeningTime>
            where TModelContact : IVmOpenApiContactDetailsInVersionBase
            where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (string.IsNullOrEmpty(serviceId))
            {
                return NotFound(new VmError { ErrorMessage = "Service id has to be set." });
            }
            //// Measure
            //var watch = new Stopwatch();
            //Logger.LogTrace("****************************************");
            //Logger.LogTrace($"PutServiceConnections starts. Id: {serviceId}");
            //watch.Start();
            //// end measure
            request.ServiceId = serviceId.ParseToGuid();

            // check that service exists
            if (!request.ServiceId.HasValue)
            {
                return NotFound(new VmError { ErrorMessage = $"Service with id '{serviceId}' not found." });
            }

            var currentPublishingStatus = serviceService.GetLatestVersionPublishingStatus(request.ServiceId.Value);//serviceService.GetServiceByIdSimple(request.ServiceId.Value, false);
            if (!currentPublishingStatus.HasValue)
            {
                return NotFound(new VmError { ErrorMessage = $"Service with id '{serviceId}' not found." });
            }

            // Validate publishing status - status can only be Published or Draft (PTV-3933).
            if (currentPublishingStatus.Value != PublishingStatus.Published && currentPublishingStatus.Value != PublishingStatus.Draft)
            {
                return NotFound(new VmError { ErrorMessage = $"Service with id '{serviceId}' not found." });
            }

            //// Measure
            //watch.Stop();
            //Logger.LogTrace($"*** Service exists: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            request.ChannelRelations.ForEach(r =>
            {
                r.ServiceGuid = request.ServiceId.Value;
                r.ChannelGuid = r.ServiceChannelId.ParseToGuidWithExeption();
                if (isASTI && r is IOpenApiAstiConnectionForService<TModelContact, TModelServiceHours, TModelOpeningTime>)
                {
                    SetAstiData(r as IOpenApiAstiConnectionForService<TModelContact, TModelServiceHours, TModelOpeningTime>);
                }
            });

            // Do we have dublicates (PTV-3812)?
            var duplicates = request.ChannelRelations.GroupBy(i => i.ChannelGuid).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicates.Count > 0)
            {
                ModelState.AddModelError("ChannelRelations", string.Format(CoreMessages.OpenApi.DublicateItemsNotAllowed, string.Join(", ", duplicates)));
                return new BadRequestObjectResult(ModelState);
            }

            // Asti users have Eeva rights - so channel visibility is not checked!
            ServiceConnectionListValidator<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime> channelListValidator;
            if (isASTI || UserRole() == UserRoleEnum.Eeva)
            {
                channelListValidator = new ServiceConnectionListValidator<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>(request.ChannelRelations, channelService, codeService, false);
            }
            else
            {
                channelListValidator = new ServiceConnectionListValidator<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>(request.ChannelRelations, channelService, codeService, true, userOrganizationService.GetAllUserOrganizationIds());
            }
            channelListValidator.Validate(ModelState);

            // Check service id access rights
            var serviceValidator = new ServiceIdListValidator(new List<Guid>{request.ServiceId.Value}, serviceService, "serviceId",true);
            serviceValidator.Validate(ModelState);

            //// Measure
            //watch.Stop();
            //Logger.LogTrace($"*** Check channels exists: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var srv = serviceAndChannelService.SaveServiceConnections(request.ConvertToLatestVersion(), versionNumber, isASTI);
            if (srv == null)
            {
                return NotFound(new VmError { ErrorMessage = $"Service with id '{serviceId}' not found." });
            }

            //// Measure
            //watch.Stop();
            //Logger.LogTrace($"*** Save end: {watch.ElapsedMilliseconds} ms.");
            //Logger.LogTrace("****************************************");
            //// end measure

            return Ok(srv);
        }

        /// <summary>
        /// Update channel and service relationships. This is for ASTI connections.
        /// </summary>
        /// <param name="serviceChannelId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActionResult PutChannelConnections<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>
            (string serviceChannelId, IOpenApiAstiChannelRelation<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime> request)
            where TModelRelation : IOpenApiAstiConnectionForChannel<TModelContact, TModelServiceHours, TModelOpeningTime>
            where TModelContact : IVmOpenApiContactDetailsInVersionBase
            where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            //// Measure
            //var watch = new Stopwatch();
            //Logger.LogTrace("****************************************");
            //Logger.LogTrace($"PutChannelServices starts. Id: {serviceChannelId}");
            //watch.Start();
            //// end measure
            if (!string.IsNullOrEmpty(serviceChannelId))
            {
                request.ChannelId = serviceChannelId.ParseToGuid();

                // check that channel exists
                if (!request.ChannelId.HasValue)
                {
                    return NotFound(new VmError { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
                }
            }
            else
            {
                return NotFound(new VmError { ErrorMessage = "Service channel id has to be set." });
            }

            //// Measure
            //watch.Stop();
            //Logger.LogTrace($"*** Channel exists: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure
            if (request.ServiceRelations?.Count > 0)
            {
                request.ServiceRelations.ForEach(r =>
                {
                    r.ChannelGuid = request.ChannelId.Value;
                    r.ServiceGuid = r.ServiceId.ParseToGuidWithExeption();
                    r.ExtraTypes.ForEach(e => { e.ServiceGuid = r.ServiceGuid; e.ChannelGuid = r.ChannelGuid; });
                    r.IsASTIConnection = true;
                });

                // Do we have dublicates (PTV-3812)?
                var duplicates = request.ServiceRelations.GroupBy(i => i.ServiceGuid).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (duplicates.Count > 0)
                {
                    ModelState.AddModelError("ServiceRelations", string.Format(CoreMessages.OpenApi.DublicateItemsNotAllowed, string.Join(", ", duplicates)));
                    return new BadRequestObjectResult(ModelState);
                }

                // Validate channel and services connections.
                // Validate channel for channel type (only service location channel can include additional connection information - PTV-2475)
                // Validate service ids.
                var channelValidator = new ServiceChannelConnectionListValidator<TModelRelation, TModelContact, TModelServiceHours, TModelOpeningTime>(request.ServiceRelations, channelService, serviceService, codeService);
                channelValidator.Validate(ModelState);

                // Validate the items
                if (!ModelState.IsValid)
                {
                    ModelStateEntry value;
                    ModelState.TryGetValue("ServiceChannelId", out value);
                    if (value != null && value.Errors.FirstOrDefault()?.ErrorMessage == CoreMessages.OpenApi.RecordNotFound)
                    {
                        return NotFound(new VmError { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
                    }
                    return new BadRequestObjectResult(ModelState);
                }
            }
            else if (request.DeleteAllServiceRelations)
            {
                if (!channelService.ChannelExists(request.ChannelId.Value))
                {
                    return NotFound(new VmError { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
                }
            }

            //// Measure
            //watch.Stop();
            //Logger.LogTrace($"*** Validate services: {watch.ElapsedMilliseconds} ms.");
            //watch.Restart();
            //// end measure

            var srv = serviceAndChannelService.SaveServiceChannelConnections(request.ConvertToLatestVersion(), versionNumber);
            if (srv == null)
            {
                return NotFound(new VmError { ErrorMessage = $"Service channel with id '{serviceChannelId}' not found." });
            }

            //// Measure
            //watch.Stop();
            //Logger.LogTrace($"*** Save connections: {watch.ElapsedMilliseconds} ms.");
            //Logger.LogTrace($"*** End ***");
            //// end measure

            return Ok(srv);
        }

        private void SetAstiData<TModelContact, TModelServiceHours, TModelOpeningTime>(IOpenApiAstiConnectionForService<TModelContact, TModelServiceHours, TModelOpeningTime> connection)
            where TModelContact : IVmOpenApiContactDetailsInVersionBase
            where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
            where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            connection.IsASTIConnection = true;
            connection.ExtraTypes.ForEach(e => { e.ServiceGuid = connection.ServiceGuid; e.ChannelGuid = connection.ChannelGuid; });
        }
    }
}
