﻿/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using System.Linq;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of printable form channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiPrintableFormChannelVersionBase" />
    public class V9VmOpenApiPrintableFormChannel : VmOpenApiPrintableFormChannelVersionBase
    {
        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public virtual new IList<V8VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V8VmOpenApiServiceHour>();

        // PTV-4184, SFIPTV-362
        /// <summary>
        /// List of linked services including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public virtual new IList<V9VmOpenApiServiceChannelService> Services { get; set; } = new List<V9VmOpenApiServiceChannelService>();

        /// <summary>
        /// List of ontology terms related to the service.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiFintoItem> OntologyTerms { get => base.OntologyTerms; set => base.OntologyTerms = value; }

        /// <summary>
        /// Sote organization that is responsible for the service channel. Notice! At the moment always empty - the property is a placeholder for later use.
        /// </summary>
        [JsonIgnore]
        public override string ResponsibleSoteOrganization { get => base.ResponsibleSoteOrganization; set => base.ResponsibleSoteOrganization = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 9;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V8VmOpenApiPrintableFormChannel>();
            this.WebPages.ForEach(w => vm.WebPages.Add(w.ConvertToPreviousVersion()));// PTV-3705
            this.Services.ForEach(s => vm.Services.Add(s.ConvertToVersion8()));// PTV-4184
            vm.ServiceHours = this.ServiceHours;// SFIPTV-1912
            return vm;
        }

        #endregion
    }
}
