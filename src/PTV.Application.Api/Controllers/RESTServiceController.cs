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
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework.ServiceManager;
using PTV.Framework.Interfaces;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Services"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/service")]
    [Controller]
    public class RESTServiceController : RESTBaseController
    {
        private readonly IServiceService service;
        private readonly IServiceManager serviceManager;
        private readonly IFintoService fintoService;

        private const string MessageAddService = "Service.AddService.MessageSaved";
        private const string MessagePublishService = "Service.EditService.MessagePublished";
        private const string MessageSaveServiceStep = "Service.EditService.MessageStepSaved";
        private const string MessageDeleteService = "Service.EditService.MessageDeleted";
        private const string MessageLockedService = "Service.Exception.MessageLock";

        private const string MessageAddServiceRole = "Service.RoleException.MessageAdd";
        private const string MessageSaveServiceRole = "Service.RoleException.MessageSave";
        private const string MessagePublishServiceRole = "Service.RoleException.MessagePublish";
        private const string MessageDeleteServiceRole = "Service.RoleException.MessageDelete";
        private const string MessageLockServiceRole = "Service.RoleException.MessageLock";

        /// <summary>
        /// Constructor of service controller
        /// </summary>
        /// <param name="service">service service responsible for operation related to service - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="fintoService">finto service responsible for operation related to finto stuff - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTServiceController(
            IServiceService service,
            IServiceManager serviceManager,
            IFintoService fintoService,
            ILogger<RESTServiceController> logger) : base(logger)
        {
            this.service = service;
            this.serviceManager = serviceManager;
            this.fintoService = fintoService;
        }

        /// <summary>
        /// Get data for service search form
        /// </summary>
        /// <returns>data needed for search form like publishing statuses, types etc.</returns>
        [Route("GetServiceSearch")]
        [HttpGet]
        public IServiceResultWrap GetServiceSearch()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = service.GetServiceSearch() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get names for service id
        /// </summary>
        /// <returns>names for service id</returns>
        [Route("GetServiceNames")]
        [HttpPost]
        public IServiceResultWrap GetServiceNames([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = service.GetServiceNames(model) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Search services for given criteria
        /// </summary>
        /// <param name="model">model containing search criteria</param>
        /// <returns>found services</returns>
        [Route("SearchServices")]
        [HttpPost]
        public IServiceResultWrap SearchServices([FromBody] VmServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = service.SearchServices(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Search service for given id
        /// </summary>
        /// <param name="model">model containing id of the service</param>
        /// <returns>found service</returns>
        [Route("SearchService")]
        [HttpPost]
        public IServiceResultWrap SearchService([FromBody] VmServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = service.SearchRelationService(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Searches services for given channel id
        /// </summary>
        /// <param name="model">containing id of the channel</param>
        /// <returns>found services</returns>
        [Route("SearchChannelServices")]
        [HttpPost]
        public IServiceResultWrap SearchChannelServices([FromBody] VmServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap { Data = service.SearchRelationServices(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for service step1
        /// </summary>
        /// <param name="model">model containing id of service or empty</param>
        /// <returns>all data needed for service step 1</returns>
        [Route("GetAddServiceStep1")]
        [HttpPost]
        public IServiceResultWrap GetAddServiceStep1([FromBody] VmGetServiceStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.GetServiceStep1(model)
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for service step 2
        /// </summary>
        /// <param name="model">model containing id of service or empty</param>
        /// <returns>all data needed for service step 2</returns>
        [Route("GetAddServiceStep2")]
        [HttpPost]
        public IServiceResultWrap GetAddServiceStep2([FromBody] VmGetServiceStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.GetServiceStep2(model),
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for service step 3
        /// </summary>
        /// <param name="model">model containing id of service or empty</param>
        /// <returns>all data needed for service step 3</returns>
        [Route("GetAddServiceStep3")]
        [HttpPost]
        public IServiceResultWrap GetAddServiceStep3([FromBody] VmGetServiceStep model)
        {
            return serviceManager.CallService(
                 () => new ServiceLocalizedResultWrap (model)
                 {
                     Data = service.GetServiceStep3(model)
                 },
                 new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the finto terms for given term
        /// </summary>
        /// <param name="model">model containing id of the term</param>
        /// <returns>the tree of the given term</returns>
        [Route("GetFintoTree")]
        [HttpPost]
        public IServiceResultWrap GetFintoTree([FromBody] VmNode model)
        {
            return serviceManager.CallService(
                 () => new ServiceResultWrap() { Data = fintoService.GetFintoTree(model) },
                 new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the finto terms for given term and search value
        /// </summary>
        /// <param name="model">model containing id of the term and search criteria</param>
        /// <returns>the tree of the given term</returns>
        [Route("GetFilteredTree")]
        [HttpPost]
        public IServiceResultWrap GetFilteredTree([FromBody] VmGetFilteredTree model)
        {
            return serviceManager.CallService(
                 () => new ServiceResultWrap() { Data = fintoService.GetFilteredTree(model) },
                 new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets flatten list of finto stuff for given search value
        /// </summary>
        /// <param name="model">model containing search criteria</param>
        /// <returns>the list of finto stuff</returns>
        [Route("GetFilteredList")]
        [HttpPost]
        public IServiceResultWrap GetFilteredList([FromBody] VmGetFilteredTree model)
        {
            return serviceManager.CallService(
                 () => new ServiceResultWrap() { Data = fintoService.Search(model) },
                 new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the data for search channel in service step 4 (obsolete 08.02.2017)
        /// </summary>
        /// <param name="model">model containing id of service or empty</param>
        /// <returns>data for form or data stored for service id</returns>
        [Obsolete]
        [Route("GetServiceStep4Channeldata")]
        [HttpPost]
        public IServiceResultWrap GetServiceStep4Channeldata([FromBody] VmGetServiceStep model)
        {
            return serviceManager.CallService(
                 () => new ServiceLocalizedResultWrap(model)
                 {
                     Data = service.GetServiceStep4Channeldata(model),
                 },
                 new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves service to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of service otherwise error message</returns>
        [Route("SaveAllChanges")]
        [HttpPost]
        public IServiceResultWrap SaveAllChanges([FromBody] VmService model)
        {
             return serviceManager.CallService(
                 ()=> new ServiceLocalizedResultWrap(model)
                 {
                     Data = service.AddService(model),
                 },
                 new Dictionary<Type, string>() {
                     { typeof(string), MessageAddService },
                     { typeof(RoleActionException), MessageAddServiceRole }
                 });
        }

        /// <summary>
        /// Publishs the service
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("PublishService")]
        [HttpPost]
        public IServiceResultWrap PublishService([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = service.PublishService(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessagePublishService },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessagePublishServiceRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }

        /// <summary>
        /// Deletes the service
        /// </summary>
        /// <param name="model">model containing the service id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("DeleteService")]
        [HttpPost]
        public IServiceResultWrap DeleteService([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = model.Id != null ? service.DeleteService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageDeleteService },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageDeleteServiceRole }
                });
        }

        /// <summary>
        /// Locks the service for given id
        /// </summary>
        /// <param name="model">id of service to be locked</param>
        /// <returns>locked id otherwise erro message</returns>
        [Route("LockService")]
        [HttpPost]
        public IServiceResultWrap LockService([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? service.LockService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageLockServiceRole },
                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked }
                });
        }

        /// <summary>
        /// Unlocks the service for given id
        /// </summary>
        /// <param name="model">id of service to be unlocked</param>
        /// <returns>unlocked id otherwise erro message</returns>
        [Route("UnLockService")]
        [HttpPost]
        public IServiceResultWrap UnLockService([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? service.UnLockService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Chekcs whether service is locked or not
        /// </summary>
        /// <param name="model">model containing the service id</param>
        /// <returns>true otherwise false</returns>
        [Route("IsServiceLocked")]
        [HttpPost]
        public IServiceResultWrap IsServiceLocked([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? service.IsServiceLocked(model.Id.Value) : null,
                },
                new Dictionary<Type, string>() { { typeof(LockException), MessageLockedService } } );
        }

        /// <summary>
        /// Saves service step 1 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of service otherwise error message</returns>
        [Route("SaveStep1Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep1Changes([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model.Step1Form)
                {
                    Data = service.SaveStep1Changes(model.Id.Value, model.Step1Form),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageSaveServiceStep },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageSaveServiceRole }
                });
        }

        /// <summary>
        /// Saves service step 2 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of service otherwise error message</returns>
        [Route("SaveStep2Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep2Changes([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model.Step2Form)
                {
                    Data = service.SaveStep2Changes(model.Id.Value, model.Step2Form),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageSaveServiceStep },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageSaveServiceRole }
                });
        }

        /// <summary>
        /// Saves service step 3 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of service otherwise error message</returns>
        [Route("SaveStep3Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep3Changes([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model.Step3Form)
                {
                    Data = service.SaveStep3Changes(model.Id.Value, model.Step3Form),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageSaveServiceStep },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageSaveServiceRole }
                });
        }

        /// <summary>
        /// Saves service step 4 to database (obsolete 08.02.2017)
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of service otherwise error message</returns>
        [Obsolete]
        [Route("SaveStep4Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep4Changes([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.SaveStep4Changes(model.Id.Value, model.Step4Form),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageSaveServiceStep },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageSaveServiceRole }
                });
        }

        /// <summary>
        /// Gets the publishing status of the service
        /// </summary>
        /// <param name="model">model containing id of the service</param>
        /// <returns>publishing status</returns>
        [Route("GetServiceStatus")]
        [HttpPost]
        public IServiceResultWrap GetServiceStatus([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.GetServiceStatus(model.Id.Value),
                },
                new Dictionary<Type, string>());
        }
    }
}
