/**
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
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.V2.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class VmConnectionsServiceSearch : IVmMultiLocalized, IVmSearchParamsBase
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Fulltext { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Languages { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ServiceTypeId { get; set; }
        /// <summary>
        /// List if sorting params.
        /// </summary>
        public List<VmSortParam> SortData { get; set; }
          /// <summary>
        /// Id of entity
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxPageCount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// Gets or sets the selected publishing statuses.
        /// </summary>
        /// <value>
        /// The selected publishing statuses.
        /// </value>
        public List<Guid> SelectedPublishingStatuses { get; set; }
        /// <summary>
        /// Gets or sets the selected service classes.
        /// </summary>
        public List<Guid> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the selected ontology terms.
        /// </summary>
        public List<Guid> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the selected area information types.
        /// </summary>
        public List<Guid> AreaInformationTypes { get; set; }
        /// <summary>
        /// Gets or sets the selected target groups.
        /// </summary>
        public List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the selected life events.
        /// </summary>
        public List<Guid> LifeEvents { get; set; }
        /// <summary>
        /// Gets or sets the selected industrial classes.
        /// </summary>
        public List<Guid> IndustrialClasses { get; set; }
        /// <summary>
        /// Gets or sets the ui language.
        /// </summary>
        public string Language { get; set; }
    }
}
