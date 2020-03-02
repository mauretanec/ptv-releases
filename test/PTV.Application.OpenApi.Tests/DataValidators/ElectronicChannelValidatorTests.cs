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
using PTV.Domain.Model.Models.OpenApi;
using System;
using PTV.Domain.Model.Enums;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Application.OpenApi.Tests.DataValidators
{    
    public class ElectronicChannelValidatorTests : ValidatorTestBase
    {
        private int version = 10;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ElectronicChannelValidator(null, organizationService, codeService, serviceService, commonService, null, null, null, version);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, null, null, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RequiredWebPageNotSet()
        {
            // Arrange
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, null, new List<string> { "fi" }, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void AllAvailableWebPagesNotSet()
        {
            // Arrange
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Value = "http://www.tieto.com", Language = "fi"} },
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, new List<string> { "se" }, null, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
