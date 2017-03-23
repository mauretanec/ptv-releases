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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of electronic channel
    /// </summary>
    public class VmElectronicChannel : VmEntityStatusBase, IVmElectronicChannel
    {
        /// <summary>
        /// Model of step 1
        /// </summary>
        public VmElectronicChannelStep1 Step1Form { get; set; }
        /// <summary>
        /// list of channel url attachments
        /// </summary>
        public List<VmChannelAttachment> UrlAttachments { get; set; }
        /// <summary>
        /// Model of step 2
        /// </summary>
        public VmOpeningHoursStep Step2Form { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public VmElectronicChannel()
        {
            Step1Form = new VmElectronicChannelStep1();
            Step2Form = new VmOpeningHoursStep();
            UrlAttachments = new List<VmChannelAttachment>();
        }

        /// <summary>
        /// Code of the language
        /// </summary>
        public LanguageCode Language { get; set; }
    }
}
