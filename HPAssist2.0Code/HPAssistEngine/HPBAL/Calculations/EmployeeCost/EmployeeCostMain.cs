using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HPAssistEngine.Models;
using HPAssistEngine.Models.Entities;
using System.Data;
using HPAssistEngine.HPDAL;
using HPAssistEngine.Resources;
using HPAssistEngine.Common;
using HPAssistEngine.HPBAL.Calculations.EmployeeCost.ExchangePlan;
using HPAssistEngine.HPBAL.Calculations.PayrollDeductions;

namespace HPAssistEngine.HPBAL.Calculations.EmployeeCost
{
    public class EmployeeCostMain
    {
        public Response.EmployeeCost CalculateEmployeeCost(CostEstimate objInputXML, CommonEntities inputObj, string dbName, string CompanyCode, string Mode = Constants.Regular)
        {
            Response.EmployeeCost ObjEmployeeCost = new Response.EmployeeCost();
            Response.Medical ObjMedical = new Response.Medical();
            Response.Dental ObjDental = new Response.Dental();
            Response.Vision ObjVision = new Response.Vision();
            List<Response.Message> lstMsg = new List<Response.Message>();
            bool medical=false , dental=false, vision=false;
            medical = (objInputXML.PlanIds.MedicalId != 0 && inputObj.NewEmpMedStatusCode == 1);
            dental = (objInputXML.PlanIds.DentalId != 0 && inputObj.NewEmpDentalStatusCode == 1);
            vision = (objInputXML.PlanIds.VisionId != 0 && inputObj.NewEmpVisionStatusCode == 1);
            try
            {
                int modelId;
                decimal[] Premium = new decimal[4];
                modelId = ExchangePlanDAL.GetModelId(dbName, CompanyCode);
                IDataReader dr;
                if (medical)
                {
                    dr = CommonDAL.GetMedicalPlanDesign(objInputXML.PlanIds.MedicalId, dbName);
                    PlanDesignMedical objPlanMedical = Utilities.DataReaderMapToList<PlanDesignMedical>(dr);
                    if (objPlanMedical != null)
                    {
                        ObjMedical = CalculateMedicalExpenses(objInputXML, inputObj, dbName, objPlanMedical);

                    }
                    else
                    {
                        medical = false;
                        Response.Message ObjMessages = new Response.Message();
                        ObjMessages.MessageText = Constants.MedicalIdNotAvail;
                        lstMsg.Add(ObjMessages);
                    }
                }
                else
                {
                    Response.Message ObjMessages = new Response.Message();
                    ObjMessages.MessageText = Constants.MedicalIdNotSel;
                    lstMsg.Add(ObjMessages);
                }
                if (dental)
                {
                    dr = CommonDAL.GetDentalPlanDesign(objInputXML.PlanIds.DentalId, dbName);
                    PlanDesignDental ObjPlanDental = Utilities.DataReaderMapToList<PlanDesignDental>(dr);
                    if (ObjPlanDental != null)
                    {
                        ObjDental = CalculateDentalExpenses(objInputXML, inputObj, dbName, ObjPlanDental);
                    }
                    else
                    {
                        dental = false;
                        Response.Message ObjMessages = new Response.Message();
                        ObjMessages.MessageText = Constants.DentalIdNotAvail;
                        lstMsg.Add(ObjMessages);
                    }
                }
                else {
                    Response.Message ObjMessages = new Response.Message();
                    ObjMessages.MessageText = Constants.DentalIdNotSel;
                    lstMsg.Add(ObjMessages);
                }
                if (vision)
                {
                    dr = CommonDAL.GetVisionPlanDesign(objInputXML.PlanIds.VisionId, dbName);
                    PlanDesignVision ObjPlanVision = Utilities.DataReaderMapToList<PlanDesignVision>(dr);
                    if (ObjPlanVision != null)
                    {
                        ObjVision = CalculateVisionExpenses(objInputXML, inputObj, dbName, ObjPlanVision);
                    }
                    else
                    {
                        vision = false;
                        Response.Message ObjMessages = new Response.Message();
                        ObjMessages.MessageText = Constants.VisionIdNotAvail;
                        lstMsg.Add(ObjMessages);
                    }
                }
                else
                {
                    Response.Message ObjMessages = new Response.Message();
                    ObjMessages.MessageText = Constants.VisionIdNotSel;
                    lstMsg.Add(ObjMessages);
                }
                PayRollDeductionsMain ObjPayroll = new PayRollDeductionsMain();
                Premium = ObjPayroll.CalculatePayRollDeductions(objInputXML, inputObj, dbName, modelId);
                if (medical)
                {
                    ObjMedical.HRAContributions = Math.Round(Premium[0],2);
                    ObjMedical.Premium = Math.Round(Premium[1],2);
                }
                //make everything as 0 if there is no medical
                else
                {
                    ObjMedical.HospitalOrPhysician = 0;
                    ObjMedical.PrescriptionDrug = 0;
                    ObjMedical.HRAContributions = 0;
                    ObjMedical.Premium = 0;
                }
                if (dental)
                    ObjDental.Premium = Math.Round(Premium[2],2);
                else //no dental plan
                    ObjDental.Premium = 0;
                if (vision)
                    ObjVision.Premium = Math.Round(Premium[3],2);
                else
                    ObjVision.Premium = 0;

                //Assign to EmployeeCost
                ObjEmployeeCost.Medical = ObjMedical;
                ObjEmployeeCost.Dental = ObjDental;
                ObjEmployeeCost.Vision = ObjVision;

                decimal[] Range = new decimal[3];
                Range = CalculateConfidenceInterval(ref ObjEmployeeCost);
                ObjEmployeeCost.Medical.RangeLow = Range[0];
                ObjEmployeeCost.Medical.RangeHigh = Range[1];
                ObjEmployeeCost.Medical.RangeAverage = Range[2];
                ObjEmployeeCost.Medical.TotalEmployeeCost = ObjEmployeeCost.Medical.TotalOOP + ObjEmployeeCost.Medical.Premium;
                bool ExchangeExist = ExchangePlanDAL.CheckIfExchangePlanExist(dbName, CompanyCode);
                if (Mode == Constants.Regular)
                {
                   
                    if (ExchangeExist)
                    {
                        ObjEmployeeCost.ExchangePlan = CalculateExchangePlan(objInputXML, inputObj, dbName, modelId, ref lstMsg);
                    
                    Range = CalculateExPlanConfidenceInterval(ref ObjEmployeeCost);
                    ObjEmployeeCost.ExchangePlan.RangeLow = Range[0];
                    ObjEmployeeCost.ExchangePlan.RangeHigh = Range[1];
                    ObjEmployeeCost.ExchangePlan.RangeAverage = Range[2];
                    ObjEmployeeCost.ExchangePlan.TotalEmployeeCost = ObjEmployeeCost.ExchangePlan.TotalOOP + ObjEmployeeCost.ExchangePlan.Premium;
                    }
                    else
                    {
                        Response.Message ObjMessages = new Response.Message();
                        ObjMessages.MessageText = Constants.ExchangePlanforModelNotExist + " "+CompanyCode;
                        lstMsg.Add(ObjMessages);
                    }
                }
                ObjEmployeeCost.Messages = lstMsg.Count > 0 ? lstMsg : null;

                //With Dental and Vision
                Range = CalculateTotalConfidenceInterval(ref ObjEmployeeCost);
                ObjEmployeeCost.RangeLow = Range[0];
                ObjEmployeeCost.RangeHigh = Range[1];
                ObjEmployeeCost.RangeAverage = Range[2];
                ObjEmployeeCost.TotalEmployeeCost = ObjEmployeeCost.TotalOOP  + (ObjEmployeeCost.Medical != null ? ObjEmployeeCost.Medical.Premium : 0) + (ObjEmployeeCost.Dental != null ? ObjEmployeeCost.Dental.Premium : 0) + (ObjEmployeeCost.Vision != null ? ObjEmployeeCost.Vision.Premium : 0);

                if (Mode == Constants.Regular)
                {
                    if (ExchangeExist)
                    {
                        Range = CalculateExTotalConfidenceInterval(ref ObjEmployeeCost);
                        ObjEmployeeCost.ExRangeLow = Range[0];
                        ObjEmployeeCost.ExRangeHigh = Range[1];
                        ObjEmployeeCost.ExRangeAverage = Range[2];
                        ObjEmployeeCost.ExTotalEmployeeCost = ObjEmployeeCost.ExTotalOOP  + ObjEmployeeCost.ExchangePlan.Premium + (ObjEmployeeCost.Dental != null ? ObjEmployeeCost.Dental.Premium : 0) + (ObjEmployeeCost.Vision != null ? ObjEmployeeCost.Vision.Premium : 0);
                    }
                }
                return ObjEmployeeCost;
            }
            catch (Exception ex)
            { throw ex; }
        }
       
