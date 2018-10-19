﻿/**
 * The MIT License
 * Copyright(c) 2016 Population Register Centre(VRK)
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
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of service service channel IN.
    /// </summary>
    public class V7VmOpenApiServiceServiceChannelAstiInBase : IOpenApiAstiConnectionForService<VmOpenApiContactDetailsInBase, V4VmOpenApiServiceHour, V2VmOpenApiDailyOpeningTime>
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonIgnore]
        [ValidGuid]
        [JsonProperty(Order = 1)]
        public virtual string ServiceId { get; set; }

        /// <summary>
        /// PTV service channel identifier.
        /// </summary>
        [Required]
        [ValidGuid]
        [JsonProperty(Order = 2)]
        public virtual string ServiceChannelId { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public string ServiceChargeType { get; set; }

        /// <summary>
        /// List of localized service channel relationship descriptions.
        /// </summary>
        [JsonProperty(Order = 4)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues("Type", allowedValues: new[] { "Description", "ChargeTypeAdditionalInfo" })]
        [ListPropertyMaxLength(500, "Value", "Description")]
        [ListPropertyMaxLength(500, "Value", "ChargeTypeAdditionalInfo")]
        public IList<VmOpenApiLocalizedListItem> Description { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<VmOpenApiExtraType> ExtraTypes { get; set; } = new List<VmOpenApiExtraType>();

        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ListWithEnum(typeof(ServiceHoursTypeEnum), "ServiceHourType")]
        public virtual IList<V4VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V4VmOpenApiServiceHour>();
        
        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual VmOpenApiContactDetailsInBase ContactDetails { get; set; }

        /// <summary>
        /// Indicates if value for property ServiceChargeType should be deleted.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual bool DeleteServiceChargeType { get; set; }

        /// <summary>
        /// Indicates if all descriptions should be deleted.
        /// </summary>
        [JsonProperty(Order = 11)]
        public virtual bool DeleteAllDescriptions { get; set; }
        
        /// <summary>
        /// Indicates if all extra types should be deleted.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual bool DeleteAllExtraTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all service hours should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all service hours should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 13)]
        public virtual bool DeleteAllServiceHours { get; set; }

        /// <summary>
        /// Gets or sets the service unique identifier.
        /// </summary>
        /// <value>
        /// The service unique identifier.
        /// </value>
        [JsonIgnore]
        public Guid ServiceGuid { get; set; }

        /// <summary>
        /// Gets or sets the channel unique identifier.
        /// </summary>
        /// <value>
        /// The channel unique identifier.
        /// </value>
        [JsonIgnore]
        public Guid ChannelGuid { get; set; }

        /// <summary>
        /// Indicates if connection between service and service channel is ASTI related.
        /// </summary>
        /// <value>
        /// The channel unique identifier.
        /// </value>
        [JsonIgnore]
        public virtual bool IsASTIConnection { get; set; }

        #region methods

        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <returns>in base version model</returns>
        public V9VmOpenApiServiceServiceChannelAstiInBase GetLatestInVersionModel()
        {
            return ConvertToLatestVersion();
        }

        /// <summary>
        /// Converts model into latest version.
        /// </summary>
        public V9VmOpenApiServiceServiceChannelAstiInBase ConvertToLatestVersion()
        {
            var vm = new V9VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = this.ServiceId,
                ServiceChannelId = this.ServiceChannelId,
                ServiceChargeType = this.ServiceChargeType,
                Description = this.Description,
                ExtraTypes = this.ExtraTypes,
                ContactDetails = this.ContactDetails?.ConvertToInBaseModel(),
                DeleteServiceChargeType = this.DeleteServiceChargeType,
                DeleteAllDescriptions = this.DeleteAllDescriptions,
                DeleteAllExtraTypes = this.DeleteAllExtraTypes,
                DeleteAllServiceHours = this.DeleteAllServiceHours,
                ServiceGuid = this.ServiceGuid,
                ChannelGuid = this.ChannelGuid,
                IsASTIConnection = this.IsASTIConnection
            };
            vm.DeleteAllExtraTypes = this.DeleteAllExtraTypes;
            this.ServiceHours?.ForEach(h => vm.ServiceHours.Add(h.ConvertToLatestVersion()));
            return vm;
        }
        #endregion
    }
}
