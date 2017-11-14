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

using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationsSahaTests : OrganizationServiceTestBase
    {
        private List<OrganizationVersioned> organizationList;

        public GetOrganizationsSahaTests()
        {
        }

        [Fact]
        public void NoDateDefined()
        {
            // Arrange
            var pageSize = 1;
            organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedAndDeletedList = organizationList.Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) ||
                e.PublishingStatusId == DeletedId || e.PublishingStatusId == OldPublishedId).ToList();
            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());

            // unitOfWork
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns(publishedAndDeletedList.AsQueryable());

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            translationManagerMockSetup.Setup(t => t.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationSaha>(It.IsAny<IList<OrganizationVersioned>>()))
                .Returns(new List<VmOpenApiOrganizationSaha> { new VmOpenApiOrganizationSaha() });

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache);
            
            // Act
            var result = service.GetOrganizationsSaha(null, 1, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(publishedAndDeletedList.Count);
        }

        [Fact]
        public void OnlyTwoSubOrganizationLevelsReturned()
        {
            // Arrange
            // set an organization list where exists three levels of suborganizations. So we need at least 4 items in a list.
            organizationList = EntityGenerator.GetOrganizationEntityList(2, PublishingStatusCache); // 2 * 5 items
            var publishedAndDeletedList = organizationList.Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) ||
                e.PublishingStatusId == DeletedId || e.PublishingStatusId == OldPublishedId).ToList(); // includes 6 items = 2 * 3
            var mainOrganization = publishedAndDeletedList.FirstOrDefault();
            var nextItem = 1;
            var parentId = mainOrganization.Id;
            publishedAndDeletedList.ForEach(o =>
            {
                if (o.Id != mainOrganization.Id)
                {
                    o.ParentId = parentId;
                    switch (nextItem)
                    {
                        case 1:
                            // First level organization
                            o.Parent = new Model.Models.Organization { Versions = new List<OrganizationVersioned>
                            {
                                new OrganizationVersioned { PublishingStatusId = PublishedId }
                            } };
                            nextItem = 2;
                            break;
                        case 2:
                            // Second level organization
                            o.Parent = new Model.Models.Organization
                            { Versions = new List<OrganizationVersioned>
                            {
                                new OrganizationVersioned
                                {
                                    PublishingStatusId = PublishedId,
                                    ParentId = Guid.NewGuid(),
                                    Parent = new Model.Models.Organization
                                    {
                                        Versions = new List<OrganizationVersioned>
                                        {
                                            new OrganizationVersioned { PublishingStatusId = PublishedId }
                                        }
                                    }
                                }
                            } };
                            nextItem = 3;
                            break;
                        case 3:
                            // Third level organization
                            o.Parent = new Model.Models.Organization
                            {
                                Versions = new List<OrganizationVersioned>
                                {
                                    new OrganizationVersioned
                                    {
                                        PublishingStatusId = PublishedId,
                                        ParentId = Guid.NewGuid(),
                                        Parent = new Model.Models.Organization
                                        {
                                            Versions = new List<OrganizationVersioned>
                                            {
                                                new OrganizationVersioned
                                                {
                                                    PublishingStatusId = PublishedId,
                                                    ParentId = Guid.NewGuid(),
                                                    Parent = new Model.Models.Organization { Versions = new List<OrganizationVersioned>
                                                    {
                                                        new OrganizationVersioned { PublishingStatusId = PublishedId }
                                                    } }
                                                }
                                            },
                                        }
                                    }
                                }
                            };
                            nextItem = 3;
                            break;
                        default:
                            break;
                    }
                    parentId = o.Id;
                }
            });
            var validLevelsList = publishedAndDeletedList.Where(o => o.ParentId == null || // main level
                (o.Parent != null && o.Parent.Versions.Any(p => (p.PublishingStatusId == PublishedId || p.PublishingStatusId == DeletedId) && (p.ParentId == null ||// first level child
                p.Parent != null && p.Parent.Versions.Any(pp => (pp.PublishingStatusId == PublishedId || pp.PublishingStatusId == DeletedId) && pp.ParentId == null)))));

            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());

            // unitOfWork
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns(validLevelsList.AsQueryable());

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            translationManagerMockSetup.Setup(t => t.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationSaha>(It.IsAny<IList<OrganizationVersioned>>()))
                .Returns(new List<VmOpenApiOrganizationSaha> { new VmOpenApiOrganizationSaha() });

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache);

            // Act
            var result = service.GetOrganizationsSaha(null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(validLevelsList.Count());
            Assert.IsType<VmOpenApiOrganizationSahaGuidPage>(result);
        }
    }
}
