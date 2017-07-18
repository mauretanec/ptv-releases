﻿using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Tests.Attributes
{
    public class ListWithUrlAttributeTests
    {
        [Fact]
        public void ListPropertyIsNull()
        {
            TestObject obj = new TestObject();
            obj.Urls = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Urls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Urls, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void CustomListPropertyIsNull()
        {
            TestObject obj = new TestObject();
            obj.CustomUrls = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "CustomUrls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.CustomUrls, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListIsEmpty()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Urls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Urls, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void CustomListIsEmpty()
        {
            TestObject obj = new TestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "CustomUrls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.CustomUrls, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void StringListCanHaveNullItem()
        {
            TestObject obj = new TestObject();
            obj.Urls.Add("http://www.google.com");
            obj.Urls.Add(null);
            obj.Urls.Add("http://www.vaestorekisterikeskus.fi");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Urls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Urls, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void StringListEmptyStringIsNotValid()
        {
            TestObject obj = new TestObject();
            obj.Urls.Add("http://www.google.com");
            obj.Urls.Add("");
            obj.Urls.Add("http://www.vaestorekisterikeskus.fi");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Urls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Urls, ctx, validationResults).Should().BeFalse();
        }

        [Theory]
        [InlineData("https://www.google.com/doodles/about")]
        [InlineData("https://esuomi.fi/palveluntarjoajille/tunnistaminen/")]
        [InlineData("http://www.example.com:80")]
        [InlineData("http://www.example.com:8080")]
        [InlineData("http://www.example.com:8080/index.htm")]
        [InlineData("ftp://www.example.com")]
        public void TestDifferentAssumedValidUrls(string urlToTest)
        {
            TestObject obj = new TestObject();
            obj.Urls.Add(urlToTest);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "Urls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.Urls, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void CustomListCanHaveNullInPropertyValue()
        {
            TestObject obj = new TestObject();

            obj.CustomUrls.Add(new CustomUrlObject()
            {
                Name = "Google",
                Url = "http://www.google.com"
            });

            obj.CustomUrls.Add(new CustomUrlObject());

            obj.CustomUrls.Add(new CustomUrlObject()
            {
                Name = "VRK",
                Url = "http://www.vaestorekisterikeskus.fi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "CustomUrls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.CustomUrls, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void CustomListEmptyUrlIsNotValid()
        {
            TestObject obj = new TestObject();

            obj.CustomUrls.Add(new CustomUrlObject()
            {
                Name = "Google",
                Url = "http://www.google.com"
            });

            obj.CustomUrls.Add(new CustomUrlObject()
            {
                Name = "Empty",
                Url = string.Empty
            });

            obj.CustomUrls.Add(new CustomUrlObject()
            {
                Name = "VRK",
                Url = "http://www.vaestorekisterikeskus.fi"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "CustomUrls";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.CustomUrls, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void UsedOnInvalidPropertyTypeShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.UsedOnStringType = "ptv";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "UsedOnStringType";

            Action act = () => Validator.ValidateProperty(obj.UsedOnStringType, ctx);

            act.ShouldThrowExactly<InvalidOperationException>().WithMessage(
                "The validation attribute is used on a property that doesn't implement IList (property name: 'UsedOnStringType').",
                "Because attribute is used on a type that doesn't implement IList.");
        }

        [Fact]
        public void ListObjectDoesntContainNamedPropertyShouldThrow()
        {
            TestObject obj = new TestObject();
            obj.PropertyNameNotFound.Add(new CustomUrlObject()
            {
                Name = "Google",
                Url = "http://www.google.com"
            });

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "PropertyNameNotFound";

            Action act = () => Validator.ValidateProperty(obj.PropertyNameNotFound, ctx);
            //Comment: it would be more helpful if themessage actually told that the requested property was not found from the list object
            act.ShouldThrowExactly<ArgumentException>().WithMessage("Item doesn't contain property named: 'NotFoundPropertyName'.*Parameter name: propertyName");
        }

        internal class TestObject
        {
            public TestObject()
            {
                Urls = new List<string>();
                CustomUrls = new List<CustomUrlObject>();

                PropertyNameNotFound = new List<CustomUrlObject>();
            }

            [ListWithUrl]
            public List<string> Urls { get; set; }

            [ListWithUrl("Url")]
            public List<CustomUrlObject> CustomUrls { get; set; }

            [ListWithUrl]
            public string UsedOnStringType { get; set; }

            [ListWithUrl("NotFoundPropertyName")]
            public List<CustomUrlObject> PropertyNameNotFound { get; set; }
        }

        internal class CustomUrlObject
        {
            public string Url { get; set; }

            public string Name { get; set; }
        }
    }
}
