using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using CsvHelper;
using System.Globalization;

namespace ExcelParsing
{
    public class DomParsing
    {
        private static readonly string url = "https://download.microsoft.com/download/1/4/E/14EDED28-6C58-4055-A65C-23B4DA81C4DE/Financial%20Sample.xlsx";
        private readonly IHttpClientFactory _httpClientFactory;
        public DomParsing(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        public async Task ProcessExcelAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();

            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url)))
            {
                // add Content, Headers, etc to request
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var contentStream = await response.Content.ReadAsStreamAsync();
                ProcessStream(contentStream);
            }
        }

        private void ProcessStream(Stream stream)
        {
            {
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
                {
                    WorkbookPart? workbookPart = doc.WorkbookPart;
                    SharedStringTablePart? sstpart = workbookPart?.GetPartsOfType<SharedStringTablePart>()?.First();
                    SharedStringTable? sst = sstpart?.SharedStringTable;

                    WorksheetPart? worksheetPart = workbookPart?.WorksheetParts.First();
                    Worksheet? sheet = worksheetPart?.Worksheet;

                    var cells = sheet?.Descendants<Cell>();
                    var rows = sheet?.Descendants<Row>();
                    if (rows is null) return;

                    foreach (Row row in rows)
                    {
                        foreach (Cell c in row.Elements<Cell>())
                        {
                            if ((c.DataType != null) && (c.DataType == CellValues.SharedString))
                            {
                                int ssid;
                                bool success = int.TryParse(c?.CellValue?.Text, out ssid);
                                if (success)
                                {
                                    string? str = sst?.ChildElements[ssid]?.InnerText;
                                    Console.Write("{0}\t", str);
                                }
                            }
                            else if (c.CellValue != null)
                            {
                                Console.Write("{0}\t", c.CellValue.Text);
                            }
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        private static String? GetCellValue(Cell cell, SharedStringTablePart stringTablePart)
        {
            if ((cell is null) || (cell.ChildElements.Count == 0))
                return null;

            String? value = cell.CellValue?.InnerText;
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                Int32 iValue;
                bool success = Int32.TryParse(value, out iValue);
                if (success) 
                    value = stringTablePart
                        .SharedStringTable
                        .ChildElements[iValue]
                        .InnerText;
            }
            return value;
        }
    }
}