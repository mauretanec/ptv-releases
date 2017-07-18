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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V5;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V6;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V6.IV6VmOpenApiOrganization" />
    public class V6VmOpenApiOrganization : VmOpenApiOrganizationVersionBase, IV6VmOpenApiOrganization
    {
        
        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 6;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V5VmOpenApiOrganization>();

            // Version 6+ allow Markdown line breaks, older versions do not (PTV-1978)
            // Actually line breaks are allowed also in older versions so replace into space is removed! PTV-2346
            //vm.OrganizationDescriptions.ForEach(description => description.Value = description.Value?.Replace(LineBreak, " "));

            return vm;
        }
        #endregion
    }
}
