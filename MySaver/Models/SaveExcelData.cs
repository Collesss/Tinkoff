using System;
using System.Collections.Generic;
using System.Text;

namespace MySaver.Models
{
    public class SaveExcelData<T>
    {
        public string FileName { get; }
        public string SheetName { get; }
        public IEnumerable<T> Data { get; }

        public SaveExcelData(string fileName, string sheetName, IEnumerable<T> data)
        {
            FileName = fileName;
            SheetName = sheetName;
            Data = data;
        }
    }
}
