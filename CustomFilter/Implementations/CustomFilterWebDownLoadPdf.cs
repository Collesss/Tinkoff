using System.Collections.Generic;
using System.Linq;
using SautinSoft;
using System.Text.RegularExpressions;
using System;
using Tinkoff.Trading.OpenApi.Models;
using CustomFilter.Interfaces;

namespace CustomFilter.Implementations
{
    public class CustomFilterWebDownLoadPdf : ICustomFilter
    {
        private readonly Uri _uriDownLoad;
        public CustomFilterWebDownLoadPdf(string urlDownLoad)
        {
            _uriDownLoad = new Uri(urlDownLoad);
        }

        IEnumerable<MarketInstrument> ICustomFilter.Filtring(IEnumerable<MarketInstrument> entities)
        {
            //new WebClient().DownloadFile(_urlDownLoad, "file.pdf");

            PdfFocus pdfFocus = new PdfFocus();

            pdfFocus.OpenPdf(_uriDownLoad);

            string res = pdfFocus.ToText();

            pdfFocus.ClosePdf();

            List<string> Isins = new Regex(@"\b[A-Z0-9]{12}\b")
                .Matches(res)
                .Select(match => match.Value)
                .Distinct()
                .ToList();

            return entities.Where(entity => Isins.Contains(entity.Isin));
        }
    }
}
