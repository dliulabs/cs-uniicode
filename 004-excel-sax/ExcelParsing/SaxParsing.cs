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
        private static readonly string url = "https://xlsxtemplates.com/wp-content/uploads/2022/03/Arabic-VAT-Invoice-Template.xlsx";
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
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
            {
                WorkbookPart? workbookPart = document.WorkbookPart;
                WorksheetPart? worksheetPart = workbookPart?.WorksheetParts.First();
                SharedStringTablePart? shareStringPart = workbookPart?.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (worksheetPart is null) return;
                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);

                //row counter
                int rcnt = 0;

                using (TextWriter writer = new StreamWriter(@"Output.csv", false, System.Text.Encoding.UTF8))
                {
                    var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                    while (reader.Read())
                    {
                        if (reader.ElementType == typeof(Row))
                        {
                            //if row has attribute means it is not a empty row
                            if (reader.HasAttributes)
                            {
                                reader.ReadFirstChild();
                                do
                                {
                                    //find xml cell element type 
                                    if (reader.ElementType == typeof(Cell))
                                    {
                                        Cell? c = (Cell?)reader.LoadCurrentElement();
                                        if (c != null)
                                        {
                                            string? cellValue;
                                            int actualCellIndex = CellReferenceToIndex(c);
                                            cellValue = GetCellValue(c, shareStringPart);
                                            csv.WriteField(cellValue);
                                        }
                                    }
                                }
                                while (reader.ReadNextSibling());
                                rcnt++;
                            }
                            csv.NextRecord();
                        }
                    }
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        private static int CellReferenceToIndex(Cell cell)
        {
            int index = 0;
            string? reference = cell?.CellReference?.ToString()?.ToUpper();
            if (reference is null) return index;
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

        private static String? GetCellValue(Cell cell, SharedStringTablePart? stringTablePart)
        {
            if ((cell is null) || (cell.ChildElements.Count == 0))
                return null;

            String? value = cell.CellValue?.InnerText;
            if ((stringTablePart != null) && (cell.DataType != null) && (cell.DataType == CellValues.SharedString))
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