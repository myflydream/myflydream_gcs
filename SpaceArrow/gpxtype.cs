using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
namespace Serialize
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlRoot("gpx", Namespace = "http://www.topografix.com/GPX/1/1", IsNullable = false)]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class gpxType
    {

        public gpxType() { }

        [XmlAttribute]
        public string creator { get; set; }
        public extensionsType extensions { get; set; }
        public metadataType metadata { get; set; }

        [XmlElement("wpt")]
        public wptType[] wpt { get; set; }

        [XmlElement("rte")]
        public rteType[] rte { get; set; }
        [XmlElement("trk")]
        public trkType[] trk { get; set; }
        [XmlAttribute]
        public string version { get; set; }
    }
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class wptType
    {
        public wptType() { }
        [XmlAttribute]
        public decimal lat { get; set; }
        [XmlAttribute]
        public decimal lon { get; set; }
        public decimal ele { get; set; }
        public DateTime time { get; set; }

       // public decimal ageofdgpsdata { get; set; }
     //   public bool ageofdgpsdataSpecified { get; set; }
     //   public string cmt { get; set; }
    //    public string desc { get; set; }
    //    public string dgpsid { get; set; }
   //     public bool eleSpecified { get; set; }
   //     public extensionsType extensions { get; set; }
 //       public fixType fix { get; set; }
 //       public bool fixSpecified { get; set; }
  //      public decimal geoidheight { get; set; }
  //      public bool geoidheightSpecified { get; set; }
  //      public decimal hdop { get; set; }
  //      public bool hdopSpecified { get; set; }
 //       public linkType[] link { get; set; }
  //      public decimal magvar { get; set; }
  //      public bool magvarSpecified { get; set; }
  //      public string name { get; set; }
  //      public decimal pdop { get; set; }
  //      public bool pdopSpecified { get; set; }
  //      public string sat { get; set; }
  //      public string src { get; set; }
  //      public string sym { get; set; }
//        public bool timeSpecified { get; set; }
 //       public string type { get; set; }
 //       public decimal vdop { get; set; }
 //       public bool vdopSpecified { get; set; }
    }

    [Serializable]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public enum fixType
    {
        none = 0,
        [XmlEnum("2d")]
        Item2d = 1,
        [XmlEnum("3d")]
        Item3d = 2,
        dgps = 3,
        pps = 4,
    }
    #region trkType
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class trkType
    {
        public trkType() { }

        public string cmt { get; set; }
        public string desc { get; set; }
        public extensionsType extensions { get; set; }
        [XmlElement("link")]
        public linkType[] link { get; set; }
        public string name { get; set; }
        [XmlElement(DataType = "nonNegativeInteger")]
        public string number { get; set; }
        public string src { get; set; }
        [XmlElement("trkseg")]
        public trksegType[] trkseg { get; set; }
        public string type { get; set; }
    }
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class trksegType
    {
        public trksegType() { }

        public extensionsType extensions { get; set; }
        [XmlElement("trkpt")]
        public wptType[] trkpt { get; set; }
    }

    #endregion
    #region rteType
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class rteType
    {
        public rteType() { }

        public string cmt { get; set; }
        public string desc { get; set; }
        public extensionsType extensions { get; set; }
        [XmlElement("link")]
        public linkType[] link { get; set; }
        public string name { get; set; }
        [XmlElement(DataType = "nonNegativeInteger")]
        public string number { get; set; }
        [XmlElement("rtept")]
        public wptType[] rtept { get; set; }
        public string src { get; set; }
        public string type { get; set; }
    }
    #endregion

      #region extensionsType

      [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class extensionsType
    {
          public extensionsType() { }

        [XmlAnyElement]
        public XmlElement[] Any { get; set; }
    }
#endregion

    #region metadataType
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class metadataType
    {
        public metadataType() { }

        public personType author { get; set; }
        public boundsType bounds { get; set; }
        public copyrightType copyright { get; set; }
        public string desc { get; set; }
        public extensionsType extensions { get; set; }
        public string keywords { get; set; }
        [XmlElement("link")]
        public linkType[] link { get; set; }
        public string name { get; set; }
        public DateTime time { get; set; }
        [XmlIgnore]
        public bool timeSpecified { get; set; }
    }

    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class boundsType
    {
        public boundsType() { }

        [XmlAttribute]
        public decimal maxlat { get; set; }
        [XmlAttribute]
        public decimal maxlon { get; set; }
        [XmlAttribute]
        public decimal minlat { get; set; }
        [XmlAttribute]
        public decimal minlon { get; set; }
    }

    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class copyrightType
    {
        public copyrightType() { }

        [XmlAttribute]
        public string author { get; set; }
        [XmlElement(DataType = "anyURI")]
        public string license { get; set; }
        [XmlElement(DataType = "gYear")]
        public string year { get; set; }
    }
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class personType
    {
        public personType() { }

        public emailType email { get; set; }
        public linkType link { get; set; }
        public string name { get; set; }
    }
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class linkType
    {
        public linkType() { }

        [XmlAttribute(DataType = "anyURI")]
        public string href { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "4.0.30319.1")]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class emailType
    {
        public emailType() { }

        [XmlAttribute]
        public string domain { get; set; }
        [XmlAttribute]
        public string id { get; set; }
    }

#endregion 

}
