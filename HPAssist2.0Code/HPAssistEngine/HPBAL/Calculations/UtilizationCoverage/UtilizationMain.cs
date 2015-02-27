#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Data;
using HPAssistEngine.Models;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.HPDAL;
using HPAssistEngine.Common;
using HPAssistEngine.HPBAL.Calculations.UtilizationCoverage;
using System.Resources;
using System.Xml;
#endregion


namespace HPAssistEngine.HPBAL.Calculations.UtilizationCoverage
{
    public class UtilizationMain
    {

        public Response.UtilizationCoverage CalculateUtilCoverage(CostEstimate objInputXML, CommonEntities inputObj)
        {
            try
            {
                Response.UtilizationCoverage objUtilStat = new Response.UtilizationCoverage();
                Response.USAverage ObjUsAvg = new Response.USAverage();
                Response.EmpCoverage ObjEmpCoverage = new Response.EmpCoverage();

                // It contains the Common variable like EEStatus, SpouseStatus,NewChildMedStatusCode
                //CommonEntities inputObj = new CommonEntities(objInputXML);//Parameterised Constructor initiated               
                decimal[] arrOutput = new decimal[2];
                int MedicalID = objInputXML.PlanIds.MedicalId;
                int DentalID = objInputXML.PlanIds.DentalId;
                int VisionID = objInputXML.PlanIds.VisionId;
                if (MedicalID != 0)
                {
                    PrescriptionDrugs objPd = new UtilizationCoverage.PrescriptionDrugs();
                    arrOutput = objPd.CalculatePrescriptionDrugs(objInputXML, inputObj);
                    ObjEmpCoverage.PrescriptionDrug = arrOutput[0];
                    ObjUsAvg.PrescriptionDrug = arrOutput[1];

                    OfficeVisits objOv = new UtilizationCoverage.OfficeVisits();
                    arrOutput = objOv.CalculateOfficeVisits(objInputXML, inputObj);
                    ObjEmpCoverage.OfficeVisits = arrOutput[0];
                    ObjUsAvg.OfficeVisits = arrOutput[1];

                    SpecialistsVisit objSv = new UtilizationCoverage.SpecialistsVisit();
                    arrOutput = objSv.CalculateSpecialistsVisit(objInputXML, inputObj);
                    ObjEmpCoverage.SpecialistVisits = arrOutput[0];
                    ObjUsAvg.SpecialistVisits = arrOutput[1];
                }
                if (DentalID != 0)
                {
                    DentalFillings objDf = new UtilizationCoverage.DentalFillings();
                    arrOutput = objDf.CalculateDentalFillings(objInputXML, inputObj);
                    ObjEmpCoverage.DentalFillings = arrOutput[0];
                    ObjUsAvg.DentalFillings = arrOutput[1];
                }
                if (VisionID != 0)
                {
                    CorrectiveLenses objCl = new UtilizationCoverage.CorrectiveLenses();
                    arrOutput = objCl.CalculateCorrectiveLenses(objInputXML, inputObj);
                    ObjEmpCoverage.CorrectiveLenses = arrOutput[0];
                    ObjUsAvg.CorrectiveLenses = arrOutput[1];
                }
                objUtilStat.EmpCoverage = ObjEmpCoverage;
                objUtilStat.USAverage = ObjUsAvg;

                return objUtilStat;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
