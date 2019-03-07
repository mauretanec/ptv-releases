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

using PTV.Framework;
using Microsoft.AspNetCore.Http;
using PTV.Database.Model.Models;
using PTV.Database.Model.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// ServiceChannel role checker
    /// </summary>
    [RegisterService(typeof(RootChannelRoleChecker), RegisterType.Transient)]
    internal class RootChannelRoleChecker : RoleCheckerBase
    {
        private IVersioningManager versioningManager;
        private IRoleCheckerManager roleChecker;

        public RootChannelRoleChecker(
            IRoleCheckerManager roleChecker,
            IVersioningManager versioningManager,
            IUserOrganizationService userOrganizationService,
            IUserInfoService userInfoService,
            ICacheManager cacheManager
        ) : base(userOrganizationService, userInfoService, cacheManager)
        {
            this.versioningManager = versioningManager;
            this.roleChecker = roleChecker;
        }


        protected override DomainEnum Domain => DomainEnum.Channels;

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var serviceChannel = entity as ServiceChannel;

            if (serviceChannel != null)
            {
                var serviceChannelVersioned = versioningManager.GetNotModifiedVersion(serviceChannel.Versions);

                if (serviceChannelVersioned != null)
                {
                    roleChecker.CheckEntity(GetEntity<ServiceChannelVersioned>(serviceChannelVersioned.Id, unitOfWork), null, unitOfWork);
                }
            }
        }
    }
}
