using MySaver.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MySaver
{
    public class SaveInExcel<T> : ISave<SaveExcelData<T>>
    {
        private readonly IEnumerable<(Func<T, object> element, string header, string format)> _columns;
        private readonly string _saveDirectory;

        public SaveInExcel(IEnumerable<(Func<T, object> element, string header, string format)> columns, string saveDirectory)
        {
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            _saveDirectory = saveDirectory;
            _columns = columns;
        }

        async Task ISave<SaveExcelData<T>>.Save(SaveExcelData<T> saveExcelData)
        {
            await Task.Run(() =>
            {
                string fileName = $@"{_saveDirectory}\{saveExcelData.FileName}";

                if (File.Exists(fileName))
                    File.Delete(fileName);

                using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(fileName)))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(saveExcelData.SheetName);

                    int i = 1;

                    foreach (var column in _columns)
                    {
                        worksheet.Cells[1, i].Value = column.header;
                        worksheet.Column(i).Style.Numberformat.Format = column.format;

                        i++;
                    }

                    i = 2;

                    foreach (var element in saveExcelData.Data)
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
