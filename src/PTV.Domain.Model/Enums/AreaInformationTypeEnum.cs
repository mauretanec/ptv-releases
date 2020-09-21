﻿/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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

using PTV.Framework.Attributes;

namespace PTV.Domain.Model.Enums
{
    /// <summary>
    /// Area information types. Enumeration items have different value for open api (PTV-2184).
    /// </summary>
    public enum AreaInformationTypeEnum
    {
        /// <summary>
        /// Available in whole country
        /// </summary>
        [EnumOpenApiValue("Nationwide")]
        WholeCountry,
        /// <summary>
        /// Available in whole country except Aland
        /// </summary>
        [EnumOpenApiValue("NationwideExceptAlandIslands")]
        WholeCountryExceptAlandIslands,
        /// <summary>
        /// Available in specific area types
        /// </summary>
        [EnumOpenApiValue("LimitedType")]
        AreaType
    }
}
