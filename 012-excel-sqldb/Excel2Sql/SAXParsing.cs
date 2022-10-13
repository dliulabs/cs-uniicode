using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Excel2Sql {
	public class SAXParsing {
		private static readonly string url =
			"https://aksjflkfjasdlkjfasd.blob.core.windows.net/container1/AL%20WAHA_AR%20TB.xlsx?sp=r&st=2022-09-16T14:11:18Z&se=2022-12-31T23:11:18Z&spr=https&sv=2021-06-08&sr=b&sig=fp%2FlvGX6Cojc1IwZq6WLDTQRlCRYeOPosXZOQcPQImM%3D";
		private readonly IHttpClientFactory _httpClientFactory;
		public SAXParsing (IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

		public async Task<DataTable> ProcessExcelAsync () {
			var httpClient = _httpClientFactory.CreateClient ();

			using (var request = new HttpRequestMessage (HttpMethod.Get, new Uri (url))) {
				// add Content, Headers, etc to request
				request.Headers.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
				var response = await httpClient.SendAsync (request, HttpCompletionOption.ResponseHeadersRead);
				response.EnsureSuccessStatusCode ();
				var contentStream = await response.Content.ReadAsStreamAsync ();
				DataTable dt = Excel2DataTable (contentStream);
				return dt;
			}
		}

		private void DataTable2Sql (DataTable dt) {

		}
		private DataTable Excel2DataTable (Stream stream) {
			var dt = new DataTable ();
			var skipRows = 3;
			using (SpreadsheetDocument document = SpreadsheetDocument.Open (stream, false)) {
				WorkbookPart? workbookPart = document.WorkbookPart;
				WorksheetPart? worksheetPart = workbookPart?.WorksheetParts.First ();
				SharedStringTablePart? shareStringPart = workbookPart?.GetPartsOfType<SharedStringTablePart> ().FirstOrDefault ();
				if (worksheetPart is null) return dt;
				OpenXmlReader reader = OpenXmlReader.Create (worksheetPart);

				//row counter
				int rcnt = 0;

				using (TextWriter writer = new StreamWriter (@"Output.csv", false, System.Text.Encoding.UTF8)) {
					var csv = new CsvWriter (writer, CultureInfo.InvariantCulture);
					while (reader.Read ()) {
						if (reader.ElementType == typeof (Row)) {
							DataRow tempRow = dt.NewRow ();
							//if row has attribute means it is not a empty row
							if (reader.HasAttributes) {
								reader.ReadFirstChild ();
								do {
									//find xml cell element type 
									if (reader.ElementType == typeof (Cell)) {

										Cell c = (Cell) reader.LoadCurrentElement ();
										string cellValue;
										int actualCellIndex = CellReferenceToIndex (c);

										cellValue = GetCellValue (c, shareStringPart);

										// if row index is the header row  
										if (rcnt == skipRows) {
											dt.Columns.Add (cellValue);
											csv.WriteField (cellValue);
											// Console.Write ("{0}\t", cellValue);
										} else if (rcnt >= skipRows) {
											// instead of tempRow[c.CellReference] = cellValue;
											tempRow[actualCellIndex] = cellValue;
											csv.WriteField (cellValue);
											// Console.Write ("{0}\t", cellValue);
										}

									}
								}
								while (reader.ReadNextSibling ());
								// if its not the header row so append rowdata to the datatable
								if (rcnt == skipRows)
									csv.NextRecord ();
								if (rcnt > skipRows) {
									dt.Rows.Add (tempRow);
									csv.NextRecord ();
									// Console.WriteLine ();
								}
								rcnt++;
							}
						}
					}
					writer.Flush ();
					writer.Close ();
				}
			}

			return dt;

		}

		private static int CellReferenceToIndex (Cell cell) {
			int index = 0;
			string? reference = cell?.CellReference?.ToString ()?.ToUpper ();
			if (reference is null) return index;
			foreach (char ch in reference) {
				if (Char.IsLetter (ch)) {
					int value = (int) ch - (int)
					'A';
					index = (index == 0) ? value : ((index + 1) * 26) + value;
				} else
					return index;
			}
			return index;
		}

		private static String? GetCellValue (Cell cell, SharedStringTablePart? stringTablePart) {
			if ((cell is null) || (cell.ChildElements.Count == 0))
				return null;

			String? value = cell.CellValue?.InnerText;
			if ((stringTablePart != null) && (cell.DataType != null) && (cell.DataType == CellValues.SharedString)) {
				Int32 iValue;
				bool success = Int32.TryParse (value, out iValue);
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