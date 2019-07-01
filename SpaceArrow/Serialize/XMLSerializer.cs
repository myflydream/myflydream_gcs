using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System;
namespace Serialize
{
    class XMLSerializer
    {
        public static void SerializeCreateOverrider<T>(T o, string filepath)
        {
            XmlAttributeOverrides overr = new XmlAttributeOverrides();
            XmlAttributes attrs = new XmlAttributes();

            attrs.XmlIgnore = false;
            overr.Add(typeof(DateTime), "time", attrs);
            overr.Add(typeof(decimal), "ele", attrs);
            XmlSerializer serilizer = new XmlSerializer(typeof(T), overr);
            StreamWriter sw = new StreamWriter(filepath, false);

            serilizer.Serialize(sw, o);
            sw.Flush();
            sw.Close();
        }

        public static void Serialize<T>(T o, string filePath)
        {
            try
            {
                //XmlSerializer formatter = new XmlSerializer(typeof(T));
                ////XmlSerializer f=new XmlSerializer()
                //StreamWriter sw = new StreamWriter(filePath, false);
                
                //formatter.Serialize(sw, o);
                //sw.Flush();
                //sw.Close();

                //string xmlstring = File.ReadAllText(filePath);
                ////  int index = xmlstring.IndexOf("<gpx", 0);
                //xmlstring=xmlstring.Replace("<gpx", "<gpx version=\"1.1\" creator=\"SpaceArrow 1.0\" ");
                
                //File.WriteAllText(filePath,xmlstring);

                XmlSerializer formatter = new XmlSerializer(typeof(T));
                StreamWriter sw = new StreamWriter(filePath, false);
                formatter.Serialize(sw, o);
                sw.Flush();
                sw.Close();
            }
            catch  { 
            
            }
        }

        public static T DeSerialize<T>(string filePath)
        {
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                StreamReader sr = new StreamReader(filePath);
                T o = (T)formatter.Deserialize(sr);
                sr.Close();
                return o;
            }
            catch 
            {
            }
            return default(T);
        }



        public static void SerializeGPX(string filePath) { 
            
        }

