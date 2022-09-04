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
    public class SaxParsing
    {
        private static readonly string url = "https://download.microsoft.com/download/1/4/E/14EDED28-6C58-4055-A65C-23B4DA81C4DE/Financial%20Sample.xlsx";
        private readonly IHttpClientFactory _httpClientFactory;
        public SaxParsing(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

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
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    SharedStringTable sst = sstpart.SharedStringTable;

                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    Worksheet sheet = worksheetPart.Worksheet;

                    var cells = sheet.Descendants<Cell>();
                    var rows = sheet.Descendants<Row>();


                    foreach (Row row in rows)
                    {
                        foreach (Cell c in row.Elements<Cell>())
                        {
                            if ((c.DataType != null) && (c.DataType == CellValues.SharedString))
                            {
                                int ssid = int.Parse(c.CellValue.Text);
                                string str = sst.ChildElements[ssid].InnerText;
                                Console.Write("{0}\t", str);
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
        private static int CellReferenceToIndex(Cell cell)
        {
            int index = 0;
            string reference = cell.CellReference.ToString().ToUpper();
            foreach (char ch in reference)
            {
                if (Char.IsLetter(ch))
                {
                    int value = (int)ch - (int)'A';
                    index = (index == 0) ? value : ((index + 1) * 26) + value;
                }
                else
                    return index;
            }
            return index;
        }

        private static String GetCellValue(Cell cell, SharedStringTablePart stringTablePart)
        {
            if (cell.ChildElements.Count == 0)
                return null;
            //get cell value
            String value = cell.CellValue.InnerText;
            //Look up real value from shared string table
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                value = stringTablePart
                    .SharedStringTable
                    .ChildElements[Int32.Parse(value)]
                    .InnerText;
            return value;
        }
    }
}