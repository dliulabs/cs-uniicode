# Non-Western Characters

## Lab 01: Console HTTP Client

```
export SUBSCRIPTION_KEY=<azure translator subscription key>
dotnet run
```

## Lab 02: Get Excel Metadata

Parsing the Core & App Properties of an Excel Workbook

## Lab 03: Parsing Excel using OpenXml DOM approach

## Lab 04: Parsing Excel using OpenXml SAX approach

- accessing an Arabic Excel
- output a CSV (`Output.csv`) with correct BOM ([Byte Order Make](https://docs.microsoft.com/en-us/globalization/encoding/byte-order-mark))

## Lab05: Unicode file upload

- enable Unicode logging

```
public void ConfigureServices (IServiceCollection services) {
    Console.OutputEncoding = System.Text.Encoding.UTF8; // enable Unicode logging
    services.AddControllers ();
}
```
- enable Unicode filename (the filename* value)

```

- saving file to Unicode filenames


if you post file via Postman using 'form data' and the filename contains Unicode text, Postman will correctly post it with a `filename*` containing `UTF-8` prefix.

```
POST https://localhost:5001/FileUpload/UploadLargeFile
200
133 ms
Warning: Unable to verify the first certificate
POST /FileUpload/UploadLargeFile HTTP/1.1
User-Agent: PostmanRuntime/7.29.2
Accept: */*
Cache-Control: no-cache
Postman-Token: 9c5879b4-102d-43a4-acca-1228c218011c
Host: localhost:5001
Accept-Encoding: gzip, deflate, br
Connection: keep-alive
Content-Type: multipart/form-data; boundary=--------------------------500863287972249715307550
Content-Length: 44253
 
----------------------------500863287972249715307550
Content-Disposition: form-data; name="file"; filename="我的Excel檔.xlsx"; filename*="UTF-8''%E6%88%91%E7%9A%84Excel%E6%AA%94.xlsx"
<我的Excel檔.xlsx>
----------------------------500863287972249715307550--
```

```
curl -X POST -F upload=@حساب.xlsx  https://localhost:7090/FileUpload/UploadLargeFile --insecure
```

## Lab06: URI containing Unicode text

