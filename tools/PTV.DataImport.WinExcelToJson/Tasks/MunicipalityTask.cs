﻿using Newtonsoft.Json;
using PTV.DataImport.WinExcelToJson.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTV.DataImport.WinExcelToJson.Tasks
{

    internal static class MunicipalityTask
    {
        private const string MunicipalitiesStartDataFile = "SourceFiles\\Municipalities-20-12-2016.xlsx";
        private static readonly string MunicipalitiesGeneratedFile = Path.Combine(AppSettings.OutputDir, "Municipality.json");
        private const string DataSheetName = "'Muncipalities to PTV$'";

        /// <summary>
        /// Generates a list of municipalities in JSON format to applications file generation folder.
        /// </summary>
        /// <exception cref="System.Exception">The workbook doesn't contain the expected sheet name.</exception>
        internal static void GenerateJsonFile()
        {
            ExcelReader excelRdr = new ExcelReader(new ExcelReaderSettings()
            {
                File = MunicipalityTask.MunicipalitiesStartDataFile
            });

            // just a check that the sheet exists
            var sheetNameCheck = excelRdr.GetSheetNames().Find(name => string.Compare(name, MunicipalityTask.DataSheetName, StringComparison.OrdinalIgnoreCase) == 0);

            if (string.IsNullOrWhiteSpace(sheetNameCheck))
            {
                throw new Exception($"The workbook doesn't contain a sheet with name: {MunicipalityTask.DataSheetName}.");
            }

            var data = excelRdr.ReadSheet(MunicipalityTask.DataSheetName, reader =>
            {
                object dataItem = null;

                // check that we have a reader
                // we are checking the isdbnull(1) here to avoid the few empty rows that gets read from Excel sheet end
                if (reader != null && !reader.IsDBNull(1))
                {
                    dataItem = new
                    {
                        municipalityCode = (reader.GetValue(0))?.ToString().PadLeft(3, '0').Trim(),
                        names = new[]
                        {
                            new {
                                language = "fi",
                                name = reader.GetString(1)
                            },
                            new {
                                language = "sv",
                                name = reader.GetString(2)
                            }
                        },
                        organizationName = reader.GetString(4),
                        businessId = (reader.GetValue(5))?.ToString()
                    };
                }

                return dataItem;
            });

            // convert data to json
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            // overwrite the file always
            File.WriteAllText(MunicipalityTask.MunicipalitiesGeneratedFile, json);
        }

    }
}
