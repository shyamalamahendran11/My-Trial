#region NameSpaces
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
using OOPCalculation;
using HPAssistEngine.HPBAL.Calculations.OOPExpenses;
#endregion

namespace HPAssistEngine.HPBAL.Calculations.EmployeeCost
{
    public class Vision
    {
        #region VisionEntity
        //Vision Entity object Creation
        VisionInNetwork objVisionInnet = new VisionInNetwork();
        #endregion

        #region Methods
        //Calculating Total ApplyCopayLimits & NoCovOption -H155
        public decimal CalculateVisionCost(CostEstimate objRequest, PlanDesignVision objVisionEntity, int eeVisionStatus, int spouseVisionStatus, int childVisionStatus)
        {
            try
            {
                decimal empIn, spouseIn, childIn;
                empIn = CalculateEmpInCopayLimits(objRequest, objVisionEntity, eeVisionStatus);
                spouseIn = objRequest.Spouse.SpouseVisionPlanExist ==1? CalculateSpouseInCopayLimits(objRequest, objVisionEntity, spouseVisionStatus):0;
                childIn = objRequest.Children.ChildVisionPlanExist ==1 ?CalculateChildInCopayLimits(objRequest, objVisionEntity, childVisionStatus):0;
                objVisionInnet.visionCost = Math.Round(childIn + spouseIn + empIn,2);

                return objVisionInnet.visionCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating EmployeeIn Values
        private decimal CalculateEmpInCopayLimits(CostEstimate objRequest, PlanDesignVision objVisionEntity, int eeVisionStatus)
        {
            try
            {
                objVisionInnet.empAreaFactor = OOPCommon.CalculateAreaFactor(objRequest.Zipcode, Resources.Constants.ColVisionCostFactor);
                objVisionInnet.empAgeGenderFactor = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColVisionUtilFactor, objRequest.Employee.EmpGender, objRequest.Employee.EmpAge);
                objVisionInnet.empLensesAAGFactor = CalculateLensesAAGFactor(objVisionInnet.empAgeGenderFactor, objVisionInnet.empAreaFactor);
                objVisionInnet.empAdjVisionCosts = CalculateAdjustedVisionCosts(objVisionInnet.empLensesAAGFactor, Resources.Constants.ColVisionClaimAmount);
                objVisionInnet.empAdjCopayLimits = CalculateCopayLimits(objVisionInnet.empAdjVisionCosts, objVisionEntity.ExamCoPay, objVisionEntity.HardwareCoPay, objRequest.Employee.EmpCorrectivelenses, eeVisionStatus);

                return objVisionInnet.empAdjCopayLimits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating SpouseIn Values
        private decimal CalculateSpouseInCopayLimits(CostEstimate objRequest, PlanDesignVision objVisionEntity, int spouseVisionStatus)
        {
            try
            {
                objVisionInnet.spouseAreaFactor = OOPCommon.CalculateAreaFactor(objRequest.Zipcode, Resources.Constants.ColVisionCostFactor);
                objVisionInnet.spouseAgeGenderFactor = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColVisionUtilFactor, objRequest.Spouse.SpouseGender, objRequest.Spouse.SpouseAge);
                objVisionInnet.spouseLensesAAGFactor = CalculateLensesAAGFactor(objVisionInnet.spouseAgeGenderFactor, objVisionInnet.spouseAreaFactor);
                objVisionInnet.spouseAdjVisionCosts = CalculateAdjustedVisionCosts(objVisionInnet.spouseLensesAAGFactor, Resources.Constants.ColVisionClaimAmount);
                objVisionInnet.spouseCopayLimits = CalculateCopayLimits(objVisionInnet.spouseAdjVisionCosts, objVisionEntity.ExamCoPay, objVisionEntity.HardwareCoPay, objRequest.Spouse.SpouseCorrectivelenses, spouseVisionStatus);

                return objVisionInnet.spouseCopayLimits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating ChildIn Values
        private decimal CalculateChildInCopayLimits(CostEstimate objRequest, PlanDesignVision objVisionEntity,int childVisionStatus)
        {
            try
            {
                objVisionInnet.childAreaFactor = OOPCommon.CalculateAreaFactor(objRequest.Zipcode, Resources.Constants.ColVisionCostFactor);
                objVisionInnet.childAgeGenderFactor = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColVisionUtilFactor, Resources.Constants.ColChildGender);
                objVisionInnet.childLensesAAGFactor = CalculateLensesAAGFactor(objVisionInnet.childAgeGenderFactor, objVisionInnet.childAreaFactor);
                objVisionInnet.childAdjVisionCosts = CalculateAdjustedVisionCosts(objVisionInnet.childLensesAAGFactor, Resources.Constants.ColVisionClaimAmount);
                objVisionInnet.childCopayLimits = CalculateCopayLimits(objVisionInnet.childAdjVisionCosts, objVisionEntity.ExamCoPay, objVisionEntity.HardwareCoPay, objRequest.Children.ChildCorrectivelenses, childVisionStatus);

                return objVisionInnet.childCopayLimits;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SubMethods
        //Calculating LensesAAGFactor-B152,D152,F152
        private decimal CalculateLensesAAGFactor(decimal ageGenderFactor, decimal areaFactor)
        {
            decimal lensesAAGFactor;
            try
            {
                lensesAAGFactor = ageGenderFactor * areaFactor;
                return lensesAAGFactor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating AdjustedVisionCosts-B154,D154,F154
        private decimal CalculateAdjustedVisionCosts(decimal lensesAAGFactor, string miscCostsandFactorsColName)
        {
            decimal miscCostsandFactorsValue, adjVisionCosts;
            try
            {
                miscCostsandFactorsValue = CommonDAL.CalculateMiscCostsAndFactors(miscCostsandFactorsColName);
                adjVisionCosts = lensesAAGFactor * miscCostsandFactorsValue;
                return adjVisionCosts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating ApplyCopayLimits&NoCovOption -B155,D155,F155
        private decimal CalculateCopayLimits(decimal adjVisionCosts, decimal  examCoPay, decimal  hardwareCoPay, decimal  correctiveLenses, decimal  visionStatusCode)
        {
            decimal coPayValue, copayLimitsValue;
            try
            {
                coPayValue = examCoPay + hardwareCoPay;
                copayLimitsValue = Math.Min(adjVisionCosts, coPayValue);
                copayLimitsValue = copayLimitsValue * correctiveLenses * visionStatusCode;
                return copayLimitsValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}