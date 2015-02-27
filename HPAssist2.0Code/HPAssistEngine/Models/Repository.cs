#region namespaces
using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Resources;
using System.Configuration;
using System.Runtime.Serialization;
using System.Data.Objects.DataClasses;
using HPAssistEngine.Resources;
using HPAssistEngine.HPBAL;
using HPAssistEngine.HPBAL.Calculations.UtilizationCoverage;
using HPAssistEngine.HPDAL;
using HPAssistEngine.Common;
using HPAssistEngine.HPBAL.Calculations;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.HPBAL.Calculations.OOPExpenses;
using HPAssistEngine.HPBAL.Calculations.EmployeeCost;
#endregion

namespace HPAssistEngine.Models
{
    public class Repository
    {
        public static string ProcessHPAssistUploadRequest(HttpRequestMessage request)
        {
            int txnID = 0;
            // Response.CostEstimateResponse ObjCostEstimate = new Response.CostEstimateResponse();
            string response = "";
            
                //Get the APIKey, Company code from Request Header
                AuthenticationHeaderValue authValue = request.Headers.Authorization;
                Credentials parsedCredentials = XMLParser.ParseAuthorizationHeader(authValue.Parameter);
                if (parsedCredentials != null)
                {
                    //Inserting into the Master Service Transaction table.
                    txnID = InsertIntoMasterTransaction(parsedCredentials.Username, parsedCredentials.CompanyCode);
                    Logging.Info("HPAssistEngine.Repository.ProcessHPAssistUploadRequest", "InsertIntoMasterTransaction", "UserData Successfully inserted into DB");

                }
                string fileName = Resources.Constants.HPAssistRequestFileName + "_" + txnID;
                XMLParser.CreateXMLFileFromRequest(request, fileName);

                //Get the Input Request XML
                string inputXML = XMLParser.GetRequestXML(request);
                string dbName = CommonDAL.GetClientDataBase(parsedCredentials.APIKey);
                try
                {    
            //Inserting Request into the Client Service Transaction table.
                InsertRequestResponseXmlIntoDB(txnID, dbName, inputXML);
                Logging.Info("HPAssistEngine.Repository.ProcessHPAssistUploadRequest", "InsertIntoClientTransaction", "Request XML Successfully inserted into DB");

                // Validation of the XML file.
                Logging.Info("HPAssistEngine.Validation", "ValidateInputXML", "ValidateInput Request Process Started");
                Validation objValidate = new Validation();
                response = objValidate.ValidateHPAssistUploadXML(txnID, fileName);
                Logging.Info("HPAssistEngine.Validation", "ValidateInputXML", "ValidateInput Request Process Completed");


                if (response.Contains("Success"))
                {
                    Response.CostEstimateResponse ObjCostEstimate = new Response.CostEstimateResponse();
                    CalculationsMain ObjMain = new CalculationsMain();
                    ObjCostEstimate = ObjMain.CalculateCostEstimateResponse(request, dbName, parsedCredentials.CompanyCode);
                    ObjCostEstimate.TransactionID = txnID;
                    ObjCostEstimate.Status = "Success";
                    ObjCostEstimate.Errors = null;
                    string calcResponse = Utilities.ConvertObjectToXML(ObjCostEstimate);
                    response = calcResponse;
                }

                DeleteFile("HPAssistUploadRequest", txnID);
                //Updating Response into the Client Service Transaction table.
                InsertRequestResponseXmlIntoDB(txnID, dbName, response: response);
                Logging.Info("HPAssistEngine.Repository.ProcessHPAssistUploadRequest", "InsertResponseXMLntoDB", "Response XML Successfully inserted into DB");
                return response;
            }
            catch (Exception ex)
            {
                List<Response.Error> lstError = new List<Response.Error>();
                Response.Error objErr = new Response.Error();
                objErr.ErrorCode = Resources.Constants.ExceptionCaught;
                objErr.ErrorMessage = ex.Message;
                lstError.Add(objErr);
                response = XMLParser.GenerateValidationResponseXML("Failure", txnID, lstError);
                InsertRequestResponseXmlIntoDB(txnID, dbName, response: response);
                DeleteFile("HPAssistUploadRequest", txnID);
                throw new Exception(response);
            }
        }
       
