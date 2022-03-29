using DBTinkoffEntities.Entities;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using SautinSoft;
using System.Text.RegularExpressions;

namespace ConsoleAppTest
{
    public class CustomFilter : ICustomFilter
    {
        private readonly string _urlDownLoad;
        public CustomFilter(string urlDownLoad)
        {
            _urlDownLoad = urlDownLoad;
        }

        void ICustomFilter.Filtring(IEnumerable<EntityMarketInstrument> entities)
        {
            new WebClient().DownloadFile(_urlDownLoad, "file.pdf");

            PdfFocus pdfFocus = new PdfFocus();

            pdfFocus.OpenPdf("file.pdf");

            string res = pdfFocus.ToText();

            pdfFocus.ToExcel("test.xlsx");

            pdfFocus.ClosePdf();

            List<string> Isins = new Regex(@"\b[A-Z0-9]{12}\b").Matches(res).Select(match => match.Value).Distinct().ToList();

            entities.Where(entity => Isins.Contains(entity.Isin));
        }
    }
}
