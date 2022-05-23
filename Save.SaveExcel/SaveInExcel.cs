using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace Save.SaveExcel
{
    public class SaveInExcel : ISaveCandleMarketInstrument
    {
        private readonly OptionsSaveInExcel _options;

        public SaveInExcel(IOptions<OptionsSaveInExcel> options)
        {
            _options = options.Value;
        }

        async Task ISave<DataSaveCandleMarketInstrument>.Save(DataSaveCandleMarketInstrument data)
        {
            await Task.Run(() =>
            {
                string fileName = $@"{_options.SaveDirectory}\{Regex.Replace(data.MarketInstrument.Name, @"[\\\/:*?""<>|]", "_")}.xlsx";

                if (File.Exists(fileName))
                    File.Delete(fileName);

                using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(fileName)))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Data");

                    worksheet.Cells[1, 1].Value = "CloseTime";
                    worksheet.Column(1).Style.Numberformat.Format = "dd.MM.yyyy HH:mm";
                    worksheet.Cells[1, 2].Value = "Open";
                    worksheet.Cells[1, 3].Value = "Close";
                    worksheet.Cells[1, 4].Value = "Low";
                    worksheet.Cells[1, 5].Value = "High";


                    int i = 2;

                    var saveData = data.CandlePayload
                        .GroupBy(el => $"{el.Time.Year}|{el.Time.Month}|{el.Time.Day}|{(el.Time.Hour + 1) / 4}")
                        .Select(group =>
                        {
                            var ordered = group.OrderBy(candle => candle.Time).ToArray();

                            return new CandlePayload(
                                ordered.First().Open,
                                ordered.Last().Close,
                                ordered.Max(candle => candle.High),
                                ordered.Min(candle => candle.Low),
                                ordered.Average(candle => candle.Volume),
                                ordered.First().Time,
                                ordered.First().Interval,
                                data.MarketInstrument.Figi);
                        })
                        .OrderBy(aggCandle => aggCandle.Time);

                    foreach (var element in saveData)
                    {
                        worksheet.Cells[i, 1].Value = element.Time + TimeSpan.FromHours(4);
                        worksheet.Cells[i, 2].Value = element.Open;
                        worksheet.Cells[i, 3].Value = element.Close;
                        worksheet.Cells[i, 4].Value = element.Low;
                        worksheet.Cells[i, 5].Value = element.High;

                        i++;
                    }

                    worksheet.Cells.AutoFitColumns();

                    excelPackage.Save();
                }
            });
        }
    }
}