        private Response.Medical CalculateMedicalExpenses(CostEstimate objInputXML, CommonEntities inputObj, string dbName, PlanDesignMedical objPlanMedical)
        {
            Response.Medical ObjMedical = new Response.Medical();
            Hospital_Physician ObjHp = new Hospital_Physician();
            decimal[] Medical = new decimal[2];

            //BOTH HOSPITAL_PHYSICIAN & PRESCRIPTION_DRUGS ARE CALCULATED WITH THE SAME OBJECT
            Medical = ObjHp.CalculateHospital_Physician(objInputXML, inputObj, objPlanMedical);

            ObjMedical.HospitalOrPhysician = Medical[0];
            ObjMedical.PrescriptionDrug = Medical[1];
            return ObjMedical;
        }
        
        private Response.Dental CalculateDentalExpenses(CostEstimate objInputXML, CommonEntities inputObj, string dbName, PlanDesignDental ObjPlanDental)
        {
            Response.Dental ObjDental = new Response.Dental();
            Dental objDental = new Dental();
            ObjDental.OOP = objDental.CalculateDentalCost(objInputXML, ObjPlanDental, inputObj);
            return ObjDental;
        }
        
        private Response.Vision CalculateVisionExpenses(CostEstimate objInputXML, CommonEntities inputObj, string dbName, PlanDesignVision ObjPlanVision)
        {
            Response.Vision ObjVision = new Response.Vision();
            Vision objVision = new Vision();
            ObjVision.OOP = objVision.CalculateVisionCost(objInputXML, ObjPlanVision, inputObj.NewEmpVisionStatusCode, inputObj.NewSpouseVisionStatusCode, inputObj.NewChildVisionStatusCode);
            return ObjVision;
        }
        
