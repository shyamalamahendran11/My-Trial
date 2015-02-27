using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.HPDAL;
using NetUtilLib.Data;
using System.Data;
using System.Reflection;
using HPAssistEngine.Models;
using HPAssistEngine.Common;
using OOPCalculation;

namespace HPAssistEngine.HPBAL.Calculations.OOPExpenses
{
    public class OOPMain
    {
        //Response.OOPExpenses ObjOOPExpenses = new Response.OOPExpenses();
        //public Response.OOPExpenses CalculateOOPExpenses(CostEstimate objInputXML, CommonEntities inputObj, string dbName)
        //{
        //    try
        //    {
        //        IDataReader dr;
        //        if (objInputXML.PlanIds.MedicalId != 0)
        //        {
        //            dr = CommonDAL.GetMedicalPlanDesign(objInputXML.PlanIds.MedicalId, dbName);
        //            if (dr != null)
        //            {
        //                PlanDesignMedical objPlanMedical = Utilities.DataReaderMapToList<PlanDesignMedical>(dr);


        //                Hospital_Physician ObjHp = new Hospital_Physician();
        //                decimal[] Medical = new decimal[2];
        //                if (objPlanMedical != null)
        //                {
        //                    //BOTH HOSPITAL_PHYSICIAN & PRESCRIPTION_DRUGS ARE CALCULATED WITH THE SAME OBJECT
        //                    Medical = ObjHp.CalculateHospital_Physician(objInputXML, inputObj, objPlanMedical);

        //                    ObjOOPExpenses.HospitalOrPhysician = Medical[0];
        //                    ObjOOPExpenses.PrescriptionDrug = Medical[1];
        //                }
        //            }
        //        }
        //        if (objInputXML.PlanIds.DentalId != 0)
        //        {
        //            dr = CommonDAL.GetDentalPlanDesign(objInputXML.PlanIds.DentalId, dbName);
        //            if (dr != null)
        //            {
        //                PlanDesignDental ObjPlanDental = Utilities.DataReaderMapToList<PlanDesignDental>(dr);
        //                if (ObjPlanDental != null)
        //                {
        //                    Dental objDental = new Dental();
        //                    ObjOOPExpenses.Dental = objDental.CalculateDentalCost(objInputXML, ObjPlanDental, inputObj);
        //                }
        //            }
        //        }
        //        if (objInputXML.PlanIds.VisionId != 0)
        //        {
        //            dr = CommonDAL.GetVisionPlanDesign(objInputXML.PlanIds.VisionId, dbName);
        //            if (dr != null)
        //            {
        //                PlanDesignVision ObjPlanVision = Utilities.DataReaderMapToList<PlanDesignVision>(dr);
        //                if (ObjPlanVision != null)
        //                {
        //                    Vision objVision = new Vision();
        //                    ObjOOPExpenses.Vision = objVision.CalculateVisionCost(objInputXML, ObjPlanVision, inputObj.NewEmpVisionStatusCode, inputObj.NewSpouseVisionStatusCode, inputObj.NewChildVisionStatusCode);
        //                }
        //            }
        //        }
        //        decimal[] Range = new decimal[3];
        //        Range = CalculateConfidenceInterval();
        //        ObjOOPExpenses.RangeLow = Range[0];
        //        ObjOOPExpenses.RangeHigh = Range[1];
        //        ObjOOPExpenses.RangeAverage = Range[2];
        //        return ObjOOPExpenses;
        //    }
        //    catch (Exception ex)
        //    { throw ex; }
        //}

        //private decimal CalculateTotalAggregateOOPCost()
        //{
        //    decimal stepl, totalAggregateOOPCost;
        //    try
        //    {
        //        stepl = Convert.ToDecimal(ObjOOPExpenses.HospitalOrPhysician) + Convert.ToDecimal(ObjOOPExpenses.PrescriptionDrug) + Convert.ToDecimal(ObjOOPExpenses.Dental) + Convert.ToDecimal(ObjOOPExpenses.Vision);
        //        totalAggregateOOPCost = Math.Round(stepl, 2);
        //        ObjOOPExpenses.Total = totalAggregateOOPCost;
        //        return totalAggregateOOPCost;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //private decimal[] CalculateConfidenceInterval()
        //{
        //    decimal stepl, miscCostFactor;
        //    decimal[] confInterval = new decimal[3];

        //    try
        //    {
        //        decimal TotalAggregate = CalculateTotalAggregateOOPCost();

        //        miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceLow");
        //        stepl = TotalAggregate * miscCostFactor;
        //        confInterval[0] = Math.Round(stepl, 0);
        //        miscCostFactor = CommonDAL.CalculateMiscCostsAndFactors("ConfidenceHigh");
        //        stepl = TotalAggregate * miscCostFactor;
        //        confInterval[1] = Math.Round(stepl, 0);
        //        confInterval[2] = Math.Round(TotalAggregate, 0);
        //        return confInterval;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


    }
}