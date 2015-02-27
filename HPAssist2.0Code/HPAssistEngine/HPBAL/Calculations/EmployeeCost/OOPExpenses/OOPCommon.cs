using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HPAssistEngine.Models;
using HPAssistEngine.HPDAL;
using HPAssistEngine.Models.Entities;
using System.Resources;
using System.Xml;

namespace HPAssistEngine.HPBAL.Calculations.OOPExpenses
{
    public class OOPCommon
    {
        public static decimal CalculateAreaFactor(int zipcode, string areaColname)
        {
            try
            {
                string inpzipcode = Convert.ToString(zipcode);
                inpzipcode = inpzipcode.PadLeft(5, '0');
                inpzipcode = inpzipcode.Substring(0, 3);
                int zipCode;
                int.TryParse(inpzipcode, out zipCode);
                decimal areaFactor = CommonDAL.CalculateAreaFactor(areaColname, zipCode);
                return areaFactor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static decimal CalculateAgeGenderFactor(string ageColName, string gender, int age = 0)
        {
            try
            {
                decimal agegender;
                if (Convert.ToChar(gender.ToUpper()) == 'C')
                {
                    agegender = CommonDAL.CalculateAgeGender(ageColName, Convert.ToChar(gender));
                }
                else
                {
                    agegender = CommonDAL.CalculateAgeGender(ageColName, Convert.ToChar(gender), Convert.ToInt16(age));
                }
                return agegender;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}