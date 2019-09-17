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
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xunit;
using System.Reflection;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V7
{
    public class V7VmOpenApiOrganizationInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData(AreaInformationTypeEnum.WholeCountry, null)]
        [InlineData(AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Municipality)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.BusinessRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.HospitalRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Province)]
        public void ValidModel(AreaInformationTypeEnum areaInformationType, AreaTypeEnum? areaType)
        {
            // Arrange
            var alternateName = "AlternateName";
            const string fi = "fi";
            var organization = new V7VmOpenApiOrganizationInBase
            {
                OrganizationNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
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
                    } },
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage> { new VmOpenApiNameTypeByLanguage
                {
                    Type = alternateName,
                    Language = fi
                } },
                OrganizationDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ShortDescription.ToString(), Language = fi, Value = "Description"}
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
                AreaType = areaInformationType.ToString(),
                SubAreaType = areaType.HasValue ? areaType.Value.ToString() : null,
                Areas = new List<string> { "area1"},
                PhoneNumbers = new List<V4VmOpenApiPhone>
                {
                    new V4VmOpenApiPhone
                    {
                        ServiceChargeType = ServiceChargeTypeEnum.Free.ToString(),
                        Language = fi
                    },
                    new V4VmOpenApiPhone
                    {
                        ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                        Language = fi
                    }
                },
                EmailAddresses = new List<V4VmOpenApiEmail>()
            };

            // Act
            var resultList = ValidateModel(organization);

            // Assert
            resultList.Count.Should().Be(0);
            // Check that Emails is not visible
            Attribute.IsDefined(typeof(V7VmOpenApiOrganizationInBase).GetProperty(nameof(V7VmOpenApiOrganizationInBase.Emails)), typeof(JsonIgnoreAttribute)).Should().BeTrue();
        }

        [Theory]
        [InlineData("Nationwide", null)]
        [InlineData("NationwideExceptAlandIslands", null)]
        [InlineData("LimitedType", "BusinessSubRegion")]
        [InlineData("LimitedType", "HospitalDistrict")]
        [InlineData("LimitedType", "Region")]
        public void InValidModel(string areaInformationType, string areaType)
        {
            // Arrange
            var alternateName = "AlternativeName";
            var fi = "fi";
            var organization = new V7VmOpenApiOrganizationInBase
            {
                OrganizationNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
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
                    } },
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage> { new VmOpenApiNameTypeByLanguage
                {
                    Type = alternateName,
                    Language = fi
                } },
                OrganizationDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = "Summary", Language = fi, Value = "Description"}
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
                AreaType = areaInformationType,
                SubAreaType = areaType,
                Areas = new List<string> { "area1" },
                PhoneNumbers = new List<V4VmOpenApiPhone>
                {
                    new V4VmOpenApiPhone
                    {
                        ServiceChargeType = "FreeOfCharge",
                        Language = fi
                    },
                    new V4VmOpenApiPhone
                    {
                        ServiceChargeType = "Chargeable",
                        Language = fi
                    }
                },
                EmailAddresses = new List<V4VmOpenApiEmail>(),
            };

            // Act
            var errorList = ValidateModel(organization);

            // Assert
            errorList.Count.Should().BeGreaterThan(0);
            errorList.FirstOrDefault(r => r.MemberNames.Contains("OrganizationNames")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("DisplayNameType")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("OrganizationDescriptions")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("AreaType")).Should().NotBeNull();
            if (!string.IsNullOrEmpty(areaType))
            {
                errorList.FirstOrDefault(r => r.MemberNames.Contains("SubAreaType")).Should().NotBeNull();
            }
            errorList.FirstOrDefault(r => r.MemberNames.Contains("PhoneNumbers")).Should().NotBeNull();
        }

        [Fact]
        public void AreasRequiredIfAreaTypeLimitedType()
        {
            // Arrange
            var organization = new V7VmOpenApiOrganizationInBase
            {
                AreaType = AreaInformationTypeEnum.AreaType.ToString()
            };

            // Act
            var resultList = ValidateModel(organization);

            // Assert
            resultList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
        }
        
        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V7VmOpenApiOrganizationInBase
            {
                PhoneNumbers = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
                EmailAddresses = new List<V4VmOpenApiEmail> { new V4VmOpenApiEmail() },
                Addresses = new List<V7VmOpenApiAddressWithForeignIn> { new V7VmOpenApiAddressWithForeignIn() },
                WebPages = new List<VmOpenApiWebPageWithOrderNumber> { new VmOpenApiWebPageWithOrderNumber() }
            };
            CheckProperties(model, 7);
            CheckProperties(model.PhoneNumbers.First(), 4);
            CheckProperties(model.EmailAddresses.First(), 4);
            CheckProperties(model.Addresses.First(), 7);
            CheckProperties(model.WebPages.First(), 8);
        }
    }
}
