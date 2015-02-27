#region
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HPAssistEngine.Models;
using NetUtilLib.Data;

#endregion

namespace HPAssistEngine.HPDAL
{
    public class CommonDAL
    {
        public static SqlServerDataUtil GetConnectionString()
        {
            try
            {
                string clientConnString = Convert.ToString(ConfigurationManager.ConnectionStrings["MasterConnectionString"]);
                return (SqlServerDataUtil)SqlServerDataUtil.GetInstance(clientConnString);
            }
            catch (Exception ex)
            { throw ex; }
        }

        public static int InsertIntoMasterTransaction(string UserName, string CompanyCode)
        {
            try
            {
                string sql = @"Insert Into [dbo].[ServiceTransaction]  (UserName, CompanyCode, LastUpdatedAt) values (?,?,GETDATE());SELECT CAST(scope_identity() AS int)";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetString(1, UserName);
                ps.SetString(2, CompanyCode);
                int i = Convert.ToInt32(ps.ExecuteScalar());
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void InsertRequestResponseXmlIntoDB(int transId, string request, string response, string dbName, string type = null)
        {
            string sql;
            if (response == null && type == null)
            {
                sql = @"Insert Into " + dbName + ".[dbo].[ServiceTransaction] values (?,?,?,?,?)";
                SqlServerPreparedStatement ps2 = new SqlServerPreparedStatement(sql, GetConnectionString());
                ps2.SetInt(1, transId);
                ps2.SetString(2, request);
                ps2.SetString(3, response);
                ps2.SetString(4, "");
                ps2.SetString(5, "");
                ps2.ExecuteNonQuery();
            }
            else
            {
                    sql = @"Update " + dbName + ".[dbo].[ServiceTransaction] set GenerateCostResponseXml =? where TransactionId=? ";
                    SqlServerPreparedStatement ps2 = new SqlServerPreparedStatement(sql, GetConnectionString());
                    ps2.SetString(1, response);
                    ps2.SetInt(2, transId);
                    ps2.ExecuteNonQuery();
            }
        }

        public static void InsertLogIntoDB(HP_Logging objLog)
        {
            try
            {
                string sql = @"Insert Into [dbo].[ServiceLog] (LogDateTime,LogType,LogSource,LogTitle,LogMessage,ServerName,UserName,IPAddress) values (?,?,?,?,?,?,?,?)";
                SqlServerPreparedStatement ps2 = new SqlServerPreparedStatement(sql, GetConnectionString());
                ps2.SetDateTime(1, objLog.LogDateTime);
                ps2.SetString(2, objLog.LogType);
                ps2.SetString(3, objLog.LogSource);
                ps2.SetString(4, objLog.LogTitle);
                ps2.SetString(5, objLog.LogMessage);
                ps2.SetString(6, objLog.ServerName);
                ps2.SetString(7, objLog.UserName);
                ps2.SetString(8, objLog.IPAddress);
                ps2.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string AuthenticateUser(string apiKey, string companyCode, string username)
        {
            try
            {
                string dbName = CommonDAL.GetClientDataBase(apiKey);
                string result;
                if (dbName != "")
                {
                    string sql = @"SELECT 'User' FROM ?.[dbo].[Models] WHERE CompanyCode=?";
                    SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, GetConnectionString());
                    ps.SetValue(1, dbName);
                    ps.SetString(2, companyCode);
                    object obj = ps.ExecuteScalar();
                    result = obj != null ? Convert.ToString(obj) : "";
                }
                else result = "";
                return result;
            }
            catch (Exception ex)
            { throw ex; }
        }

        public static string GetClientDataBase(string apikey)
        {
            try
            {
                string sql = @"SELECT DataBaseName FROM Agents WHERE APIKey= ?";
                SqlServerPreparedStatement ps1 = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps1.SetString(1, apikey);
                string dbName = Convert.ToString(ps1.ExecuteScalar());
                return dbName;
            }
            catch (Exception ex) { throw ex; }
        }

        public static IDataReader GetMedicalPlanDesign(int medicalId, string dbName)
        {
            try
            {
                string sql = @"SELECT MedicalID, ModelID, PlanName, Deductible, FamilyDeductible,FamilyOOPMax, MemberCoinsurance/100 MemberCoinsurance,
                OOPMax, OVCopay, OVCoinsurance/100 OVCoinsurance, SpecialistCopay, SpecialistCoinsurance/100 SpecialistCoinsurance, InNetDiscount/100 InNetDiscount, 
                OutNetDiscount/100 OutNetDiscount, OutNetDeductible, OutNetFamilyDeductible, OutNetFamilyOOPMax,OutNetMemberCoinsurance/100 OutNetMemberCoinsurance, 
                OutNetOOPMax, NetworkUtil/100 NetworkUtil, RxCopayRetailGeneric,RxCopayRetailForm,RxCopayRetailNonForm,RxCopayMailGeneric, RxCopayMailForm,RxCopayMailNonForm, 
                CPDTableDeductible, CPDTableOOPMax, RxCoInsuranceRetailGeneric/100 RxCoInsuranceRetailGeneric, RxCoInsuranceRetailForm/100 RxCoInsuranceRetailForm,
                RxCoInsuranceRetailNonForm/100 RxCoInsuranceRetailNonForm, RxCopayMaxRetailGeneric, RxCopayMinRetailGeneric, RxCopayMaxRetailForm, RxCopayMinRetailForm, 
                RxCopayMaxRetailNonForm,RxCopayMinRetailNonForm, RxCopayMaxMailGeneric, RxCopayMinMailGeneric, RxCopayMaxMailForm,RxCopayMinMailForm, RxCopayMaxMailNonForm, 
                RxCopayMinMailNonForm, RxIncludedInDeductibleAmt,CPDTableOutNetwork, RxOOPMax, RxMACDiscount/100 RxMACDiscount, RxAWPDiscount/100 RxAWPDiscount, 
                RxCoInsuranceMailGeneric/100 RxCoInsuranceMailGeneric, RxCoInsuranceMailForm/100 RxCoInsuranceMailForm, RxCoInsuranceMailNonForm/100 RxCoInsuranceMailNonForm  
                FROM ?.[dbo].[Medical] WHERE MedicalId =?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, dbName);
                ps.SetInt(2, medicalId);
                System.Data.IDataReader dr = ps.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IDataReader GetDentalPlanDesign(int dentalId, string dbName)
        {
            try
            {

                string sql = @"SELECT DentalID, ModelID, PlanName, ClassIDeductible, ClassIIDeductible, 
                ClassIIIDeductible, ClassICoinsurance/100 ClassICoinsurance,  ClassIICoinsurance/100 ClassIICoinsurance, 
                ClassIIICoinsurance/100 ClassIIICoinsurance,AnnualMax, OrthoMax, OutNetClassIDeductible, OutNetClassIIDeductible, 
                OutNetClassIIIDeductible, OutNetClassICoinsurance/100 OutNetClassICoinsurance,OutNetClassIICoinsurance/100 OutNetClassIICoinsurance, 
                OutNetClassIIICoinsurance/100 OutNetClassIIICoinsurance, OutNetAnnualMax, OutNetOrthoMax, NetworkUtil/100 NetworkUtil, 
                InNetDiscount/100 InNetDiscount, OutNetDiscount/100 OutNetDiscount, OrthoLengthOfCoverage, Status FROM ?.[dbo].[Dental] WHERE DentalId =?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, dbName);
                ps.SetInt(2, dentalId);
                System.Data.IDataReader dr = ps.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IDataReader GetVisionPlanDesign(int visionId, string dbName)
        {
            try
            {
                string sql = @"SELECT * FROM " + dbName + ".[dbo].[Vision] WHERE VisionId =?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetInt(1, visionId);
                System.Data.IDataReader dr = ps.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //Common for both OOP and UtilCoverage
        public static decimal CalculateAreaFactor(string factor, int zipcode)
        {
            try
            {
                string sql = @"Select ? from [dbo].[AreaFactor] Where zipcode= ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, factor);
                ps.SetInt(2, zipcode);
                object area = ps.ExecuteScalar();
                return Convert.ToDecimal(area != null ? area : 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static decimal CalculateAgeGender(string factor, char gender, int age = 0)
        {
            try
            {
                string sql = @"Select ?  from [dbo].[AgeGender] Where Gender= ? and ? between MinAge and MaxAge";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetValue(1, factor);
                ps.SetString(2, Convert.ToString(gender));
                ps.SetInt(3, age);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static decimal CalculateMiscCostsAndFactors(string name)
        {
            try
            {
                string sql = @"Select Value from [dbo].[MiscCostsandFactors] Where Name= ?";
                SqlServerPreparedStatement ps = new SqlServerPreparedStatement(sql, CommonDAL.GetConnectionString());
                ps.SetString(1, name);
                return Convert.ToDecimal(ps.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int InsertCompareCostRequestResponseXmlIntoDB(int transId, string request, string response, string dbName, string type = null)
        {
            string sql;
            if (type == "CompareCostRequest" && response == "")
            {
               sql = @"Insert Into " + dbName + ".[dbo].[ServiceTransaction] values (?,?,?,?,?);SELECT CAST(scope_identity() AS int)";
                SqlServerPreparedStatement ps3 = new SqlServerPreparedStatement(sql, GetConnectionString());
                ps3.SetInt(1, transId);
                ps3.SetString(2, null);
                ps3.SetString(3, null);
                ps3.SetString(4, request);
                ps3.SetString(5, "");
                int i = Convert.ToInt32(ps3.ExecuteScalar());
                return i;
            }
            else
            {
                sql = @"Update " + dbName + ".[dbo].[ServiceTransaction] set CompareCostlResponseXml = ? where ID=? ";
                SqlServerPreparedStatement ps3 = new SqlServerPreparedStatement(sql, GetConnectionString());
                ps3.SetString(1, response);
                ps3.SetInt(2, transId);
                ps3.ExecuteNonQuery();
                return 0;
            }

        }

    }
}


