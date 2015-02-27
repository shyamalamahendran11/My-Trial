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
    public class SpecialistsVisit
    {
        #region MainFunction
        public decimal[] CalculateSpecialistsVisit(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal[] sp = new decimal[2];
                sp[0] = CalculateSpecialistsVisitSpecificValue(objRequest, objEntity);
                sp[1] = CalculateSpecialistsVisitUSAverage(objRequest, objEntity);
                return sp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Main-SubFunction
        private decimal CalculateSpecialistsVisitSpecificValue(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal specialistsVisitEmpInNet, specialistsVisitSpouseInNet = 0, specialistsVisitChildInNet = 0, specialistsVisitSpecificValue;
                specialistsVisitEmpInNet = CalculateSPEmpInNet(objRequest, objEntity);
                if (objEntity.NewSpouseMedStatusCode != 0)
                {
                    specialistsVisitSpouseInNet = CalculateSPSpouseInNet(objRequest, objEntity);
                }
                if (objEntity.NewChildMedStatusCode != 0)
                {
                    specialistsVisitChildInNet = CalculateSPChildInNet(objRequest, objEntity);
                }
                specialistsVisitSpecificValue = specialistsVisitEmpInNet + specialistsVisitSpouseInNet + specialistsVisitChildInNet;
                specialistsVisitSpecificValue = Math.Round(specialistsVisitSpecificValue, 1);
                return specialistsVisitSpecificValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateSpecialistsVisitUSAverage(CostEstimate objRequest, CommonEntities objEntity)
        {
            decimal specialistsVisitEmpOutNet, specialistsVisitSpouseOutNet = 0, specialistsVisitChildOutNet = 0, specialistsVisitUSAverage;
            specialistsVisitEmpOutNet = CalculateSPEmpOutNet(objRequest, objEntity);
            if (objEntity.NewSpouseMedStatusCode != 0)
            {
                specialistsVisitSpouseOutNet = CalculateSPSpouseOutNet(objRequest, objEntity);
            }
            if (objEntity.NewChildMedStatusCode != 0)
            {
                specialistsVisitChildOutNet = CalculateSPChildOutNet(objRequest, objEntity);
            }
            specialistsVisitUSAverage = specialistsVisitEmpOutNet + specialistsVisitSpouseOutNet + specialistsVisitChildOutNet;
            specialistsVisitUSAverage = Math.Round(specialistsVisitUSAverage, 1);
            return specialistsVisitUSAverage;
        }
        #endregion

        #region In-NetWork
        private decimal CalculateSPEmpInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal SPAAGFactor = UtilizationCommon.CalculateNoofScripts("SpecialistVisits", Resources.Constants.ColSpecialistFactor,
                    Resources.Constants.ColSpecialist, objRequest, objRequest.Employee.EmpGender, objRequest.Employee.EmpAge, Resources.Constants.ColOVAAGAdjFactor);
                decimal newMedicalutilSpecialist = UtilizationCommon.CalculateUtilization(Resources.Constants.ColSpecialistUtil, objRequest.Employee.EmpMedicalUtil);
                decimal spEmpInNet = SPAAGFactor * objEntity.EEStatus * objEntity.NewEmpMedStatusCode * newMedicalutilSpecialist;
                return spEmpInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateSPSpouseInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal SPAAGFactor = UtilizationCommon.CalculateNoofScripts("SpecialistVisits", Resources.Constants.ColSpecialistFactor,
                    Resources.Constants.ColSpecialist, objRequest, objRequest.Spouse.SpouseGender, objRequest.Spouse.SpouseAge, Resources.Constants.ColOVAAGAdjFactor);
                decimal newMedicalutilSpecialist = UtilizationCommon.CalculateUtilization(Resources.Constants.ColSpecialistUtil, objRequest.Spouse.SpouseMedicalUtil);
                decimal spSpouseInNet = SPAAGFactor * objEntity.SpouseStatus * objEntity.NewSpouseMedStatusCode * newMedicalutilSpecialist;
                return spSpouseInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateSPChildInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal SPAAGFactor = UtilizationCommon.CalculateNoofScripts("SpecialistVisits", Resources.Constants.ColSpecialistFactor,
                    Resources.Constants.ColSpecialist, objRequest, Resources.Constants.ColChildGender, 0, Resources.Constants.ColOVAAGAdjFactor);
                decimal newMedicalutilSpecialist = UtilizationCommon.CalculateUtilization(Resources.Constants.ColSpecialistUtil, objRequest.Children.ChildMedicalUtil);
                decimal spChildInNet = SPAAGFactor * objEntity.ChildStatus * objEntity.NewChildMedStatusCode * newMedicalutilSpecialist;
                return spChildInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Out-NetWork
        private decimal CalculateSPEmpOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal specialistCopayUtil = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColSpecialistCopay);
                decimal ovaagAdjFactor = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColOVAAGAdjFactor);
                decimal spEmpOutNet = specialistCopayUtil * objEntity.EEStatus * objEntity.NewEmpMedStatusCode * ovaagAdjFactor;
                return spEmpOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateSPSpouseOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal specialistCopayUtil = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColSpecialistCopay);
                decimal ovaagAdjFactor = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColOVAAGAdjFactor);
                decimal spSpouseOutNet = specialistCopayUtil * objEntity.SpouseStatus * objEntity.NewSpouseMedStatusCode * ovaagAdjFactor;
                return spSpouseOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateSPChildOutNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal specialistCopayUtil = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColSpecialistCopay);
                decimal ovaagAdjFactor = UtilizationCommon.CalculateMiscCostsAndFactors(Resources.Constants.ColOVAAGAdjFactor);
                decimal spChildOutNet = specialistCopayUtil * objEntity.ChildStatus * objEntity.NewChildMedStatusCode * ovaagAdjFactor;
                return spChildOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }

}