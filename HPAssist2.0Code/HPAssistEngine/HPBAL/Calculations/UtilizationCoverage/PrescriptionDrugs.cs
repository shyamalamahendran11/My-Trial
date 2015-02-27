#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HPAssistEngine.Models;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.HPDAL;
using System.Resources;
using System.Xml;
#endregion

namespace HPAssistEngine.HPBAL.Calculations.UtilizationCoverage
{
    public class PrescriptionDrugs
    {

        public decimal[] CalculatePrescriptionDrugs(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal[] pd = new decimal[2];
                pd[0] = CalculatePrescDrugsSpecificValue(objRequest, objEntity);
                pd[1] = CalculatePrescDrugsUSAverage(objRequest, objEntity);
                return pd;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculatePrescDrugsSpecificValue(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal PDEmpInNet, PDSpouseInNet = 0, PDChildInNet = 0, PrescDrugsSpecificValue;
                PDEmpInNet = CalculatePDEmpInNet(objRequest, objEntity);
                if (objEntity.NewSpouseMedStatusCode != 0)
                {
                    PDSpouseInNet = CalculatPDSpouseInNet(objRequest, objEntity);
                }
                if (objEntity.NewChildMedStatusCode != 0)
                {
                    PDChildInNet = CalculatePDChildInNet(objRequest, objEntity);
                }
                PrescDrugsSpecificValue = PDEmpInNet + PDSpouseInNet + PDChildInNet;
                return Math.Round(PrescDrugsSpecificValue, 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculatePrescDrugsUSAverage(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal PDEmpOutNet, PDSpouseOutNet = 0, PDChildOutNet = 0, PrescDrugsUSAvg;
                PDEmpOutNet = CalculatePDEmpOutNet(objRequest, objEntity);
                if (objEntity.NewSpouseMedStatusCode != 0)
                {
                    PDSpouseOutNet = CalculatePDSpouseOutNet(objRequest, objEntity);
                }
                if (objEntity.NewChildMedStatusCode != 0)
                {
                    PDChildOutNet = CalculatePDChildOutNet(objRequest, objEntity);
                }
                PrescDrugsUSAvg = PDEmpOutNet + PDSpouseOutNet + PDChildOutNet;
                return Math.Round(PrescDrugsUSAvg, 1);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculatePDEmpInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal RxNoOfScripts = UtilizationCommon.CalculateNoofScripts("PrescriptionDrugs", Resources.Constants.ColRxFactor,
                    Resources.Constants.ColRxNumberofScripts, objRequest, objRequest.Employee.EmpGender, objRequest.Employee.EmpAge, Resources.Constants.ColRxAAGAdjFactor);
                decimal Util = UtilizationCommon.CalculateUtilization(Resources.Constants.ColRxUtilization, objRequest.Employee.EmpDrugsUtil);
                decimal PDEmpInNet = RxNoOfScripts * objEntity.EEStatus * objEntity.NewEmpMedStatusCode * Util;
                return PDEmpInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculatPDSpouseInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal RxNoOfScripts = UtilizationCommon.CalculateNoofScripts("PrescriptionDrugs", Resources.Constants.ColRxFactor,
                    Resources.Constants.ColRxNumberofScripts, objRequest, objRequest.Spouse.SpouseGender, objRequest.Spouse.SpouseAge, Resources.Constants.ColRxAAGAdjFactor);
                decimal Util = UtilizationCommon.CalculateUtilization(Resources.Constants.ColRxUtilization, objRequest.Spouse.SpouseDrugsUtil);
                decimal PDSpouseInNet = RxNoOfScripts * objEntity.SpouseStatus * objEntity.NewSpouseMedStatusCode * Util;
                return PDSpouseInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculatePDChildInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal RxNoOfScripts = UtilizationCommon.CalculateNoofScripts("PrescriptionDrugs", Resources.Constants.ColRxFactor,
                    Resources.Constants.ColRxNumberofScripts, objRequest, "C", misConsCol: Resources.Constants.ColRxAAGAdjFactor);
                decimal Util = UtilizationCommon.CalculateUtilization(Resources.Constants.ColRxUtilization, objRequest.Children.ChildDrugsUtil);
                decimal PDChildInNet = RxNoOfScripts * objEntity.ChildStatus * objEntity.NewChildMedStatusCode * Util;
                return PDChildInNet;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculatePDEmpOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {

            try
            {
                decimal utilstat = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColPrescriptions);
                decimal misconst = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColRxAAGAdjFactor);
                decimal PDEmpOutNet = utilstat * objEntity.EEStatus * objEntity.NewEmpMedStatusCode * misconst;
                return PDEmpOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculatePDSpouseOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal misconst = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColRxAAGAdjFactor);
                decimal utilstat = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColPrescriptions);
                decimal PDSpouseOutNet = misconst * objEntity.SpouseStatus * objEntity.NewSpouseMedStatusCode * utilstat;
                return PDSpouseOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculatePDChildOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal misconst = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColRxAAGAdjFactor);
                decimal utilstat = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColPrescriptions);
                decimal PDChildOutNet = misconst * objEntity.ChildStatus * objEntity.NewChildMedStatusCode * utilstat;
                return PDChildOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}