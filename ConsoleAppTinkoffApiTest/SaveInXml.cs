using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ConsoleAppTinkoffApiTest
{
    public class SaveInXml
    {

        public static void Save(string fileName, IEnumerable<IEnumerable<object>> elements, IEnumerable<string> columnsHeaders, IEnumerable<string> formats)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(fileName)))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A:B"].Style.Numberformat.Format = "dd.MM.yyyy HH:mm";

                int d = 1;

                foreach (var header in columnsHeaders)
                {
                    worksheet.Cells[1, d].Value = header;
                    d++;
                }

                int p = 1;

                foreach (var format in formats)
                {
                    if(format != null)
                        worksheet.Column(p).Style.Numberformat.Format = format;
                    p++;
                }

                int i = 2;

                foreach (var item in elements)
                {
                    int j = 1;

                    foreach (var prop in item)
                    {
                        worksheet.Cells[i, j].Value = prop;
                        j++;
                    }

                    i++;
                }

                worksheet.Cells.AutoFitColumns();

                excelPackage.Save();
            }
        }

        public static void Save<T>(string fileName, string sheetName, IEnumerable<T> elements, IEnumerable<(Func<T, object> element, string header, string format)> columns)
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            fileName = $@"Data\{fileName}";

            if (File.Exists(fileName))
                File.Delete(fileName);

            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(fileName)))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                int i = 1;

                foreach (var column in columns)
                {
                    worksheet.Cells[1, i].Value = column.header;
                    worksheet.Column(i).Style.Numberformat.Format = column.format;

                    i++;
                }

                i = 2;

                foreach (var element in elements)
                {
                    int j = 1;

                    foreach (var column in columns)
                    {
                        worksheet.Cells[i, j].Value = column.element(element);
                        j++;
                    }
   
                    i++;
                }

                worksheet.Cells.AutoFitColumns();

                excelPackage.Save();
            }
        }
    }
}
