#region namespaces
using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using System.Data.OleDb;
using System.Collections;
using HPAssistEngine.Models;
using HPAssistEngine.Resources;
using HPAssistEngine.Common;
using HPAssistEngine.HPBAL;
using HPAssistEngine.HPBAL.Calculations;
using HPAssistEngine.Models.Entities;
using System.Globalization;
#endregion

namespace HPAssistEngine.HPBAL
{
    public class XMLParser
    {

        public static string GetFilepath(string Filename, string FileExtension)
        {
            string fName = Filename;
            string filePath = Convert.ToString(ConfigurationManager.AppSettings["HPRequestFilePath"]);
            string fileFullPath = filePath + "//" + Filename + FileExtension;
            return fileFullPath;
        }

        public static string GenerateValidationResponseXML(string status, int txnID, List<Response.Error> lstError)
        {
            try
            {
                Response.CostEstimateResponse objResp = new Response.CostEstimateResponse();
                objResp.TransactionID = txnID;
                objResp.Status = status;
                objResp.Errors = lstError;
                string response = Utilities.ConvertObjectToXML(objResp);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CreateXMLFileFromRequest(HttpRequestMessage request, string fileName)
        {
            try
            {
                var stream = request.Content.ReadAsStreamAsync().Result;
                stream.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                string fileLocation = GetFilepath(fileName, ".xml");
                doc.Save(fileLocation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetRequestXML(HttpRequestMessage request)
        {
            try
            {
                var stream = request.Content.ReadAsStreamAsync().Result;
                stream.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                return doc.InnerXml;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static CostEstimate GetRequestObject(HttpRequestMessage request)
        {
            try
            {
                string xml = GetRequestXML(request);
                CostEstimate objCostEstimate = new CostEstimate();
                objCostEstimate = Utilities.ConvertXMLtoObject<CostEstimate>(xml);
                return objCostEstimate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static Credentials ParseAuthorizationHeader(string authHeader)
        {
            try
            {
                string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader)).Split(new[] { ':' });
                if (credentials.Length != 3 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]) || string.IsNullOrEmpty(credentials[2]))
                    return null;
                return new Credentials()
                {
                    APIKey = credentials[0],
                    CompanyCode = credentials[1],
                    Username = credentials[2],
                };
            }
            catch
            {
                return null;
            }
        }
    }
}

