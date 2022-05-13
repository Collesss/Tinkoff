using Microsoft.Extensions.Options;
using SautinSoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tinkoff.Trading.OpenApi.Models;

namespace Filter.WebDownLoad
{
    public class FilterWebDownLoadPdf : IFilter
    {
        private readonly Uri _uriDownLoad;
        public FilterWebDownLoadPdf(IOptions<FilterWebDownLoadOptions> filterOptions)
        {
            _uriDownLoad = new Uri(filterOptions.Value.UrlDownLoad);
        }

        IEnumerable<MarketInstrument> IFilter.Filtring(IEnumerable<MarketInstrument> entities)
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
