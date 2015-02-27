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
    public class DentalFillings
    {
        decimal Util;
        public decimal[] CalculateDentalFillings(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal[] df = new decimal[2];
                df[0] = CalculateDentalFillingsSpecific(objRequest, objEntity);
                df[1] = CalculateDentalFillingsNatAvg(objEntity);
                return df;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private decimal CalculateDentalFillingsSpecific(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal DFEmpInNet, DFSpouseInNet = 0, DFChildInNet = 0, DentFillSpecificValue;
                DFEmpInNet = CalulateDFEmpInNet(objRequest, objEntity);
                if (objEntity.NewSpouseDentalStatusCode != 0)
                {
                    DFSpouseInNet = CalculatDFSpouseInNet(objRequest, objEntity);
                }
                if (objEntity.NewChildDentalStatusCode != 0)
                {
                    DFChildInNet = CalculateDFChildInNet(objRequest, objEntity);
                }
                DentFillSpecificValue = DFEmpInNet + DFSpouseInNet + DFChildInNet;
                return Math.Round(DentFillSpecificValue, 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateDentalFillingsNatAvg(CommonEntities objEntity)
        {
            try
            {
                decimal DFEmpOutNet, DFSpouseOutNet = 0, DFChildOutNet = 0, DentFillUSAvg;
                DFEmpOutNet = CalulateDFEmpOutNet(objEntity);
                if (objEntity.NewSpouseDentalStatusCode != 0)
                {
                    DFSpouseOutNet = CalculateDFSpouseOutNet(objEntity);
                }
                if (objEntity.NewChildDentalStatusCode != 0)
                {
                    DFChildOutNet = CalculateDFChildOutNet(objEntity);
                }
                DentFillUSAvg = DFEmpOutNet + DFSpouseOutNet + DFChildOutNet;
                return Math.Round(DentFillUSAvg, 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalulateDFEmpInNet(CostEstimate objRequest, CommonEntities objEntity)
        {

            try
            {

                decimal NoOfScripts = UtilizationCommon.CalculateNoofScripts("DentalFillings", Resources.Constants.ColOrthoCostFactor,
                    Resources.Constants.ColDentalFillingsFactor, objRequest, objRequest.Employee.EmpGender, objRequest.Employee.EmpAge);
                Util = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColDentalFillings);
                decimal DFEmpInNet = NoOfScripts * objEntity.EEStatus * objEntity.NewEmpDentalStatusCode * Util;
                return DFEmpInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculatDFSpouseInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {

                decimal NoOfScripts = UtilizationCommon.CalculateNoofScripts("DentalFillings", Resources.Constants.ColOrthoCostFactor,

                    Resources.Constants.ColDentalFillingsFactor, objRequest, objRequest.Spouse.SpouseGender, objRequest.Spouse.SpouseAge);
                Util = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColDentalFillings);
                decimal DFSpouseInNet = NoOfScripts * objEntity.SpouseStatus * objEntity.NewSpouseDentalStatusCode * Util;
                return DFSpouseInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateDFChildInNet(CostEstimate objRequest, CommonEntities objEntity)
        {
            try
            {
                decimal NoOfScripts = UtilizationCommon.CalculateNoofScripts("DentalFillings", Resources.Constants.ColOrthoCostFactor,
                    Resources.Constants.ColDentalFillingsFactor, objRequest, "C");
                Util = UtilizationCommon.CalculateUtilStat(Resources.Constants.ColDentalFillings);
                decimal DFChildInNet = NoOfScripts * objEntity.ChildStatus * objEntity.NewChildDentalStatusCode * Util;
                return DFChildInNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalulateDFEmpOutNet(CommonEntities objEntity)
        {
            try
            {
                decimal DFEmpOutNet = Util * objEntity.EEStatus * objEntity.NewEmpDentalStatusCode;
                return DFEmpOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private decimal CalculateDFSpouseOutNet(CommonEntities objEntity)
        {
            try
            {
                decimal DFSpouseOutNet = Util * objEntity.SpouseStatus * objEntity.NewSpouseDentalStatusCode;
                return DFSpouseOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal CalculateDFChildOutNet(CommonEntities objEntity)
        {
            try
            {
                decimal DFChildOutNet = Util * objEntity.ChildStatus * objEntity.NewChildDentalStatusCode;
                return DFChildOutNet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}