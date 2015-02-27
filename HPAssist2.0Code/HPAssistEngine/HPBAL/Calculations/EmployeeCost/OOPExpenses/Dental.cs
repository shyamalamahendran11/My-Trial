#region NameSpaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HPAssistEngine.Models;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.HPDAL;
using System.Resources;
using System.Xml;
using System.Globalization;
using OOPCalculation;
using HPAssistEngine.HPBAL.Calculations.OOPExpenses;
#endregion
namespace HPAssistEngine.HPBAL.Calculations.EmployeeCost
{
    public class Dental
    {
        #region DentalEntity
        //DentalIn-DentalOut Entity object creation
        DentalInNetwork objDentalIn = new DentalInNetwork();
        DentalOutNetwork objDentalOut = new DentalOutNetwork();
        #endregion

        #region VariableDeclaration
        //Class level variable Declaration
        decimal plandesignClass1, plandesignCalss2, plandesignClass3,
                   dentalWeight1, dentalWeight2, dentalWeight3, sumDentalweight,
                   deductibleTrigger1, deductibleTrigger2, deductibleTrigger3, sumDeductibleTrigger, deductibleTriggerAvg,
                   memberCoinsuranceAvg1, memberCoinsuranceAvg2, memberCoinsuranceAvg3, sumMemberCoinsuranceAvg, memberCoinsuranceAverage;
        #endregion

