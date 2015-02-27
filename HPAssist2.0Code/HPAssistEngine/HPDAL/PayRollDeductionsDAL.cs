using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetUtilLib.Data;
using HPAssistEngine.Models.Entities;

namespace HPAssistEngine.HPDAL
{
    public class PayRollDeductionsDAL
    {
        public static decimal CalculateHRAContribution(string TierType, string dbName, int MedicalPlanId)
        {
            try
            {
                string sql = @"SELECT ? FROM  ?.[dbo].[HRATier] WHERE MedicalId =?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, TierType);
                ps.SetValue(2, dbName);
                ps.SetInt(3, MedicalPlanId);
                decimal HRAcontribution = Convert.ToDecimal(ps.ExecuteScalar());
                return HRAcontribution;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateEmpMedicalContribution(int MedPlanid, string TierType, string dbName, string DeductionBand = "", string SpousesurCharge = "", string WellNessPlan = "", string TobaccoUse = "", int ModelId = 0)
        {
            try
            {
                string sql;
                if (DeductionBand != "No")
                {
                    sql = @"SELECT ? FROM ?.[dbo].[MedTier] WHERE MedicalId = ?  AND PlanDedBandId IN (SELECT Id from ?.[dbo].[PlanDedBand]  WHERE ModelId = ? AND DeductionBand =?) And ISNULL(SpouseSurCharge,'No')= ? And ISNULL(WellnessPlan,'No') = ? And ISNULL(TobaccoUse,'No' )= ?";
                    SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                    ps.SetValue(1, TierType);
                    ps.SetValue(2, dbName);
                    ps.SetInt(3, MedPlanid);
                    ps.SetValue(4, dbName);
                    ps.SetInt(5, ModelId);
                    ps.SetString(6, DeductionBand);
                    ps.SetString(7, SpousesurCharge);
                    ps.SetString(8, WellNessPlan);
                    ps.SetString(9, TobaccoUse);
                    return Convert.ToDecimal(ps.ExecuteScalar());
                }
                else
                {
                    sql = @"SELECT ? FROM ?.[dbo].[MedTier] WHERE MedicalId = ? AND ISNULL(PlanDedBandId,0)=0 And ISNULL(SpouseSurCharge,'No')= ? And ISNULL(WellnessPlan,'No') = ? And ISNULL(TobaccoUse,'No')= ?";
                    SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                    ps.SetValue(1, TierType);
                    ps.SetValue(2, dbName);
                    ps.SetValue(3, MedPlanid);
                    ps.SetString(4, SpousesurCharge);
                    ps.SetString(5, WellNessPlan);
                    ps.SetString(6, TobaccoUse);
                    return Convert.ToDecimal(ps.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateEmpDentalContribution(int DentPlanId, string TierType, string dbName)
        {
            try
            {
                string sql = @"SELECT ? FROM ?.[dbo].[DenTier] WHERE DentalId= ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, TierType);
                ps.SetValue(2, dbName);
                ps.SetInt(3, DentPlanId);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateEmpVisionContribution(int Visplanid, string TierType, string dbName)
        {
            try
            {
                string sql = @"SELECT ? FROM ?.[dbo].[VisTier] WHERE VisionId= ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, TierType);
                ps.SetValue(2, dbName);
                ps.SetInt(3, Visplanid);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}