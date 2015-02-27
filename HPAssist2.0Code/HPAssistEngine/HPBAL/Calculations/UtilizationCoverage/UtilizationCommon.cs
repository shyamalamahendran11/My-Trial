#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HPAssistEngine.Models;
using HPAssistEngine.HPDAL;
using HPAssistEngine.Models.Entities;
using System.Resources;
using System.Xml;
#endregion

namespace HPAssistEngine.HPBAL.Calculations.UtilizationCoverage
{
    public class UtilizationCommon
    {

        public static decimal CalculateUtilStat(string colName)
        {
            try
            {
                decimal utilstat = UtilizationCoverageDAL.CalculateUtilStat(colName);
                return utilstat;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateUtilization(string colName, int utilLevel)
        {
            try
            {
                decimal util = UtilizationCoverageDAL.CalculateUtilization(colName, utilLevel);
                return util;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateMiscCostsAndFactors(string colname)
        {
            try
            {
                decimal micconst = CommonDAL.CalculateMiscCostsAndFactors(colname);
                return micconst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateNoofScripts(string util, string areaCol, string ageCol, CostEstimate objRequest, string gender, int age = 0, string misConsCol = null)
        {
            decimal NoOfScripts = 0;
            string Inpzipcode = Convert.ToString(objRequest.Zipcode);
            Inpzipcode = Inpzipcode.PadLeft(5, '0');
            Inpzipcode = Inpzipcode.Substring(0, 3);
            int zipCode;
            int.TryParse(Inpzipcode, out zipCode);
            decimal areaFactor = CommonDAL.CalculateAreaFactor(areaCol, zipCode);

            decimal agegender;
            if (Convert.ToChar(gender) == 'C')
            {
                agegender = CommonDAL.CalculateAgeGender(ageCol, Convert.ToChar(gender));
            }
            else
            {
                agegender = CommonDAL.CalculateAgeGender(ageCol, Convert.ToChar(gender), Convert.ToInt16(age));
            }

            decimal micconst = CalculateMiscCostsAndFactors(misConsCol);
            switch (util)
            {
                case Resources.Constants.PrescDrug:
                    NoOfScripts = areaFactor * agegender * micconst;
                    break;
                case Resources.Constants.OfficeVisits:
                    NoOfScripts = areaFactor * agegender * micconst;
                    break;
                case Resources.Constants.SpecialistVisits:
                    NoOfScripts = areaFactor * agegender * micconst;
                    break;
                case Resources.Constants.Dental:
                    NoOfScripts = areaFactor * agegender;
                    break;
                case Resources.Constants.Vision:
                    NoOfScripts = areaFactor * agegender;
                    break;
                default:
                    break;
            }
            return NoOfScripts;
        }


    }

}

