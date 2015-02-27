using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HPAssistEngine.Models;
using NetUtilLib.Data;

namespace HPAssistEngine.HPDAL
{
    public class OOPDAL
    {
        public static decimal CalculateDentalWeightTable(string factor, int Dentalweightid)
        {
            try
            {
                string sql = @"Select ? from [dbo].[Dentalweights] Where DentalWeightsID = ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, factor);
                ps.SetInt(2, Dentalweightid);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static decimal CalculateAdjustedCPDDental(string colname, string Tier, decimal value)
        {
            try
            {
                string sql = @"Select ? from [dbo].[CCPD_Dental] d Where Tier=? and d.TotalAnnualClaimAmount = (Select MAX(TotalAnnualClaimAmount) from CCPD_Dental d1 where Tier = ? and d1.TotalAnnualClaimAmount <= ?)";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, colname);
                ps.SetString(2, Tier);
                ps.SetString(3, Tier);
                ps.SetValue(4, value);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static decimal GetAnnClaimAmount(string tier, decimal totalannualclaim, string colname)
        {
            try
            {
                string sql;
                if (totalannualclaim == 0)
                    sql = @"SELECT ? FROM [dbo].[CCPD_Medical] WHERE Tier =? and TotalAnnualClaimAmount =?";
                else

                    sql = @"SELECT ? FROM [dbo].[CCPD_Medical] WHERE Tier =? and TotalAnnualClaimAmount =(SELECT  MAX(TotalAnnualClaimAmount) FROM [dbo].[CCPD_Medical] where TotalAnnualClaimAmount<=? and Tier =? )";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, colname);
                ps.SetString(2, tier);
                ps.SetValue(3, totalannualclaim);
                ps.SetString(4, tier);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static decimal CalculateMiscCostsAndFactors()
        {
            try
            {
                string sql = @"Select Value from [dbo].[MiscCostsandFactors] Where Name= 'MedBaseAdjFactor'";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static decimal GetRXDistributionofCharges(string type, string Source, string colname)
        {
            try
            {
                string sql = @"SELECT ? FROM dbo.RXDistributionofCharges WHERE  Type = ? and Source = ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, colname);
                ps.SetString(2, type);
                ps.SetString(3, Source);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetCPDTable(string CPDCode)
        {
            try
            {
                string sql = @"SELECT CPDValue FROM dbo.CPDTable WHERE  CPDCode =?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetString(1, CPDCode);
                string i = Convert.ToString(ps.ExecuteScalar());
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetRequestXMLFromDB(int transID, string dbName)
        {
            try
            {
                string sql2 = @"Select TOP 1 GenerateCostRequestXml from " + dbName + " .[dbo].[ServiceTransaction] where TransactionID = ? AND GenerateCostRequestXml IS NOT NULL ORDER BY ID ASC";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql2, CommonDAL.GetConnectionString());
                ps.SetInt(1, transID);
                return Convert.ToString(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}