        private static void DeleteFile(string reqType, int transID)
        {
            try
            {
                reqType = reqType.ToUpper();
                string fileName;
                string path = Convert.ToString(ConfigurationManager.AppSettings["HPRequestFilePath"]);
                if (Directory.Exists(path))
                {
                    if (reqType == "HPASSISTUPLOADREQUEST")
                    {
                        fileName = Resources.Constants.HPAssistRequestFileName + "_" + transID + ".xml";
                        File.Delete(Path.Combine(path, fileName));
                    }
                    else if (reqType == "HPASSISTCOMPARECOSTREQUEST")
                    {
                        fileName = Resources.Constants.HPAssistCompareCostRequestFile + "_" + transID + ".xml";
                        File.Delete(Path.Combine(path, fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        private static int InsertIntoMasterTransaction(string user, string companycode)
        {
            return CommonDAL.InsertIntoMasterTransaction(user, companycode);
        }
        
        private static void InsertRequestResponseXmlIntoDB(int transid, string dbName, string request = null, string response = null)
        {
            CommonDAL.InsertRequestResponseXmlIntoDB(transid, request, response, dbName);
        }
        
        public static string ProcessCompareCostRequest(HttpRequestMessage request)
        {
            int txnID = 0;
            int  reqTransID = 0;
            Response.CostEstimateResponse ObjCostEstimate = new Response.CostEstimateResponse();
            string response = "";
            string  medicalID = "0", dentalID = "0", visionID = "0";

            try
            {
                //Get the APIKey, Company code from Request Header
                AuthenticationHeaderValue authValue = request.Headers.Authorization;
                Credentials parsedCredentials = XMLParser.ParseAuthorizationHeader(authValue.Parameter);

                string inputXML = XMLParser.GetRequestXML(request);

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(inputXML);

                reqTransID = Convert.ToInt32(xdoc.SelectSingleNode("//CompareCostRequest/TransactionID").InnerText);


                string fileName = Resources.Constants.HPAssistCompareCostRequestFile + "_" + reqTransID;
                XMLParser.CreateXMLFileFromRequest(request, fileName);
                string dbName = CommonDAL.GetClientDataBase(parsedCredentials.APIKey);
                //Inserting Request into the Client Service Transaction table.
                txnID = CommonDAL.InsertCompareCostRequestResponseXmlIntoDB(reqTransID, inputXML, "", dbName, "CompareCostRequest");
                Logging.Info("HPAssistEngine.Repository.ProcessHPAssistCompareCostRequest", "InsertIntoClientTransaction", "Request XML Successfully inserted into DB");


                // Validation of the XML file.
                Logging.Info("HPAssistEngine.Validation", "ValidateCompareCostInputXML", "Validate CompareCost Input Request Process Started");
                Validation objValidate = new Validation();
                response = objValidate.ValidateHPAssistCompareCostXML(reqTransID, fileName);
                Logging.Info("HPAssistEngine.Validation", "ValidateInputXML", "Validate CompareCost Input Request Process Completed");

                if (response.Contains("Success"))
                {
                    string genCostRequestXml = OOPDAL.GetRequestXMLFromDB(reqTransID, dbName);
                    XmlDocument newXdoc = new XmlDocument();
                    if (genCostRequestXml == "")
                    {
                        throw new Exception(Resources.Constants.msgTransIDnotPresent);
                    }
                    if (genCostRequestXml != "")
                    {
                        if (xdoc.SelectSingleNode("//CompareCostRequest/PlanIds/MedicalId") != null)
                            medicalID = xdoc.SelectSingleNode("//CompareCostRequest/PlanIds/MedicalId").InnerText;
                        if (xdoc.SelectSingleNode("//CompareCostRequest/PlanIds/DentalId") != null)
                            dentalID = xdoc.SelectSingleNode("//CompareCostRequest/PlanIds/DentalId").InnerText;
                        if (xdoc.SelectSingleNode("//CompareCostRequest/PlanIds/VisionId") != null)
                            visionID = xdoc.SelectSingleNode("//CompareCostRequest/PlanIds/VisionId").InnerText;

                        newXdoc.LoadXml(genCostRequestXml);
                        XmlNode PlanIds = newXdoc.SelectSingleNode("//CostEstimate/PlanIds");
                        PlanIds.RemoveAll(); //Remove all the childnodes under planids
                        XmlNode medNode = newXdoc.CreateNode(XmlNodeType.Element, "MedicalId", null);
                        medNode.InnerText = medicalID;
                        XmlNode dentNode = newXdoc.CreateNode(XmlNodeType.Element, "DentalId", null);
                        dentNode.InnerText = dentalID;
                        XmlNode visNode = newXdoc.CreateNode(XmlNodeType.Element, "VisionId", null);
                        visNode.InnerText = visionID;
                        PlanIds.AppendChild(medNode); //Add the childnodes
                        PlanIds.AppendChild(dentNode);
                        PlanIds.AppendChild(visNode);


                        string xdocresult = newXdoc.InnerXml;
                        CostEstimate objCostEstimate = new CostEstimate();
                        //object Obj = Utilities.ConvertXMLtoObject(xdocresult, objCostEstimate);

                        //objCostEstimate = (CostEstimate)Obj;
                        objCostEstimate = Utilities.ConvertXMLtoObject<CostEstimate>(xdocresult);
                        CommonEntities inputObj = new CommonEntities(objCostEstimate);
                        EmployeeCostMain ObjEmpCost = new EmployeeCostMain();
                        ObjCostEstimate.EmployeeCost = ObjEmpCost.CalculateEmployeeCost(objCostEstimate, inputObj, dbName, parsedCredentials.CompanyCode, Constants.Compare);

                        ObjCostEstimate.TransactionID = reqTransID;
                        ObjCostEstimate.Status = "Success";
                        ObjCostEstimate.Errors = null;
                        string calcResponse = Utilities.ConvertObjectToXML(ObjCostEstimate);
                        response = calcResponse;
                    }
                   
                }

                DeleteFile("HPAssistCompareCostRequest", reqTransID);
                CommonDAL.InsertCompareCostRequestResponseXmlIntoDB(txnID, "", response, dbName, "CompareCostResponse");
                //Updating Response into the Client Service Transaction table.
                Logging.Info("HPAssistEngine.Repository.ProcessHPAssistCompareCostResponse", "InsertResponseXMLntoDB", "Response XML Successfully inserted into DB");

                return response;
            }
            catch (Exception ex)
            {
                List<Response.Error> lstError = new List<Response.Error>();
                Response.Error objErr = new Response.Error();
                objErr.ErrorCode = Resources.Constants.ExceptionCaught;
                objErr.ErrorMessage = ex.Message;
                lstError.Add(objErr);
                response = XMLParser.GenerateValidationResponseXML("Failure", reqTransID, lstError);
                DeleteFile("HPAssistCompareCostRequest", reqTransID);
                throw new Exception(response.ToString());
            }
        }
    }
}



