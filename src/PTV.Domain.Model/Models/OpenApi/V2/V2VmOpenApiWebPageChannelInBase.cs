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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    public class V2VmOpenApiWebPageChannelInBase : VmOpenApiServiceChannelIn, IV2VmOpenApiWebPageChannelInBase
    {
        /// <summary>
        /// List of localized urls.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListWithUrl("Value")]
        public virtual IReadOnlyList<VmOpenApiLanguageItem> Urls { get; set; }

        [JsonIgnore]
        public override IList<VmOpenApiWebPage> WebPages
        {
            get
            {
                return base.WebPages;
            }

            set
            {
                base.WebPages = value;
            }
        }

        [JsonIgnore]
        public override IReadOnlyList<V2VmOpenApiServiceHour> ServiceHours
        {
            get
            {
                return base.ServiceHours;
            }

            set
            {
                base.ServiceHours = value;
            }
        }

        [JsonIgnore]
        public override bool DeleteAllServiceHours
        {
            get
            {
                return base.DeleteAllServiceHours;
            }

            set
            {
                base.DeleteAllServiceHours = value;
            }
        }
    }
}
