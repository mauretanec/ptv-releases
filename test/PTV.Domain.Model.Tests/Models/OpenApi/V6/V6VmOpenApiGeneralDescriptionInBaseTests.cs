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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V6;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V6
{
    public class V6VmOpenApiGeneralDescriptionInBaseTests : ModelsTestBase
    {

        [Theory]
        [InlineData(ServiceTypeEnum.Service, ServiceChargeTypeEnum.Free)]
        [InlineData(ServiceTypeEnum.PermissionAndObligation, ServiceChargeTypeEnum.Charged)]
        [InlineData(ServiceTypeEnum.ProfessionalQualifications, null)]
        public void ValidModel(ServiceTypeEnum type, ServiceChargeTypeEnum? serviceChargeType)
        {
            // Arrange
            const string fi = "fi";
            var service = new V6VmOpenApiGeneralDescriptionInBase
            {
                Type = type.ToString(),
                ServiceChargeType = serviceChargeType.HasValue ? serviceChargeType.Value.ToString() : null,
                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Type = NameTypeEnum.AlternateName.ToString(),
                        Language = fi,
                        Value = "Text for name"
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Type = NameTypeEnum.Name.ToString(),
                        Language = fi,
                        Value = "Text for name"
                    }
                },
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.Description.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ShortDescription.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ServiceUserInstruction.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.DeadLineAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            // Act
            var errorList = ValidateModel(service);

            // Assert
            errorList.Count.Should().Be(0);
        }

        [Theory]
        [InlineData("PermitOrObligation", "FreeOfCharge")]
        [InlineData("ProfessionalQualification", "Chargeable")]
        public void NotValidModel(string type, string serviceChargeType)
        {
            // Arrange
            var alternateName = "AlternativeName";
            const string fi = "fi";
            var gd = new V6VmOpenApiGeneralDescriptionInBase
            {
                Type = type,
                ServiceChargeType = serviceChargeType,
                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Type = alternateName,
                        Language = fi,
                        Value = "Text for name"
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Type = NameTypeEnum.Name.ToString(),
                        Language = fi,
                        Value = "Text for name"
                    }
                },
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = "Description", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "Summary", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "UserInstruction", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "ValidityTime", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "ProcessingTime", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "DeadLine", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "ChargeTypeAdditionalInfo", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "ServiceType", Language = fi, Value = "Description"},
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            // Act
            var errorList = ValidateModel(gd);

            // Assert
            errorList.Count.Should().BeGreaterThan(0);
            if (type != ServiceTypeEnum.Service.ToString()) { errorList.FirstOrDefault(r => r.MemberNames.Contains("Type")).Should().NotBeNull(); }
            if (serviceChargeType != null) { errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceChargeType")).Should().NotBeNull(); }
            errorList.FirstOrDefault(r => r.MemberNames.Contains("Names")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("Descriptions")).Should().NotBeNull();
        }

        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V6VmOpenApiGeneralDescriptionInBase
            {
                Legislation = new List<V4VmOpenApiLaw> { new V4VmOpenApiLaw() },
            };
            CheckProperties(model, 6);
            CheckProperties(model.Legislation.First(), 4);
        }
    }
}
