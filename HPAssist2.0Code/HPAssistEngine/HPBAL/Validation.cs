#region namespaces
using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using HPAssistEngine.Models;
using HPAssistEngine.Common;
using System.Resources;
using System.Data;
using System.Globalization;
using HPAssistEngine.Resources;
using System.IO;
using System.Xml.Linq;
#endregion

namespace HPAssistEngine.HPBAL
{

    public class Validation
    {
        #region VariableDeclarations
        private int nErrors = 0;
        List<Response.Error> lstError = new List<Response.Error>();
        string requestType;
        #endregion

        public string ValidateHPAssistUploadXML(int txnID, string HPAssistReqFileName)
        {
            try
            {
                string HPAssistUploadXMLPath = "";
                string HPAssistUploadXSDPath = "";
                string response = "";
                requestType = "HPAssistUpload";
                HPAssistUploadXMLPath = XMLParser.GetFilepath(HPAssistReqFileName, ".xml");
                HPAssistUploadXSDPath = HttpContext.Current.Server.MapPath("~/Resources/XSDFiles/HPAssistUpload.xsd");
                HPAssistUploadXSDPath = File.Exists(HPAssistUploadXSDPath) ? HPAssistUploadXSDPath : "";
                if (!IsValidXml(HPAssistUploadXMLPath, HPAssistUploadXSDPath))
                    response = XMLParser.GenerateValidationResponseXML("Failure", txnID, lstError);
                else
                    response = XMLParser.GenerateValidationResponseXML("Success", txnID, lstError);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsValidXml(string xmlPath, string xsdPath)
        {
            bool bStatus = false;
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.ValidationType = ValidationType.Schema;
            rs.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings;
            // Event Handler for handling exception & this will be called whenever any mismatch between XML & XSD
            rs.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
            rs.Schemas.Add(null, XmlReader.Create(xsdPath));

            // reading xml
            using (XmlReader xmlValidatingReader = XmlReader.Create(xmlPath, rs))
            {
                while (xmlValidatingReader.Read())
                {
                }
            }

            DoCustomValidations(xmlPath);
            ////Exception if error.
            if (nErrors > 0)
            {
                bStatus = false;
            }
            else
            {
                bStatus = true;
            }//Success

            return bStatus;
        }

        #region DoCustomValidations
        public string ValidateHPAssistCompareCostXML(int txnID, string HPAssistReqFileName)
        {
            try
            {
                string HPAssistCompareCostXMLPath = "";
                string HPAssistCompareCostXSDPath = "";
                string response = "";
                requestType = "HPAssistCompareCost";
                HPAssistCompareCostXMLPath = XMLParser.GetFilepath(HPAssistReqFileName, ".xml");
                HPAssistCompareCostXSDPath = HttpContext.Current.Server.MapPath("~/Resources/XSDFiles/CompareCost.xsd");

                if (!IsValidXml(HPAssistCompareCostXMLPath, HPAssistCompareCostXSDPath))
                    response = XMLParser.GenerateValidationResponseXML("Failure", txnID, lstError);
                else
                    response = XMLParser.GenerateValidationResponseXML("Success", txnID, lstError);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoCustomValidations(string filePath)
        {
            try
            {
                if (requestType == "HPAssistUpload")
                {
                    ValidatePlanIdSelected(filePath);
                    ValidateEmployeePlanExists(filePath);
                    ValidateSpouse(filePath);
                    ValidateDependentsPlanId(filePath);
                    ValidateNoofChild(filePath);
                    ValidateUtilities(filePath);
                    ValidateSpecialConditions(filePath);
                }
                else if (requestType == "HPAssistCompareCost")
                {
                    ValidatePlanIdSelected(filePath, "CompareCost");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ValidatePlanIdSelected(string filePath)
        {
            XmlDocument XDoc = new XmlDocument();
            XDoc.Load(filePath);
            bool PlanIdSelected = false;
            try
            {
                XmlElement MedicalPlanId = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/PlanIds/MedicalId");
                XmlElement DentalPlanId = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/PlanIds/DentalId");
                XmlElement VisionPlanId = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/PlanIds/VisionId");
                XmlElement EmpMedPlanExist = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpMedicalPlanExist");
                XmlElement EmpDentPlanExist = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpDentalPlanExist");
                XmlElement EmpVisPlanExist = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpVisionPlanExist");
                bool medical = false, dental = false, vision = false;
                int check;
                if (MedicalPlanId != null && MedicalPlanId.InnerXml != "")
                    medical = true;
                if (DentalPlanId != null && DentalPlanId.InnerXml != "")
                    dental = true;
                if (VisionPlanId != null && VisionPlanId.InnerXml != "")
                    vision = true;
                PlanIdSelected = medical == true || dental == true || vision == true ? true : false;
                if (PlanIdSelected == false)
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("PlanId");
                    lstError.Add(objErr);
                    nErrors++;
                }
                if (!string.IsNullOrEmpty(EmpMedPlanExist.InnerText))
                {
                    int.TryParse(EmpMedPlanExist.InnerText, out check);
                    if (check == 1 && MedicalPlanId != null && (MedicalPlanId.InnerXml == "" || Convert.ToInt16(MedicalPlanId.InnerXml) == 0))
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("MedicalPlanId");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (!string.IsNullOrEmpty(EmpDentPlanExist.InnerText))
                {
                    int.TryParse(EmpDentPlanExist.InnerText, out check);
                    if (check == 1 && DentalPlanId != null && (DentalPlanId.InnerXml == "" || Convert.ToInt16(DentalPlanId.InnerXml) == 0))
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("DentalPlanId");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (!string.IsNullOrEmpty(EmpVisPlanExist.InnerText))
                {
                    int.TryParse(EmpVisPlanExist.InnerText, out check);
                    if (check == 1 && VisionPlanId != null && (VisionPlanId.InnerXml == "" || Convert.ToInt16(VisionPlanId.InnerXml) == 0))
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("VisionPlanId");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ValidateSpecialConditions(string filePath)
        {
            XmlDocument XDoc = new XmlDocument();
            XDoc.Load(filePath);
            try
            {
                string empDentalPlan = XDoc.SelectSingleNode("/CostEstimate/Employee/EmpDentalPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Employee/EmpDentalPlanExist").InnerText : "";
                string empVisionPlan = XDoc.SelectSingleNode("/CostEstimate/Employee/EmpVisionPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Employee/EmpVisionPlanExist").InnerText : "";
                string empOrtho = XDoc.SelectSingleNode("/CostEstimate/Employee/EmpOrthoDontic") != null ? XDoc.SelectSingleNode("/CostEstimate/Employee/EmpOrthoDontic").InnerText : "";
                string empLenses = XDoc.SelectSingleNode("/CostEstimate/Employee/EmpCorrectivelenses") != null ? XDoc.SelectSingleNode("/CostEstimate/Employee/EmpCorrectivelenses").InnerText : "";

                string spouseDentalPlan = XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseDentalPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseDentalPlanExist").InnerText : "";
                string spouseVisionPlan = XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseVisionPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseVisionPlanExist").InnerText : "";
                string spouseOrtho = XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseOrthoDontic") != null ? XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseOrthoDontic").InnerText : "";
                string spouseLenses = XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseCorrectivelenses") != null ? XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseCorrectivelenses").InnerText : "";

                string childDentalPlan = XDoc.SelectSingleNode("/CostEstimate/Children/ChildDentalPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Children/ChildDentalPlanExist").InnerText : "";
                string childVisionPlan = XDoc.SelectSingleNode("/CostEstimate/Children/ChildVisionPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Children/ChildVisionPlanExist").InnerText : "";
                string childOrtho = XDoc.SelectSingleNode("/CostEstimate/Children/ChildOrthoDontic") != null ? XDoc.SelectSingleNode("/CostEstimate/Children/ChildOrthoDontic").InnerText : "";
                string childLenses = XDoc.SelectSingleNode("/CostEstimate/Children/ChildCorrectivelenses") != null ? XDoc.SelectSingleNode("/CostEstimate/Children/ChildCorrectivelenses").InnerText : "";
                int check;
                if (Convert.ToInt16(!string.IsNullOrEmpty(empOrtho)) >= 1 && empDentalPlan != "1")
                {
                    Int32.TryParse(empOrtho, out check);
                    if (check == 1)
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("EmpOrtho");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (Convert.ToInt16(!string.IsNullOrEmpty(empLenses)) == 1 && empVisionPlan != "1")
                {
                    Int32.TryParse(empLenses, out check);
                    if (check == 1)
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("EmpLenses");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (Convert.ToInt16(!string.IsNullOrEmpty(spouseOrtho)) == 1 && spouseDentalPlan != "1")
                {
                    Int32.TryParse(spouseOrtho, out check);
                    if (check == 1)
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpouseOrtho");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (Convert.ToInt16(!string.IsNullOrEmpty(spouseLenses)) == 1 && spouseVisionPlan != "1")
                {
                    Int32.TryParse(spouseLenses, out check);
                    if (check == 1)
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpouseLenses");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (Convert.ToInt16(!string.IsNullOrEmpty(childOrtho)) >= 1 && childDentalPlan != "1")
                {
                    Int32.TryParse(childOrtho, out check);
                    if (check >= 1)
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("ChildOrtho");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (Convert.ToInt16(!string.IsNullOrEmpty(childLenses)) >= 1 && childVisionPlan != "1")
                {
                    Int32.TryParse(childLenses, out check);
                    if (check >= 1)
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("ChildLenses");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
            }
            catch (Exception ex)
            { throw ex; }

        }

        private void ValidateUtilities(string filePath)
        {
            XmlDocument XDoc = new XmlDocument();
            XDoc.Load(filePath);
            try
            {
                string empMedicalPlan = XDoc.SelectSingleNode("/CostEstimate/Employee/EmpMedicalPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Employee/EmpMedicalPlanExist").InnerText : "";
                string empMedicalUtil = XDoc.SelectSingleNode("/CostEstimate/Employee/EmpMedicalUtil") != null ? XDoc.SelectSingleNode("/CostEstimate/Employee/EmpMedicalUtil").InnerText : "";
                string empDrugsUtil = XDoc.SelectSingleNode("/CostEstimate/Employee/EmpDrugsUtil") != null ? XDoc.SelectSingleNode("/CostEstimate/Employee/EmpDrugsUtil").InnerText : "";

                string spouseMedicalPlan = XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseMedicalPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseMedicalPlanExist").InnerText : "";
                string spouseMedicalUtil = XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseMedicalUtil") != null ? XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseMedicalUtil").InnerText : "";
                string spouseDrugsUtil = XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseDrugsUtil") != null ? XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseDrugsUtil").InnerText : "";

                string childMedicalPlan = XDoc.SelectSingleNode("/CostEstimate/Children/ChildMedicalPlanExist") != null ? XDoc.SelectSingleNode("/CostEstimate/Children/ChildMedicalPlanExist").InnerText : "";
                string childMedicalUtil = XDoc.SelectSingleNode("/CostEstimate/Children/ChildMedicalUtil") != null ? XDoc.SelectSingleNode("/CostEstimate/Children/ChildMedicalUtil").InnerText : "";
                string childDrugsUtil = XDoc.SelectSingleNode("/CostEstimate/Children/ChildDrugsUtil") != null ? XDoc.SelectSingleNode("/CostEstimate/Children/ChildDrugsUtil").InnerText : "";
                int check;
                int.TryParse(empMedicalUtil, out check);
                if (!String.IsNullOrEmpty(empMedicalUtil))
                {

                    if (check >= 1 && empMedicalPlan != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("EmpMedUtilInvalid");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (check == 0 && empMedicalPlan == "1")
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("EmpMedUtilError");
                    lstError.Add(objErr);
                    nErrors++;
                }
                int.TryParse(empDrugsUtil, out check);
                if (!string.IsNullOrEmpty(empDrugsUtil))
                {

                    if (check >= 1 && empMedicalPlan != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("EmpPrescDrugsInvalid");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (check == 0 && empMedicalPlan == "1")
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("EmpPrescDrugsError");
                    lstError.Add(objErr);
                    nErrors++;
                }
                int.TryParse(spouseMedicalUtil, out check);
                if (!string.IsNullOrEmpty(spouseMedicalUtil))
                {

                    if (check >= 1 && spouseMedicalPlan != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpouseMedUtilInvalid");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (check == 0 && spouseMedicalPlan == "1")
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("SpouseMedUtilError");
                    lstError.Add(objErr);
                    nErrors++;
                }
                int.TryParse(spouseDrugsUtil, out check);
                if (!string.IsNullOrEmpty(spouseDrugsUtil))
                {

                    if (check >= 1 && spouseMedicalPlan != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpousePrescDrugsInvalid");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (check == 0 && spouseMedicalPlan == "1")
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("SpousePrescDrugsError");
                    lstError.Add(objErr);
                    nErrors++;
                }
                int.TryParse(childMedicalUtil, out check);
                if (!string.IsNullOrEmpty(childMedicalUtil))
                {

                    if (check >= 1 && childMedicalPlan != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("ChildMedUtilInvalid");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (check == 0 && childMedicalPlan == "1")
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("ChildMedUtilError");
                    lstError.Add(objErr);
                    nErrors++;
                }
                int.TryParse(childDrugsUtil, out check);
                if (!string.IsNullOrEmpty(childDrugsUtil))
                {

                    if (check >= 1 && childMedicalPlan != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("ChildPrescDrugsInvalid");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if (check == 0 && childMedicalPlan == "1")
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("ChildPrescDrugsError");
                    lstError.Add(objErr);
                    nErrors++;
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void ValidateEmployeePlanExists(string filePath)
        {
            XmlDocument XDoc = new XmlDocument();
            XDoc.Load(filePath);
            bool EmployeePlanExists = false;
            try
            {
                XmlElement EmpMedicalPlan = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpMedicalPlanExist");
                XmlElement EmpDentalPlan = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpDentalPlanExist");
                XmlElement EmpVisinPlan = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpVisionPlanExist");
                bool medicalplan = false, dentalplan = false, visionplan = false;
                if (EmpMedicalPlan != null && EmpMedicalPlan.InnerXml == "1")
                    medicalplan = true;
                if (EmpDentalPlan != null && EmpDentalPlan.InnerXml == "1")
                    dentalplan = true;
                if (EmpVisinPlan != null && EmpVisinPlan.InnerXml == "1")
                    visionplan = true;
                EmployeePlanExists = medicalplan == true || dentalplan == true || visionplan == true ? true : false;
                if (EmployeePlanExists == false)
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("PlanExists");
                    lstError.Add(objErr);
                    nErrors++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void ValidateDependentsPlanId(string filePath)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(filePath);
                XmlElement EmpMedical = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpMedicalPlanExist");
                XmlElement EmpDental = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpDentalPlanExist");
                XmlElement EmpVision = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpVisionPlanExist");
                XmlElement SpouseMedical = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseMedicalPlanExist");
                XmlElement SpouseDental = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseDentalPlanExist");
                XmlElement SpouseVision = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseVisionPlanExist");
                XmlElement ChildMedical = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/ChildMedicalPlanExist");
                XmlElement ChildDental = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/ChildDentalPlanExist");
                XmlElement ChildVision = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/ChildVisionPlanExist");

                if ((SpouseMedical != null) && (SpouseMedical.InnerXml == "1"))
                {
                    if (EmpMedical == null || EmpMedical.InnerXml != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpouseMedicalPlanError");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if ((SpouseDental != null) && (SpouseDental.InnerXml == "1"))
                {
                    if (EmpDental == null || EmpDental.InnerXml != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpouseDentalPlanError");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if ((SpouseVision != null) && (SpouseVision.InnerXml == "1"))
                {
                    if (EmpVision == null || EmpVision.InnerXml != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpouseVisionPlanError");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if ((ChildMedical != null) && (ChildMedical.InnerXml == "1"))
                {
                    if (EmpMedical == null || EmpMedical.InnerXml != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("ChildMedicalPlanError");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if ((ChildDental != null) && (ChildDental.InnerXml == "1"))
                {
                    if (EmpDental == null || EmpDental.InnerXml != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("ChildDentalPlanError");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
                if ((ChildVision != null) && (ChildVision.InnerXml == "1"))
                {
                    if (EmpVision == null || EmpVision.InnerXml != "1")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("ChildVisionPlanError");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ValidateSpouse(string filePath)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(filePath);
                XmlElement EmpGender = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Employee/EmpGender");
                XmlElement SpouseMedical = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseMedicalPlanExist");
                XmlElement SpouseDental = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseDentalPlanExist");
                XmlElement SpouseVision = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseVisionPlanExist");
                XmlElement SpouseAge = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseAge");
                XmlElement SpouseGender = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Spouse/SpouseGender");

                bool spousemedical = false, spousedental = false, spousevision = false, SpousePlanExists = false;
                if (SpouseMedical != null && SpouseMedical.InnerXml == "1")
                    spousemedical = true;
                if (SpouseDental != null && SpouseDental.InnerXml == "1")
                    spousedental = true;
                if (SpouseVision != null && SpouseVision.InnerXml == "1")
                    spousevision = true;
                SpousePlanExists = spousemedical == true || spousedental == true || spousevision == true ? true : false;
                if (SpousePlanExists == true)
                {

                    if (SpouseAge == null || SpouseAge.InnerXml == "")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpouseAge");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                    if (SpouseGender == null || SpouseGender.InnerXml == "")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("SpouseGender");
                        lstError.Add(objErr);
                        nErrors++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ValidateNoofChild(string filePath)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                int check, child;
                XDoc.Load(filePath);
                XmlElement ChildMedical = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/ChildMedicalPlanExist");
                XmlElement ChildDental = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/ChildDentalPlanExist");
                XmlElement ChildVision = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/ChildVisionPlanExist");
                XmlElement NoOfChild = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/NoOfChildren");
                XmlElement ChildOrtho = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/ChildOrthoDontic");
                XmlElement ChildLenses = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/Children/ChildCorrectivelenses");
                bool childplan = false, childmedical = false, childdental = false, childvision = false;
                if ((ChildMedical != null && ChildMedical.InnerXml != "0") || (ChildMedical != null && ChildMedical.InnerXml != ""))
                    childmedical = true;
                if ((ChildDental != null && ChildDental.InnerXml != "0") || (ChildDental != null && ChildDental.InnerXml != ""))
                    childdental = true;
                if ((ChildVision != null && ChildVision.InnerXml != "0") || (ChildVision != null && ChildVision.InnerXml != ""))
                    childvision = true;
                childplan = (childmedical == true) || (childdental == true) || (childvision == true) ? true : false;
                if (childplan == false)
                {
                    if (NoOfChild != null && NoOfChild.InnerXml != "")
                    {
                        Response.Error objErr = new Response.Error();
                        objErr = GetErrorcode("NoOfChildren");
                        lstError.Add(objErr);
                        nErrors++;
                    }

                }
                if (ChildOrtho != null && ChildOrtho.InnerXml != "")
                {
                    if (NoOfChild != null && NoOfChild.InnerXml != "")
                    {
                        Int32.TryParse(ChildOrtho.InnerText, out check);
                        Int32.TryParse(NoOfChild.InnerText, out child);
                        if (check > child)
                        {
                            Response.Error objErr = new Response.Error();
                            objErr = GetErrorcode("ChildOrthoTreatmentError");
                            lstError.Add(objErr);
                            nErrors++;
                        }
                    }
                }
                if (ChildLenses != null && ChildLenses.InnerXml != "")
                {
                    if (NoOfChild != null && NoOfChild.InnerXml != "")
                    {
                        Int32.TryParse(ChildLenses.InnerXml, out check);
                        Int32.TryParse(NoOfChild.InnerText, out child);
                        if (check > child)
                        {
                            Response.Error objErr = new Response.Error();
                            objErr = GetErrorcode("ChildCorrectivelensesError");
                            lstError.Add(objErr);
                            nErrors++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ValidatePlanIdSelected(string filePath, string reqtype = null)
        {
            XmlDocument XDoc = new XmlDocument();
            XDoc.Load(filePath);
            bool PlanIdSelected = false;
            try
            {
                XmlElement MedicalPlanId = null, DentalPlanId = null, VisionPlanId = null;
                if (reqtype == "CompareCost")
                {
                    MedicalPlanId = (XmlElement)XDoc.SelectSingleNode("/CompareCostRequest/PlanIds/MedicalId");
                    DentalPlanId = (XmlElement)XDoc.SelectSingleNode("/CompareCostRequest/PlanIds/DentalId");
                    VisionPlanId = (XmlElement)XDoc.SelectSingleNode("/CompareCostRequest/PlanIds/VisionId");
                }
                else
                {
                    MedicalPlanId = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/PlanIds/MedicalId");
                    DentalPlanId = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/PlanIds/DentalId");
                    VisionPlanId = (XmlElement)XDoc.SelectSingleNode("/CostEstimate/PlanIds/VisionId");
                }
                bool medical = false, dental = false, vision = false;

                if (MedicalPlanId != null && MedicalPlanId.InnerXml != "")
                    medical = true;
                if (DentalPlanId != null && DentalPlanId.InnerXml != "")
                    dental = true;
                if (VisionPlanId != null && VisionPlanId.InnerXml != "")
                    vision = true;
                PlanIdSelected = medical == true || dental == true || vision == true ? true : false;
                if (PlanIdSelected == false)
                {
                    Response.Error objErr = new Response.Error();
                    objErr = GetErrorcode("PlanId");
                    lstError.Add(objErr);
                    nErrors++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            try
            {
                nErrors++;
                Response.Error objError = new Response.Error();
                if (requestType == "HPAssistUpload")
                {
                    objError = GetErrorCodesForHPAssistUpload(e.Exception.Message);
                    lstError.Add(objError);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private Response.Error GetErrorCodesForHPAssistUpload(string errMsg)
        {
            try
            {
                if (errMsg.Contains("The 'CustomerId' element is invalid - The value '\n    ' is"))
                {
                    return GetErrorcode("CompanyCode");
                }
                else if (errMsg.Contains("The element 'CostEstimate' has invalid child element 'PlanIds'"))
                {
                    return GetErrorcode("CompanyCode");
                }
                else if (errMsg.Contains("The 'CustomerId' element is invalid"))
                {
                    return GetErrorcode("CompanyIdError");
                }
                else if (errMsg.Contains("The 'MedicalId' element is invalid"))
                {
                    return GetErrorcode("MedicalPlanIdError");
                }
                else if (errMsg.Contains("The 'DentalId' element is invalid"))
                {
                    return GetErrorcode("DentalPlanIdError");
                }
                else if (errMsg.Contains("The 'VisionId' element is invalid"))
                {
                    return GetErrorcode("VisionPlanIdError");
                }
                else if (errMsg.Contains("element is not declared"))
                {
                    return GetErrorcode("InvalidElementName");
                }
                else if (errMsg.Contains("The 'Zipcode' element is invalid - The value '\n  ' is invalid"))
                {
                    return GetErrorcode("Zipcode");
                }
                else if (errMsg.Contains("The 'Zipcode' element is invalid"))
                {
                    return GetErrorcode("ZipcodeError");
                }
                else if (errMsg.Contains("elements expected: 'Zipcode'"))
                {
                    return GetErrorcode("ZipcodeMissing");
                }
                else if (errMsg.Contains("The 'EmpAge' element is invalid - The value '\n    ' is"))
                {
                    return GetErrorcode("EmployeeAge");
                }
                else if (errMsg.Contains("The 'EmpAge' element is invalid"))
                {
                    return GetErrorcode("EmployeeAgeError");
                }
                else if (errMsg.Contains("elements expected: 'EmpAge'"))
                {
                    return GetErrorcode("EmployeeAgeMissing");
                }
                else if (errMsg.Contains("The 'EmpGender' element is invalid - The value '\n    ' is"))
                {
                    return GetErrorcode("EmployeeGender");
                }
                else if (errMsg.Contains("The 'EmpGender' element is invalid"))
                {
                    return GetErrorcode("EmployeeGenderError");
                }
                else if (errMsg.Contains("elements expected: 'EmpGender'"))
                {
                    return GetErrorcode("EmployeeGenderMissing");
                }
                else if (errMsg.Contains("The 'EmpMedicalPlanExist' element is invalid"))
                {
                    return GetErrorcode("EmployeeIsMedicalPlanSelectedError");
                }
                else if (errMsg.Contains("The 'EmpDentalPlanExist' element is invalid"))
                {
                    return GetErrorcode("EmployeeIsDentalPlanSelectedError");
                }
                else if (errMsg.Contains("The 'EmpVisionPlanExist' element is invalid"))
                {
                    return GetErrorcode("EmployeeIsVisionPlanSelectedError");
                }
                else if (errMsg.Contains("The 'EmpOrthoDontic' element is invalid"))
                {
                    return GetErrorcode("EmployeeOrthoTreatmentError");
                }
                else if (errMsg.Contains("The 'EmpCorrectivelenses' element is invalid"))
                {
                    return GetErrorcode("EmployeeCorrectivelensesError");
                }
                else if (errMsg.Contains("The 'EmpMedicalUtil' element is invalid"))
                {
                    return GetErrorcode("EmpMedUtil");
                }
                else if (errMsg.Contains("The 'EmpDrugsUtil' element is invalid"))
                {
                    return GetErrorcode("EmpPrescDrugs");
                }
                else if (errMsg.Contains("The 'HouseHoldIncome' element is invalid"))
                {
                    return GetErrorcode("HouseHoldIncomeError");
                }
                else if (errMsg.Contains("The 'SpouseAge' element is invalid"))
                {
                    return GetErrorcode("SpouseAgeError");
                }
                else if (errMsg.Contains("The 'SpouseGender' element is invalid"))
                {
                    return GetErrorcode("SpouseGenderError");
                }
                else if (errMsg.Contains("The 'SpouseMedicalPlanExist' element is invalid"))
                {
                    return GetErrorcode("SpouseIsMedicalPlanSelectedError");
                }
                else if (errMsg.Contains("The 'SpouseDentalPlanExist' element is invalid"))
                {
                    return GetErrorcode("SpouseIsDentalPlanSelectedError");
                }
                else if (errMsg.Contains("The 'SpouseVisionPlanExist' element is invalid"))
                {
                    return GetErrorcode("SpouseIsVisionPlanSelectedError");
                }
                else if (errMsg.Contains("The 'SpouseOrthoDontic' element is invalid"))
                {
                    return GetErrorcode("SpouseOrthoTreatmentError");
                }
                else if (errMsg.Contains("The 'SpouseCorrectivelenses' element is invalid"))
                {
                    return GetErrorcode("SpouseCorrectivelensesError");
                }
                else if (errMsg.Contains("The 'SpouseMedicalUtil' element is invalid"))
                {
                    return GetErrorcode("SpouseMedUtil");
                }
                else if (errMsg.Contains("The 'SpouseDrugsUtil' element is invalid"))
                {
                    return GetErrorcode("SpousePrescDrugs");
                }
                else if (errMsg.Contains("The 'NoOfChildren' element is invalid"))
                {
                    return GetErrorcode("NumberofChildrenError");
                }
                else if (errMsg.Contains("The 'ChildMedicalPlanExist' element is invalid"))
                {
                    return GetErrorcode("ChildIsMedicalPlanSelectedError");
                }
                else if (errMsg.Contains("The 'ChildDentalPlanExist' element is invalid"))
                {
                    return GetErrorcode("ChildIsDentalPlanSelectedError");
                }
                else if (errMsg.Contains("The 'ChildVisionPlanExist' element is invalid"))
                {
                    return GetErrorcode("ChildIsVisionPlanSelectedError");
                }
                else if (errMsg.Contains("The 'ChildOrthoDontic' element is invalid"))
                {
                    return GetErrorcode("ChildOrthoTreatment");
                }
                else if (errMsg.Contains("The 'ChildCorrectivelenses' element is invalid"))
                {
                    return GetErrorcode("ChildCorrectivelenses");
                }
                else if (errMsg.Contains("The 'ChildMedicalUtil' element is invalid"))
                {
                    return GetErrorcode("ChildMedUtil");
                }
                else if (errMsg.Contains("The 'ChildDrugsUtil' element is invalid"))
                {
                    return GetErrorcode("ChildPrescDrugs");
                }
                else
                {
                    Response.Error objError = new Response.Error();
                    objError.ErrorCode = Constants.ExceptionCaught;
                    objError.ErrorMessage = errMsg;
                    return objError;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Response.Error GetErrorCodesForValidationDataRequest(string errMsg)
        {
            try
            {
                if (errMsg.Contains("element is not declared"))
                {
                    return GetErrorcode("InvalidElementName");
                }
                else if (errMsg.Contains("has invalid child element"))
                {
                    return GetErrorcode("InvalidElement");
                }
                else if (errMsg.Contains("The 'TransactionID' element is invalid"))
                {
                    return GetErrorcode("TransactionID");
                }
                else
                {
                    Response.Error objError = new Response.Error();
                    objError.ErrorCode = Constants.ExceptionCaught;
                    objError.ErrorMessage = errMsg;
                    return objError;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Response.Error GetErrorcode(string errorKey)
        {
            try
            {
                string fileName = HttpContext.Current.Server.MapPath("~/Resources/ErrorCodes.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                XmlElement eleError = (XmlElement)doc.SelectSingleNode("/Errors/Error[@Key='" + errorKey + "']");
                if (eleError != null)
                {
                    Response.Error objError = new Response.Error();
                    objError.ErrorCode = eleError["Code"].InnerText;
                    objError.ErrorMessage = eleError["Message"].InnerText;
                    return objError;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}