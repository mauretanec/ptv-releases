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
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.Localization;

namespace PTV.Domain.Model.Models.Localization
{
    /// <summary>
    /// View model of localized messages
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.Localization.IVmLocalizationMessages" />
    public class VmLocalizationMessages : IVmLocalizationMessages
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmLocalizationMessages"/> class.
        /// </summary>
        public VmLocalizationMessages()
        {
            Translations = new List<IVmLanguageMessages>();
        }

        /// <summary>
        /// Gets or sets the translations.
        /// </summary>
        /// <value>
        /// The translations.
        /// </value>
        public List<IVmLanguageMessages> Translations { get; set; }
    }
}
