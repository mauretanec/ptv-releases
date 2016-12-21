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
using PTV.Domain.Model.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    public class VmPrintableFormChannelStep1 : VmEnumEntityStatusBase, IVmPrintableFormChannelStep1
    {
        [JsonIgnore]
        public Guid? PrintableFormChannelChannelId { get; set; }
        public VmPhone PhoneNumber { get; set; }
        public VmEmailData Email { get; set; }
        public string FormIdentifier { get; set; }
        public string FormReceiver { get; set; }

        public VmAddressSimple DeliveryAddress { get; set; }
        public List<VmChannelAttachment> UrlAttachments { get; set; }
        public List<VmWebPage> WebPages { get; set; }

        public VmPrintableFormChannelStep1()
        {
            PhoneNumber = new VmPhone();
            Email = new VmEmailData();
            FormIdentifier = string.Empty;
            FormReceiver = string.Empty;
            UrlAttachments = new List<VmChannelAttachment>();
            WebPages = new List<VmWebPage>();
        }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public Guid? OrganizationId { get; set; }
        public LanguageCode Language { get; set; }
    }
}
