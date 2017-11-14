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

using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationVersionBase" />
    public class VmOpenApiOrganizationVersionBase : VmOpenApiOrganizationBase, IVmOpenApiOrganizationVersionBase
    {
        /// <summary>
        /// Organizations parent organization identifier if exists.
        /// </summary>
        [JsonProperty(Order = 3)]
        public Guid? ParentOrganization { get; set; }

        /// <summary>
        /// Municipality including municipality code and a localized list of municipality names.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual VmOpenApiMunicipality Municipality { get; set; }

        /// <summary>
        /// List of organization areas.
        /// </summary>
        [JsonProperty(Order = 18)]
        public virtual IList<VmOpenApiArea> Areas { get; set; }

        /// <summary>
        /// List of organization area municipalities
        /// </summary>
        [JsonProperty(Order = 19)]
        [JsonIgnore]
        public virtual IList<VmOpenApiMunicipality> AreaMunicipalities { get; set; }

        /// <summary>
        /// List of organizations email addresses.
        /// </summary>
        [JsonProperty(Order = 21)]
        public IReadOnlyList<V4VmOpenApiEmail> EmailAddresses { get; set; } = new List<V4VmOpenApiEmail>();

        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public IList<V7VmOpenApiAddress> Addresses { get; set; } = new List<V7VmOpenApiAddress>();

        /// <summary>
        /// List of organizations services.
        /// </summary>
        [JsonProperty(Order = 31)]
        public IList<V5VmOpenApiOrganizationService> Services { get; set; }

        /// <summary>
        /// List of organizations services where organization is main responsible.
        /// </summary>
        [JsonIgnore]
        public IList<VmOpenApiItem> ResponsibleOrganizationServices { get; set; }

        /// <summary>
        /// List of organizations services where organization is a producer.
        /// </summary>
        [JsonIgnore]
        public IList<V5VmOpenApiOrganizationService> ProducerOrganizationServices { get; set; }

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        [JsonProperty(Order = 40)]
        public virtual DateTime Modified { get; set; }

        /// <summary>
        /// The sub organizations
        /// </summary>
        [JsonProperty(Order = 41)]
        public virtual IList<VmOpenApiItem> SubOrganizations { get; set; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        public virtual int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>
        /// model of previous version
        /// </returns>
        public virtual IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V7VmOpenApiOrganization>();
            // AreaType needs to be null for Out api if organization type is Municipality. In db the area (information) type is required and the default value is 'WholeCountry'.
            vm.AreaType = this.OrganizationType == OrganizationTypeEnum.Municipality.ToString() ? null : this.AreaType;
            return vm;
        }

        /// <summary>
        /// Gets the base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiOrganizationVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.DisplayNameType = this.DisplayNameType;
            vm.Municipality = this.Municipality;
            vm.Areas = this.Areas;
            vm.AreaMunicipalities = this.AreaMunicipalities;
            vm.OrganizationDescriptions = this.OrganizationDescriptions;
            vm.EmailAddresses = this.EmailAddresses;
            vm.PhoneNumbers = this.PhoneNumbers;
            vm.Addresses = this.Addresses;
            vm.ParentOrganization = this.ParentOrganization;
            vm.Services = this.Services;
            vm.Modified = this.Modified;
            vm.SubOrganizations = this.SubOrganizations;
            return vm;
        }
        #endregion
    }
}
