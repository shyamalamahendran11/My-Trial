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
    public class UtilizationCoverageDAL
    {
         public static decimal CalculateUtilization(string colname, int utillvel)
        {
            try
            {
                string sql = @"Select ? from [dbo].[Utilization] Where UtilizationLevel= ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, colname);
                ps.SetInt(2, utillvel);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal CalculateUtilStat(string colname)
        {
            try
            {
                string sql = @"Select ? from [dbo].[UtilStat] ";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, colname);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}