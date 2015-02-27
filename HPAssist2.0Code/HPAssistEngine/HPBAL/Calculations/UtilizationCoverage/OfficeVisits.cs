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
    public class OfficeVisits
    {
        #region MainFunction
        public decimal[] CalculateOfficeVisits(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal[] ov = new decimal[2];
                ov[0] = CalculateOfficeVisitsSpecificValue(objRequest, objEntity);
                ov[1] = CalculateOfficeVisitsUSAverage(objRequest, objEntity);
                return ov;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Main-SubFunction
        private decimal CalculateOfficeVisitsSpecificValue(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal officeVisitsEmpInNet, officeVisitsSpouseInNet = 0, officeVisitsChildInNet = 0, officeVisitsSpecificValue;
                officeVisitsEmpInNet = CalculateOVEmpInNet(objRequest, objEntity);
                if (objEntity.NewSpouseMedStatusCode != 0)
                {
                    officeVisitsSpouseInNet = CalculateOVSpouseInNet(objRequest, objEntity);
                }
                if (objEntity.NewChildMedStatusCode != 0)
                {
                    officeVisitsChildInNet = CalculateOVChildInNet(objRequest, objEntity);
                }
                officeVisitsSpecificValue = officeVisitsEmpInNet + officeVisitsSpouseInNet + officeVisitsChildInNet;
                officeVisitsSpecificValue = Math.Round(officeVisitsSpecificValue, 1);
                return officeVisitsSpecificValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateOfficeVisitsUSAverage(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal officeVisitsEmpOutNet, officeVisitsSpouseOutNet = 0, officeVisitsChildOutNet = 0, officeVisitsUSAverage;

                officeVisitsEmpOutNet = CalculateOVEmpOutNet(objRequest, objEntity);
                if (objEntity.NewSpouseMedStatusCode != 0)
                {
                    officeVisitsSpouseOutNet = CalculateOVSpouseOutNet(objRequest, objEntity);
                }
                if (objEntity.NewChildMedStatusCode != 0)
                {
                    officeVisitsChildOutNet = CalculateOVChildOutNet(objRequest, objEntity);
                }
                officeVisitsUSAverage = officeVisitsEmpOutNet + officeVisitsSpouseOutNet + officeVisitsChildOutNet;
                officeVisitsUSAverage = Math.Round(officeVisitsUSAverage, 1);
                return officeVisitsUSAverage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IN-NetWork
        private decimal CalculateOVEmpInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal OVAAGAdjFactor = UtilizationCommon.CalculateNoofScripts("OfficeVisits", Resources.Constants.ColOVFactor,
                    Resources.Constants.ColOVFactor, objRequest, objRequest.Employee.EmpGender, objRequest.Employee.EmpAge, Resources.Constants.ColOVAAGAdjFactor);
                decimal newMedicalUtilization = UtilizationCommon.CalculateUtilization(Resources.Constants.ColUtilisationOV, objRequest.Employee.EmpMedicalUtil);
                decimal ovEmpInNet = OVAAGAdjFactor * objEntity.EEStatus * objEntity.NewEmpMedStatusCode * newMedicalUtilization;
                ovEmpInNet = Math.Round(ovEmpInNet, 4);
                return ovEmpInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateOVSpouseInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal OVAAGAdjFactor = UtilizationCommon.CalculateNoofScripts("OfficeVisits", Resources.Constants.ColOVFactor,
                    Resources.Constants.ColOVFactor, objRequest, objRequest.Spouse.SpouseGender, objRequest.Spouse.SpouseAge, Resources.Constants.ColOVAAGAdjFactor);
                decimal spouseNewMedicalUtilization = UtilizationCommon.CalculateUtilization(Resources.Constants.ColUtilisationOV, objRequest.Spouse.SpouseMedicalUtil);
                decimal ovSpouseInNet = OVAAGAdjFactor * objEntity.SpouseStatus * objEntity.NewSpouseMedStatusCode * spouseNewMedicalUtilization;
                return ovSpouseInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateOVChildInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal OVAAGAdjFactor = UtilizationCommon.CalculateNoofScripts("OfficeVisits", Resources.Constants.ColOVFactor,
                    Resources.Constants.ColOVFactor, objRequest, Resources.Constants.ColChildGender, 0, Resources.Constants.ColOVAAGAdjFactor);
                decimal childNewMedicalUtilization = UtilizationCommon.CalculateUtilization(Resources.Constants.ColUtilisationOV, objRequest.Children.ChildMedicalUtil);
                decimal ovChildInNet = OVAAGAdjFactor * objEntity.ChildStatus * objEntity.NewChildMedStatusCode * childNewMedicalUtilization;
                return ovChildInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OUT-NetWork
        private decimal CalculateOVEmpOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal OVCopayUtilization = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColOVCopay);
                decimal OVAAGAdjFactor = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColOVAAGAdjFactor);
                decimal ovEmpOutNet = OVCopayUtilization * objEntity.EEStatus * objEntity.NewEmpMedStatusCode * OVAAGAdjFactor;
                return ovEmpOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateOVSpouseOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal OVCopayUtilization = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColOVCopay);
                decimal OVAAGAdjFactor = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColOVAAGAdjFactor);
                decimal ovSpouseOutNet = OVCopayUtilization * objEntity.SpouseStatus * objEntity.NewSpouseMedStatusCode * OVAAGAdjFactor;
                return ovSpouseOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateOVChildOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal OVCopayUtilization = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColOVCopay);
                decimal OVAAGAdjFactor = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColOVAAGAdjFactor);
                decimal ovChildOutNet = OVCopayUtilization * objEntity.ChildStatus * objEntity.NewChildMedStatusCode * OVAAGAdjFactor;
                return ovChildOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }

}