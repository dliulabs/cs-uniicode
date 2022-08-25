using System.Xml;
using System.Xml.Serialization;

namespace ExcelOpenXml {
	[XmlRoot("coreProperties", Namespace="http://schemas.openxmlformats.org/package/2006/metadata/core-properties")]
	public class CoreProperties {
		[XmlElement("creator", Namespace="http://purl.org/dc/elements/1.1/")]
		public string? Creator { get; set; }
		[XmlElement("lastModifiedBy")]
		public string? LastModifiedBy { get; set; }
		[XmlElement("created", Namespace="http://purl.org/dc/terms/")]
		public string? Created { get; set; }
		[XmlElement("modified", Namespace="http://purl.org/dc/terms/")]
		public string? Modified { get; set; }
	}
}