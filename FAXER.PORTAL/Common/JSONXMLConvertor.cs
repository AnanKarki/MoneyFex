using FAXER.PORTAL.BankApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace FAXER.PORTAL.Common
{
    public class JSONXMLConvertor
    {

        public string ParseModel(EmergentApiRequestParamModel model)
        {

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            var xmlval = JsonToXML(json, "refnumber");
            return xmlval;
        }
        public string JsonToXML(string model, string key)
        {
            XmlDocument node = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(model, key);
            string xmlmodel = ConvertXMLtoString(node);
            return xmlmodel;
        }

        public string ConvertXMLtoString(XmlDocument xmlDoc)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter tx = new XmlTextWriter(sw))
                {
                    xmlDoc.WriteTo(tx);
                    string strXmlText = sw.ToString();
                    return strXmlText;
                }
            }
        }
        XmlNodeList NodeList;
        public void GetNodeList(XmlNode node, int level)
        {

            var rnode = node.ChildNodes;
            if (level == 0)
            {
                NodeList = node.ChildNodes;
            }
            else
            {
                level--;
                GetNodeList(node.ChildNodes[0], level);

            }

        }

        public string XMLToJson(string xml, string path)
        {

            //var path = "C:/Users/Riddhasoft-003/Desktop/cashpotrequest.xml";
            //XDocument doc = XDocument.Load(path);
            XmlDocument doc = new XmlDocument();

            //doc.Load(path);

            doc.LoadXml(xml);


            string[] paths = path.Split('/');
            string val = "{ ";
            GetNodeList(doc.ChildNodes[1], paths.Length - 1);
            var childs = NodeList;
            //var childs = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes;
            var count = childs.Count;
            int index = 0;
            foreach (XmlNode child in childs)
            {

                if (child.ChildNodes.Count > 1)
                {
                    var gval = "\"" + child.Name + "\":{";
                    var gcount = child.ChildNodes.Count;
                    int gindex = 0;
                    foreach (XmlNode gchild in child.ChildNodes)
                    {

                        var a = gchild.Name;
                        var b = gchild.InnerText;
                        if (gindex < gcount - 1)
                        {
                            gval += "\"" + a + "\":" + "\"" + b + "\",";
                        }
                        else
                        {


                            gval += "\"" + a + "\":" + "\"" + b + "\"";

                        }
                        gindex++;
                    }
                    gval += "}";
                    val += gval;
                }
                else
                {
                    var a = child.Name;
                    var b = child.InnerText;
                    if (index < count - 1)
                    {
                        val += "\"" + a + "\":" + "\"" + b + "\",";
                    }
                    else
                    {


                        val += "\"" + a + "\":" + "\"" + b + "\"";

                    }
                    index++;
                }

            }
            val += "}";
            return val;
        }
    }
}