        public static gpxType DeSerializeGPX(string filePath)
        {
            gpxType gpx = new gpxType();
            StreamReader sr = new StreamReader(filePath);
            string str = "";
            string line = "";
            str = sr.ReadLine();
            while (str != null)
            {
                line += str;
                str = sr.ReadLine();
            }

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(line);

            System.Xml.XmlNodeList wptList = xmlDoc.GetElementsByTagName("wpt");
            System.Xml.XmlNodeList rteList = xmlDoc.GetElementsByTagName("rte");
            System.Xml.XmlNodeList trkList = xmlDoc.GetElementsByTagName("trk");

            //   System.Xml.XmlAttribute at=

            if (wptList[0] != null)
            {
                System.Xml.XmlNodeList at = wptList[0].ChildNodes;

                gpx.wpt = new wptType[wptList.Count];
                int index = 0;
                #region WPG航点数据
                foreach (System.Xml.XmlNode node in wptList)
                {
                    wptType waypointvalue = new wptType();
                    int cnt = 0;

                    System.Xml.XmlNodeList childList = node.ChildNodes;
                    System.Xml.XmlAttributeCollection chileAtt = node.Attributes;

                    for (int i = 0; i < chileAtt.Count; i++)
                    {
                        try
                        {
                            switch (chileAtt[i].Name)
                            {
                                case "lat":
                                    // waypointvalue.Latitude
                                    waypointvalue.lat = (decimal)double.Parse(chileAtt[i].InnerText); cnt++;
                                    break;
                                case "lon":
                                    waypointvalue.lon = (decimal)double.Parse(chileAtt[i].InnerText); cnt++;
                                    break;
                                default: break;
                            }
                        }
                        catch
                        {

                        }
                    }
                    for (int i = 0; i < childList.Count; i++)
                    {
                        try
                        {
                            switch (childList[i].Name)
                            {
                                case "ele":
                                    waypointvalue.ele = (decimal)float.Parse(childList[i].InnerText);
                                    break;
                                // case "cmt":
                                //  waypointvalue.cm = (childList[i].InnerText);
                                //   break;
                                // case "desc":
                                //    waypointvalue = (childList[i].InnerText);
                                // break;
                                default: break;
                            }

                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }

                    if (cnt < 2)
                    {
                        MessageBox.Show("数据文件缺少必要的内容...");
                        break;
                    }
                    else
                    {
                        gpx.wpt[index] = (waypointvalue);
                        index++;
                    }
                }

                #endregion

            }


            #region WPG Routes数据

            #endregion

            #region WPG Tarck数据

            if (trkList != null)
            {
                int wptIndex = 0;
                int trkIndex = 0;
                int gpxtrkIndex = 0;

                gpx.trk = new trkType[trkList.Count];

                foreach (System.Xml.XmlNode node in trkList)
                {
                    System.Xml.XmlNodeList childList = node.ChildNodes;
                    trkType trkvalue = new trkType();
                    trkvalue.trkseg = new trksegType[childList.Count];
                    #region 获取track
                    int cnt = 0;
                    for (int i = 0; i < childList.Count; i++)
                    {
                        try
                        {
                            switch (childList[i].Name)
                            {
                                case "name":
                                    trkvalue.name = (childList[i].InnerText);
                                    break;
                                case "cmt":
                                    trkvalue.cmt = (childList[i].InnerText);
                                    break;
                                case "trkseg":
                                    trksegType tracksegment = new trksegType();
                                    tracksegment.trkpt = new wptType[childList[i].ChildNodes.Count];
                                    #region 获得坐标信息

                                    // trkvalue.cmt = (childList[i].InnerText);
                                    foreach (System.Xml.XmlNode node1 in childList[i])
                                    {
                                        System.Xml.XmlNodeList childList1 = node1.ChildNodes;
                                        System.Xml.XmlAttributeCollection chileAtt1 = node1.Attributes;
                                        wptType trackPoint = new wptType();
                                        cnt = 0;
                                        for (int j = 0; j < chileAtt1.Count; j++)
                                        {
                                            #region 获得经纬度
                                            try
                                            {
                                                switch (chileAtt1[j].Name)
                                                {
                                                    case "lat":
                                                        trackPoint.lat = (decimal)double.Parse(chileAtt1[j].Value); cnt++;
                                                        break;
                                                    case "lon":
                                                        trackPoint.lon = (decimal)double.Parse(chileAtt1[j].Value); cnt++;
                                                        break;
                                                    default: break;
                                                }
                                            }
                                            catch
                                            {

                                            }
                                            #endregion
                                            #region 获得其他信息
                                            try
                                            {
                                                switch (childList1[j].Name)
                                                {
                                                    case "ele":
                                                        trackPoint.ele = (decimal)float.Parse(childList1[j].InnerText);
                                                        break;
                                                    case "time":
                                                        trackPoint.time = DateTime.Parse(childList1[j].InnerText);
                                                        break;
                                                    default: break;
                                                }
                                            }
                                            catch
                                            {

                                            }
                                            #endregion



                                        }

                                        if (cnt < 2)
                                        {
                                            MessageBox.Show("该文件缺少必要的数据...");
                                            break;
                                        }
                                        else
                                        {
                                            tracksegment.trkpt[wptIndex] = (trackPoint);
                                            wptIndex++;
                                        }

                                    }
                                    #endregion
                                    trkvalue.trkseg[trkIndex] = tracksegment;
                                    trkIndex++;

                                    break;
                                default: break;
                            }
                        }
                        catch 
                        {
                            MessageBox.Show("gpx文件错误");
                            return null;
                        }
                    }
                    #endregion

                    gpx.trk[gpxtrkIndex] = (trkvalue);
                    gpxtrkIndex++;
                }
            }
            #endregion
            return gpx;
        }
    }
}
