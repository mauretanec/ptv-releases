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
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using System;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using Moq;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceValidatorTests : ValidatorTestBase
    {
        private const string FI = "fi";
        private const string TEXT = "text";

        private List<string> _availableLanguages;

        private int _defaultVersion = 9;

        public ServiceValidatorTests()
        {
            _availableLanguages = new List<string> { FI };
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ServiceValidator(null, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, _availableLanguages, null, UserRoleEnum.Eeva, _defaultVersion);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange & act
            var validator = new ServiceValidator(new VmOpenApiServiceInVersionBase { PublishingStatus = PublishingStatus.Published.ToString() },
                generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GeneralDescriptionSet_NotValid(string gdId)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId
            };
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void GeneralDescriptionSet_NotExists()
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString()
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns((VmOpenApiGeneralDescriptionVersionBase)null);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(DescriptionTypeEnum.Description)]
        [InlineData(DescriptionTypeEnum.ShortDescription)]
        [InlineData(DescriptionTypeEnum.ServiceUserInstruction)]
        [InlineData(DescriptionTypeEnum.ValidityTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ProcessingTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.DeadLineAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ChargeTypeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ServiceTypeAdditionalInfo)]
        public void GeneralDescriptionSet_DescriptionNotSetWithinGD_NotValid(DescriptionTypeEnum type)
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = type.ToString()}
                }
            };
            var vmGd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns(vmGd);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, _availableLanguages, _availableLanguages, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void GeneralDescriptionSet_DescriptionNotSetWithinGD_Valid()
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ShortDescription.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.Description.ToString()}
                }
            };
            var vmGd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                PublishingStatus = PublishingStatus.Published.ToString(),                 
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns(vmGd);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, _availableLanguages, _availableLanguages, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(DescriptionTypeEnum.BackgroundDescription)]
        [InlineData(DescriptionTypeEnum.ServiceUserInstruction)]
        [InlineData(DescriptionTypeEnum.ValidityTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ProcessingTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.DeadLineAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ChargeTypeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ServiceTypeAdditionalInfo)]
        public void GeneralDescriptionSet_DescriptionSetWithinGD_NotValid(DescriptionTypeEnum type)
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = type.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ShortDescription.ToString()}
                }
            };
            var vmGd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                PublishingStatus = PublishingStatus.Published.ToString(),
                Descriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.BackgroundDescription.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ShortDescription.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ServiceUserInstruction.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString()},
                }
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns(vmGd);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, _availableLanguages, _availableLanguages, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }


        [Theory]
        [InlineData(DescriptionTypeEnum.Description)]
        [InlineData(DescriptionTypeEnum.ServiceUserInstruction)]
        [InlineData(DescriptionTypeEnum.ValidityTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ProcessingTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.DeadLineAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ChargeTypeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ServiceTypeAdditionalInfo)]
        public void GeneralDescriptionSet_DescriptionSetWithinGD_Valid(DescriptionTypeEnum type)
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = type.ToString()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ShortDescription.ToString()}
                }
            };
            var vmGd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                PublishingStatus = PublishingStatus.Published.ToString(),
                Descriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.Description.ToString()},
                }
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns(vmGd);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, null, _availableLanguages, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIsValid()
        {
            // Arrange
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString()
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase { Id = Guid.NewGuid() });
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void TargetGroups_LifeEventsAttached_NotValid(bool targetGroupsSet, bool currentTargetGroupsSet)
        {
            // Arrange
            var targetGroupUri = "uri";
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupUri } : null,
                LifeEvents = new List<string> { "SomeLifeEvent" }
            };
            var vmTargetGroup = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "NotKR1" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(vmTargetGroup);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion,
                currentTargetGroupsSet ? new VmOpenApiServiceVersionBase { TargetGroups = new List<V4VmOpenApiFintoItem> { vmTargetGroup } } : null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            if (targetGroupsSet)
            {
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupUri), Times.Once);
            }
            controller.ModelState.ContainsKey("LifeEvents").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("LifeEvents", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("Target group 'Citizens (KR1)' or one of the sub target groups");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void TargetGroupNotKR2_IndustrialClassesAttached_NotValid(bool targetGroupsSet, bool currentTargetGroupsSet)
        {
            // Arrange
            var targetGroupUri = "uri";
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupUri } : null,
                IndustrialClasses = new List<string> { "SomeIndustrialClass" }
            };
            var vmTargetGroup = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "NotKR2" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(vmTargetGroup);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService,  ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion,
                currentTargetGroupsSet ? new VmOpenApiServiceVersionBase { TargetGroups = new List<V4VmOpenApiFintoItem> { vmTargetGroup } } : null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            if (targetGroupsSet)
            {
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupUri), Times.Once);
            }
            controller.ModelState.ContainsKey("IndustrialClasses").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("IndustrialClasses", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("Target group 'Businesses and non-government organizations (KR2)' or one of the sub target groups");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void TargetGroups_LifeEvents_IndustrialClasses_Valid(bool targetGroupsSet, bool currentTargetGroupsSet)
        {
            // Arrange
            var targetGroupK1Uri = "KR1";
            var targetGroupK2Uri = "KR2";
            var subTargetGroupK2Uri = "KR2.1";
            var lifeEventList = new List<string> { "SomeLifeEvent" };
            var industrialClassList = new List<string> { "SomeIndustrialClass" };

            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupK1Uri, targetGroupK2Uri, subTargetGroupK2Uri } : null,
                LifeEvents = lifeEventList,
                IndustrialClasses = industrialClassList,
                MainResponsibleOrganization = Guid.NewGuid().ToString()
            };
            var vmTargetGroupKR1 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR1", Uri = targetGroupK1Uri};
            var vmTargetGroupKR2 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR2", Uri = targetGroupK2Uri};
            var vmSubTargetGroupKR2 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR2.1", ParentId = vmTargetGroupKR2.Id, ParentUri = targetGroupK2Uri};
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupK1Uri)).Returns(vmTargetGroupKR1);
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupK2Uri)).Returns(vmTargetGroupKR2);
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(subTargetGroupK2Uri)).Returns(vmSubTargetGroupKR2);
            fintoServiceMockSetup.Setup(x => x.CheckLifeEvents(lifeEventList)).Returns((List<string>)null);
            fintoServiceMockSetup.Setup(x => x.CheckIndustrialClasses(industrialClassList)).Returns((List<string>)null);
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(It.IsAny<Guid>())).Returns(new List<string>());

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion,
                currentTargetGroupsSet ? new VmOpenApiServiceVersionBase { TargetGroups = new List<V4VmOpenApiFintoItem> { vmTargetGroupKR1, vmTargetGroupKR2, vmSubTargetGroupKR2 } } : null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            if (targetGroupsSet)
            {
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupK1Uri), Times.Once);
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupK2Uri), Times.Once);
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(subTargetGroupK2Uri), Times.Once);
            }

            fintoServiceMockSetup.Verify(x => x.CheckLifeEvents(lifeEventList), Times.Once);
            fintoServiceMockSetup.Verify(x => x.CheckIndustrialClasses(industrialClassList), Times.Once);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void TargetGroupsSet_AttachedTargetGroupsNotKR1OrKR2_NotValid(bool lifeEventSet, bool industrialClassSet)
        {
            // Arrange
            var targetGroupUri = "uri";
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = new List<string> { targetGroupUri },
                LifeEvents = lifeEventSet ? new List<string> { "SomeLifeEvent" } : null,
                IndustrialClasses = industrialClassSet ? new List<string> { "SomeIndustrialClass" } : null
            };
            var vmTargetGroupKR1 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR1" };
            var vmTargetGroupKR2 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR2" };

            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "Code" });

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion,
                new VmOpenApiServiceVersionBase { TargetGroups = new List<V4VmOpenApiFintoItem> { vmTargetGroupKR1, vmTargetGroupKR2 } });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            if (lifeEventSet)
            {
                controller.ModelState.ContainsKey("LifeEvents").Should().BeTrue();
                ModelStateEntry value;
                controller.ModelState.TryGetValue("LifeEvents", out value);
                value.Should().NotBeNull();
                var error = value.Errors.First();
                error.ErrorMessage.Should().StartWith("Target group 'Citizens (KR1)' or one of the sub target groups");
            }
            if (industrialClassSet)
            {
                controller.ModelState.ContainsKey("IndustrialClasses").Should().BeTrue();
                ModelStateEntry value;
                controller.ModelState.TryGetValue("IndustrialClasses", out value);
                value.Should().NotBeNull();
                var error = value.Errors.First();
                error.ErrorMessage.Should().StartWith("Target group 'Businesses and non-government organizations (KR2)' or one of the sub target groups");
            }
        }

        [Fact]
        public void MainOrganizationExistsButNotOwnOrganization_Eeva()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                MainResponsibleOrganization = id.ToString()
            };

            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(id)).Returns(_availableLanguages);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void MainOrganizationExistsButNotOwnOrganization_Pete()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                MainResponsibleOrganization = id.ToString()
            };

            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(id)).Throws(new Exception());
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void MainOrganizationNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                MainResponsibleOrganization = id.ToString()
            };

            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(id)).Returns((List<string>)null);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Eeva, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("MainResponsibleOrganization").Should().BeTrue();
        }

        [Fact]
        public void MoreThan10OntologyTerms_V9_NotValid()
        {
            // Arrange
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) =>
                {
                    return list;
                });

            var vm = new VmOpenApiServiceInVersionBase
            {
                OntologyTerms = new List<string> { "term1", "term2", "term3", "term4", "term5", "term6", "term7", "term8", "term9", "term10", "term11" }
            };
            ontologyTermDataCacheMock.Setup(x => x.HasUris(It.IsAny<IEnumerable<string>>()))
                .Returns((IList<string> ots) => ots);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("OntologyTerms").Should().BeTrue();
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void MoreThan10OntologyTerms_OlderVersions_Valid(int version)
        {
            // Arrange
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) =>
                {
                    return list;
                });

            var vm = new VmOpenApiServiceInVersionBase
            {
                OntologyTerms = new List<string> { "term1", "term2", "term3", "term4", "term5", "term6", "term7", "term8", "term9", "term10", "term11" }
            };
            ontologyTermDataCacheMock.Setup(x => x.HasUris(It.IsAny<IEnumerable<string>>()))
                .Returns((IList<string> ots) => ots);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void MoreThan4ServiceClasses_V9_NotValid()
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = new List<string> { "term1", "term2", "term3", "term4", "term5" }
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceClasses").Should().BeTrue();
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void MoreThan4ServiceClasses_OlderVersions_Valid(int version)
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = new List<string> { "term1", "term2", "term3", "term4", "term5" }
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void AllMainServiceClasses_V9_NotValid()
        {
            // Arrange
            var list = new List<string> { "term1", "term2", "term3", "term4", "term5" };
            fintoServiceMockSetup.Setup(x => x.CheckServiceClasses(It.IsAny<List<string>>()))
                .Returns((null, list));
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = list
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, 9, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceClasses").Should().BeTrue();
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void AllMainServiceClasses_OlderVersions_Valid(int version)
        {
            // Arrange
            var list = new List<string> { "term1", "term2", "term3", "term4", "term5" };
            fintoServiceMockSetup.Setup(x => x.CheckServiceClasses(It.IsAny<List<string>>()))
                .Returns((null, list));
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = list
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OrganizationLanguageAvailability_Publish_V9_Valid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var languageList = new List<string> { "fi" };
            var vm = new VmOpenApiServiceInVersionBase
            {
                MainResponsibleOrganization = id.ToString(),
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(languageList);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, new VmOpenApiServiceVersionBase { AvailableLanguages = languageList });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetValidDates))]
        public void TimedPublishing_Valid(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new ServiceValidator(
                new VmOpenApiServiceInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInValidDates))]
        public void TimedPublishing_NotValid(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new ServiceValidator(
                new VmOpenApiServiceInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ValidTo").Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInValidPublishingStatus))]
        public void TimedPublishing_NotValid_PublishingStatus(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new ServiceValidator(
                new VmOpenApiServiceInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("PublishingStatus").Should().BeTrue();
        }

        [Fact]
        public void NameAndSummary_DescriptionsNotSet_Valid()
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = "Name", Type = NameTypeEnum.Name.ToString(), Language = "fi" } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NameAndSummary_Same_NotValid()
        {
            // Arrange
            var text = "Text";
            var lang = "fi";
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = text, Type = NameTypeEnum.Name.ToString(), Language = lang } },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = text, Type = DescriptionTypeEnum.ShortDescription.ToString(), Language = lang } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void NameAndSummary_Same_OlderVersions_Valid(int version)
        {
            // Arrange
            var text = "Text";
            var lang = "fi";
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = text, Type = NameTypeEnum.Name.ToString(), Language = lang } },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = text, Type = DescriptionTypeEnum.ShortDescription.ToString(), Language = lang } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, null, null, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        public static IEnumerable<object[]> GetValidDates()
        {
            yield return new object[] { DateTime.Now, DateTime.Now.AddDays(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMonths(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddYears(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, null, PublishingStatus.Draft };
            yield return new object[] { null, DateTime.Now, PublishingStatus.Published };
        }

        public static IEnumerable<object[]> GetInValidDates()
        {
            yield return new object[] { DateTime.Now.AddDays(1), DateTime.Now, PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddSeconds(10), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMinutes(10), PublishingStatus.Draft };
        }

        public static IEnumerable<object[]> GetInValidPublishingStatus()
        {
            yield return new object[] { DateTime.Now, DateTime.Now.AddDays(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMonths(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, DateTime.Now.AddYears(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, null, PublishingStatus.Published };
            yield return new object[] { null, DateTime.Now, PublishingStatus.Draft };
        }
    }
}
