using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using HPAssistEngine.Models;
using HPAssistEngine.HPDAL;

namespace HPAssistEngine.Controllers
{
    [ValidateInput(false)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompareCost()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult MultipleCommand(HttpPostedFileBase fileInputData, HttpPostedFileBase fileInputDataCC, string command, string txtOOPResult, string txtMedicalOnlyResult)
        {
            switch (command)
            {
                case "Upload":
                    return UploadInput(fileInputData);
                case "CompareCost":
                    TempData["DentalOnly"] = txtMedicalOnlyResult;
                    TempData["oopResult"] = txtOOPResult;
                    return CompareCostInput(fileInputDataCC);
                case "nxtPage":
                    return RedirectToAction("CompareCost");
                default:
                    return View();
            }
        }

        private ActionResult UploadInput(HttpPostedFileBase fileInputData)
        {
            try
            {
                StringBuilder strbindTrans = new StringBuilder();
                StringBuilder strbindUtil = new StringBuilder();
                StringBuilder strbindOOP = new StringBuilder();
                StringBuilder strbindTest = new StringBuilder();
                StringBuilder strbindPayroll = new StringBuilder();
                StringBuilder strDentalonly = new StringBuilder();
                StringBuilder strMsg = new StringBuilder();
                int medicalID = 0, dentalID = 0, visionID = 0;
                if (fileInputData != null)
                {
                    string requestBody = new StreamReader(fileInputData.InputStream).ReadToEnd();
                    string errorMessage = "";
                    string appURL = Convert.ToString(ConfigurationManager.AppSettings["HPAssistURL"]);
                    appURL = appURL + "/GenerateCost/";
                    HttpWebRequest request1 = WebRequest.Create(appURL) as HttpWebRequest;
                    request1.ContentType = @"application/xml";
                    request1.Method = @"POST";
                    string userCredentials = Convert.ToString(ConfigurationManager.AppSettings["APIKey"]) + ":" + Convert.ToString(ConfigurationManager.AppSettings["CompanyCode"]) +
                        ":" + Convert.ToString(ConfigurationManager.AppSettings["UserName"]);
                    byte[] base64Credentials = System.Text.Encoding.ASCII.GetBytes(userCredentials);
                    Logging.Info(Convert.ToString(this.GetType()), "GenerateCost Request", userCredentials);
                    string credentials = Convert.ToBase64String(base64Credentials);
                    request1.Headers.Add(HttpRequestHeader.Authorization, "Basic " + credentials);
                    StreamWriter requestWriter = new StreamWriter(request1.GetRequestStream());
                    requestWriter.Write(requestBody);
                    requestWriter.Close();
                    HttpWebResponse httpwebresponse = (HttpWebResponse)request1.GetResponse();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(httpwebresponse.GetResponseStream());
                    XmlNodeList nodelist = doc.SelectNodes("/CostEstimateResponse");
                    XmlElement Status = (XmlElement)doc.SelectSingleNode("/CostEstimateResponse/Status");
                    XmlElement transactionID = (XmlElement)doc.SelectSingleNode("/CostEstimateResponse/TransactionID");
                    //Getting Plan Id's

                    if (Status != null && transactionID != null)
                    {
                        string checkStatus = Convert.ToString(Status.InnerText);
                        if (checkStatus == "Success")
                        {
                    XmlDocument newRequest = new XmlDocument();
                    newRequest.LoadXml(requestBody);

                    if (newRequest.SelectSingleNode("//CostEstimate/PlanIds/MedicalId") != null && newRequest.SelectSingleNode("//CostEstimate/PlanIds/MedicalId").InnerText!="")
                        medicalID = Convert.ToInt32(newRequest.SelectSingleNode("//CostEstimate/PlanIds/MedicalId").InnerText);
                    if (newRequest.SelectSingleNode("//CostEstimate/PlanIds/DentalId") != null && newRequest.SelectSingleNode("//CostEstimate/PlanIds/DentalId").InnerText!="")                        
                        dentalID = Convert.ToInt32(newRequest.SelectSingleNode("//CostEstimate/PlanIds/DentalId").InnerText);
                    if (newRequest.SelectSingleNode("//CostEstimate/PlanIds/VisionId") != null && newRequest.SelectSingleNode("//CostEstimate/PlanIds/VisionId").InnerText!="")
                        visionID = Convert.ToInt32(newRequest.SelectSingleNode("//CostEstimate/PlanIds/VisionId").InnerText);                   

                            string prescriptionDrug, officeVisits, specialistVisits, dentalFillings, correctiveLenses, usprescriptionDrug, usofficeVisits, usspecialistVisits, usdentalFillings, uscorrectiveLenses,
                                HospitalPhysician = "", PrescriptionDrug = "", Dental = "", Vision = "", Low = "", Average = "", High = "", TotalAggregate = "", dvLow = "", dvAverage = "", dvHigh = "", dvTotalAggregate = "", HRAContribution = "", EmpMedicalDeduction = "", EmpDentalDeduction = "", EmpVisionDeduction = "", TotalEmpCost = "",
                                ExHospitalPhysician = "", ExPrescriptionDrug = "", ExDental = "", ExVision = "", ExLow = "", ExAverage = "", ExHigh = "", ExTotalAggregate = "", dvExLow = "", dvExAverage = "", dvExHigh = "", dvExTotalAggregate = "", ExEmpMedicalDeduction = "", ExEmpDentalDeduction = "", ExEmpVisionDeduction = "", ExTotalEmpCost = "", MsgText = "";
                            prescriptionDrug = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/EmpCoverage/PrescriptionDrug").InnerText;
                            officeVisits = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/EmpCoverage/OfficeVisits").InnerText;
                            specialistVisits = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/EmpCoverage/SpecialistVisits").InnerText;
                            dentalFillings = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/EmpCoverage/DentalFillings").InnerText;
                            correctiveLenses = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/EmpCoverage/CorrectiveLenses").InnerText;


                            usprescriptionDrug = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/USAverage/PrescriptionDrug").InnerText;
                            usofficeVisits = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/USAverage/OfficeVisits").InnerText;
                            usspecialistVisits = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/USAverage/SpecialistVisits").InnerText;
                            usdentalFillings = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/USAverage/DentalFillings").InnerText;
                            uscorrectiveLenses = doc.SelectSingleNode("/CostEstimateResponse/UtilizationCoverage/USAverage/CorrectiveLenses").InnerText;
                            if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost") != null)
                            {
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Messages") != null)
                                {
                                    XmlNodeList msgnodelist = doc.SelectNodes("/CostEstimateResponse/EmployeeCost/Messages/Message/MessageText");
                                    foreach (XmlNode node in msgnodelist)
                                    {
                                        MsgText += "<span>*</span>&nbsp" + node.InnerText + "</br>";
                                    }
                                }
                                else
                                {
                                    MsgText = "";
                                }
                                //Medical
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical") != null)
                                {
                                    HospitalPhysician = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/HospitalOrPhysician").InnerText;
                                    PrescriptionDrug = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/PrescriptionDrug").InnerText;
                                    Low = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/RangeLow").InnerText;
                                    High = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/RangeHigh").InnerText;
                                    Average = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/RangeAverage").InnerText;
                                    TotalAggregate = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/TotalEmployeeCost").InnerText;
                                    HRAContribution = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/HRAContributions").InnerText;
                                    EmpMedicalDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Medical/Premium").InnerText;
                                    TotalEmpCost = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Medical/TotalEmployeeCost").InnerText;
                                }
                                //Dental
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Dental") != null)
                                {
                                    Dental = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Dental/OOP").InnerText;
                                    EmpDentalDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Dental/Premium").InnerText;
                                    ExDental = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Dental/OOP").InnerText;
                                    ExEmpDentalDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Dental/Premium").InnerText;
                                }
                                //Vision
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Vision") != null)
                                {
                                    Vision = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Vision/OOP").InnerText;
                                    EmpVisionDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Vision/Premium").InnerText;
                                    ExVision = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Vision/OOP").InnerText;
                                    ExEmpVisionDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Vision/Premium").InnerText;
                                }
                                //For Regular plan(With Dental and Vision)
                                dvLow = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/RangeLow").InnerText;
                                dvHigh = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/RangeHigh").InnerText;
                                dvAverage = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/RangeAverage").InnerText;
                                dvTotalAggregate = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/TotalEmployeeCost").InnerText;

                                //For Exchange Plan(Without Dental and Vision)
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExchangePlan") != null)
                                {
                                    ExHospitalPhysician = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExchangePlan/HospitalOrPhysician").InnerText;
                                    ExPrescriptionDrug = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExchangePlan/PrescriptionDrug").InnerText;
                                    ExLow = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExchangePlan/RangeLow").InnerText;
                                    ExHigh = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExchangePlan/RangeHigh").InnerText;
                                    ExAverage = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExchangePlan/RangeAverage").InnerText;
                                    ExTotalAggregate = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExchangePlan/TotalEmployeeCost").InnerText;



                                    //For Exchange Plan(With Dental and Vision)
                                    dvExLow = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExRangeLow").InnerText;
                                    dvExHigh = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExRangeHigh").InnerText;
                                    dvExAverage = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExRangeAverage").InnerText;
                                    dvExTotalAggregate = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/ExTotalOOP").InnerText;
                                    ExEmpMedicalDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/ExchangePlan/Premium").InnerText;
                                    ExTotalEmpCost = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/ExTotalEmployeeCost").InnerText;
                                }
                            }
                            //Transaction Div
                            strbindTrans.Append("<Table>" +
                                                "<tr style='background-color:#E25744;border:5px solid white'>" +
                                                "<td colspan='2' style='Padding-left:10px;'><font style='color:White'><b>Transaction Details</b></font></td></tr>" +
                                                "<tr style='background-color:#DCDFE5;border:5px solid white'>" +
                                                "<td style='Padding-left:10px;border:5px solid white'><b>TransactionID</b></td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + Convert.ToString(transactionID.InnerText) + "</td>" +
                                                "<tr style='background-color:#EDEFF2;border:5px solid white'>" +
                                                "<td style='Padding-left:10px;border:5px solid white'><b>Status</b></td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + Convert.ToString(Status.InnerText) + "</td></tr></Table>");

                            //Utilization Coverage
                            strbindUtil.Append("<table><tr style='background-color:#E25744;border:5px solid white'><td colspan='3' style='Padding-left:10px;'><font style='color:White'><b>Utilization of Coverage</b></font></td></tr>" +
                                                "<tr><td style='Padding-left:10px;'></td><td style='Padding-left:10px;'><b>You</b></td><td style='Padding-left:10px;'><b>Nation Average</b></td></tr>" +
                                                "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;border:5px solid white'><b>Prescriptions Filled</b></td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + prescriptionDrug + "</td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + usprescriptionDrug + "</td></tr>" +
                                                "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;border:5px solid white'><b>Doctor Office Visits</b></td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + officeVisits + "</td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + usofficeVisits + "</td></tr>" +
                                                "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;border:5px solid white'><b>SpecialistVisits</b></td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + specialistVisits + "</td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + usspecialistVisits + "</td></tr>" +
                                                "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;border:5px solid white'><b>DentalFillings</b></td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + dentalFillings + "</td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + usdentalFillings + "</td></tr>" +
                                                "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;border:5px solid white'><b>CorrectiveLenses</b></td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + correctiveLenses + "</td>" +
                                                "<td style='Padding-left:10px;border:5px solid white'>" + uscorrectiveLenses + "</td></tr></table>");

                            //With Dental and Vision for both Regular and Exchange Plan
                            strbindOOP.Append("<table>" +
                                            "<tr>" +
                                            "<td>" +
                                            "<table style='width:420px'>" +
                                            "<tr style='background-color:#E55D4A;border:5px solid white'>" +
                                            "<tr style='background-color:#73868E;border:5px solid white'><td colspan='3' style='Padding-left:10px'><font style='color:white'>Medical Plan</font></td></tr>" +
                                            "<tr style='background-color:#73868E;border:5px solid white'><td colspan='3' style='Padding-left:10px'><font style='color:white'>Dental Plan</font></td></tr>" +
                                            "<tr style='background-color:#73868E;border:5px solid white'><td colspan='3' style='Padding-left:10px'><font style='color:white'>Vision Plan</font></td></tr>" +
                                            "<tr><td colspan='3' style='Padding-left:10px;'><font style='color:#F65D4A'><b>Out-of-Pocket Expenses</b></font></td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Hospital/Physician</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Prescription Drug</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Dental</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Vision</td></tr>" +
                                             "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Employer HRA/HSA Contributions</b></td></tr>" +
                                            "<tr><td colspan='3' style='Padding-left:10px'><font style='color:#9BAAB1'><b>Estimated Range of Cost</b></font></td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Low</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Average</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>High</td></tr>" +
                                            "</tr>" +
                                            "<tr><td colspan='3' style='Padding-left:10px'><font style='color:#F65D4A'><b>Your Annual Premium Responsibility</b></font></td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Medical</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Dental</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Vision</td></tr>" +
                                // "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Employer HRA/HSA Contributions</b></td></tr>" +
                                            "<tr style='background-color:#7E838B'><td colspan='3' style='Padding-left:10px'><b>Total Employee Costs</b></td></tr>" +
                                            "</table>" +
                                            "</td>" +
                                //OOP Result
                                            "<td>" +
                                            "<table style='margin-left:-20px;margin-top:10px'>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;width:145px'>&nbsp;Plan&nbsp;" + medicalID + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;Plan&nbsp;" + dentalID + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;Plan&nbsp;" + visionID + "</td></tr>" +
                                            "<tr><td style='Padding-left:10px;'><font style='color:White'><b>Test</b></font></td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;width:125px'>&nbsp;$&nbsp;" + HospitalPhysician + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + PrescriptionDrug + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + Dental + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + Vision + "</td></tr>" +
                                             "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + HRAContribution + "</td></tr>" +
                                            "<tr><td><font style='color:White'>The</font></td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'>" +
                                            "<td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvLow + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvAverage + "</td>" +
                                            "<tr style='background-color:#EDEFF2'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvHigh + "</td></tr>" +
                                            "</tr>" +
                                            "<tr><td><font style='color:White'>The</font></td></tr>" +
                                            "<tr  style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + EmpMedicalDeduction + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + EmpDentalDeduction + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + EmpVisionDeduction + "</td></tr>" +
                                            "<tr style='background-color:#7E838B'><td style='Padding-left:10px;'><font style='color:white'><b>&nbsp;$&nbsp;" + dvTotalAggregate + "</b></font></td></tr>" +
                                            "</table>" +
                                            "</td>" +
                                            //Exchange Plan Result
                                            "<td>" +
                                            "<table style='margin-left:-20px;margin-top:10px'>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;width:145px'>Silver Plan</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;Plan&nbsp;" + dentalID + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;Plan&nbsp;" + visionID + "</td></tr>" +
                                            "<tr><td style='Padding-left:10px;'><font style='color:White'><b>Test</b></font></td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;width:125px'>&nbsp;$&nbsp;" + ExHospitalPhysician + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ExPrescriptionDrug + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ExDental + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ExVision + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;N/A&nbsp;</td></tr>" +
                                            "<tr><td><font style='color:White'><b>The</b></font></td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'>" +
                                            "<td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvExLow + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvExAverage + "</td>" +
                                            "<tr style='background-color:#EDEFF2'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvExHigh + "</td></tr>" +
                                            "</tr>" +
                                            "<tr><td><font style='color:White'><b>The</b></font></td></tr>" +
                                            "<tr  style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ExEmpMedicalDeduction + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ExEmpDentalDeduction + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ExEmpVisionDeduction + "</td></tr>" +
                                            "<tr style='background-color:#7E838B'><td style='Padding-left:10px;'><font style='color:white'><b>&nbsp;$&nbsp;" + ExTotalEmpCost + "</b></font></td></tr>" +
                                            "</table>" +
                                            "</td>" +
                                            "</tr></table>");
                            //Medical Result Only
                            strDentalonly.Append("<table>" +
                                            "<tr>" +
                                            "<td>" +
                                            "<table style='width:420px'>" +
                                            "<tr style='background-color:#73868E;border:5px solid white'><td colspan='3' style='Padding-left:10px;width:145px'>Medical Plan</td></tr>" +
                                            "<tr><td colspan='3' style='Padding-left:10px;'><font style='color:#F65D4A'><b>Out-of-Pocket Expenses</b></font></td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Hospital/Physician</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Prescription Drug</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Employer HRA/HSA Contributions</b></td></tr>" +
                                            "<tr><td colspan='3' style='Padding-left:10px'><font style='color:#9BAAB1'><b>Estimated Range of Cost</b></font></td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Low:</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Average:</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>High:</td></tr>" +
                                            "</tr>" +
                                            "<tr><td colspan='3' style='Padding-left:10px'><font style='color:#F65D4A'><b>Your Annual Premium Responsibility</b></font></td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td colspan='3' style='Padding-left:10px'>Medical</td></tr>" +
                                            "<tr style='background-color:#7E838B'><td colspan='3' style='Padding-left:10px'><b>Total Employee Costs</b></td></tr>" +
                                            "</table>" +
                                            "</td>" +
                                //OOP Result For Medical Only
                                            "<td>" +
                                            "<table style='margin-left:-20px;margin-top:10px'>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;;width:145px'>&nbsp;Plan&nbsp;" + medicalID + "</td></tr>" +
                                            "<tr><td style='Padding-left:10px;'><font style='color:White'><b>Test</b></font></td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;width:125px'>&nbsp;$&nbsp;" + HospitalPhysician + "</td></tr>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + PrescriptionDrug + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + HRAContribution + "</td></tr>" +

                                            "<tr>" +
                                                "<td><font style='color:White'>The</font></td></tr>" +
                                                "<tr style='background-color:#EDEFF2;border:5px solid white'>" +
                                                "<td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + Low + "</td></tr>" +
                                                "<tr style='background-color:#DCDFE5'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + Average + "</td>" +
                                                "<tr style='background-color:#EDEFF2'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + High + "</td></tr>" +
                                                "</tr>" +

                                                "<tr><td><font style='color:White'>The</font></td></tr>" +
                                                "<tr  style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + EmpMedicalDeduction + "</td></tr>" +
                                                "<tr style='background-color:#7E838B'><td style='Padding-left:10px;'><font style='color:white'><b>&nbsp;$&nbsp;" + TotalAggregate + "</b></font></td></tr>" +
                                                "</table>" +
                                                "</td>" +
                                //Exchange Result For Medical Only
                                            "<td>" +
                                            "<table style='margin-left:-20px;margin-top:10px'>" +
                                            "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;width:145px'>Silver Plan</td></tr>" +

                                             "<tr><td style='Padding-left:10px;'><font style='color:White'><b>Test</b></font></td></tr>" +
                                             "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;width:125px'>&nbsp;$&nbsp;" + ExHospitalPhysician + "</td></tr>" +
                                             "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ExPrescriptionDrug + "</td></tr>" +
                                             "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;N/A&nbsp;</td></tr>" +

                                          "<tr>" +
                                                "<td><font style='color:White'><b>The</b></font></td></tr>" +
                                                "<tr style='background-color:#EDEFF2;border:5px solid white'>" +
                                                "<td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + ExLow + "</td></tr>" +
                                                "<tr style='background-color:#DCDFE5'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + ExAverage + "</td></tr>" +
                                                "<tr style='background-color:#EDEFF2'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + ExHigh + "</td></tr>" +
                                                "</tr>" +
                                             "<tr><td><font style='color:White'><b>The</b></font></td></tr>" +
                                                "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ExEmpMedicalDeduction + "</td></tr>" +
                                                "<tr style='background-color:#7E838B'><td style='Padding-left:10px'><font style='color:white'><b>&nbsp;$&nbsp;" + ExTotalAggregate + "</b></font></td></tr>" +
                                                "</table>" +
                                                "</td>" +
                                            "</tr></table>");

                            //Medical Only Result
                            string medicalResult = Convert.ToString(strDentalonly);
                            TempData["DentalOnly"] = medicalResult;
                            //Medical + Dental + Vision Result
                            string oopResult = Convert.ToString(strbindOOP);
                            TempData["oopResult"] = oopResult;

                            //Transaction ID Result
                            string tranresult = Convert.ToString(strbindTrans);
                            TempData["tranresult"] = tranresult;
                            //Utilization Result
                            string result = Convert.ToString(strbindUtil);
                            TempData["Result"] = result;
                            strMsg.Append("<span style='color:red'><b>" + MsgText + "</b></span>");
                            string msg = Convert.ToString(strMsg);
                            TempData["msgResult"] = msg;
                        }
                        else
                        {
                            XmlNodeList nodeList = doc.SelectNodes("/CostEstimateResponse/Errors/Error/ErrorMessage");
                            foreach (XmlNode node in nodeList)
                            {
                                errorMessage += "<span>*</span>&nbsp" + node.InnerText + "</br>";
                            }
                            strbindTrans.Append("<Table style='margin-left:5px;margin-top:5px;font-family:Myriad Pro'><tr><th colspan='2'>Utilization Statistics Quote contains error(s)</th><th></th></tr>" +
                                           "<tr><td>Status</th><td>:&nbsp<font color='#F93420'>" + checkStatus + "</font></td></tr> " +
                                           "<tr><td>TransactionID</td><td>:&nbsp" + Convert.ToString(transactionID.InnerText) + "</td></tr>" +
                                           "<tr><td><b>Errors</b></th></tr><tr><td>&nbsp;" + errorMessage + "</td></tr></table> ");
                            string ErrorResult = Convert.ToString(strbindTrans);
                            TempData["ValidateError"] = ErrorResult;
                        }

                    }
                    else
                    {

                        strbindTrans.Append("<Table><tr><th><b>Status</b></th><td>:&nbsp;Failure</td></tr><tr><td colspan='2'> Error occured while processing Utilization Statistics quote request</td></tr></Table>");
                        string errorResult = Convert.ToString(strbindTrans);
                        TempData["ValidateError"] = errorResult;
                    }

                }
                else
                {
                    if (fileInputData == null)
                    {
                        strbindTrans.Append("<table style='margin-left: 5px;'><tr><td><b>Please select a XML File to Generate</b></td></tr></table>");

                    }
                    else
                    {
                        strbindTrans.Append("<table style='margin-left: 5px;'><tr><td><b>File format is incorrect.Please select XML file format</b></td></tr></table>");

                    }
                    string validateResult = Convert.ToString(strbindTrans);
                    TempData["ValidateError"] = validateResult;
                }

            }
            catch (Exception ex)
            {
                string validateResult = Convert.ToString(ex.Message);
                TempData["ValidateError"] = validateResult;
            }
            return RedirectToAction("Index");
        }

        private ActionResult CompareCostInput(HttpPostedFileBase fileInputDataCC)
        {
            TempData["msgResult"] = "";
            TempData["ccmsgResult"] = "";
            StringBuilder ccstrMsg = new StringBuilder();
            StringBuilder strCompareCost = new StringBuilder();
            StringBuilder strCCDentalvisionPlan = new StringBuilder();
            StringBuilder strCCDentalOnly = new StringBuilder();
            StringBuilder strCCmedicalplanonly = new StringBuilder();
            int medicalID = 0, dentalID = 0, visionID = 0;
            try
            {
                if (fileInputDataCC != null)
                {
                    string requestBody = new StreamReader(fileInputDataCC.InputStream).ReadToEnd();
                    string ErrorMessage = "";
                    string appURL = Convert.ToString(ConfigurationManager.AppSettings["HPAssistURL"]);
                    appURL = appURL + "/CompareCost/";
                    HttpWebRequest request1 = WebRequest.Create(appURL) as HttpWebRequest;
                    request1.ContentType = @"application/xml";
                    request1.Method = @"POST";
                    string userCredentials = Convert.ToString(ConfigurationManager.AppSettings["APIKey"]) + ":" + Convert.ToString(ConfigurationManager.AppSettings["CompanyCode"]) +
                        ":" + Convert.ToString(ConfigurationManager.AppSettings["UserName"]);
                    byte[] base64Credentials = System.Text.Encoding.ASCII.GetBytes(userCredentials);

                    XmlDocument newRequest = new XmlDocument();
                    newRequest.LoadXml(requestBody);
                    if (newRequest.SelectSingleNode("//CompareCostRequest/PlanIds/MedicalId") != null)
                        medicalID = Convert.ToInt32(newRequest.SelectSingleNode("//CompareCostRequest/PlanIds/MedicalId").InnerText);
                    if (newRequest.SelectSingleNode("//CompareCostRequest/PlanIds/DentalId") != null)
                        dentalID = Convert.ToInt32(newRequest.SelectSingleNode("//CompareCostRequest/PlanIds/DentalId").InnerText);
                    if (newRequest.SelectSingleNode("//CompareCostRequest/PlanIds/VisionId") != null)
                        visionID = Convert.ToInt32(newRequest.SelectSingleNode("//CompareCostRequest/PlanIds/VisionId").InnerText);                   

                    //Logging.Info(this.GetType().ToString(), "CompareCost Request", userCredentials);
                    string credentials = Convert.ToBase64String(base64Credentials);
                    request1.Headers.Add(HttpRequestHeader.Authorization, "Basic " + credentials);
                    StreamWriter requestWriter = new StreamWriter(request1.GetRequestStream());
                    requestWriter.Write(requestBody);
                    requestWriter.Close();

                    HttpWebResponse httpwebresponse = (HttpWebResponse)request1.GetResponse();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(httpwebresponse.GetResponseStream());
                    XmlNodeList nodelist = doc.SelectNodes("/CostEstimateResponse");
                    XmlElement Status = (XmlElement)doc.SelectSingleNode("/CostEstimateResponse/Status");
                    XmlElement transactionID = (XmlElement)doc.SelectSingleNode("/CostEstimateResponse/TransactionID");
                    if (Status != null && transactionID != null)
                    {
                        string checkStatus = Convert.ToString(Status.InnerText);
                        if (checkStatus == "Success")
                        {
                            string ccHospitalPhysician = "", ccMsgText="",ccPrescriptionDrug = "", ccDental = "", ccVision = "", ccLow = "", ccHigh = "", ccAverage = "", ccTotalAggregate = "", dvccLow = "", dvccHigh = "", dvccAverage = "", dvccTotalAggregate = "", dvccTotalEmpCost = "", ccHRAContribution = "", ccEmpMedicalDeduction = "", ccEmpDentalDeduction = "", ccEmpVisionDeduction = "", ccTotalEmpCost = "";


                            if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost") != null)
                            {
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Messages") != null)
                                {
                                    XmlNodeList msgnodelist = doc.SelectNodes("/CostEstimateResponse/EmployeeCost/Messages/Message/MessageText");
                                    foreach (XmlNode node in msgnodelist)
                                    {
                                        ccMsgText += "<span>*</span>&nbsp" + node.InnerText + "</br>";
                                    }
                                }
                                else
                                {
                                    ccMsgText = "";
                                }
                                //Medical
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical") != null)
                                {
                                    ccHospitalPhysician = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/HospitalOrPhysician").InnerText;
                                    ccPrescriptionDrug = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/PrescriptionDrug").InnerText;
                                    ccLow = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/RangeLow").InnerText;
                                    ccHigh = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/RangeHigh").InnerText;
                                    ccAverage = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/RangeAverage").InnerText;
                                    ccTotalAggregate = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/TotalOOP").InnerText;
                                    ccHRAContribution = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Medical/HRAContributions").InnerText;
                                    ccEmpMedicalDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Medical/Premium").InnerText;
                                    ccTotalEmpCost = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Medical/TotalEmployeeCost").InnerText;
                                }

                                //Dental                                 
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Dental") != null)
                                {
                                    ccDental = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Dental/OOP").InnerText;
                                    ccEmpDentalDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Dental/Premium").InnerText;
                                }
                                if ((XmlElement)doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Vision") != null)
                                {
                                    ccVision = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/Vision/OOP").InnerText;
                                    ccEmpVisionDeduction = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/Vision/Premium").InnerText;
                                }

                                dvccLow = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/RangeLow").InnerText;
                                dvccHigh = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/RangeHigh").InnerText;
                                dvccAverage = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/RangeAverage").InnerText;
                                dvccTotalAggregate = doc.SelectSingleNode("/CostEstimateResponse/EmployeeCost/TotalOOP").InnerText;
                                dvccTotalEmpCost = doc.SelectSingleNode("CostEstimateResponse/EmployeeCost/TotalEmployeeCost").InnerText;
                            }

                            //Medical + Dental + Vision for CompareCost
                            strCompareCost.Append("<td>" +
                                           "<table style='margin-left:-20px;margin-top:10px'>" +
                                           "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;;width:145px'>&nbsp;Plan&nbsp;" + medicalID + "</td></tr>" +
                                           "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;Plan&nbsp;" + dentalID + "</td></tr>" +
                                           "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;Plan&nbsp;" + visionID + "</td></tr>" +
                                           "<tr><td style='Padding-left:10px;'><font style='color:White'><b>Test</b></font></td></tr>" +
                                           "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;width:125px'>&nbsp;$&nbsp;" + ccHospitalPhysician + "</td></tr>" +
                                           "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccPrescriptionDrug + "</td></tr>" +
                                           "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccDental + "</td></tr>" +
                                           "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccVision + "</td></tr>" +
                                            "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccHRAContribution + "</td></tr>" +
                                           "<tr><td><font style='color:White'><b>The</b></font></td></tr>" +
                                           "<tr style='background-color:#EDEFF2;border:5px solid white'>" +
                                           "<td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvccLow + "</td></tr>" +
                                           "<tr style='background-color:#DCDFE5'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvccAverage + "</td>" +
                                           "<tr style='background-color:#EDEFF2'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + dvccHigh + "</td></tr>" +
                                           "</tr>" +
                                           "<tr><td><font style='color:White'><b>The</b></font></td></tr>" +
                                           "<tr  style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccEmpMedicalDeduction + "</td></tr>" +
                                           "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccEmpDentalDeduction + "</td></tr>" +
                                           "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccEmpVisionDeduction + "</td></tr>" +
                                // "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccHRAContribution.InnerText.ToString() + "</td></tr>" +
                                           "<tr style='background-color:#7E838B'><td style='Padding-left:10px;'><font style='color:white'><b>&nbsp;$&nbsp;" + dvccTotalEmpCost + "</b></td></font></tr>" +
                                           "</table>" +
                                           "</td></tr></table>");

                            //Medical For CompareCost
                            strCCDentalOnly.Append("<td>" +
                                         "<table style='margin-left:-20px;margin-top:10px'>" +
                                         "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;;width:145px'>&nbsp;Plan&nbsp;" + medicalID + "</td></tr>" +
                                         "<tr><td style='Padding-left:10px;'><font style='color:White'><b>Test</b></font></td></tr>" +
                                         "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;width:125px'>&nbsp;$&nbsp;" + ccHospitalPhysician + "</td></tr>" +
                                         "<tr style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccPrescriptionDrug + "</td></tr>" +
                                         "<tr style='background-color:#DCDFE5;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccHRAContribution + "</td></tr>" +
                                         "<tr>" +
                                         "<td><font style='color:White'><b>The</b></font></td></tr>" +
                                         "<tr style='background-color:#EDEFF2;border:5px solid white'>" +
                                         "<td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + ccLow + "</td></tr>" +
                                         "<tr style='background-color:#DCDFE5'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + ccAverage + "</td></tr>" +
                                         "<tr style='background-color:#EDEFF2'><td style='Padding-left:10px;border:5px solid white;Padding-left:10px'>&nbsp;$&nbsp;" + ccHigh + "</td></tr>" +
                                         "</tr>" +
                                         "<tr><td><font style='color:White'><b>The</b></font></td></tr>" +
                                         "<tr  style='background-color:#EDEFF2;border:5px solid white'><td style='Padding-left:10px;'>&nbsp;$&nbsp;" + ccEmpMedicalDeduction + "</td></tr>" +
                                         "<tr style='background-color:#7E838B'><td style='Padding-left:10px;'><font style='color:white'><b>&nbsp;$&nbsp;" + ccTotalEmpCost + "</b></td></font></tr>" +
                                         "</table>" +
                                         "</td></tr></table>");

                            //Medical Only Result
                            string CCDentalData = TempData["DentalOnly"] as string;
                            CCDentalData = CCDentalData.Substring(0, CCDentalData.Length - 13);
                            string CCDentalOnlyResult = Convert.ToString(strCCDentalOnly);
                            string CC = CCDentalData + CCDentalOnlyResult;
                            TempData["DentalOnly"] = CC;

                            //Medical + Dental + Vision Result
                            string OOPData = TempData["oopResult"] as string;
                            OOPData = OOPData.Substring(0, OOPData.Length - 13);
                            string CompareCostResult = Convert.ToString(strCompareCost);
                            string con = OOPData + CompareCostResult;
                            TempData["oopResult"] = con;
                          
                            ccstrMsg.Append("<span style='color:red'><b>" + ccMsgText + "</b></span>");
                            string ccmsg = Convert.ToString(ccstrMsg);
                            TempData["ccmsgResult"] = ccmsg;
                        }
                        else
                        {
                            XmlNodeList nodeList = doc.SelectNodes("/CostEstimateResponse/Errors/Error/ErrorMessage");
                            foreach (XmlNode node in nodeList)
                            {
                                ErrorMessage += "<span>*</span>&nbsp" + node.InnerText + "</br>";
                            }
                            strCompareCost.Append("<Table style='margin-left:5px;margin-top:5px;font-family:Myriad Pro'><tr><th colspan='2'>Utilization Statistics Quote contains error(s)</th><th></th></tr>" +
                                           "<tr><td>Status</th><td>:&nbsp<font color='#F93420'>" + checkStatus + "</font></td></tr> " +
                                           "<tr><td>TransactionID</td><td>:&nbsp" + Convert.ToString(transactionID.InnerText) + "</td></tr>" +
                                           "<tr><td><b>Errors</b></th></tr><tr><td>&nbsp;" + ErrorMessage + "</td></tr></table> ");
                            string ErrorResult = Convert.ToString(strCompareCost);
                            TempData["CCValidateError"] = ErrorResult;
                        }
                    }
                    else
                    {
                        strCompareCost.Append("<Table><tr><th><b>Status</b></th><td>:&nbsp;Failure</td></tr><tr><td colspan='2'> Error occured while processing CompareCost request</td></tr></Table>");
                        string ErrorResult = Convert.ToString(strCompareCost);
                        TempData["CCValidateError"] = ErrorResult;
                    }
                }
                else
                {

                    if (fileInputDataCC == null)
                    {
                        strCompareCost.Append("<table style='margin-left: 5px;'><tr><td><b>Please select a XML File to Generate</b></td></tr></table>");
                    }
                    else
                    {
                        strCompareCost.Append("<table style='margin-left: 5px;'><tr><td><b>File format is incorrect.Please select XML file format</b></td></tr></table>");
                    }
                    string validateResult = Convert.ToString(strCompareCost);
                    TempData["CCValidateError"] = validateResult;
                }
            }
            catch (Exception ex)
            {
                string validateResult = Convert.ToString(ex.Message);
                TempData["CCValidateError"] = validateResult;
            }
            return RedirectToAction("CompareCost");
        }

        private string ConvertToBase64(Stream stream)
        {
            Byte[] inArray = new Byte[(int)stream.Length];
            stream.Read(inArray, 0, (int)stream.Length);
            return Convert.ToBase64String(inArray);
        }
    }
}
