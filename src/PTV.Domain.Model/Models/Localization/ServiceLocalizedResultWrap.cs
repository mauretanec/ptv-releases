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

using System.Linq;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models.Localization
{
    /// <summary>
    /// Wrapper of the service call with language
    /// </summary>
    /// <seealso cref="PTV.Framework.Interfaces.IServiceResultWrap" />
    public class ServiceLocalizedResultWrap : ServiceResultWrap, IServiceLocalizedResultWrap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocalizedResultWrap"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public ServiceLocalizedResultWrap(IVmLocalized model = null)
        {
            if (model != null)
            {
                Language = model.Language.ToString();
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocalizedResultWrap"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public ServiceLocalizedResultWrap(IVmMultiLocalized model)
        {
            if (model != null)
            {
                Language = model.Languages.FirstOrDefault().ToString();
            }
        }

        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; private set; } = "fi";
    }
}
