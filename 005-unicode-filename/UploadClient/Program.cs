using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

var filepath = @"我的Excel檔.xlsx";
var url = "http://localhost:5000/FileUpload/UploadLargeFile";

using (var client = new HttpClient ()) {
	using (var stream = System.IO.File.OpenRead (filepath)) {
		var filename = Path.GetFileName (filepath);
		var content = new MultipartFormDataContent { { new StreamContent (stream), "file", filename } };

		// BUG: .NET incorrectly insert double-quote to boundary, must remove!!!
		// See: https://stackoverflow.com/questions/30926645/httpcontent-boundary-double-quotes
		var boundary = content.Headers.ContentType?.Parameters?.First (o => o.Name == "boundary");
		if ((boundary != null) && !string.IsNullOrEmpty (boundary.Value))
			boundary.Value = boundary.Value.Replace ("\"", String.Empty);

		var response = await client.PostAsync (new Uri (url), content);
		response.EnsureSuccessStatusCode ();
	}
}
