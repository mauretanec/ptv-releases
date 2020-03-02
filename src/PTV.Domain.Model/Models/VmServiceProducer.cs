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
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using Newtonsoft.Json.Converters;

namespace PTV.Domain.Model.Models
{

    /// <summary>
    /// View model of service producer
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
    public class VmServiceProducer : VmEntityBase, IVmOwnerReference, IVmOrderable
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        [JsonIgnore]
        public int? OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the provision type identifier.
        /// </summary>
        /// <value>
        /// The provision type identifier.
        /// </value>
        public Guid ProvisionType { get; set; }

        /// <summary>
        /// Gets or sets the producer organization identifiers.
        /// </summary>
        /// <value>
        /// The producers organizations.
        /// </value>
        public List<Guid> SelfProducers { get; set; }

        /// <summary>
        /// Gets or sets the producer organization identifier.
        /// </summary>
        /// <value>
        /// The producer organization identifier.
        /// </value>
        public Guid? Organization { get; set; }

        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        public Dictionary<string, string> AdditionalInformation { get; set; }

        /// <summary>
        /// Gets or sets other producer type
        /// </summary>
        /// /// <value>
        /// Other producer type
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public OtherProducerTypeEnum OtherProducerType { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
