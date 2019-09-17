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
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces
{
    public class SahaMappingDefinition : SahaMappingDefinitionBase
    {
        public Guid Id => (PtvOrganizationId.ToString()+SahaId).GetGuid();
        public Guid EntityId { get; set; }
        public Dictionary<string, string> OrganizationName { get; set; }
        public string SahaOrganizationName { get; set; }
        public List<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        public Guid OrganizationTypeId { get; set; }
        public int ServiceCount { get; set; }
        public int ChannelCount { get; set; }
    }
    
    public class SahaMappingDefinitionBase
    {
        public Guid SahaId { get; set; }
        public Guid PtvOrganizationId { get; set; }
    }
}