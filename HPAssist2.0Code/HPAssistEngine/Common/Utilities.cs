#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions; 
#endregion

namespace HPAssistEngine.Common
{
    public class Utilities
    {
        #region Methods
        public static string ConvertListToXmlString<T>(List<T> lstToConvert)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            serializer.Serialize(xmlWriter, lstToConvert);
            string xmlResult = Convert.ToString(stringWriter);
            return xmlResult;
        }

        public static string ConvertObjectToXML(Object objToConvert)
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(objToConvert.GetType());
            serializer.Serialize(stringWriter, objToConvert);
            return Convert.ToString(stringWriter);
        }

        public static T ConvertXMLtoObject<T>(string xml)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                string cleanXml = Regex.Replace(xml, @"<[a-zA-Z].[^(><.)]+/>",
                                                new MatchEvaluator(RemoveText));

                cleanXml = Regex.Replace(cleanXml, @"<[a-zA-Z]>.*?</[a-zA-Z]>",
                                                new MatchEvaluator(RemoveText));
                MemoryStream memoryStream = new MemoryStream((new System.Text.UTF8Encoding()).GetBytes(cleanXml));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, System.Text.Encoding.UTF8);
                return (T)xs.Deserialize(memoryStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static string RemoveText(Match m)
        {
            return "";
        }

        public static T DataReaderMapToList<T>(IDataReader dr)
        {
            try
            {
                T obj = default(T);
                while (dr.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (!object.Equals(dr[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[prop.Name], null);
                        }
                    }
                    break;
                }
                return obj;
            }
            catch
            {
                throw;
            }
        }
        #endregion

    }
}