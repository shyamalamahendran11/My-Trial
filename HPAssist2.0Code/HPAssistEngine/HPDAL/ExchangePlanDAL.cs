using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetUtilLib.Data;
using System.Data;
using HPAssistEngine.Resources;

namespace HPAssistEngine.HPDAL
{
    public class ExchangePlanDAL
    {
        public static bool CheckIfExchangePlanExist(string dbName, string CompanyCode)
        {
            try
            {
                string sql = @"SELECT ExchangePlan FROM ?.[dbo].[Models] WHERE CompanyCode = ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, dbName);
                ps.SetString(2, CompanyCode);
                bool i = Convert.ToBoolean(ps.ExecuteScalar());
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetExchangePlanId(string state, string Ratingarea)
        {
            try
            {
                string sql = @"SELECT top 1 PlanId  FROM  SilverStatePremiums WHERE State = ? and RatingArea=?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetString(1, state);
                ps.SetString(2, Ratingarea);
                string i = Convert.ToString(ps.ExecuteScalar());
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IDataReader GetExchangePlanDesign(string ExchangePlanId)
        {
            try
            {
                string sql = @"  SELECT ExPlanID, Deductible, FamilyDeductible, FamilyOOPMax, MemberCoinsurance/100 MemberCoinsurance, 
                OOPMax, OVCopay, OVCoinsurance/100 OVCoinsurance, SpecialistCopay, SpecialistCoinsurance/100 SpecialistCoinsurance, 
                InNetDiscount/100 InNetDiscount, OutNetDiscount/100 OutNetDiscount, OutNetDeductible, OutNetFamilyDeductible, 
                OutNetFamilyOOPMax, OutNetMemberCoinsurance/100 OutNetMemberCoinsurance, OutNetOOPMax, NetworkUtil/100 NetworkUtil, 
                RxCopayRetailGeneric, RxCopayRetailForm, RxCopayRetailNonForm, RxCopayMailGeneric, 
                RxCopayMailForm, RxCopayMailNonForm, CPDTableDeductible, CPDTableOOPMax, 
                RxCoInsuranceRetailGeneric/100 RxCoInsuranceRetailGeneric, RxCoInsuranceRetailForm/100 RxCoInsuranceRetailForm, RxCoInsuranceRetailNonForm/100 RxCoInsuranceRetailNonForm, 
                RxCopayMaxRetailGeneric, RxCopayMinRetailGeneric, RxCopayMaxRetailForm, RxCopayMinRetailForm, 
                RxCopayMaxRetailNonForm, RxCopayMinRetailNonForm, RxCopayMaxMailGeneric, RxCopayMinMailGeneric, 
                RxCopayMaxMailForm, RxCopayMinMailForm, RxCopayMaxMailNonForm, RxCopayMinMailNonForm, 
                RxIncludedInDeductibleAmt, CPDTableOutNetwork, RxOOPMax, RxMACDiscount/100 RxMACDiscount, RxAWPDiscount/100 RxAWPDiscount,
                RxCoInsuranceMailGeneric/100 RxCoInsuranceMailGeneric, RxCoInsuranceMailForm/100 RxCoInsuranceMailForm, RxCoInsuranceMailNonForm/100  RxCoInsuranceMailNonForm
                FROM dbo.ExchangePlans WHERE ExPlanId =?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetString(1, ExchangePlanId);
                System.Data.IDataReader dr = ps.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateAreaFactor(string state, int Age)
        {
            try
            {
                string sql;
                if (Age <= 64)
                    sql = @"SELECT Value FROM dbo.Age_Curves WHERE State = ?and Age = ?";
                else
                    sql = @"SELECT Value FROM dbo.Age_Curves WHERE State = ?and Age = 64";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetString(1, state);
                ps.SetInt(2, Age);
                decimal area = Convert.ToDecimal(ps.ExecuteScalar());
                return area;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetState(int zipcode)
        {
            try
            {
                string sql = @"SELECT State from [dbo].[AreaFactor] Where zipcode= ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetInt(1, zipcode);
                string state = Convert.ToString(ps.ExecuteScalar());
                return state;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetRatingArea(int ZipCode, int ThreeDigitZip)
        {
            try
            {
                string sql = @"SELECT Top 1 RatingArea FROM SilverStatePremiums WHERE ZipCode = ?  ORDER BY State, County";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetInt(1, ZipCode);

                string RatingArea = Convert.ToString(ps.ExecuteScalar());

                if (string.IsNullOrEmpty(RatingArea))
                {
                    sql = @"SELECT Top 1 RatingArea FROM SilverStatePremiums WHERE ThreeDigitZipCode = ? ORDER BY State, County";
                    SqlServerPreparedStatement ps1 = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                    ps1.SetInt(1, ThreeDigitZip);
                    RatingArea = Convert.ToString(ps1.ExecuteScalar());
                }
                return RatingArea;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static int GetModelId(string dbName, string CompanyCode)
        {
            try
            {
                string sql = @"SELECT ModelId FROM ?.dbo.Models WHERE CompanyCode =?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, dbName);
                ps.SetString(2, CompanyCode);
                int ModelId = Convert.ToInt16(ps.ExecuteScalar());
                return ModelId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateMonthlyPremium(string state, string RatingArea)
        {
            try
            {
                string StateRatingArea = state + " - " + RatingArea;
                string sql = @"SELECT Top 1 SilverPremium FROM SilverStatePremiums WHERE StateRatingArea=?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetString(1, StateRatingArea);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            { throw ex; }
        }

        public static decimal CalculateIndividualPenality(int salary, int statussum, string TaxPercentage, string TaxDollar, string MaxIndividualPenality, string HHI)
        {
            try
            {
                decimal IndividualPenality, HHIthreshold, IndiTaxPercentagee, IndiTaxDollar, MaxIndiPenality;
                HHIthreshold = CommonDAL.CalculateMiscCostsAndFactors(HHI);
                IndiTaxPercentagee = CommonDAL.CalculateMiscCostsAndFactors(TaxPercentage);
                IndiTaxDollar = CommonDAL.CalculateMiscCostsAndFactors(TaxDollar);
                MaxIndiPenality = CommonDAL.CalculateMiscCostsAndFactors(MaxIndividualPenality);
                IndividualPenality = Math.Max((salary - HHIthreshold) * IndiTaxDollar, Math.Min((statussum * IndiTaxDollar), MaxIndiPenality));
                return IndividualPenality;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculatePovertLevel(decimal salary, string state, int status)
        {
            try
            {
                decimal PovertLevel;
                string sql;
                if (status <= 15)
                    sql = @"SELECT Value FROM dbo.Poverty_Guidelines WHERE State = ? and HouseholdMembers = ?";
                else
                    sql = @"SELECT Value FROM dbo.Poverty_Guidelines WHERE State = ? and HouseholdMembers = 15";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetString(1, state);
                ps.SetInt(2, status);
                decimal i = Convert.ToDecimal(ps.ExecuteScalar());
                PovertLevel = salary / i;
                return PovertLevel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateERPlan(decimal LowestEEContribution, decimal salary)
        {
            try
            {
                return (LowestEEContribution * 12) / salary;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public static string TaxCreditEligibility(decimal salary, string state, int status, decimal LowestEEContribution)
        {
            if (CalculatePovertLevel(salary, state, status) < 4 & Convert.ToDouble(CalculateERPlan(LowestEEContribution, salary)) > 0.095)
                return Constants.Eligible;
            else
                return Constants.NotEligible;
        }

        public static decimal CalculatePercentageOfIncome(decimal HHI)
        {
            string sql = @"SELECT PercentofIncome FROM dbo.Premium_Credits WHERE [HHI/FPL] in (SELECT Max([HHI/FPL]) FROM dbo.Premium_Credits  WHERE [HHI/FPL]<=?) ";
            SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
            ps.SetValue(1, HHI);
            decimal PercentageOfIncome = Convert.ToDecimal(ps.ExecuteScalar());
            return PercentageOfIncome;
        }

        public static decimal GetLowestEEContribution(int ModelId, string dbName, string DeductionBand)
        {
            string sql = @"SELECT ISNULL(LowestEEContribution,0) FROM ?.dbo.PlanDedBand WHERE ModelId = ? and isnull(DeductionBand,'') =?";
            SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
            ps.SetValue(1, dbName);
            ps.SetValue(2, ModelId);
            ps.SetString(3, DeductionBand);
            decimal LowestEEContribution =  Convert.ToDecimal(ps.ExecuteScalar());
            return LowestEEContribution;
        }
    }
}