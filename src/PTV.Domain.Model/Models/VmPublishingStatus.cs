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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of publishing status
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmListItem" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmPublishingStatus" />
    public class VmPublishingStatus : VmEnumType, IVmPublishingStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmPublishingStatus"/> class.
        /// </summary>
        public VmPublishingStatus()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmPublishingStatus"/> class.
        /// </summary>
        /// <param name="item">The publishing status item to fill in.</param>
        public VmPublishingStatus(VmEnumType item)
        {
            Id = item.Id;
            Name = item.Name;
            Code = item.Code;
            OrderNumber = item.OrderNumber;
            Translation = item.Translation;
            OwnerReferenceId = item.OwnerReferenceId;
            Access = EVmRestrictionFilterType.Allowed;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public PublishingStatus Type { get; set; }
    }
}
