/**
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
using Newtonsoft.Json;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PTV.DataImport.WinExcelToJson.Model;

namespace PTV.DataImport.WinExcelToJson.Tasks
{
    internal static class IndustrialClassesTask
    {
        private const string IndustrialClassesFile = @"SourceFiles\industrialClasses.xlsx";
//        MunicipalitiesGeneratedFile = Path.Combine(AppSettings.OutputDir,
//        private const string OutputFolder = Path.Combine(AppSettings.OutputDir, \Resources\Finto";
        private const string DataSheetName = "'Industrial Classes$'";

        public static void GenerateJsonFile()
        {
            ExcelReader excelRdr = new ExcelReader(new ExcelReaderSettings()
            {
                File = IndustrialClassesFile,
                SkipRows = 0,
                FirstRowContainsColumnNames = false
            });

            // just a check that the sheet exists
            Console.WriteLine(string.Join(", ",excelRdr.GetSheetNames()));
            var sheetNameCheck = excelRdr.GetSheetNames().Find(name => string.Compare(name, DataSheetName, StringComparison.OrdinalIgnoreCase) == 0);

            if (string.IsNullOrWhiteSpace(sheetNameCheck))
            {
                throw new Exception($"The workbook doesn't contain a sheet with name: {DataSheetName}.");
            }
            int index = 1;

            var data = excelRdr.ReadSheet(DataSheetName, reader =>
            {
                object dataItem = null;

                // check that we have a reader
                // we are checking the isdbnull(1) here to avoid the few empty rows that gets read from Excel sheet end
                if (reader != null && !reader.IsDBNull(1))
                {
                    try
                    {
                        Console.WriteLine($"Row {index++} {index + 2}:");
                        string code = GetString(reader, 0);
                        string level = GetString(reader, 1);
                        int levelInt = 6;

                        if (!int.TryParse(level, out levelInt))
                        {
                            levelInt = 6;
                            Console.Error.WriteLine($"Wrong format of level {level} for {code}.");
                        }

                        int codeInt;
                        if (levelInt > 1 && int.TryParse(code, out codeInt))
                        {
                            code = string.Format(codeInt.ToString($"D{levelInt}"));
                        }

                        dataItem = new
                        {
                            Code = code,
                            Uri = "http://www.stat.fi/meta/luokitukset/toimiala/001-2008/",
                            Level = levelInt,
                            Names = new List<LanguageLabel>
                            {
                                new LanguageLabel { Lang = "en", Label = GetString(reader, 2)},
                                new LanguageLabel { Lang = "sv", Label = GetString(reader, 3)},
                                new LanguageLabel { Lang = "fi", Label = GetString(reader, 4)},
                            }
                        };
                    }
                    catch (InvalidCastException e)
                    {
                        Console.WriteLine($"Row {index + 2} -> {e.Message}");
                    }
                    finally
                    {
                        Console.WriteLine();
                    }
                }

                return dataItem;
            });

            // convert data to json
            var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            // overwrite the file always
            var outputFile = new FileInfo(Path.Combine(AppSettings.OutputResourcesDir, "Finto", "IndustrialClasses.json"));
//            if (!outputFile.Directory.Exists)
//            {
//                outputFile.Directory.Create();
//            }
            File.WriteAllText(outputFile.FullName, json);
            Console.WriteLine($"File generated to {outputFile.FullName}.");
        }

        private static string GetString(DbDataReader reader, int index)
        {
            string value = (reader.GetValue(index) as string)?.Trim();
            Console.Write($"{index}; ");
            return value;
        }


    }
}
