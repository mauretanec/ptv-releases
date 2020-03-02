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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IOrganizationTreeDataCache : ILiveDataCache<Guid, OrganizationTreeItem>
    {
        Dictionary<Guid, VmListItemWithStatus> GetFlatDataVm();
        Guid? FindBySahaId(Guid sahaId);
        Dictionary<Guid, Guid> SahaMappings();
        List<Guid> GetAllSubOrganizationIds(Guid mainOrganizationId);
        /// <summary>
        /// Searches for the top level parent organization in the organization tree, which is
        /// related to provided unific root Id. If found, returns the unific root Id of the
        /// top level parent organization and corresponding SAHA Id, if mapping exists.
        /// </summary>
        /// <param name="unificRootId">Unific root Id of an organization of any level.</param>
        /// <returns></returns>
        (Guid? OrganizationId, Guid? SahaId) GetMainOrganizationIds(Guid unificRootId);
    }
}