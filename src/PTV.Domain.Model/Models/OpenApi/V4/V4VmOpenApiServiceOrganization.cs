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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of service organization
    /// </summary>
    public class V4VmOpenApiServiceOrganization : VmOpenApiServiceOrganizationVersionBase, IV4VmOpenApiOrganizationService
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonIgnore]
        public override string ServiceId { get; set; }

        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        [ListPropertyMaxLength(150, "Value")]
        public override IList<VmOpenApiLanguageItem> AdditionalInformation
        {
            get {
                return base.AdditionalInformation;
            }
            set {
                base.AdditionalInformation = value;
            }
        }


        /// <summary>
        /// Converts to version3.
        /// </summary>
        /// <returns>model converted to version 3</returns>
        public V3VmOpenApiServiceOrganization ConvertToVersion3()
        {
            var organization = ConvertToVersion<V3VmOpenApiServiceOrganization>();

            foreach (var webPage in this.WebPages)
            {
                organization.WebPages.Add(webPage.ConvertToVersion3());
            }

            return organization;
        }
    }
}
