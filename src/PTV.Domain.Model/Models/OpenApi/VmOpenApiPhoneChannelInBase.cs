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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiPhoneChannelInBase : V2VmOpenApiPhoneChannelInBase, IVmOpenApiPhoneChannelInBase
    {
        /// <summary>
        /// Phone number type. Valid values are: Phone, Sms or Fax.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ValidEnum(typeof(PhoneNumberTypeEnum))]
        public virtual string PhoneType { get; set; }

        /// <summary>
        /// List of service charge types. Valid values are: Charged, Free and Other.
        /// </summary>
        [JsonProperty(Order = 11)]
        [ListWithEnum(typeof(ServiceChargeTypeEnum))]
        public IReadOnlyList<string> ServiceChargeTypes { get; set; } = new List<string>();

        /// <summary>
        /// List of localized phone numbers.
        /// </summary>
        [JsonProperty(Order = 12)]
        [MaxLength(100)]
        [ListWithPhone("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual new IReadOnlyList<VmOpenApiLanguageItem> PhoneNumbers { get; set; }

        /// <summary>
        /// Phone charge description.
        /// </summary>
        [JsonProperty(Order = 14)]
        public virtual string PhoneChargeDescription { get; set; }

        /// <summary>
        /// Support contact email.
        /// </summary>
        [JsonProperty(Order = 16)]
        [EmailAddress]
        [MaxLength(100)]
        public string SupportContactEmail { get; set; }

        [JsonProperty(Order = 25)]
        public new IReadOnlyList<VmOpenApiServiceHour> ServiceHours { get; set; } = new List<VmOpenApiServiceHour>();

        /// <summary>
        /// Delete all existing service charge types. This property is not used in the API anymore. Do not use.
        /// </summary>
        [JsonProperty(Order = 34)]
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        public virtual bool DeleteAllServiceChargeTypes { get; set; }

        /// <summary>
        /// Set to true to delete all existing support contacts for the service channel. The SupportContacts collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 45)]
        public virtual bool DeleteAllSupportContacts { get; set; }

        [JsonIgnore]
        public override IList<VmOpenApiPhone> SupportPhones
        {
            get
            {
                return base.SupportPhones;
            }

            set
            {
                base.SupportPhones = value;
            }
        }

        [JsonIgnore]
        public override IList<VmOpenApiLanguageItem> SupportEmails
        {
            get
            {
                return base.SupportEmails;
            }

            set
            {
                base.SupportEmails = value;
            }
        }

        [JsonIgnore]
        public override bool DeleteAllSupportEmails
        {
            get
            {
                return base.DeleteAllSupportEmails;
            }

            set
            {
                base.DeleteAllSupportEmails = value;
            }
        }

        [JsonIgnore]
        public override bool DeleteAllSupportPhones
        {
            get
            {
                return base.DeleteAllSupportPhones;
            }

            set
            {
                base.DeleteAllSupportPhones = value;
            }
        }
    }
}
