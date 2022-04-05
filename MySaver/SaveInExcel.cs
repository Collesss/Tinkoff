using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MySaver
{
    public class SaveInExcel<V> : ISave<(string fileName, string sheetName), V>
    {
        private readonly IEnumerable<(Func<V, object> element, string header, string format)> _columns;
        private readonly string _saveDirectory;

        public SaveInExcel(IEnumerable<(Func<V, object> element, string header, string format)> columns, string saveDirectory)
        {
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            _saveDirectory = saveDirectory;
            _columns = columns;
        }

        async Task ISave<(string fileName, string sheetName), V>.Save((string fileName, string sheetName) metaDataForSave, IEnumerable<V> elements)
        {
            await Task.Run(() =>
            {
                metaDataForSave.fileName = $@"{_saveDirectory}\{metaDataForSave.fileName}";

                if (File.Exists(metaDataForSave.fileName))
                    File.Delete(metaDataForSave.fileName);

                using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(metaDataForSave.fileName)))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(metaDataForSave.sheetName);

                    int i = 1;

                    foreach (var column in _columns)
                    {
                        worksheet.Cells[1, i].Value = column.header;
                        worksheet.Column(i).Style.Numberformat.Format = column.format;

                        i++;
                    }

                    i = 2;

                    foreach (var element in elements)
                    {
                        int j = 1;

                        foreach (var column in _columns)
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
