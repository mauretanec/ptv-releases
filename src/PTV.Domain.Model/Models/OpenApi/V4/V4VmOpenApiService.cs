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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V4.IV4VmOpenApiService" />
    public class V4VmOpenApiService : VmOpenApiServiceVersionBase, IV4VmOpenApiService
    {
        /// <summary>
        /// Service funding type. Possible values are: PubliclyFunded or MarketFunded.
        /// </summary>
        [JsonIgnore]
        public override string FundingType { get => base.FundingType; set => base.FundingType = value; }

        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public new IList<V4VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V4VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        [JsonProperty(Order = 30)]
        public new IList<V4VmOpenApiServiceOrganization> Organizations { get; set; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of service areas.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiArea> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// Date when item was modified/created..
        /// </summary>
        [JsonIgnore]
        public override DateTime Modified { get => base.Modified; set => base.Modified = value; }

        /// <summary>
        /// List of service vouchers.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 4;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V3VmOpenApiService>();
            vm.ServiceClasses = GetV3FintoItemList(ServiceClasses.ToList());
            vm.OntologyTerms = GetV3FintoItemList(OntologyTerms.ToList());
            vm.TargetGroups = GetV3FintoItemList(TargetGroups.ToList());
            vm.LifeEvents = GetV3FintoItemList(LifeEvents.ToList());
            vm.IndustrialClasses = GetV3FintoItemList(IndustrialClasses.ToList());
            Organizations.ForEach(o => vm.Organizations.Add(o.ConvertToVersion3()));
            ServiceChannels.ForEach(c => vm.ServiceChannels.Add(c.ConvertToVersion3()));
            return vm;
        }
        #endregion
    }
}
