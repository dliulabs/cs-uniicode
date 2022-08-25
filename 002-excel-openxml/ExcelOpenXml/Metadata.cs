using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.IO.Compression;

namespace ExcelOpenXml
{
    public class ExcelMetadata
    {
        private static readonly string url = "https://download.microsoft.com/download/1/4/E/14EDED28-6C58-4055-A65C-23B4DA81C4DE/Financial%20Sample.xlsx";
        private readonly IHttpClientFactory _httpClientFactory;
        public ExcelMetadata(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;
        // public async Task<Properties?> ProcessExcelAsync()
        public async Task<(Properties? p, CoreProperties? c)> ProcessExcelAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            Properties? p = null;
            CoreProperties? c = null;

            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url)))
            {
                // add Content, Headers, etc to request
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var contentStream = await response.Content.ReadAsStreamAsync();
                ZipArchive archive = new ZipArchive(contentStream, ZipArchiveMode.Read);

                foreach (var entry in archive.Entries)
                {
                    switch (entry.FullName)
                    {
                        case "docProps/core.xml":
                            using (var xmlStream = entry.Open())
                            {
                                c = ProcessCore(xmlStream);
                            }
                            break;
                        case "docProps/app.xml":
                            using (var xmlStream = entry.Open())
                            {
                                p = ProcessApp(xmlStream);
                            }
                            break;
                        default:
                            break;
                    }
                }
                return (p, c);
            }

        }
        static string PrettyXml(string xml)
        {
            var stringBuilder = new StringBuilder();

            var element = XElement.Parse(xml);

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }

        protected CoreProperties? ProcessCore(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CoreProperties));
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            // Declares an object variable of the type to be deserialized.
            CoreProperties? core = serializer.Deserialize(stream) as CoreProperties;
            return core;
        }

        protected Properties? ProcessApp(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Properties));
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            // Declares an object variable of the type to be deserialized.
            Properties? app = serializer.Deserialize(stream) as Properties;
            // Console.WriteLine("Excel Version: {0}", app?.AppVersion);
            return app;
        }

        protected void serializer_UnknownNode
            (object? sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        protected void serializer_UnknownAttribute
            (object? sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
        }
    }
}