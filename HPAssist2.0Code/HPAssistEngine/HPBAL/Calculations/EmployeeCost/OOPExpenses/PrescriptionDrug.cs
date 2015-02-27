#region Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HPAssistEngine.Models;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.HPDAL;
using System.Resources;
using System.Xml;
using System.Globalization;
using HPAssistEngine.Resources;
#endregion

namespace HPAssistEngine.HPBAL.Calculations.EmployeeCost
{
    public class PrescriptionDrug
    {
        //H110
        public decimal CalculatePrescriptionDrugs(CommonEntities inputObj, PlanDesignMedical objPlanMedical, PrescriptionDrugsEntities objPDEntities)
        {
            try
            {
                decimal empPlanBsdRxCost, totalRXCost;
                totalRXCost = CalculateTotalRXCost(inputObj, objPlanMedical, objPDEntities);
                if (objPlanMedical.CPDTableOOPMax == "1B")
                {
                    if (objPlanMedical.RxOOPMax * (inputObj.EEStatus + inputObj.SpouseStatus + inputObj.ChildStatus) > totalRXCost)
                        empPlanBsdRxCost = totalRXCost;
                    else
                        empPlanBsdRxCost = objPlanMedical.RxOOPMax * (inputObj.EEStatus + inputObj.SpouseStatus + inputObj.ChildStatus);
                }
                else
                {
                    if (((objPDEntities.EmpRxOOPMax + objPDEntities.EmpRxDeductibleTrigger) * inputObj.EEStatus + (objPDEntities.SpouseRxOOPMax + objPDEntities.SpouseRxDeduxtibleTrigger) * inputObj.SpouseStatus + (objPDEntities.ChildRxOOPMax + objPDEntities.ChildRxDeductibleTrigger) * inputObj.ChildStatus) > totalRXCost)
                        empPlanBsdRxCost = totalRXCost;
                    else
                        empPlanBsdRxCost = ((objPDEntities.EmpRxOOPMax + objPDEntities.EmpRxDeductibleTrigger) * inputObj.EEStatus + (objPDEntities.SpouseRxOOPMax + objPDEntities.SpouseRxDeduxtibleTrigger) * inputObj.SpouseStatus + (objPDEntities.ChildRxOOPMax + objPDEntities.ChildRxDeductibleTrigger) * inputObj.ChildStatus);
                }
                return empPlanBsdRxCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //H108
        private decimal CalculateTotalRXCost( CommonEntities inputObj, PlanDesignMedical objPlanMedical, PrescriptionDrugsEntities objPDEntities)
        {
            try
            {
                decimal totalRXCost, totalCost;
                //B107, //D107, //F107 
                decimal rxEffectiveCopay = (objPlanMedical.RxCopayRetailGeneric * objPDEntities.RGeneric) + (objPlanMedical.RxCopayRetailForm * objPDEntities.RForm) + (objPlanMedical.RxCopayRetailNonForm * objPDEntities.RNonForm);
                rxEffectiveCopay = (rxEffectiveCopay + (objPlanMedical.RxCopayMailGeneric * objPDEntities.MGeneric) + (objPlanMedical.RxCopayMailForm * objPDEntities.MForm) + (objPlanMedical.RxCopayMailNonForm * objPDEntities.MNonForm)) * inputObj.NewEmpMedStatusCode;
                totalCost = CalculateTotalCost(inputObj, objPlanMedical, objPDEntities);
                totalRXCost = objPDEntities.EmpTotalRxCopay + objPDEntities.SpouseTotalRxCopay + objPDEntities.ChildTotalRxCopay + totalCost;//H108
                return totalRXCost;
            }
            catch (Exception ex)
            { throw ex; }
        }

        //H105
        private decimal CalculateTotalCost(CommonEntities inputObj, PlanDesignMedical objPlanMedical, PrescriptionDrugsEntities objPDEntities)
        {
            try
            {
                decimal miscCons, CoinsuranceOOPMax, totalCost;
                miscCons = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColRxBaseAdjFactor);//H104
                CoinsuranceOOPMax = CalculateCoinsuranceOOPMax( inputObj, objPlanMedical, objPDEntities);
                totalCost = miscCons * CoinsuranceOOPMax;
                return totalCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //H103
        private decimal CalculateCoinsuranceOOPMax( CommonEntities inputObj, PlanDesignMedical objPlanMedical, PrescriptionDrugsEntities objPDEntities)
        {
            try
            {
                decimal coinsuranceOOPMax;
                decimal condition1 = (objPDEntities.EmpRxOOPMax + objPDEntities.EmpRxDeductibleTrigger) * inputObj.NewEmpMedStatusCode + (objPDEntities.SpouseRxOOPMax + objPDEntities.SpouseRxDeduxtibleTrigger) * inputObj.NewSpouseMedStatusCode + (objPDEntities.ChildRxOOPMax + objPDEntities.ChildRxDeductibleTrigger) * inputObj.NewChildMedStatusCode*inputObj.ChildStatus;
                decimal condition2 = CalculateTotalValueOfDeductible(inputObj, objPDEntities) + objPDEntities.EmpRxCoinOOPMax + objPDEntities.SpouseRxCoinOOPMax + objPDEntities.ChildRxCoinOOPMax;
                coinsuranceOOPMax = condition1 < condition2 ? condition1 : condition2;
                return coinsuranceOOPMax;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //H102
        private decimal CalculateTotalValueOfDeductible(CommonEntities inputObj, PrescriptionDrugsEntities objPDEntities)
        {
            try
            {
                decimal totalValueOfDeductible;
                decimal condition1 = objPDEntities.EmpRxDeductibleTrigger * inputObj.NewEmpMedStatusCode + objPDEntities.SpouseRxDeduxtibleTrigger * inputObj.NewSpouseMedStatusCode + objPDEntities.ChildRxDeductibleTrigger * inputObj.NewChildMedStatusCode * inputObj.ChildStatus;
                decimal condition2 = objPDEntities.EmpRxValofDeductible + objPDEntities.SpouseRxValofDeductible + objPDEntities.ChildRxValofDeductible;
                objPDEntities.TotalRxCoinOOPMax = totalValueOfDeductible = condition1 < condition2 ? condition1 : condition2;
                return totalValueOfDeductible;
            }
            catch (Exception ex)
            { throw ex; }
        }




    }
}
