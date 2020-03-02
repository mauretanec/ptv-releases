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

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PTV.DataImport.Console.Tasks
{
    public class CreatePostalCodesJsonTask
    {
        private static readonly string PostalCodesStartDataFile = Path.Combine("ImportSources", "PCF_20170407.dat");

        private static readonly string PostalCodesGeneratedFile = Path.Combine("Generated", "PostalCode.json");

        public CreatePostalCodesJsonTask(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            ILogger logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreatePostalCodesJsonTask>();

            logger.LogDebug("CreatePostalCodesJsonTask .ctor");
        }

        /// <summary>
        /// Generates a list of postal codes in JSON format to applications file generation folder.
        /// </summary>
        public void Generate()
        {
            var postalCodes = new List<object>();

            // Read postal codes from file (line by line)
            var lines = File.ReadLines(CreatePostalCodesJsonTask.PostalCodesStartDataFile, System.Text.Encoding.GetEncoding("ISO-8859-1"));

            foreach (var line in lines)
            {
                postalCodes.Add(new
                {
                    //Id = Guid.NewGuid(),
                    Code = line.Substring(13, 5).Trim(),
                    Type = line.Substring(110, 1).Trim(),
                    MunicipalityCode = line.Substring(176, 3).Trim(),
                    Names = new[] {
                        new { Language = "fi", Name = line.Substring(18, 30).Trim() },
                        new { Language = "sv", Name = line.Substring(48, 30).Trim() }
                    }
                });
            }

            postalCodes.Add(new {Code = "Undefined", Type="Undefined", MunicipalityCode = "Undefined",
                Names = new[] {
                        new { Language = "fi", Name = "Undefined" },
                        new { Language = "sv", Name = "Undefined" }
                    }
            });
            // Convert data to json
            var json = JsonConvert.SerializeObject(postalCodes, Formatting.Indented);

            // Overwrite the file always
            File.WriteAllText(CreatePostalCodesJsonTask.PostalCodesGeneratedFile, json, System.Text.Encoding.UTF8);
        }

    }
}
