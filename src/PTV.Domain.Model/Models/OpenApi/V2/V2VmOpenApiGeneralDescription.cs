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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of general description
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiGeneralDescription" />
    public class V2VmOpenApiGeneralDescription : VmOpenApiGeneralDescriptionVersionBase, IV2VmOpenApiGeneralDescription
    {
        /// <summary>
        /// List of service classes.
        /// </summary>
        [JsonProperty(Order = 5)]
        public new IList<VmOpenApiFintoItem> ServiceClasses { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of ontology terms.
        /// </summary>
        [JsonProperty(Order = 6)]
        public new IList<VmOpenApiFintoItem> OntologyTerms { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of target groups.
        /// </summary>
        [JsonProperty(Order = 7)]
        public new IList<VmOpenApiFintoItem> TargetGroups { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of life events.
        /// </summary>
        [JsonProperty(Order = 8)]
        public new IList<VmOpenApiFintoItem> LifeEvents { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of industrial classes.
        /// </summary>
        [JsonProperty(Order = 9)]
        public virtual new IList<VmOpenApiFintoItem> IndustrialClasses { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// Laws that a general description is based on.
        /// </summary>
        [JsonProperty(Order = 13)]
        public IList<V2VmOpenApiLaw> Laws { get; set; }

        /// <summary>
        /// Laws that a general description is based on.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiLaw> Legislation
        {
            get
            {
                return base.Legislation;
            }

            set
            {
                base.Legislation = value;
            }
        }

        #region methods
        /// <summary>
        /// Open api version number.
        /// </summary>
        /// <returns>Open api version number</returns>
        public override int VersionNumber()
        {
            return 2;
        }

        /// <summary>
        /// Converts model to previous version.
        /// </summary>
        /// <returns></returns>
        public override IVmOpenApiGeneralDescriptionVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<VmOpenApiGeneralDescription>();
            vm.ServiceClasses = ServiceClasses;
            vm.OntologyTerms = OntologyTerms;
            vm.TargetGroups = TargetGroups;
            vm.LifeEvents = LifeEvents;
            return vm;
        }
        #endregion
    }
}
