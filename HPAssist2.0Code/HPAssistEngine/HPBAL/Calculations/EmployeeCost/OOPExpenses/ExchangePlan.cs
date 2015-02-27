using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HPAssistEngine.Models;
using System.Data;
using HPAssistEngine.HPDAL;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.Common;
using HPAssistEngine.HPBAL.Calculations.EmployeeCost;
using HPAssistEngine.Resources;

namespace HPAssistEngine.HPBAL.Calculations.EmployeeCost.ExchangePlan
{
    public class ExchangePlanMain
    {
        public Response.ExchangePlan GetExchangePlan(CostEstimate objInputXML, CommonEntities inputObj, string dbName, int modelId, ref List<Response.Message> lstMsg)
        { 
            Response.ExchangePlan objExchangePlan = new Response.ExchangePlan();
            //Response.Message objMessages = new Response.Message();
            string inpzipcode = Convert.ToString(objInputXML.Zipcode);
            inpzipcode = inpzipcode.PadLeft(5, '0');
            inpzipcode = inpzipcode.Substring(0, 3);
            int zipCode;
            int.TryParse(inpzipcode, out zipCode);
            string state = ExchangePlanDAL.GetState(zipCode);
            string ratingArea =ExchangePlanDAL.GetRatingArea(objInputXML.Zipcode,zipCode) ;
            string exchangePlanId = ExchangePlanDAL.GetExchangePlanId(state, ratingArea);
            IDataReader dr = ExchangePlanDAL.GetExchangePlanDesign(exchangePlanId);
            PlanDesignMedical objExPlandesign = Utilities.DataReaderMapToList<PlanDesignMedical>(dr);
            Hospital_Physician objHp = new Hospital_Physician();
            decimal[] medical = new decimal[2];
            if (objExPlandesign != null)
            {
                // BOTH HOSPITAL_PHYSICIAN & PRESCRIPTION_DRUGS ARE CALCULATED WITH THE SAME OBJECT
                medical = objHp.CalculateHospital_Physician(objInputXML, inputObj, objExPlandesign);

                objExchangePlan.HospitalOrPhysician = medical[0];
                objExchangePlan.PrescriptionDrug = medical[1];
                objExchangePlan.Premium = Math.Round(CalculateExchangePremium(zipCode, ratingArea, inputObj.EEStatus + inputObj.SpouseStatus + inputObj.ChildStatus, objInputXML.Employee.HouseHoldIncome, objInputXML.Employee.EmpAge, objInputXML.Spouse.SpouseAge, inputObj.ChildStatus, dbName, objInputXML.DeductionBand, modelId),2);
            }
            else
            {
                Response.Message ObjMessages = new Response.Message();
                ObjMessages.MessageText = Constants.ExchangePlanforZipCodeNotExist +" "+ objInputXML.Zipcode;
                lstMsg.Add(ObjMessages);
            }
            return objExchangePlan;
        }

        private decimal CalculateExchangePremium(int zipCode, string ratingArea, int statussum, decimal salary, int empAge, int spouseAge, int childStatus, string dbName, string deductionBand, int modelId)
        {
            string eligibility, state;
            decimal empArea, spouseArea, childArea, annualPremium, maximumPremium, povertLevel, lowestEEContribution;
            state = ExchangePlanDAL.GetState(zipCode);
            lowestEEContribution = ExchangePlanDAL.GetLowestEEContribution(modelId, dbName, deductionBand = deductionBand==null?"":deductionBand);
            eligibility = ExchangePlanDAL.TaxCreditEligibility(salary, state, statussum, lowestEEContribution);
            empArea = ExchangePlanDAL.CalculateAreaFactor(state, empAge);
            spouseArea = ExchangePlanDAL.CalculateAreaFactor(state, spouseAge);
            childArea = childStatus*ExchangePlanDAL.CalculateAreaFactor(state, childStatus);
            annualPremium = (empArea + spouseArea + childArea) * ExchangePlanDAL.CalculateMonthlyPremium(state, ratingArea)*12;
            if (eligibility == Constants.NotEligible)
            {
               return annualPremium;
            }
            else
            {
                povertLevel = ExchangePlanDAL.CalculatePovertLevel(salary, state, statussum);
                maximumPremium = salary * ExchangePlanDAL.CalculatePercentageOfIncome(povertLevel);
                return Math.Min(maximumPremium,annualPremium);
            }
        }
    }
}