        #region Methods
        //Calculating TotalDentalCosts - H146
        public decimal CalculateDentalCost(CostEstimate objRequest, PlanDesignDental objDentalEntity, CommonEntities objEntity)
        {
            try
            {
                decimal totalCovrageDentalCost, totalAnnualBenefitMax, totalDentalCosts;
                totalAnnualBenefitMax = CalculateAnnualBenefitMax(objRequest, objDentalEntity, objEntity);
                totalCovrageDentalCost = CalculateCoverageDentalCost(objRequest, objDentalEntity, objEntity);
                totalDentalCosts = totalCovrageDentalCost + totalAnnualBenefitMax;
                objDentalIn.totalDentalCost = Math.Round(totalDentalCosts, 2);
                return objDentalIn.totalDentalCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating Employee,Spouse,Child InnetworkValues
        private decimal CalculateAnnualBenefitMax(CostEstimate objRequest, PlanDesignDental objDentalEntity, CommonEntities objEntity)
        {
            try
            {
                decimal dentalCostEmpInNet, dentalCostSpouseInNet, dentalCostChildInNet;
                dentalCostEmpInNet = CalculateDentalCostEmpInNet(objRequest, objDentalEntity, objEntity.EEStatus, objEntity.NewEmpDentalStatusCode);
                dentalCostSpouseInNet = objRequest.Spouse.SpouseDentalPlanExist == 1 ? CalculateDentalCostSpouseInNet(objRequest, objDentalEntity, objEntity.SpouseStatus, objEntity.NewSpouseDentalStatusCode) : 0;
                dentalCostChildInNet = objRequest.Children.ChildDentalPlanExist == 1 ? CalculateDentalCostChildInNet(objRequest, objDentalEntity, objEntity.NewChildDentalStatusCode) : 0;
                objDentalIn.totalAnnualBenefitMax = dentalCostEmpInNet + dentalCostSpouseInNet + dentalCostChildInNet;
                return objDentalIn.totalAnnualBenefitMax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating Employee,Spouse,Child OutnetworkValues
        private decimal CalculateCoverageDentalCost(CostEstimate objRequest, PlanDesignDental objDentalEntity, CommonEntities objEntity)
        {
            try
            {
                decimal dentalCostEmpOutNet, dentalCostSpouseOutNet, dentalCostChildOutNet;
                dentalCostEmpOutNet = CalculateDentalCostEmpOutNet(objRequest.Zipcode, objRequest.Employee.EmpGender, objRequest.Employee.EmpAge, objDentalEntity, objEntity.EEStatus, objEntity.NewEmpDentalStatusCode);
                dentalCostSpouseOutNet = objRequest.Spouse.SpouseDentalPlanExist == 1 ? CalculateDentalCostSpouseOutNet(objRequest.Zipcode, objRequest.Spouse.SpouseGender, objRequest.Spouse.SpouseAge, objDentalEntity, objEntity.SpouseStatus, objEntity.NewSpouseDentalStatusCode) : 0;
                dentalCostChildOutNet = objRequest.Children.ChildDentalPlanExist == 1 ? CalculateDentalCostChildOutNet(objRequest.Zipcode, objDentalEntity, objEntity.ChildStatus, objEntity.NewChildDentalStatusCode) : 0;
                objDentalIn.totalCoverageDentalCost = dentalCostEmpOutNet + dentalCostSpouseOutNet + dentalCostChildOutNet;
                return objDentalIn.totalCoverageDentalCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region InNetworkMethods
        //Calculating EmployeeInNetwork Values
        private decimal CalculateDentalCostEmpInNet(CostEstimate objRequest, PlanDesignDental objDentalEntity, int EEstatus, int eeDentalStatus)
        {
            try
            {
                objDentalIn.empInAAGFactorAverage = CalculateAAGFactorAvg(objRequest.Zipcode, Resources.Constants.ColDentalFactor, objRequest.Employee.EmpGender, Resources.Constants.ColDentalweightAdult, objRequest.Employee.EmpAge);

                objDentalIn.empIndeductibleTrigger = CalculateDeductibleTriggerAverage(Resources.Constants.ColDentalweightAdult, objDentalEntity.ClassIDeductible, objDentalEntity.ClassIIDeductible, objDentalEntity.ClassIIIDeductible);
                objDentalIn.empInMemCoinAvg = CalculateMemberCoinsuranceAverage(Resources.Constants.ColDentalweightAdult, objDentalEntity.ClassICoinsurance, objDentalEntity.ClassIICoinsurance, objDentalEntity.ClassIIICoinsurance);
                objDentalIn.empInannualMaxTrigger = CalculateAnnualMaxTrigger(objDentalIn.empInMemCoinAvg, objDentalIn.empIndeductibleTrigger, objDentalEntity.AnnualMax);

                objDentalIn.empInValueofDeductible = CalculateValueofDeductible(Resources.Constants.ColTierTypeA, objDentalEntity.InNetDiscount, objDentalIn.empInAAGFactorAverage, objDentalIn.empIndeductibleTrigger);
                objDentalIn.empInValueofCoin = CalculateValueofCoinsurance(Resources.Constants.ColTierTypeA, objDentalEntity.InNetDiscount, objDentalIn.empInAAGFactorAverage, objDentalIn.empIndeductibleTrigger, objDentalIn.empInMemCoinAvg, objDentalIn.empInannualMaxTrigger);
                objDentalIn.empInvalaboveAnnualMax = CalculateValueaboveAnnualMax(Resources.Constants.ColTierTypeA, objDentalEntity.InNetDiscount, objDentalIn.empInAAGFactorAverage, objDentalIn.empInannualMaxTrigger);

                objDentalIn.empInDentalCost = CalculateDentalCost(objDentalIn.empInValueofDeductible, objDentalIn.empInValueofCoin, objDentalIn.empInvalaboveAnnualMax);
                objDentalIn.empInDentalCost = objDentalIn.empInDentalCost * EEstatus;

                objDentalIn.empInAreaFactor = OOPCommon.CalculateAreaFactor(objRequest.Zipcode, Resources.Constants.ColOrthoCostFactor);
                objDentalIn.empInageGenderFactor = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColDentalOrthoFactor, objRequest.Employee.EmpGender, objRequest.Employee.EmpAge);
                objDentalIn.empInorthodontiaAAGFactor = CalculateOrthodontiaAAGFactor(objDentalIn.empInAreaFactor, objDentalIn.empInageGenderFactor);
                objDentalIn.empInOrthodontiaCosts = CalculateOrthodontiaCosts(objDentalIn.empInorthodontiaAAGFactor);
                objDentalIn.empInapplyCoinsAnnualBenefitMax = CalculateApplyCoinsAnnualBenefitMax(objDentalIn.empInOrthodontiaCosts, objDentalEntity.ClassIIICoinsurance, objDentalEntity.OrthoMax, objRequest.Employee.EmpOrthoDontic, eeDentalStatus);

                return objDentalIn.empInapplyCoinsAnnualBenefitMax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating SpouseInNetwork Values
        private decimal CalculateDentalCostSpouseInNet(CostEstimate objRequest, PlanDesignDental objDentalEntity, int spouseStatus, int spouseDentalStatus)
        {
            try
            {
                objDentalIn.spouseInAAGFactorAverage = CalculateAAGFactorAvg(objRequest.Zipcode, Resources.Constants.ColDentalFactor, objRequest.Spouse.SpouseGender, Resources.Constants.ColDentalweightAdult, objRequest.Spouse.SpouseAge);

                objDentalIn.spouseIndeductibleTrigger = CalculateDeductibleTriggerAverage(Resources.Constants.ColDentalweightAdult, objDentalEntity.ClassIDeductible, objDentalEntity.ClassIIDeductible, objDentalEntity.ClassIIIDeductible);
                objDentalIn.spouseInMemCoinAvg = CalculateMemberCoinsuranceAverage(Resources.Constants.ColDentalweightAdult, objDentalEntity.ClassICoinsurance, objDentalEntity.ClassIICoinsurance, objDentalEntity.ClassIIICoinsurance);
                objDentalIn.spouseInannualMaxTrigger = CalculateAnnualMaxTrigger(objDentalIn.spouseInMemCoinAvg, objDentalIn.spouseIndeductibleTrigger, objDentalEntity.AnnualMax);

                objDentalIn.spouseInValueofDeductible = CalculateValueofDeductible(Resources.Constants.ColTierTypeA, objDentalEntity.InNetDiscount, objDentalIn.spouseInAAGFactorAverage, objDentalIn.spouseIndeductibleTrigger);
                objDentalIn.spouseInValueofCoin = CalculateValueofCoinsurance(Resources.Constants.ColTierTypeA, objDentalEntity.InNetDiscount, objDentalIn.spouseInAAGFactorAverage, objDentalIn.spouseIndeductibleTrigger, objDentalIn.spouseInMemCoinAvg, objDentalIn.spouseInannualMaxTrigger);
                objDentalIn.spouseInvalaboveAnnualMax = CalculateValueaboveAnnualMax(Resources.Constants.ColTierTypeA, objDentalEntity.InNetDiscount, objDentalIn.spouseInAAGFactorAverage, objDentalIn.spouseInannualMaxTrigger);

                objDentalIn.spouseInDentalCost = CalculateDentalCost(objDentalIn.spouseInValueofDeductible, objDentalIn.spouseInValueofCoin, objDentalIn.spouseInvalaboveAnnualMax);
                objDentalIn.spouseInDentalCost = objDentalIn.spouseInDentalCost * spouseStatus;

                objDentalIn.spouseInAreaFactor = OOPCommon.CalculateAreaFactor(objRequest.Zipcode, Resources.Constants.ColOrthoCostFactor);
                objDentalIn.spouseInageGenderFactor = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColDentalOrthoFactor, objRequest.Spouse.SpouseGender, objRequest.Spouse.SpouseAge);
                objDentalIn.spouseInorthodontiaAAGFactor = CalculateOrthodontiaAAGFactor(objDentalIn.spouseInAreaFactor, objDentalIn.spouseInageGenderFactor);
                objDentalIn.spouseInOrthodontiaCosts = CalculateOrthodontiaCosts(objDentalIn.spouseInorthodontiaAAGFactor);
                objDentalIn.spouseInapplyCoinsAnnualBenefitMax = CalculateApplyCoinsAnnualBenefitMax(objDentalIn.spouseInOrthodontiaCosts, objDentalEntity.ClassIIICoinsurance, objDentalEntity.OrthoMax, objRequest.Spouse.SpouseOrthoDontic, spouseDentalStatus);

                return objDentalIn.spouseInapplyCoinsAnnualBenefitMax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating ChildInNetwork Values
        private decimal CalculateDentalCostChildInNet(CostEstimate objRequest, PlanDesignDental objDentalEntity, int childDentalStatus)
        {
            try
            {
                objDentalIn.childInAAGFactorAverage = CalculateAAGFactorAvg(objRequest.Zipcode, Resources.Constants.ColDentalFactor, Resources.Constants.ColChildGender, Resources.Constants.ColDentalweightChild);

                objDentalIn.childIndeductibleTrigger = CalculateDeductibleTriggerAverage(Resources.Constants.ColDentalweightChild, objDentalEntity.ClassIDeductible, objDentalEntity.ClassIIDeductible, objDentalEntity.ClassIIIDeductible);
                objDentalIn.childInMemCoinAvg = CalculateMemberCoinsuranceAverage(Resources.Constants.ColDentalweightChild, objDentalEntity.ClassICoinsurance, objDentalEntity.ClassIICoinsurance, objDentalEntity.ClassIIICoinsurance);
                objDentalIn.childInannualMaxTrigger = CalculateAnnualMaxTrigger(objDentalIn.childInMemCoinAvg, objDentalIn.childIndeductibleTrigger, objDentalEntity.AnnualMax);

                objDentalIn.childInValueofDeductible = CalculateValueofDeductible(Resources.Constants.ColTierTypeC, objDentalEntity.InNetDiscount, objDentalIn.childInAAGFactorAverage, objDentalIn.childIndeductibleTrigger);
                objDentalIn.childInValueofCoin = CalculateValueofCoinsurance(Resources.Constants.ColTierTypeC, objDentalEntity.InNetDiscount, objDentalIn.childInAAGFactorAverage, objDentalIn.childIndeductibleTrigger, objDentalIn.childInMemCoinAvg, objDentalIn.childInannualMaxTrigger);
                objDentalIn.childInvalaboveAnnualMax = CalculateValueaboveAnnualMax(Resources.Constants.ColTierTypeC, objDentalEntity.InNetDiscount, objDentalIn.childInAAGFactorAverage, objDentalIn.childInannualMaxTrigger);

                objDentalIn.childInDentalCost = CalculateDentalCost(objDentalIn.childInValueofDeductible, objDentalIn.childInValueofCoin, objDentalIn.childInvalaboveAnnualMax);

                objDentalIn.childInAreaFactor = OOPCommon.CalculateAreaFactor(objRequest.Zipcode, Resources.Constants.ColOrthoCostFactor);
                objDentalIn.childInageGenderFactor = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColDentalOrthoFactor, Resources.Constants.ColChildGender);
                objDentalIn.childInorthodontiaAAGFactor = CalculateOrthodontiaAAGFactor(objDentalIn.childInAreaFactor, objDentalIn.childInageGenderFactor);
                objDentalIn.childInOrthodontiaCosts = CalculateOrthodontiaCosts(objDentalIn.childInorthodontiaAAGFactor);
                objDentalIn.childInapplyCoinsAnnualBenefitMax = CalculateApplyCoinsAnnualBenefitMax(objDentalIn.childInOrthodontiaCosts, objDentalEntity.ClassIIICoinsurance, objDentalEntity.OrthoMax, objRequest.Children.ChildOrthoDontic, childDentalStatus);

                return objDentalIn.childInapplyCoinsAnnualBenefitMax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OutNetworkMethods
        //Calculating EmployeeOutNetwork Values
        private decimal CalculateDentalCostEmpOutNet(int zipcode, string empGender, int empAge, PlanDesignDental objDentalEntity, int eeStatus, int eeDentalStatus)
        {
            try
            {
                objDentalOut.empOutAAGFactorAverage = CalculateAAGFactorAvg(zipcode, Resources.Constants.ColDentalFactor, empGender, Resources.Constants.ColDentalweightAdult, empAge);

                objDentalOut.empOutdeductibleTrigger = CalculateDeductibleTriggerAverage(Resources.Constants.ColDentalweightAdult, objDentalEntity.OutNetClassIDeductible, objDentalEntity.OutNetClassIIDeductible, objDentalEntity.OutNetClassIIIDeductible);
                objDentalOut.empOutMemCoinAvg = CalculateMemberCoinsuranceAverage(Resources.Constants.ColDentalweightAdult, objDentalEntity.OutNetClassICoinsurance, objDentalEntity.OutNetClassIICoinsurance, objDentalEntity.OutNetClassIIICoinsurance);
                objDentalOut.empOutannualMaxTrigger = CalculateAnnualMaxTrigger(objDentalOut.empOutMemCoinAvg, objDentalOut.empOutdeductibleTrigger, objDentalEntity.OutNetAnnualMax);

                objDentalOut.empOutValueofDeductible = CalculateValueofDeductible(Resources.Constants.ColTierTypeA, objDentalEntity.OutNetDiscount, objDentalOut.empOutAAGFactorAverage, objDentalOut.empOutdeductibleTrigger);
                objDentalOut.empOutValueofCoin = CalculateValueofCoinsurance(Resources.Constants.ColTierTypeA, objDentalEntity.OutNetDiscount, objDentalOut.empOutAAGFactorAverage, objDentalOut.empOutdeductibleTrigger, objDentalOut.empOutMemCoinAvg, objDentalOut.empOutannualMaxTrigger);
                objDentalOut.empOutvalaboveAnnualMax = CalculateValueaboveAnnualMax(Resources.Constants.ColTierTypeA, objDentalEntity.OutNetDiscount, objDentalOut.empOutAAGFactorAverage, objDentalOut.empOutannualMaxTrigger);

                objDentalOut.empOutDentalCost = CalculateDentalCost(objDentalOut.empOutValueofDeductible, objDentalOut.empOutValueofCoin, objDentalOut.empOutvalaboveAnnualMax);
                objDentalOut.empOutDentalCost = objDentalOut.empOutDentalCost * eeStatus;

                objDentalOut.empOutTotBaseDentalCost = CalculateTotalBaseDentalCost(objDentalEntity.NetworkUtil, objDentalOut.empOutDentalCost, objDentalIn.empInDentalCost);

                objDentalOut.empOutApplyNoCovDentalCosts = objDentalOut.empOutTotBaseDentalCost * eeDentalStatus * eeStatus;

                return objDentalOut.empOutApplyNoCovDentalCosts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating SpouseOutNetwork Values
        private decimal CalculateDentalCostSpouseOutNet(int zipcode, string spouseGender, int spouseAge, PlanDesignDental objDentalEntity, int spouseStatus, int spouseDentalStatus)
        {
            try
            {
                objDentalOut.spouseOutAAGFactorAverage = CalculateAAGFactorAvg(zipcode, Resources.Constants.ColDentalFactor, spouseGender, Resources.Constants.ColDentalweightAdult, spouseAge);

                objDentalOut.spouseOutdeductibleTrigger = CalculateDeductibleTriggerAverage(Resources.Constants.ColDentalweightAdult, objDentalEntity.OutNetClassIDeductible, objDentalEntity.OutNetClassIIDeductible, objDentalEntity.OutNetClassIIIDeductible);
                objDentalOut.spouseOutMemCoinAvg = CalculateMemberCoinsuranceAverage(Resources.Constants.ColDentalweightAdult, objDentalEntity.OutNetClassICoinsurance, objDentalEntity.OutNetClassIICoinsurance, objDentalEntity.OutNetClassIIICoinsurance);
                objDentalOut.spouseOutannualMaxTrigger = CalculateAnnualMaxTrigger(objDentalOut.spouseOutMemCoinAvg, objDentalOut.spouseOutdeductibleTrigger, objDentalEntity.OutNetAnnualMax);

                objDentalOut.spouseOutValueofDeductible = CalculateValueofDeductible(Resources.Constants.ColTierTypeA, objDentalEntity.OutNetDiscount, objDentalOut.spouseOutAAGFactorAverage, objDentalOut.spouseOutdeductibleTrigger);
                objDentalOut.spouseOutValueofCoin = CalculateValueofCoinsurance(Resources.Constants.ColTierTypeA, objDentalEntity.OutNetDiscount, objDentalOut.spouseOutAAGFactorAverage, objDentalOut.spouseOutdeductibleTrigger, objDentalOut.spouseOutMemCoinAvg, objDentalOut.spouseOutannualMaxTrigger);
                objDentalOut.spouseOutvalaboveAnnualMax = CalculateValueaboveAnnualMax(Resources.Constants.ColTierTypeA, objDentalEntity.OutNetDiscount, objDentalOut.spouseOutAAGFactorAverage, objDentalOut.spouseOutannualMaxTrigger);

                objDentalOut.spouseOutDentalCost = CalculateDentalCost(objDentalOut.spouseOutValueofDeductible, objDentalOut.spouseOutValueofCoin, objDentalOut.spouseOutvalaboveAnnualMax);
                objDentalOut.spouseOutDentalCost = objDentalOut.spouseOutDentalCost * spouseStatus;

                objDentalOut.spouseOutTotBaseDentalCost = CalculateTotalBaseDentalCost(objDentalEntity.NetworkUtil, objDentalOut.spouseOutDentalCost, objDentalIn.spouseInDentalCost);

                objDentalOut.spouseOutApplyNoCovDentalCosts = objDentalOut.spouseOutTotBaseDentalCost * spouseDentalStatus * spouseStatus;

                return objDentalOut.spouseOutApplyNoCovDentalCosts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating ChildOutNetwork Values
        private decimal CalculateDentalCostChildOutNet(int zipcode, PlanDesignDental objDentalEntity, int childStatus, int childDentalStatus)
        {
            try
            {
                objDentalOut.childOutAAGFactorAverage = CalculateAAGFactorAvg(zipcode, Resources.Constants.ColDentalFactor, Resources.Constants.ColChildGender, Resources.Constants.ColDentalweightChild);

                objDentalOut.childOutdeductibleTrigger = CalculateDeductibleTriggerAverage(Resources.Constants.ColDentalweightChild, objDentalEntity.OutNetClassIDeductible, objDentalEntity.OutNetClassIIDeductible, objDentalEntity.OutNetClassIIIDeductible);
                objDentalOut.childOutMemCoinAvg = CalculateMemberCoinsuranceAverage(Resources.Constants.ColDentalweightChild, objDentalEntity.OutNetClassICoinsurance, objDentalEntity.OutNetClassIICoinsurance, objDentalEntity.OutNetClassIIICoinsurance);
                objDentalOut.childOutannualMaxTrigger = CalculateAnnualMaxTrigger(objDentalOut.childOutMemCoinAvg, objDentalOut.childOutdeductibleTrigger, objDentalEntity.OutNetAnnualMax);

                objDentalOut.childOutValueofDeductible = CalculateValueofDeductible(Resources.Constants.ColTierTypeC, objDentalEntity.OutNetDiscount, objDentalOut.childOutAAGFactorAverage, objDentalOut.childOutdeductibleTrigger);
                objDentalOut.childOutValueofCoin = CalculateValueofCoinsurance(Resources.Constants.ColTierTypeC, objDentalEntity.OutNetDiscount, objDentalOut.childOutAAGFactorAverage, objDentalOut.childOutdeductibleTrigger, objDentalOut.childOutMemCoinAvg, objDentalOut.childOutannualMaxTrigger);
                objDentalOut.childOutvalaboveAnnualMax = CalculateValueaboveAnnualMax(Resources.Constants.ColTierTypeC, objDentalEntity.OutNetDiscount, objDentalOut.childOutAAGFactorAverage, objDentalOut.childOutannualMaxTrigger);

                objDentalOut.childOutDentalCost = CalculateDentalCost(objDentalOut.childOutValueofDeductible, objDentalOut.childOutValueofCoin, objDentalOut.childOutvalaboveAnnualMax);

                objDentalOut.childOutTotBaseDentalCost = CalculateTotalBaseDentalCost(objDentalEntity.NetworkUtil, objDentalOut.childOutDentalCost, objDentalIn.childInDentalCost);

                objDentalOut.childOutApplyNoCovDentalCosts = objDentalOut.childOutTotBaseDentalCost * childDentalStatus * childStatus;

                return objDentalOut.childOutApplyNoCovDentalCosts;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region SubMethods
        //Calculating AAGFactorAverage-(B114 to B123)
        private decimal CalculateAAGFactorAvg(int zipcode, string areacolName, string gender, string dentalweightColName, int age = 0)
        {
            try
            {
                decimal dentalAreaFactor, ageGenderFactorClass1, ageGenderFactorClass2, ageGenderFactorClass3,
                        aagFactorClass1, aagFactorClass2, aagFactorClass3, aagFactorAverage,
                        dentalweightClass1, dentalweightClass2, dentalweightClass3, sumDentalWeight,
                        aagFactoravg1, aagFactoravg2, aagFactoravg3, sumaagFactoravg;
                //Calculate AreaFactor - (B114)
                dentalAreaFactor = OOPCommon.CalculateAreaFactor(zipcode, areacolName);
                //Calculating AgeGenderFactor - (B116 to B118)
                ageGenderFactorClass1 = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColDentalAgeGenClassl, gender, age);
                ageGenderFactorClass2 = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColDentalAgeGenClassll, gender, age);
                ageGenderFactorClass3 = OOPCommon.CalculateAgeGenderFactor(Resources.Constants.ColDentalAgeGenClasslll, gender, age);
                //Calculating DentalAAGFactor - (B120 to B122)
                aagFactorClass1 = dentalAreaFactor * ageGenderFactorClass1;
                aagFactorClass2 = dentalAreaFactor * ageGenderFactorClass2;
                aagFactorClass3 = dentalAreaFactor * ageGenderFactorClass3;
                //Calculating AAGFactorAvg - (B123)
                dentalweightClass1 = CalculateDentalweighttablevalue(dentalweightColName, 1);
                dentalweightClass2 = CalculateDentalweighttablevalue(dentalweightColName, 2);
                dentalweightClass3 = CalculateDentalweighttablevalue(dentalweightColName, 3);
                sumDentalWeight = dentalweightClass1 + dentalweightClass2 + dentalweightClass3;
                aagFactoravg1 = aagFactorClass1 * dentalweightClass1;
                aagFactoravg2 = aagFactorClass2 * dentalweightClass2;
                aagFactoravg3 = aagFactorClass3 * dentalweightClass3;
                sumaagFactoravg = aagFactoravg1 + aagFactoravg2 + aagFactoravg3;
                aagFactorAverage = sumaagFactoravg / sumDentalWeight;

                return aagFactorAverage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating DeductibleTriggerAverage (B125 to G125)
        private decimal CalculateDeductibleTriggerAverage(string dentalWeightcolname, decimal class1, decimal class2, decimal class3)
        {
            try
            {
                plandesignClass1 = class1;
                plandesignCalss2 = class2;
                plandesignClass3 = class3;
                dentalWeight1 = CalculateDentalweighttablevalue(dentalWeightcolname, 1);
                dentalWeight2 = CalculateDentalweighttablevalue(dentalWeightcolname, 2);
                dentalWeight3 = CalculateDentalweighttablevalue(dentalWeightcolname, 3);
                sumDentalweight = dentalWeight1 + dentalWeight2 + dentalWeight3;
                deductibleTrigger1 = plandesignClass1 * dentalWeight1;
                deductibleTrigger2 = plandesignCalss2 * dentalWeight2;
                deductibleTrigger3 = plandesignClass3 * dentalWeight3;
                sumDeductibleTrigger = deductibleTrigger1 + deductibleTrigger2 + deductibleTrigger3;
                deductibleTriggerAvg = sumDeductibleTrigger / sumDentalweight;

                return deductibleTriggerAvg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating MemberCoinsuranceAverage (B126 to G126)
        private decimal CalculateMemberCoinsuranceAverage(string dentalWeightcolname, decimal class1, decimal class2, decimal class3)
        {
            try
            {
                plandesignClass1 = class1;
                plandesignCalss2 = class2;
                plandesignClass3 = class3;
                dentalWeight1 = CalculateDentalweighttablevalue(dentalWeightcolname, 1);
                dentalWeight2 = CalculateDentalweighttablevalue(dentalWeightcolname, 2);
                dentalWeight3 = CalculateDentalweighttablevalue(dentalWeightcolname, 3);
                sumDentalweight = dentalWeight1 + dentalWeight2 + dentalWeight3;
                memberCoinsuranceAvg1 = dentalWeight1 * plandesignClass1;
                memberCoinsuranceAvg2 = dentalWeight2 * plandesignCalss2;
                memberCoinsuranceAvg3 = dentalWeight3 * plandesignClass3;
                sumMemberCoinsuranceAvg = memberCoinsuranceAvg1 + memberCoinsuranceAvg2 + memberCoinsuranceAvg3;
                memberCoinsuranceAverage = 1 - ((sumMemberCoinsuranceAvg) / (sumDentalweight));

                return memberCoinsuranceAverage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating AnnualMaxTrigger (B127 to G127)
        private decimal CalculateAnnualMaxTrigger(decimal memberCoinsurance, decimal deductibleTriggerAvg, decimal plandesign)
        {
            try
            {
                decimal memCoinValue, annualMaxTrigger;
                decimal PlanDesignAnnualMax = plandesign;
                decimal stepl, stepll;

                if (memberCoinsurance == 1)
                {
                    memCoinValue = 0;
                    annualMaxTrigger = memCoinValue + Math.Round(deductibleTriggerAvg, 8);
                }
                else
                {
                    stepl = 1 - Math.Round(memberCoinsurance, 9);
                    stepll = plandesign / stepl;
                    memCoinValue = Math.Round(stepll, 9);
                    annualMaxTrigger = memCoinValue + Math.Round(deductibleTriggerAvg, 8);
                }
                return Math.Round(annualMaxTrigger, 6);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating ValueofDeductible (B129 to G129)
        private decimal CalculateValueofDeductible(string tier, decimal discount, decimal aagFactor, decimal lookupValue)
        {
            try
            {
                decimal lookUP1, lookUP2, lookUP3, lookUP4, deductibleValue;
                decimal totalAnnuall, totalAnnualll, totalAnnuallll, totalAnnuallv;
                totalAnnuall = CalculateTotalAnnualValue(1, discount, aagFactor, 0);
                lookUP1 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColTotalCummulativeAmt, tier, totalAnnuall);
                lookUP1 = lookUP1 * (1 - discount) * aagFactor;
                totalAnnualll = CalculateTotalAnnualValue(1, discount, aagFactor, lookupValue);
                lookUP2 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColTotalCummulativeAmt, tier, totalAnnualll);
                lookUP2 = lookUP2 * (1 - discount) * aagFactor;
                totalAnnuallll = CalculateTotalAnnualValue(1, discount, aagFactor, lookupValue);
                lookUP3 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColCumulativeFrequency, tier, totalAnnuallll);
                totalAnnuallv = CalculateTotalAnnualValue(1, discount, aagFactor, 0);
                lookUP4 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColCumulativeFrequency, tier, totalAnnuallv);
                deductibleValue = 1 * (lookUP1 - lookUP2 + (lookUP3 * lookupValue) - (lookUP4 * 0));

                return deductibleValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating ValueofCoinsurance (B130 to G130)
        private decimal CalculateValueofCoinsurance(string tier, decimal discount, decimal aagFactor, decimal lookupValue1, decimal lookupValue2, decimal lookupValue3)
        {
            try
            {
                decimal lookUp1, lookUp2, lookUp3, lookUp4, coinsuranceValue;
                decimal totalAnnual1, totalAnnual2, totalAnnual3, totalAnnual4;
                totalAnnual1 = CalculateTotalAnnualValue(1, discount, aagFactor, lookupValue1);
                lookUp1 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColTotalCummulativeAmt, tier, totalAnnual1);
                lookUp1 = lookUp1 * (1 - discount) * aagFactor;
                totalAnnual2 = CalculateTotalAnnualValue(1, discount, aagFactor, lookupValue3);
                lookUp2 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColTotalCummulativeAmt, tier, totalAnnual2);
                lookUp2 = lookUp2 * (1 - discount) * aagFactor;
                totalAnnual3 = CalculateTotalAnnualValue(1, discount, aagFactor, lookupValue3);
                lookUp3 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColCumulativeFrequency, tier, totalAnnual3);
                totalAnnual4 = CalculateTotalAnnualValue(1, discount, aagFactor, lookupValue1);
                lookUp4 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColCumulativeFrequency, tier, totalAnnual4);
                coinsuranceValue = lookupValue2 * (lookUp1 - lookUp2 + (lookUp3 * lookupValue3) - (lookUp4 * lookupValue1));

                return coinsuranceValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating ValueaboveAnnualMax (B131 to G131)
        private decimal CalculateValueaboveAnnualMax(string tier, decimal discount, decimal aagFactor, decimal lookUpValue)
        {
            try
            {
                decimal lookUp1, lookUp2, lookUp3, lookUp4, annualMaxValue;
                decimal totalAnnual1, totalAnnual2, totalAnnual3, totalAnnual4;
                totalAnnual1 = CalculateTotalAnnualValue(1, discount, aagFactor, lookUpValue);
                lookUp1 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColTotalCummulativeAmt, tier, totalAnnual1);
                lookUp1 = lookUp1 * (1 - discount) * aagFactor;
                totalAnnual2 = CalculateTotalAnnualValue(1, discount, aagFactor, 1000000000);
                lookUp2 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColTotalCummulativeAmt, tier, totalAnnual2);
                lookUp2 = lookUp2 * (1 - discount) * aagFactor;
                totalAnnual3 = CalculateTotalAnnualValue(1, discount, aagFactor, 1000000000);
                lookUp3 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColCumulativeFrequency, tier, totalAnnual3);
                totalAnnual4 = CalculateTotalAnnualValue(1, discount, aagFactor, lookUpValue);
                lookUp4 = OOPDAL.CalculateAdjustedCPDDental(Resources.Constants.ColCumulativeFrequency, tier, totalAnnual4);
                annualMaxValue = 1 * (lookUp1 - lookUp2 + (lookUp3 * 1000000000) - (lookUp4 * lookUpValue));

                return annualMaxValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating DentalCosts (B133 to G133)
        private decimal CalculateDentalCost(decimal deductible, decimal coinsurance, decimal annualMax)
        {
            try
            {
                return deductible + coinsurance + annualMax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating TotalBaseDentalCosts (C135,E135,G135)
        private decimal CalculateTotalBaseDentalCost(decimal plandesignNetUtil, decimal dentalCostOut, decimal dentalCostIn)
        {
            try
            {
                decimal step1, step2, step3, totalBaseDentalCost;
                step1 = 1 - plandesignNetUtil;
                step2 = step1 * dentalCostOut;
                step3 = plandesignNetUtil * dentalCostIn;
                totalBaseDentalCost = step3 + step2;
                return totalBaseDentalCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculate OrthodontiaAAGFactor (B141,D141,F141)
        private decimal CalculateOrthodontiaAAGFactor(decimal areafactor, decimal agegenderfactor)
        {
            try
            {
                return agegenderfactor * areafactor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating OrthodontiaCosts (B142,D142,F142)
        private decimal CalculateOrthodontiaCosts(decimal aagFactor)
        {
            try
            {
                decimal orthodontiaCost, OrthoClaimAmt;
                OrthoClaimAmt = CommonDAL.CalculateMiscCostsAndFactors(Resources.Constants.ColOrthoClaimAmt);
                orthodontiaCost = aagFactor * OrthoClaimAmt;
                return orthodontiaCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating ApplyCoins.&AnnualBenefitMax (B143,D143,F143)
        private decimal CalculateApplyCoinsAnnualBenefitMax(decimal orthodontiaCosts, decimal coinsuranceClassIII, decimal orthoMax, int orthoStatus, int newDentCode)
        {
            try
            {
                decimal step1, step2, step3, step4, annualBenefitMax;
                step1 = coinsuranceClassIII * orthodontiaCosts;
                step2 = Math.Min(step1, orthoMax);
                step3 = orthodontiaCosts - step2;
                step4 = step3 * orthoStatus;
                annualBenefitMax = step4 * newDentCode;
                return annualBenefitMax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating Dentalweighttablevalue
        private decimal CalculateDentalweighttablevalue(string column, int dentalweightID)
        {
            try
            {
                return OOPDAL.CalculateDentalWeightTable(column, dentalweightID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calculating TotalAnnualValue
        private decimal CalculateTotalAnnualValue(int value, decimal discountVal, decimal aagFactorVal, decimal lookupVal)
        {
            decimal totalAnnual, totalAnnualAmount;
            try
            {
                totalAnnual = (value - discountVal) * aagFactorVal;
                totalAnnualAmount = lookupVal / totalAnnual;
                return totalAnnualAmount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}