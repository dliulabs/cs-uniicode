using System.Xml;
using System.Xml.Serialization;

namespace ExcelOpenXml {

    [XmlRoot ("Properties", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
    public class Properties {
        [XmlElement ("Application")]
        public string? Application { get; set; }

        [XmlElement ("AppVersion")]
        public string? AppVersion { get; set; }

        [XmlElement ("Company")]
        public string? Company { get; set; }

        [XmlElement ("DocSecurity")]
        public string? DocSecurity { get; set; }

        [XmlElement ("HeadingPairs")]
        public HeadingPairs? HeadingPairs { get; set; }

        [XmlElement ("HyperlinksChanged")]
        public string? HyperlinksChanged { get; set; }

        [XmlElement ("LinksUpToDate")]
        public string? LinksUpToDate { get; set; }

        [XmlElement ("ScaleCrop")]
        public string? ScaleCrop { get; set; }

        [XmlElement ("SharedDoc")]
        public string? SharedDoc { get; set; }

        [XmlElement ("TitlesOfParts")]
        public TitlesOfParts? TitlesOfParts { get; set; }
    }

    [XmlRoot ("HeadingPairs", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
    public class HeadingPairs {
        [XmlElement ("vector", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
        public Vector? Vector { get; set; }
    }

    [XmlRoot ("TitlesOfParts", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
    public class TitlesOfParts {
        [XmlElement ("vector", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
        public Vector? Vector { get; set; }
    }

    [XmlRoot ("variant", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
    public class Variant {
        [XmlElement ("i4")]
        public string? I4 { get; set; }

        [XmlElement ("lpstr")]
        public string? Lpstr { get; set; }
    }

    [XmlRoot ("vector", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
    public class Vector {
        [XmlAttribute (AttributeName = "baseType")]
        public string? BaseType { get; set; }

        [XmlElement ("lpstr")]
        public string? Lpstr { get; set; }

        [XmlAttribute (AttributeName = "size")]
        public string? Size { get; set; }

        [XmlElement ("variant")]
        public List<Variant> ? Variant { get; set; }
    }
}