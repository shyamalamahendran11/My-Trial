using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using HPAssistEngine.Common;
using HPAssistEngine.Models;
using HPAssistEngine.Models.Entities;
using System.Net.Http;
using HPAssistEngine.HPBAL.Calculations.UtilizationCoverage;
using HPAssistEngine.HPBAL.Calculations.OOPExpenses;
using HPAssistEngine.HPBAL.Calculations;
using HPAssistEngine.HPBAL.Calculations.EmployeeCost;
using HPAssistEngine.HPDAL;
using System.Data.Entity;

namespace HPAssistEngine.HPBAL.Calculations
{
    public class CalculationsMain
    {
        public Response.CostEstimateResponse CalculateCostEstimateResponse(HttpRequestMessage request, string dbName , string CompanyCode)
        {
            try
            {
                Response.CostEstimateResponse ObjCostEstimate = new Response.CostEstimateResponse();
                CostEstimate objInputXML = XMLParser.GetRequestObject(request);
                // It contains the Common variable like EEStatus, SpouseStatus,NewChildMedStatusCode
                CommonEntities inputObj = new CommonEntities(objInputXML);//Parameterised Constructor initiated  

                UtilizationMain objUtil = new UtilizationMain();
                ObjCostEstimate.UtilizationCoverage = objUtil.CalculateUtilCoverage(objInputXML, inputObj);

                EmployeeCostMain ObjEmpCost = new EmployeeCostMain();
                ObjCostEstimate.EmployeeCost = ObjEmpCost.CalculateEmployeeCost(objInputXML, inputObj, dbName, CompanyCode);
                return ObjCostEstimate;
            }
            catch (Exception ex)
            { throw ex; }
        }
    }
}