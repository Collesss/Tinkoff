using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MySaver
{
    public class SaveInXml : ISave<(string fileName, string sheetName)>
    {
        async Task ISave<(string fileName, string sheetName)>.Save<V>((string fileName, string sheetName) metaDataForSave, IEnumerable<V> elements, IEnumerable<(Func<V, object> element, string header, string format)> columns)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists("Data"))
                    Directory.CreateDirectory("Data");

                metaDataForSave.fileName = $@"Data\{metaDataForSave.fileName}";

                if (File.Exists(metaDataForSave.fileName))
                    File.Delete(metaDataForSave.fileName);

                using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(metaDataForSave.fileName)))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(metaDataForSave.sheetName);

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
            });
        }
    }
}
