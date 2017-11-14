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

using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using PTV.Framework;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Enums;
using PTV.Framework.Attributes;
using System;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of organization for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V4.IV4VmOpenApiOrganizationInBase" />
    public class V4VmOpenApiOrganizationInBase : VmOpenApiOrganizationInVersionBase, IV4VmOpenApiOrganizationInBase
    {
        /// <summary>
        /// List of organization names.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListRequiredIfProperty("Type", "DisplayNameType", typeof(string))]
        public override IList<VmOpenApiLocalizedListItem> OrganizationNames { get => base.OrganizationNames; set => base.OrganizationNames = value; }

        /// <summary>
        /// Display name type (Name or AlternateName). Which name type should be used as the display name for the organization (OrganizationNames list).
        /// </summary>
        [JsonProperty(Order = 14)]
        public virtual new string DisplayNameType { get; set; }

        /// <summary>
        /// Localized list of organization descriptions.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [JsonProperty(Order = 15)]
        [ListPropertyMaxLength(2500, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public new IReadOnlyList<VmOpenApiLanguageItem> OrganizationDescriptions { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V4VmOpenApiAddressWithTypeIn> Addresses { get; set; } = new List<V4VmOpenApiAddressWithTypeIn>();

        /// <summary>
        /// Area type. 
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// Sub area type (Municipality, Province, BusinessRegions, HospitalRegions).
        /// </summary>
        [JsonIgnore]
        public override string SubAreaType { get => base.SubAreaType; set => base.SubAreaType = value; }
        
        /// <summary>
        /// Area codes related to sub area type. For example if SubAreaType = Municipality, Areas-list need to include municipality codes like 491 or 091.
        /// </summary>
        [JsonIgnore]
        public override IList<string> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// List of organizations electronic invoicing information.
        /// </summary>
        [JsonIgnore]
        override public IList<VmOpenApiOrganizationEInvoicing> ElectronicInvoicings { get => base.ElectronicInvoicings; set => base.ElectronicInvoicings = value; }

        /// <summary>
        /// Set to true to delete all existing electronic invoicing addresses (the ElectronicInvoicings collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        override public bool DeleteAllElectronicInvoicings { get => base.DeleteAllElectronicInvoicings; set => base.DeleteAllElectronicInvoicings = value; }

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus
        {
            get => base.PublishingStatus;
            set => base.PublishingStatus = value;
        }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 4;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiOrganizationInVersionBase VersionBase()
        {
            var vm = base.GetInVersionBaseModel<VmOpenApiOrganizationInVersionBase>();

            if (!string.IsNullOrEmpty(this.DisplayNameType))
            {
                this.GetAvailableLanguages(VersionNumber()).ForEach(lang =>
                {
                    vm.DisplayNameType.Add(new VmOpenApiNameTypeByLanguage() { Language = lang, Type = this.DisplayNameType });
                });
            }

            Addresses.ForEach(a => vm.Addresses.Add(a.LatestVersion<V7VmOpenApiAddressWithForeignIn>()));

            vm.OrganizationDescriptions = new List<VmOpenApiLocalizedListItem>();
            OrganizationDescriptions.ForEach(o =>
                vm.OrganizationDescriptions.Add(
                    new VmOpenApiLocalizedListItem()
                    {
                        Language = o.Language,
                        Value = o.Value,
                        OwnerReferenceId = o.OwnerReferenceId,
                        Type = DescriptionTypeEnum.Description.ToString()
                    })
                );

            return vm;
        }
        #endregion
    }
}