        private Response.ExchangePlan CalculateExchangePlan(CostEstimate objInputXML, CommonEntities inputObj, string dbName, int ModelId, ref List<Response.Message> lstMsg)
        {
            Response.ExchangePlan objExchangePlan = new Response.ExchangePlan();
            ExchangePlanMain ObjExchangePlan = new ExchangePlanMain();
            objExchangePlan = ObjExchangePlan.GetExchangePlan(objInputXML, inputObj, dbName, ModelId, ref  lstMsg);
            return objExchangePlan;
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
        
        private decimal[] CalculateConfidenceInterval(ref Response.EmployeeCost ObjEmployeeCost)
        {
            decimal stepl, miscCostFactor;
            decimal[] confInterval = new decimal[3];
            decimal oopstepl, TotalAggregate;
            try
            {
                oopstepl = Convert.ToDecimal(ObjEmployeeCost.Medical.HospitalOrPhysician) + Convert.ToDecimal(ObjEmployeeCost.Medical.PrescriptionDrug);
                TotalAggregate = Math.Round(oopstepl, 2);
                ObjEmployeeCost.Medical.TotalOOP = Math.Max(oopstepl - ObjEmployeeCost.Medical.HRAContributions,0);
                miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceLow");
                stepl = (TotalAggregate * miscCostFactor) - ObjEmployeeCost.Medical.HRAContributions;
                stepl = stepl >= 0 ? stepl : 0;
                confInterval[0] = Math.Round(stepl, 0);
                miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceHigh");
                stepl = (TotalAggregate * miscCostFactor) - ObjEmployeeCost.Medical.HRAContributions;
                stepl = stepl >= 0 ? stepl : 0;
                confInterval[1] = Math.Round(stepl, 0);
                confInterval[2] = Math.Max(TotalAggregate - ObjEmployeeCost.Medical.HRAContributions,0);
                return confInterval;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private decimal[] CalculateExPlanConfidenceInterval(ref Response.EmployeeCost ObjEmployeeCost)
        {
            decimal stepl, miscCostFactor;
            decimal[] confInterval = new decimal[3];

            try
            {
                decimal oopstepl, totalAggregateOOPCost;
                oopstepl = Convert.ToDecimal(ObjEmployeeCost.ExchangePlan.HospitalOrPhysician) + Convert.ToDecimal(ObjEmployeeCost.ExchangePlan.PrescriptionDrug);
                totalAggregateOOPCost = Math.Round(oopstepl, 2);
                ObjEmployeeCost.ExchangePlan.TotalOOP =Math.Max( totalAggregateOOPCost,0);
                miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceLow");
                stepl = totalAggregateOOPCost * miscCostFactor;
                confInterval[0] = Math.Round(stepl, 0);
                miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceHigh");
                stepl = totalAggregateOOPCost * miscCostFactor;
                confInterval[1] = Math.Round(stepl, 0);
                confInterval[2] = Math.Max(totalAggregateOOPCost,0);
                return confInterval;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private decimal[] CalculateTotalConfidenceInterval(ref Response.EmployeeCost ObjEmployeeCost)
        {
            decimal stepl, miscCostFactor;
            decimal[] confInterval = new decimal[3];
            decimal oopstepl = 0, TotalAggregate;
            try
            {              
                oopstepl = (ObjEmployeeCost.Medical != null ? Convert.ToDecimal(ObjEmployeeCost.Medical.HospitalOrPhysician) + Convert.ToDecimal(ObjEmployeeCost.Medical.PrescriptionDrug) : 0) +(ObjEmployeeCost.Dental != null ? Convert.ToDecimal(ObjEmployeeCost.Dental.OOP) : 0) + (ObjEmployeeCost.Vision != null ? Convert.ToDecimal(ObjEmployeeCost.Vision.OOP) : 0);
                TotalAggregate = Math.Round(oopstepl, 2);
                ObjEmployeeCost.TotalOOP = Math.Max(TotalAggregate - ObjEmployeeCost.Medical.HRAContributions,0);
                miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceLow");
                stepl = (TotalAggregate * miscCostFactor) - ObjEmployeeCost.Medical.HRAContributions;
                stepl = stepl >= 0 ? stepl : 0;
                confInterval[0] = Math.Round(stepl, 0);
                miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceHigh");
                stepl = (TotalAggregate * miscCostFactor) - ObjEmployeeCost.Medical.HRAContributions;
                stepl = stepl >= 0 ? stepl : 0;
                confInterval[1] = Math.Round(stepl, 0);
                confInterval[2] = Math.Max(TotalAggregate - ObjEmployeeCost.Medical.HRAContributions,0);
                return confInterval;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private decimal[] CalculateExTotalConfidenceInterval(ref Response.EmployeeCost ObjEmployeeCost)
        {
            decimal stepl, miscCostFactor;
            decimal[] confInterval = new decimal[3];
            decimal oopstepl, TotalAggregate;
            try
            {
                oopstepl = Convert.ToDecimal(ObjEmployeeCost.ExchangePlan.HospitalOrPhysician) + Convert.ToDecimal(ObjEmployeeCost.ExchangePlan.PrescriptionDrug) + (ObjEmployeeCost.Dental != null ? Convert.ToDecimal(ObjEmployeeCost.Dental.OOP) : 0) + (ObjEmployeeCost.Vision != null ? Convert.ToDecimal(ObjEmployeeCost.Vision.OOP) : 0);
                TotalAggregate = Math.Round(oopstepl, 2);
                ObjEmployeeCost.ExTotalOOP = Math.Max(TotalAggregate,0);
                miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceLow");
                stepl = (TotalAggregate * miscCostFactor);
                stepl = stepl >= 0 ? stepl : 0;
                confInterval[0] = Math.Round(stepl, 0);
                miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceHigh");
                stepl = (TotalAggregate * miscCostFactor);
                stepl = stepl >= 0 ? stepl : 0;
                confInterval[1] = Math.Round(stepl, 0);
                confInterval[2] = Math.Max(TotalAggregate,0);
                return confInterval;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}