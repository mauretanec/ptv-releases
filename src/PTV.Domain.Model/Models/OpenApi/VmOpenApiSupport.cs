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
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiSupport : IVmOpenApiSupport
    {
        /// <summary>
        /// Email address.
        /// </summary>
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        [MaxLength(100)]
        [Phone]
        public string Phone { get; set; }

        /// <summary>
        /// Phone charge description.
        /// </summary>
        [MaxLength(150)]
        [RequiredIf("ServiceChargeTypes", "Other")]
        public string PhoneChargeDescription { get; set; }

        /// <summary>
        /// Language code for this object. Valid values are: fi, sv or en.
        /// </summary>
        [ValidEnum(typeof(LanguageCode))]
        public string Language { get; set; }

        /// <summary>
        /// List of service charge types for this object. Valid values are: Charged, Free and Other.
        /// </summary>
        [ListWithEnum(typeof(ServiceChargeTypeEnum))]
        public IReadOnlyList<string> ServiceChargeTypes { get; set; } = new List<string>();

        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
