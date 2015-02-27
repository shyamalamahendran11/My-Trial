#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.HPDAL;
using System.Resources;
using System.Xml;
#endregion

namespace HPAssistEngine.HPBAL.Calculations.UtilizationCoverage
{
    public class CorrectiveLenses
    {
        decimal Util;

        public decimal[] CalculateCorrectiveLenses(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal[] cl = new decimal[2];
                cl[0] = CalculateCorrectiveLensesSpecific(objRequest, objEntity);
                cl[1] = CalculateCorrectiveLensesNatAvg(objEntity);
                return cl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateCorrectiveLensesSpecific(CostEstimate objRequest, CommonEntities objEntity)
        {

            try
            {
                decimal CLEmpInNet, CLSpouseInNet = 0, CLChildInNet = 0, CorrLensSpecificValue;
                CLEmpInNet = CalulateCLEmpInNet(objRequest, objEntity);
                if (objEntity.NewSpouseVisionStatusCode != 0)
                {
                    CLSpouseInNet = CalculatCLSpouseInNet(objRequest, objEntity);
                }
                if (objEntity.NewChildVisionStatusCode != 0)
                {
                    CLChildInNet = CalculateCLChildInNet(objRequest, objEntity);
                }
                CorrLensSpecificValue = CLEmpInNet + CLSpouseInNet + CLChildInNet;
                return Math.Round(CorrLensSpecificValue, 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateCorrectiveLensesNatAvg(CommonEntities objEntity)
        {
            try
            {
                decimal CLEmpOutNet, CLSpouseOutNet = 0, CLChildOutNet = 0, CorrLensUSAvg;
                CLEmpOutNet = CalulateCLEmpOutNet(objEntity);
                if (objEntity.NewSpouseVisionStatusCode != 0)
                {
                    CLSpouseOutNet = CalculateCLSpouseOutNet(objEntity);
                }
                if (objEntity.NewChildVisionStatusCode != 0)
                {
                    CLChildOutNet = CalculateCLChildOutNet(objEntity);
                }
                CorrLensUSAvg = CLEmpOutNet + CLSpouseOutNet + CLChildOutNet;
                return Math.Round(CorrLensUSAvg, 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalulateCLEmpInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal glassEEstatus, CLEmpInNet, NoOfScripts;
                Util = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColCorrectiveLenses);
                if (objRequest.Employee.EmpCorrectivelenses == 1)
                {
                    glassEEstatus = 1;
                }
                else
                {
                    glassEEstatus = 0;
                    NoOfScripts = UtilizationCommon.CalculateNoofScripts("Correctivelenses", Resources.Constants.ColVisionCostFactor,
                    Resources.Constants.ColVisionUtilFactor, objRequest, objRequest.Employee.EmpGender, objRequest.Employee.EmpAge);
                    glassEEstatus = NoOfScripts * Util * objEntity.EEStatus * objEntity.NewEmpVisionStatusCode;
                }
                CLEmpInNet = glassEEstatus * objEntity.NewEmpVisionStatusCode;
                return CLEmpInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculatCLSpouseInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal glassSpousestatus, CLSpouseInNet, NoOfScripts;
                Util = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColCorrectiveLenses);
                if (objRequest.Spouse.SpouseCorrectivelenses == 1)
                {
                    glassSpousestatus = 1;
                }
                else
                {
                    glassSpousestatus = 0;
                    NoOfScripts = UtilizationCommon.CalculateNoofScripts("Correctivelenses", Resources.Constants.ColVisionCostFactor,
                    Resources.Constants.ColVisionUtilFactor, objRequest, objRequest.Spouse.SpouseGender, objRequest.Spouse.SpouseAge);
                    glassSpousestatus = NoOfScripts * Util * objEntity.SpouseStatus * objEntity.NewSpouseVisionStatusCode;
                }
                CLSpouseInNet = glassSpousestatus * objEntity.NewSpouseVisionStatusCode;
                return CLSpouseInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateCLChildInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal glassChildstatus, CLChildInNet, NoOfScripts;
                Util = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColCorrectiveLenses);
                glassChildstatus = 0;
                 NoOfScripts = UtilizationCommon.CalculateNoofScripts("Correctivelenses", Resources.Constants.ColVisionCostFactor,
                    Resources.Constants.ColVisionUtilFactor, objRequest, "C");
                glassChildstatus = NoOfScripts * objEntity.ChildStatus * objEntity.NewChildVisionStatusCode * Util;
                CLChildInNet = (objRequest.Children.ChildCorrectivelenses + ((glassChildstatus * objEntity.NewChildVisionStatusCode) / objEntity.ChildStatus * (objEntity.ChildStatus - objRequest.Children.ChildCorrectivelenses))) * objEntity.NewChildVisionStatusCode;
                return CLChildInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalulateCLEmpOutNet(CommonEntities objEntity)
        {
            try
            {
                decimal clEmpOutNet, miscconst;
                miscconst = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColVisionAAGAdjFactor);
                clEmpOutNet = miscconst * objEntity.EEStatus * objEntity.NewEmpVisionStatusCode * Util;
                return clEmpOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateCLSpouseOutNet(CommonEntities objEntity)
        {
            try
            {
                decimal clSpouseOutNet, miscconst;
                miscconst = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColVisionAAGAdjFactor);
                clSpouseOutNet = miscconst * objEntity.SpouseStatus * objEntity.NewSpouseVisionStatusCode * Util;
                return clSpouseOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateCLChildOutNet(CommonEntities objEntity)
        {
            try
            {
                decimal clChildOutNet, miscconst;
                miscconst = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColVisionAAGAdjFactor);
                clChildOutNet = miscconst * objEntity.ChildStatus * objEntity.NewChildVisionStatusCode * Util;
                return clChildOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }

}