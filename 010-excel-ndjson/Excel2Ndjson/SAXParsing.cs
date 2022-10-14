using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CsvHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Excel2Ndjson;
public class SAXParsing
{
    private static readonly string url = "https://dliustorage0001.blob.core.windows.net/container1/13_400k_6_20221013120444350.xlsx?sp=r&st=2022-10-13T20:31:45Z&se=2023-10-01T04:31:45Z&spr=https&sv=2021-06-08&sr=b&sig=9ZNQxmeyxhLkTGEyUNJr9Iqp4eCZKc1jEJapoT9VLBg%3D";

    private readonly IHttpClientFactory _httpClientFactory;
    public SAXParsing(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;
    private static readonly byte[] _newlineDelimiter = Encoding.UTF8.GetBytes("\n");

    public async Task<DataTable> ProcessExcelAsync2()
    {
        var filepath = "./Test.xlsx";
        using (var stream = System.IO.File.OpenRead(filepath))
        {
            DataTable dt = Excel2DataTable(stream);
            return dt;
        }
    }
    public async Task<DataTable> ProcessExcelAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();

        using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url)))
        {
            // add Content, Headers, etc to request
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var contentStream = await response.Content.ReadAsStreamAsync();
            DataTable dt = Excel2DataTable(contentStream);
            return dt;
        }
    }

    //public record Data(int Id = default, string Column1 = null, string Column2 = null);
    public record KeyValue(string key, string value);

    private DataTable Excel2DataTable(Stream stream)
    {
        var dt = new DataTable();
        var skipRows = 0;
        var col = 0;
        List<KeyValue> header = new List<KeyValue>();
        
        using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
        {
            WorkbookPart? workbookPart = document.WorkbookPart;
            // WorksheetPart? worksheetPart = workbookPart?.WorksheetParts.First();
            WorksheetPart? worksheetPart = GetWorksheetPartByName(workbookPart, "Data");
            SharedStringTablePart? shareStringPart = workbookPart?.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            if (worksheetPart is null) return dt;
            OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);

            //row counter
            int rcnt = 0;

            using (TextWriter writer = new StreamWriter(@"Output.ndjson", false, System.Text.Encoding.UTF8))
            {
                // var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                while (reader.Read())
                {
                    if (reader.ElementType == typeof(Row))
                    {
                        List<KeyValue> data = new List<KeyValue>();
                        // DataRow tempRow = dt.NewRow();
                        //if row has attribute means it is not a empty row
                        if (reader.HasAttributes)
                        {
                            reader.ReadFirstChild();
                            do
                            {
                                //find xml cell element type 
                                if (reader.ElementType == typeof(Cell))
                                {

                                    Cell c = (Cell)reader.LoadCurrentElement();
                                    string cellValue;
                                    int actualCellIndex = CellReferenceToIndex(c);

                                    cellValue = GetCellValue(c, shareStringPart);
                                    // cellValue = c.CellValue?.InnerText;

                                    // if row index is the header row  
                                    if (rcnt == skipRows)
                                    {
                                        // dt.Columns.Add(cellValue);
                                        // dt.Columns.Add(c.CellReference); // A1, B1, etc.
                                        // csv.WriteField (cellValue);
                                        header.Add(new KeyValue(c.CellReference, cellValue ));
                                        Console.WriteLine("Header {0}: {1}\t{2}", col, cellValue, c.CellReference);
                                    }
                                    else if (rcnt >= skipRows)
                                    {
                                        // instead of tempRow[c.CellReference] = cellValue;
                                        // tempRow[actualCellIndex] = cellValue;
                                        // csv.WriteField (cellValue);
                                        data.Add(new KeyValue(c.CellReference, cellValue ));
                                        // Console.Write ("{0}\t", cellValue);
                                    }
                                    col++;
                                }
                            }
                            while (reader.ReadNextSibling());
                            // if its not the header row so append rowdata to the datatable
                            // if (rcnt == skipRows) csv.NextRecord();
                            if (rcnt > skipRows)
                            {
                                //var str = JsonConvert.SerializeObject(tempRow, Formatting.None);
                                /* var str = JsonConvert.SerializeObject(
                                    new Data
                                    {
                                        Id = rcnt,
                                        Column1 = tempRow[1].ToString(),
                                        Column2 = tempRow[2].ToString(),
                                    },
                                    Formatting.None
                                );*/
                                var str = JsonConvert.SerializeObject(new {id = rcnt, data= data.Select(r => r.value).ToArray()});
                                writer.WriteLine(str); 
                                // dt.Rows.Add (tempRow);
                                // csv.NextRecord ();
                                // Console.WriteLine ();
                            }
                            rcnt++;
                        }
                    }
                }
                writer.Flush();
                writer.Close();
            }
            return dt;
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
                int value = (int)ch - (int)
                'A';
                index = (index == 0) ? value : ((index + 1) * 26) + value;
            }
            else
                return index;
        }
        return index;
    }

    private static String? GetCellValue(Cell cell, SharedStringTablePart? stringTablePart)
    {
        String? value = cell.CellValue?.InnerText;
        if ((stringTablePart != null) && (cell.DataType != null) && (cell.DataType == CellValues.SharedString))
        {
            Int32 iValue;
            bool success = Int32.TryParse(value, out iValue);
            return stringTablePart
                    .SharedStringTable
                    .ChildElements[iValue]
                    .InnerText;
        }
        return cell.CellValue?.InnerText;
    }
    private static WorksheetPart GetWorksheetPartByName(WorkbookPart workbookPart, string sheetName)
    {
        IEnumerable<Sheet> sheets = workbookPart.Workbook
                .GetFirstChild<Sheets>()
                .Elements<Sheet>()
                .Where(s => s.Name == sheetName);

        if (sheets?.Count() == 0) return null;
        string relationshipId = sheets?.First().Id.Value;
        if (relationshipId == null) return null;
        return (WorksheetPart)workbookPart.GetPartById(relationshipId);
    }
}