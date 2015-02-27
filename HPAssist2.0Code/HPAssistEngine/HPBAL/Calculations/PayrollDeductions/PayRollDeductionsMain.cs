using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HPAssistEngine.HPDAL;
using HPAssistEngine.Models;
using HPAssistEngine.Models.Entities;

namespace HPAssistEngine.HPBAL.Calculations.PayrollDeductions
{
    public class PayRollDeductionsMain
    {
        public decimal[] CalculatePayRollDeductions(CostEstimate objInputXML, CommonEntities inputObj, string dbName,int modelId)
        {
            try
            {
                decimal[] premium = new decimal[4];
                string MedicalTier = GetTierType(inputObj.NewEmpMedStatusCode, inputObj.NewSpouseMedStatusCode, inputObj.NewChildMedStatusCode, inputObj.ChildStatus);
                string DentalTier = GetTierType(inputObj.NewEmpDentalStatusCode, inputObj.NewSpouseDentalStatusCode, inputObj.NewChildDentalStatusCode, inputObj.ChildStatus);
                string VisionTier = GetTierType(inputObj.NewEmpVisionStatusCode, inputObj.NewSpouseVisionStatusCode, inputObj.NewChildVisionStatusCode, inputObj.ChildStatus);
                if (objInputXML.PlanIds.MedicalId != 0)
                {
                    premium[0] = CalculateHRAorHSAContribution(objInputXML.PlanIds.MedicalId, MedicalTier, dbName);
                    premium[1] = CalculateEmployeeMedicaldeduction(objInputXML.PlanIds.MedicalId, MedicalTier, dbName, objInputXML, modelId);
                }
                if (objInputXML.PlanIds.DentalId != 0) 
                   premium[2] = CalculateEmployeeDentaldeduction(objInputXML.PlanIds.DentalId, DentalTier, dbName);
               if (objInputXML.PlanIds.VisionId != 0)
                   premium[3] = CalculateEmployeeVisiondeduction(objInputXML.PlanIds.VisionId, VisionTier, dbName);
                return premium;
            }
            catch (Exception Exception)
            {
                throw Exception;
            }
        }
     
        private decimal CalculateHRAorHSAContribution(int MedicalId, string Tier, string dbName)
        {
            try
            {
                decimal HRAContribution;
                HRAContribution = PayRollDeductionsDAL.CalculateHRAContribution(Tier, dbName, MedicalId);
                return HRAContribution;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private decimal CalculateEmployeeMedicaldeduction(int MedicalId, string Tier, string dbName, CostEstimate objInputXML, int modelId)
        {
            try
            {
                decimal medicaldeduction;
                //Calculating annualy hence multiplied by 12
                medicaldeduction = 12*PayRollDeductionsDAL.CalculateEmpMedicalContribution(MedicalId, Tier, dbName, objInputXML.DeductionBand == null ? "No" : objInputXML.DeductionBand, objInputXML.SpouseSurcharge = objInputXML.SpouseSurcharge == null ? "No" : objInputXML.SpouseSurcharge, objInputXML.WellnessPlan = objInputXML.WellnessPlan == null ? "No" : objInputXML.WellnessPlan, objInputXML.TobaccoUse = objInputXML.TobaccoUse == null ? "No" : objInputXML.TobaccoUse, modelId);
                return medicaldeduction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private decimal CalculateEmployeeDentaldeduction(int DentalId, string Tier, string dbName)
        {
            try
            {
                decimal dentaldeduction;
                //Calculating annualy hence multiplied by 12
                dentaldeduction =  12*PayRollDeductionsDAL.CalculateEmpDentalContribution(DentalId, Tier, dbName);
                return dentaldeduction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private decimal CalculateEmployeeVisiondeduction(int VisionId, string Tier, string dbName)
        {
            try
            {
                decimal Visiondeduction;
                //Calculating annualy hence multiplied by 12
                Visiondeduction = 12 * PayRollDeductionsDAL.CalculateEmpVisionContribution(VisionId, Tier, dbName);
                return Visiondeduction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private string GetTierType(int EmpCode, int SpouseCode, int ChildCode, int ChildStatusCode)
        {
            try
            {
                string Tier;
                if ((ChildCode * ChildStatusCode >= 3) && (SpouseCode == 1) && (EmpCode == 1))
                    Tier = "Tier8";
                else if ((ChildCode * ChildStatusCode == 2) && (SpouseCode == 1) && (EmpCode == 1))
                    Tier = "Tier7";
                else if ((ChildCode * ChildStatusCode == 1) && (SpouseCode == 1) && (EmpCode == 1))
                    Tier = "Tier6";
                else if ((ChildCode * ChildStatusCode >= 3) && (EmpCode == 1))
                    Tier = "Tier5";
                else if ((ChildCode * ChildStatusCode == 2) && (EmpCode == 1))
                    Tier = "Tier4";
                else if ((ChildCode * ChildStatusCode == 1) && (EmpCode == 1))
                    Tier = "Tier3";
                else if ((SpouseCode == 1) && (EmpCode == 1))
                    Tier = "Tier2";
                else
                    Tier = "Tier1";
                return Tier;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}