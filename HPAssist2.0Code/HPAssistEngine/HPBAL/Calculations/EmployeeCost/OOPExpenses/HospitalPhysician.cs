using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HPAssistEngine.HPDAL;
using HPAssistEngine.Resources;
using HPAssistEngine.Models.Entities;
using HPAssistEngine.HPBAL.Calculations.UtilizationCoverage;
using OOPCalculation;


namespace HPAssistEngine.HPBAL.Calculations.EmployeeCost
{
    public class Hospital_Physician
    {
        #region Varible_Declaration
        decimal Rxareafactor;
        decimal miscconConservFact;
        decimal RxoopTrigger;
        decimal SplstTotCostProjection;
        #endregion

        MedicalInNetwork objMedInNet = new MedicalInNetwork();
        MedicalOutNetwork objMedOutNet = new MedicalOutNetwork();
        PrescriptionDrugsEntities objPDEntities = new PrescriptionDrugsEntities();

        public decimal[] CalculateHospital_Physician(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical)
        {
            try
            {

                decimal[] medical = new decimal[2];
                decimal totalMedicalCost = 0, medicalbaseAdjFactor, empplanbsdmedcode, totalOVCopayCost, splstCopayCost, rxCost;
                medicalbaseAdjFactor = CalculateMedicalBaseAdjFactor(objPlanMedical.CPDTableDeductible);
                empplanbsdmedcode = CalculateEmpPlanBasedMedCode(objInputXML, inputObj, objPlanMedical);
                totalOVCopayCost = CalculateTotalOVCopayCost();
                splstCopayCost = CalculateSplstCopayCost();
                PrescriptionDrug objPrescriptionDrugs = new PrescriptionDrug();
                rxCost = objPrescriptionDrugs.CalculatePrescriptionDrugs(inputObj, objPlanMedical, objPDEntities);
                medical[1] = Math.Round(rxCost, 2);
                if (objPlanMedical.CPDTableOOPMax.ToUpper() == "1A")
                {
                    if ((inputObj.NewEmpMedStatusCode + inputObj.NewSpouseMedStatusCode + inputObj.NewChildMedStatusCode) > 1)
                    {
                        if ((splstCopayCost + totalOVCopayCost + (medicalbaseAdjFactor * empplanbsdmedcode) + rxCost) > (objPlanMedical.FamilyDeductible + objPlanMedical.FamilyOOPMax))

                            totalMedicalCost = (objPlanMedical.FamilyDeductible + objPlanMedical.FamilyOOPMax) - rxCost;
                        else
                            totalMedicalCost = splstCopayCost + totalOVCopayCost + (medicalbaseAdjFactor * empplanbsdmedcode);
                    }
                    else
                    {
                        if ((splstCopayCost + totalOVCopayCost + (medicalbaseAdjFactor * empplanbsdmedcode) + rxCost) > (objPlanMedical.Deductible + objPlanMedical.OOPMax))
                            totalMedicalCost = (objPlanMedical.Deductible + objPlanMedical.OOPMax) - rxCost;
                        else
                            totalMedicalCost = splstCopayCost + totalOVCopayCost + (medicalbaseAdjFactor * empplanbsdmedcode);
                    }
                }
                else if (objPlanMedical.CPDTableOOPMax.ToUpper() == "1B")
                {
                    if ((inputObj.NewEmpMedStatusCode + inputObj.NewSpouseMedStatusCode + inputObj.NewChildMedStatusCode) > 1)
                        totalMedicalCost = Math.Min(splstCopayCost + totalOVCopayCost + (medicalbaseAdjFactor * empplanbsdmedcode), objPlanMedical.FamilyDeductible + objPlanMedical.FamilyOOPMax);
                    else
                        totalMedicalCost = Math.Min(splstCopayCost + totalOVCopayCost + (medicalbaseAdjFactor * empplanbsdmedcode), objPlanMedical.Deductible + objPlanMedical.OOPMax);
                }
                else
                {
                    totalMedicalCost = splstCopayCost + totalOVCopayCost + (medicalbaseAdjFactor * empplanbsdmedcode);
                }
                medical[0] = Math.Round(totalMedicalCost, 2);
                return medical;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateDeductibleReduction(string tier, CostEstimate objInputXML, string individual)
        {
            int medDeductionInclude;
            try
            {
                decimal deductibleAdjFactorEmp, deductibleAdjFactorSpouse, deductibleAdjFactorChild, deductibleReduction;
                deductibleAdjFactorEmp = CalculateDeductibleAdjFactor(objInputXML, "Employee");
                deductibleAdjFactorSpouse = objInputXML.Spouse.SpouseMedicalPlanExist == 1 ? CalculateDeductibleAdjFactor(objInputXML, "Spouse") : 0;
                deductibleAdjFactorChild = objInputXML.Children.ChildMedicalPlanExist >= 1 ? CalculateDeductibleAdjFactor(objInputXML, "Child") : 0;
                if (tier == "1A" || tier == "16B")
                {
                    medDeductionInclude = 1;
                    if (individual == "Employee")
                    {
                        objMedInNet.EmpDeductibleReduction = deductibleAdjFactorEmp * medDeductionInclude * objMedInNet.Deductible;//B89
                        deductibleReduction = objMedInNet.EmpDeductibleReduction;
                    }
                    else if (individual == "Spouse")
                    {
                        objMedInNet.SpouseDeductibleReduction = deductibleAdjFactorSpouse * medDeductionInclude * objMedInNet.Deductible;//D89
                        deductibleReduction = objMedInNet.SpouseDeductibleReduction;
                    }
                    else
                    {
                        objMedInNet.ChildDeductibleReduction = deductibleAdjFactorChild * medDeductionInclude * objMedInNet.Deductible;//F89
                        deductibleReduction = objMedInNet.ChildDeductibleReduction;
                    }
                    return deductibleReduction;
                }
                else
                    medDeductionInclude = 0;
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateDeductibleTriggerMedical(CostEstimate objInputXML, PlanDesignMedical objPlanMedical, string inividual)
        {
            try
            {
                decimal deductibleReduction, deductibletrigger;
                objMedInNet.Deductible = objPlanMedical.Deductible;//B21
                deductibleReduction = CalculateDeductibleReduction(objPlanMedical.CPDTableDeductible, objInputXML, inividual);
                deductibletrigger = objMedInNet.Deductible - deductibleReduction;
                return deductibletrigger;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateOOPReduction(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical, string individual)
        {
            try
            {
                decimal oopReduction = 0, rxEffectiveCopay;
                objPDEntities.RGenericRatio = OOPDAL.GetRXDistributionofCharges("Generic", "R", "Ratio");
                objPDEntities.RFormRatio = OOPDAL.GetRXDistributionofCharges("Form", "R", "Ratio");
                objPDEntities.RNonFormRatio = OOPDAL.GetRXDistributionofCharges("NonForm", "R", "Ratio");
                objPDEntities.MGenericRatio = OOPDAL.GetRXDistributionofCharges("Generic", "M", "Ratio");
                objPDEntities.MFormRatio = OOPDAL.GetRXDistributionofCharges("Form", "M", "Ratio");
                objPDEntities.MNonFormRatio = OOPDAL.GetRXDistributionofCharges("NonForm", "M", "Ratio");
                objPDEntities.RGeneric = OOPDAL.GetRXDistributionofCharges("Generic", "R", "Distribution");
                objPDEntities.RForm = OOPDAL.GetRXDistributionofCharges("Form", "R", "Distribution");
                objPDEntities.RNonForm = OOPDAL.GetRXDistributionofCharges("NonForm", "R", "Distribution");
                objPDEntities.MGeneric = OOPDAL.GetRXDistributionofCharges("Generic", "M", "Distribution");
                objPDEntities.MForm = OOPDAL.GetRXDistributionofCharges("Form", "M", "Distribution");
                objPDEntities.MNonForm = OOPDAL.GetRXDistributionofCharges("NonForm", "M", "Distribution");

                rxEffectiveCopay = (objPlanMedical.RxCopayRetailGeneric * objPDEntities.RGeneric) + (objPlanMedical.RxCopayRetailForm * objPDEntities.RForm) + (objPlanMedical.RxCopayRetailNonForm * objPDEntities.RNonForm);
                rxEffectiveCopay = (rxEffectiveCopay + (objPlanMedical.RxCopayMailGeneric * objPDEntities.MGeneric) + (objPlanMedical.RxCopayMailForm * objPDEntities.MForm) + (objPlanMedical.RxCopayMailNonForm * objPDEntities.MNonForm)) * inputObj.NewEmpMedStatusCode;
                CalculateAdjRxNoScripts(objInputXML, objPlanMedical);
                if (individual == "Employee")
                {
                    objPDEntities.EmpTotalRxCopay = rxEffectiveCopay * objPDEntities.EmpAdjRxNoofScripts * inputObj.NewEmpMedStatusCode;//B108
                    objPDEntities.EmpRxValofDeductible = CalculateAdjCPDDeductibleValue(inputObj.EEStatus, inputObj.NewEmpMedStatusCode, objPlanMedical, "Employee");
                    objPDEntities.EmpRxCoinOOPMax = CalculateRxCoinsuranceOOPMaxValue(inputObj.EEStatus, inputObj.NewEmpMedStatusCode, objPlanMedical, "Employee");
                }
                else if (individual == "Spouse")
                {
                    objPDEntities.SpouseTotalRxCopay = rxEffectiveCopay * objPDEntities.SpouseAdjRxNoofScripts * inputObj.NewSpouseMedStatusCode;//D108
                    objPDEntities.SpouseRxValofDeductible = CalculateAdjCPDDeductibleValue(inputObj.SpouseStatus, inputObj.NewSpouseMedStatusCode, objPlanMedical, "Spouse");
                    objPDEntities.SpouseRxCoinOOPMax = CalculateRxCoinsuranceOOPMaxValue(inputObj.SpouseStatus, inputObj.NewSpouseMedStatusCode, objPlanMedical, "Spouse");
                }
                else
                {
                    objPDEntities.ChildTotalRxCopay = rxEffectiveCopay * objPDEntities.ChildAdjRxNoofScripts * inputObj.NewChildMedStatusCode * inputObj.ChildStatus;//F108
                    objPDEntities.ChildRxValofDeductible = CalculateAdjCPDDeductibleValue(inputObj.ChildStatus, inputObj.NewChildMedStatusCode, objPlanMedical, "Child");
                    objPDEntities.ChildRxCoinOOPMax = CalculateRxCoinsuranceOOPMaxValue(inputObj.ChildStatus, inputObj.NewChildMedStatusCode, objPlanMedical, "Child");
                }
                if (objPlanMedical.CPDTableOOPMax.ToUpper() == "1B")
                {
                    oopReduction = 0;
                    if (individual == "Employee")
                    {
                        objPDEntities.EmpOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                        objMedInNet.EmpTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                        oopReduction = objPDEntities.EmpOVTotCopayCost + objMedInNet.EmpTotSplstCopayCost;
                    }
                    else if (individual == "Spouse")
                    {
                        objPDEntities.SpouseOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                        objMedInNet.SpouseTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                        oopReduction = objPDEntities.SpouseOVTotCopayCost + objMedInNet.SpouseTotSplstCopayCost;
                    }
                    else
                    {
                        objPDEntities.ChildOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                        objMedInNet.ChildTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                        oopReduction = (objPDEntities.ChildOVTotCopayCost + objMedInNet.ChildTotSplstCopayCost) / inputObj.ChildStatus;
                    }

                }
                else
                {
                    if (objPlanMedical.CPDTableDeductible.ToUpper() == "16A" || objPlanMedical.CPDTableDeductible.ToUpper() == "1B")
                    {
                        if (individual == "Employee")
                        {
                            objPDEntities.EmpOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                            objMedInNet.EmpTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                            oopReduction = objPDEntities.EmpTotalRxCopay + objPDEntities.EmpRxCoinOOPMax + objPDEntities.EmpRxValofDeductible;
                            oopReduction = oopReduction + objPDEntities.EmpOVTotCopayCost + objMedInNet.EmpTotSplstCopayCost;
                            objMedInNet.EmpOOPReduction = oopReduction;
                        }
                        else if (individual == "Spouse")
                        {
                            objPDEntities.SpouseOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                            objMedInNet.SpouseTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                            oopReduction = objPDEntities.SpouseTotalRxCopay + objPDEntities.SpouseRxCoinOOPMax + objPDEntities.SpouseRxValofDeductible;
                            oopReduction = oopReduction + objPDEntities.SpouseOVTotCopayCost + objMedInNet.SpouseTotSplstCopayCost;
                            objMedInNet.SpouseOOPReduction = oopReduction;
                        }
                        else
                        {
                            objPDEntities.ChildOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                            objMedInNet.ChildTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                            oopReduction = objPDEntities.ChildTotalRxCopay + objPDEntities.ChildRxCoinOOPMax + objPDEntities.ChildRxValofDeductible;
                            oopReduction = (oopReduction + objPDEntities.ChildOVTotCopayCost + objMedInNet.ChildTotSplstCopayCost) / inputObj.ChildStatus;
                            objMedInNet.ChildOOPReduction = oopReduction;
                        }
                    }
                    else
                    {
                        if (individual == "Employee")
                        {
                            objPDEntities.EmpOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                            objMedInNet.EmpTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                            oopReduction = objPDEntities.EmpTotalRxCopay + objPDEntities.EmpRxCoinOOPMax;
                            oopReduction = oopReduction + objPDEntities.EmpOVTotCopayCost + objMedInNet.EmpTotSplstCopayCost;
                            objMedInNet.EmpOOPReduction = oopReduction;
                        }
                        else if (individual == "Spouse")
                        {
                            objPDEntities.SpouseOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                            objMedInNet.SpouseTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                            oopReduction = objPDEntities.SpouseTotalRxCopay + objPDEntities.SpouseRxCoinOOPMax;
                            oopReduction = oopReduction + objPDEntities.SpouseOVTotCopayCost + objMedInNet.SpouseTotSplstCopayCost;
                            objMedInNet.SpouseOOPReduction = oopReduction;
                        }
                        else
                        {
                            objPDEntities.ChildOVTotCopayCost = CalculateOVCopayCost(objPlanMedical, inputObj, objInputXML, individual);
                            objMedInNet.ChildTotSplstCopayCost = CalculateSepecialistCopayCost(objInputXML, inputObj, objPlanMedical, individual);
                            oopReduction = objPDEntities.ChildTotalRxCopay + objPDEntities.ChildRxCoinOOPMax;
                            oopReduction = (oopReduction + objPDEntities.ChildOVTotCopayCost + objMedInNet.ChildTotSplstCopayCost) / inputObj.ChildStatus;
                            objMedInNet.ChildOOPReduction = oopReduction;
                        }
                    }
                }
                return oopReduction;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private decimal CalculateAdjustedOOP(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical, string individual)
        {
            try
            {
                decimal oopReduction, adjustedOOP;
                objMedInNet.BeginningOOP = objPlanMedical.OOPMax;//B25
                oopReduction = CalculateOOPReduction(objInputXML, inputObj, objPlanMedical, individual);
                adjustedOOP = objMedInNet.BeginningOOP - oopReduction;
                adjustedOOP = Math.Max(adjustedOOP, 0);
                return adjustedOOP;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateOOPTrigger(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical, string individual)
        {
            try
            {
                decimal oopTrigger = 0;
                switch (individual)
                {
                    case "Employee":
                        objMedInNet.EmpAdjustedOOP = CalculateAdjustedOOP(objInputXML, inputObj, objPlanMedical, "Employee");
                        if (objMedInNet.MemberCoinsurance != 0)
                            oopTrigger = objMedInNet.EmpAdjustedOOP / objMedInNet.MemberCoinsurance + objMedInNet.EmpDeductibleTrigger;
                        else
                            oopTrigger = objMedInNet.EmpAdjustedOOP / 1000000 + objMedInNet.EmpDeductibleTrigger;
                        break;
                    case "Spouse":
                        objMedInNet.SpouseAdjustedOOP = CalculateAdjustedOOP(objInputXML, inputObj, objPlanMedical, "Spouse");
                        if (objMedInNet.MemberCoinsurance != 0)
                            oopTrigger = objMedInNet.SpouseAdjustedOOP / objMedInNet.MemberCoinsurance + objMedInNet.SpouseDeductibleTrigger;
                        else
                            oopTrigger = objMedInNet.SpouseAdjustedOOP / 1000000 + objMedInNet.SpouseDeductibleTrigger;
                        break;
                    case "Child":
                        objMedInNet.ChildAdjustedOOP = CalculateAdjustedOOP(objInputXML, inputObj, objPlanMedical, "Child");
                        if (objMedInNet.MemberCoinsurance != 0)
                            oopTrigger = objMedInNet.ChildAdjustedOOP / objMedInNet.MemberCoinsurance + objMedInNet.ChildDeductibleTrigger;
                        else
                            oopTrigger = objMedInNet.ChildAdjustedOOP / 1000000 + objMedInNet.ChildDeductibleTrigger;
                        break;
                    default:
                        break;
                }

                return oopTrigger;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private decimal CalculateValueofDeductible(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical, string cpdTier, string net)
        {
            try
            {
                decimal empDeductible, spouseDeductible, childDeductible, valueofDeductible, planDesignFamilyDeductible, planDesignDeductible;
                empDeductible = CalculateDeductible(objInputXML, inputObj, objPlanMedical, "Employee", cpdTier, net);
                spouseDeductible = inputObj.NewSpouseMedStatusCode == 1 ? CalculateDeductible(objInputXML, inputObj, objPlanMedical, "Spouse", cpdTier, net) : 0;
                childDeductible = inputObj.NewChildMedStatusCode >= 1 ? CalculateDeductible(objInputXML, inputObj, objPlanMedical, "Child", cpdTier, net) : 0;
                if (net == "In")
                {
                    planDesignFamilyDeductible = objPlanMedical.FamilyDeductible;
                    planDesignDeductible = objPlanMedical.Deductible;
                }
                else
                {
                    planDesignFamilyDeductible = objPlanMedical.OutNetFamilyDeductible;
                    planDesignDeductible = objPlanMedical.OutNetDeductible;
                }
                if ((inputObj.NewEmpMedStatusCode + inputObj.NewSpouseMedStatusCode + inputObj.NewChildMedStatusCode) > 1)
                {
                    valueofDeductible = Math.Min(planDesignFamilyDeductible, empDeductible + spouseDeductible + childDeductible);
                    return valueofDeductible;
                }
                else
                {
                    valueofDeductible = Math.Min(planDesignDeductible, empDeductible + spouseDeductible + childDeductible);
                    return valueofDeductible;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateDeductible(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical, string individual, string cpdTier, string Net)
        {
            try
            {
                decimal deductibletrigger = 0, dividingfactor = 0, valueofDeductible = 0, cumAmtlukupone, cumAmtlukuptwo, cumFreqlukupone, cumFreqlukuptwo;
                miscconConservFact = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColConservative_Factor);
                int medCode = 0;
                switch (individual)
                {
                    case "Employee":
                        if (Net == "In")
                        {
                            deductibletrigger = objMedInNet.EmpDeductibleTrigger = CalculateDeductibleTriggerMedical(objInputXML, objPlanMedical, "Employee");//B23
                            dividingfactor = (1 - objPlanMedical.InNetDiscount) * (objMedInNet.EmpAgeGenderFactor * objMedInNet.AreaFactor) * objMedInNet.EmpMedUtil * miscconConservFact;
                            medCode = inputObj.NewEmpMedStatusCode;
                            cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColCumulativeFrequency) * deductibletrigger;
                            cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColCumulativeFrequency);
                            objMedInNet.EmpMedValofDeductible = (1 * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo * 0)) * medCode;
                            valueofDeductible = objMedInNet.EmpMedValofDeductible;
                        }
                        else
                        {
                            deductibletrigger = objMedOutNet.EmpDeductibleTrigger = objPlanMedical.OutNetDeductible;
                            dividingfactor = (1 - objPlanMedical.OutNetDiscount) * (objMedInNet.EmpAgeGenderFactor * objMedInNet.AreaFactor) * objMedInNet.EmpMedUtil * miscconConservFact;
                            medCode = inputObj.NewEmpMedStatusCode;
                            cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColCumulativeFrequency) * deductibletrigger;
                            cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColCumulativeFrequency);
                            objMedOutNet.EmpMedValofDeductible = (1 * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo * 0)) * medCode;
                            valueofDeductible = objMedOutNet.EmpMedValofDeductible;
                        }
                        break;
                    case "Spouse":
                        if (Net == "In")
                        {
                            deductibletrigger = objMedInNet.SpouseDeductibleTrigger = CalculateDeductibleTriggerMedical(objInputXML, objPlanMedical, "Spouse");
                            dividingfactor = (1 - objPlanMedical.InNetDiscount) * (objMedInNet.SpouseAgeGenderFactor * objMedInNet.AreaFactor) * objMedInNet.SpouseMedUtil * miscconConservFact;
                            medCode = inputObj.NewSpouseMedStatusCode;
                            cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColCumulativeFrequency) * deductibletrigger;
                            cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColCumulativeFrequency);
                            objMedInNet.SpouseMedValofDeductible = (1 * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo * 0)) * medCode;
                            valueofDeductible = objMedInNet.SpouseMedValofDeductible;
                        }
                        else
                        {
                            deductibletrigger = objMedOutNet.SpouseDeductibleTrigger = objPlanMedical.OutNetDeductible;
                            dividingfactor = (1 - objPlanMedical.OutNetDiscount) * (objMedInNet.SpouseAgeGenderFactor * objMedInNet.AreaFactor) * objMedInNet.SpouseMedUtil * miscconConservFact;
                            medCode = inputObj.NewSpouseMedStatusCode;
                            cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColCumulativeFrequency) * deductibletrigger;
                            cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColCumulativeFrequency);
                            objMedOutNet.SpouseMedValofDeductible = (1 * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo * 0)) * medCode;
                            valueofDeductible = objMedOutNet.SpouseMedValofDeductible;
                        }
                        break;
                    case "Child":
                        if (Net == "In")
                        {
                            deductibletrigger = objMedInNet.ChildDeductibleTrigger = CalculateDeductibleTriggerMedical(objInputXML, objPlanMedical, "Child");
                            dividingfactor = (1 - objPlanMedical.InNetDiscount) * (objMedInNet.ChildAgeGenderFactor * objMedInNet.AreaFactor) * objMedInNet.ChildMedUtil * miscconConservFact;
                            medCode = inputObj.NewChildMedStatusCode * inputObj.ChildStatus;
                            cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColCumulativeFrequency) * deductibletrigger;
                            cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColCumulativeFrequency);
                            objMedInNet.ChildMedValofDeductible = (1 * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo * 0)) * medCode;
                            valueofDeductible = objMedInNet.ChildMedValofDeductible;
                        }
                        else
                        {
                            deductibletrigger = objMedOutNet.ChildDeductibleTrigger = objPlanMedical.OutNetDeductible;
                            dividingfactor = (1 - objPlanMedical.OutNetDiscount) * (objMedInNet.ChildAgeGenderFactor * objMedInNet.AreaFactor) * objMedInNet.ChildMedUtil * miscconConservFact;
                            medCode = inputObj.NewChildMedStatusCode * inputObj.ChildStatus;
                            cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                            cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibletrigger / dividingfactor, Resources.Constants.ColCumulativeFrequency) * deductibletrigger;
                            cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColCumulativeFrequency);
                            objMedOutNet.ChildMedValofDeductible = (1 * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo * 0)) * medCode;
                            valueofDeductible = objMedOutNet.ChildMedValofDeductible;
                        }

                        break;
                    default:
                        break;
                }
                return valueofDeductible;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateTotalCoinsuranceOOPMaxValueInNet(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical)
        {
            try
            {
                string cpdTableOOPMax, cpdTierOOPMax = "", cpdTierDeductible = "", cpdTableDeductible;
                decimal deductibleValue, totalCoinsuranceOOPMaxValue;
                cpdTableOOPMax = OOPDAL.GetCPDTable(objPlanMedical.CPDTableOOPMax);
                switch (cpdTableOOPMax.ToUpper())
                {
                    case "CCPD_MEDICAL_TABLE_UNADJ_16":
                        cpdTierOOPMax = "L";
                        break;
                    case "CCPD_MEDICAL_TABLE_UNADJ_1B":
                        cpdTierOOPMax = "M";
                        break;
                    case "CCPD_MEDICAL_TABLE_UNADJ_1A":
                        cpdTierOOPMax = "A";
                        break;
                    default:
                        break;
                }
                cpdTableDeductible = OOPDAL.GetCPDTable(objPlanMedical.CPDTableDeductible);
                switch (cpdTableDeductible.ToUpper())
                {
                    case "CCPD_MEDICAL_TABLE_UNADJ_16":
                        cpdTierDeductible = "L";
                        break;
                    case "CCPD_MEDICAL_TABLE_UNADJ_1B":
                        cpdTierDeductible = "M";
                        break;
                    case "CCPD_MEDICAL_TABLE_UNADJ_1A":
                        cpdTierDeductible = "A";
                        break;
                    default:
                        break;
                }
                deductibleValue = CalculateValueofDeductible(objInputXML, inputObj, objPlanMedical, cpdTierDeductible, "In");
                objMedInNet.EmpCoinOOPMax = CalculateCoinsuranceOOPMaxValueMedical(objInputXML, inputObj, objPlanMedical, "Employee", cpdTierOOPMax, "In");
                objMedInNet.SpouseCoinOOPMax = inputObj.NewSpouseMedStatusCode == 1 ? CalculateCoinsuranceOOPMaxValueMedical(objInputXML, inputObj, objPlanMedical, "Spouse", cpdTierOOPMax, "In") : 0;
                objMedInNet.ChildCoinOOPMax = inputObj.NewChildMedStatusCode >= 1 ? CalculateCoinsuranceOOPMaxValueMedical(objInputXML, inputObj, objPlanMedical, "Child", cpdTierOOPMax, "In") : 0;
                totalCoinsuranceOOPMaxValue = 0;
                if ((inputObj.NewEmpMedStatusCode + inputObj.NewSpouseMedStatusCode + inputObj.NewChildMedStatusCode) > 1)
                    totalCoinsuranceOOPMaxValue = Math.Min(objPlanMedical.FamilyDeductible + objPlanMedical.FamilyOOPMax, deductibleValue + objMedInNet.EmpCoinOOPMax + objMedInNet.SpouseCoinOOPMax + objMedInNet.ChildCoinOOPMax);
                else
                    totalCoinsuranceOOPMaxValue = Math.Min(objPlanMedical.Deductible + objPlanMedical.OOPMax, deductibleValue + objMedInNet.EmpCoinOOPMax + objMedInNet.SpouseCoinOOPMax + objMedInNet.ChildCoinOOPMax);
                return totalCoinsuranceOOPMaxValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateCoinsuranceOOPMaxValueMedical(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical, string individual, string cpdTier, string Net)
        {
            try
            {
                decimal membercoinsurance, oopTrigger, deductibleTrigger, dividingfactor, coinsuranceOOPMaxValue, status = 0;
                decimal cumAmtlukupone, cumAmtlukuptwo, cumFreqlukupone, cumFreqlukuptwo;
                if (Net == "In")
                    membercoinsurance = objMedInNet.MemberCoinsurance = 1 - objPlanMedical.MemberCoinsurance;//B24
                else
                    membercoinsurance = objMedOutNet.MemberCoinsurance = 1 - objPlanMedical.OutNetMemberCoinsurance;//C24
                if (individual == "Employee")
                {
                    if (Net == "In")
                    {
                        oopTrigger = objMedInNet.EmpOOPTrigger = CalculateOOPTrigger(objInputXML, inputObj, objPlanMedical, "Employee");//B28
                        deductibleTrigger = objMedInNet.EmpDeductibleTrigger;
                        dividingfactor = (1 - objPlanMedical.InNetDiscount) * objMedInNet.EmpMedicalAAGFactor * objMedInNet.EmpMedUtil;
                    }
                    else
                    {
                        oopTrigger = objMedOutNet.EmpOOPTrigger = objMedOutNet.MemberCoinsurance == 0 ? objPlanMedical.OutNetDeductible : (objPlanMedical.OutNetOOPMax / objMedOutNet.MemberCoinsurance) + objPlanMedical.OutNetDeductible;
                        deductibleTrigger = objPlanMedical.OutNetDeductible;
                        dividingfactor = (1 - objPlanMedical.OutNetDiscount) * objMedInNet.EmpMedicalAAGFactor * objMedInNet.EmpMedUtil;
                    }
                    status = inputObj.NewEmpMedStatusCode;
                }
                else if (individual == "Spouse")
                {
                    if (Net == "In")
                    {
                        oopTrigger = objMedInNet.SpouseOOPTrigger = CalculateOOPTrigger(objInputXML, inputObj, objPlanMedical, "Spouse");//D28
                        deductibleTrigger = objMedInNet.SpouseDeductibleTrigger;
                        dividingfactor = (1 - objPlanMedical.InNetDiscount) * objMedInNet.SpouseMedicalAAGFactor * objMedInNet.SpouseMedUtil;
                    }
                    else
                    {
                        oopTrigger = objMedOutNet.SpouseOOPTrigger = objMedOutNet.MemberCoinsurance == 0 ? 0 + objPlanMedical.OutNetDeductible : (objPlanMedical.OutNetOOPMax / objMedOutNet.MemberCoinsurance) + objPlanMedical.OutNetDeductible;
                        deductibleTrigger = objPlanMedical.OutNetDeductible;
                        dividingfactor = (1 - objPlanMedical.OutNetDiscount) * objMedInNet.SpouseMedicalAAGFactor * objMedInNet.SpouseMedUtil;
                    }
                    status = inputObj.NewSpouseMedStatusCode;
                }
                else
                {
                    if (Net == "In")
                    {
                        oopTrigger = objMedInNet.ChildOOPTrigger = CalculateOOPTrigger(objInputXML, inputObj, objPlanMedical, "Child");//F28
                        deductibleTrigger = objMedInNet.ChildDeductibleTrigger;
                        dividingfactor = (1 - objPlanMedical.InNetDiscount) * objMedInNet.ChildMedicalAAGFactor * objMedInNet.ChildMedUtil;
                    }
                    else
                    {
                        oopTrigger = objMedOutNet.ChildOOPTrigger = objMedOutNet.MemberCoinsurance == 0 ? 0 + objPlanMedical.OutNetDeductible : (objPlanMedical.OutNetOOPMax / objMedOutNet.MemberCoinsurance) + objPlanMedical.OutNetDeductible;
                        deductibleTrigger = objPlanMedical.OutNetDeductible;
                        dividingfactor = (1 - objPlanMedical.OutNetDiscount) * objMedInNet.ChildMedicalAAGFactor * objMedInNet.ChildMedUtil;
                    }
                    status = inputObj.NewChildMedStatusCode * inputObj.ChildStatus;
                }
                cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibleTrigger / dividingfactor, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, oopTrigger / dividingfactor, Resources.Constants.ColTotalCummulativeAmt) * dividingfactor;
                cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, oopTrigger / dividingfactor, Resources.Constants.ColCumulativeFrequency) * oopTrigger;
                cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibleTrigger / dividingfactor, Resources.Constants.ColCumulativeFrequency) * deductibleTrigger;

                coinsuranceOOPMaxValue = (membercoinsurance * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo)) * status;
                return coinsuranceOOPMaxValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateTotalCoinsuranceOOPMaxValueOutNet(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical)
        {
            try
            {
                string cpdTable, cpdTierOOPMax = "", cpdTableDeductible, cpdTierDeductible = "";
                decimal deductibleValue, totalCoinsuranceOOPMaxValue, familydeductible, deductible, familyoopmax, oopmax;
                cpdTable = OOPDAL.GetCPDTable(objPlanMedical.CPDTableOutNetwork);
                switch (cpdTable.ToUpper())
                {
                    case "CCPD_MEDICAL_TABLE_UNADJ_16":
                        cpdTierOOPMax = "L";
                        break;
                    case "CCPD_MEDICAL_TABLE_UNADJ_1B":
                        cpdTierOOPMax = "M";
                        break;
                    case "CCPD_MEDICAL_TABLE_UNADJ_1A":
                        cpdTierOOPMax = "A";
                        break;
                    default:
                        break;
                }
                cpdTableDeductible = OOPDAL.GetCPDTable(objPlanMedical.CPDTableOutNetwork);
                switch (cpdTableDeductible.ToUpper())
                {
                    case "CCPD_MEDICAL_TABLE_UNADJ_16":
                        cpdTierDeductible = "L";
                        break;
                    case "CCPD_MEDICAL_TABLE_UNADJ_1B":
                        cpdTierDeductible = "M";
                        break;
                    case "CCPD_MEDICAL_TABLE_UNADJ_1A":
                        cpdTierDeductible = "A";
                        break;
                    default:
                        break;
                }
                deductibleValue = CalculateValueofDeductible(objInputXML, inputObj, objPlanMedical, cpdTierDeductible, "Out");
                objMedOutNet.EmpCoinOOPMax = CalculateCoinsuranceOOPMaxValueMedical(objInputXML, inputObj, objPlanMedical, "Employee", cpdTierOOPMax, "Out");
                objMedOutNet.SpouseCoinOOPMax = inputObj.NewSpouseMedStatusCode == 1 ? CalculateCoinsuranceOOPMaxValueMedical(objInputXML, inputObj, objPlanMedical, "Spouse", cpdTierOOPMax, "Out") : 0;
                objMedOutNet.ChildCoinOOPMax = inputObj.NewChildMedStatusCode >= 1 ? CalculateCoinsuranceOOPMaxValueMedical(objInputXML, inputObj, objPlanMedical, "Child", cpdTierOOPMax, "Out") : 0;
                totalCoinsuranceOOPMaxValue = 0;
                if ((inputObj.NewEmpMedStatusCode + inputObj.NewSpouseMedStatusCode + inputObj.NewChildMedStatusCode) > 1)
                {
                    familydeductible = objPlanMedical.OutNetFamilyDeductible;
                    familyoopmax = objPlanMedical.OutNetFamilyOOPMax;
                    totalCoinsuranceOOPMaxValue = Math.Min(familydeductible + familyoopmax, deductibleValue + objMedOutNet.EmpCoinOOPMax + objMedOutNet.SpouseCoinOOPMax + objMedOutNet.ChildCoinOOPMax);
                }
                else
                {
                    deductible = objPlanMedical.Deductible;
                    oopmax = objPlanMedical.OOPMax;
                    totalCoinsuranceOOPMaxValue = Math.Min(deductible + oopmax, deductibleValue + objMedOutNet.EmpCoinOOPMax + objMedOutNet.SpouseCoinOOPMax + objMedOutNet.ChildCoinOOPMax);
                }
                return totalCoinsuranceOOPMaxValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateEmpPlanBasedMedCode(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical)
        {
            try
            {
                decimal networkUtil, valueCoinsuranceOOPMaxIn, valueCoinsuranceOOPMaxOut, empPlanBasedMedCode;
                networkUtil = objPlanMedical.NetworkUtil;
                valueCoinsuranceOOPMaxIn = CalculateTotalCoinsuranceOOPMaxValueInNet(objInputXML, inputObj, objPlanMedical);
                valueCoinsuranceOOPMaxOut = CalculateTotalCoinsuranceOOPMaxValueOutNet(objInputXML, inputObj, objPlanMedical);
                empPlanBasedMedCode = networkUtil * valueCoinsuranceOOPMaxIn + (1 - networkUtil) * valueCoinsuranceOOPMaxOut;
                return empPlanBasedMedCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateMedicalBaseAdjFactor(string deductibleCode)
        {
            try
            {
                decimal medicalBaseAdjFactor;
                if (deductibleCode.ToUpper() == "1A")
                    medicalBaseAdjFactor = 1;
                else
                    medicalBaseAdjFactor = OOPDAL.CalculateMiscCostsAndFactors();
                return medicalBaseAdjFactor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateOVAAGFactor(CostEstimate objInputXML, string individual)
        {
            try
            {
                decimal area, empAgefactor, spouseAgeFactor, childAgeFactor, misCons, ovaagFactor;
                string inpzipcode = Convert.ToString(objInputXML.Zipcode);
                inpzipcode = inpzipcode.PadLeft(5, '0');
                inpzipcode = inpzipcode.Substring(0, 3);
                int zipCode;
                int.TryParse(inpzipcode, out zipCode);
                area = CommonDAL.CalculateAreaFactor(Constants.ColOVFactor, zipCode);
                empAgefactor = CommonDAL.CalculateAgeGender(Constants.ColOVFactor, Convert.ToChar(objInputXML.Employee.EmpGender), objInputXML.Employee.EmpAge);
                spouseAgeFactor = objInputXML.Spouse.SpouseAge!=0?CommonDAL.CalculateAgeGender(Constants.ColOVFactor, Convert.ToChar(objInputXML.Spouse.SpouseGender), objInputXML.Spouse.SpouseAge):0;
                childAgeFactor = CommonDAL.CalculateAgeGender(Constants.ColOVFactor, 'C');
                misCons = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColOVAAGAdjFactor);

                if (individual == "Employee")
                    ovaagFactor = area * empAgefactor * misCons;
                else if (individual == "Spouse")
                    ovaagFactor = area * spouseAgeFactor * misCons;
                else
                    ovaagFactor = area * childAgeFactor * misCons;
                return ovaagFactor;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private decimal CalculateTotalOVCopayCost()
        {
            try
            {
                decimal totalOVCopayCost;
                totalOVCopayCost = objPDEntities.EmpOVTotCopayCost + objPDEntities.SpouseOVTotCopayCost + objPDEntities.ChildOVTotCopayCost;
                return totalOVCopayCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateOVCopayCost(PlanDesignMedical objPlanMedical, CommonEntities inputObj, CostEstimate objInputXML, string Individual)
        {
            try
            {
                decimal ovCopayCost, ovaagFactor, medicalUtilOV, ovCopayAmount, misCons, ageGenderOV, areaFactorMedical;
                string deductible = objPlanMedical.CPDTableDeductible;
                decimal status;
                decimal ovTotCost, ovDeductibleShare, ovaagFactorCost, misContotPCP;
                misCons = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColAverageCostPCP);
                misContotPCP = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColPCPTotalCost);

                ovCopayAmount = objPlanMedical.OVCopay + (1 - objPlanMedical.OVCoinsurance) * misCons;//B47
                ovaagFactor = CalculateOVAAGFactor(objInputXML, Individual);
                if (Individual == "Employee")
                {
                    medicalUtilOV = UtilizationCommon.CalculateUtilization(Constants.ColOVCost, objInputXML.Employee.EmpMedicalUtil);//C5
                    ageGenderOV = CommonDAL.CalculateAgeGender(Constants.ColOVFactorNormalized, Convert.ToChar(objInputXML.Employee.EmpGender), objInputXML.Employee.EmpAge);//B40
                    areaFactorMedical = objMedInNet.EmpAgeGenderFactor;
                    ovaagFactorCost = ageGenderOV * objMedInNet.AreaFactor;//B45
                    ovTotCost = ovaagFactorCost * misCons * medicalUtilOV;//B52
                    ovDeductibleShare = (ageGenderOV * medicalUtilOV) / (areaFactorMedical * objMedInNet.EmpMedUtil) * misContotPCP * objMedInNet.EmpMedValofDeductible; //B51
                    status = inputObj.EEStatus * inputObj.NewEmpMedStatusCode;
                }
                else if (Individual == "Spouse")
                {
                    medicalUtilOV = UtilizationCommon.CalculateUtilization(Constants.ColOVCost, objInputXML.Spouse.SpouseMedicalUtil);//E5
                    ageGenderOV = CommonDAL.CalculateAgeGender(Constants.ColOVFactorNormalized, Convert.ToChar(objInputXML.Spouse.SpouseGender), objInputXML.Spouse.SpouseAge);
                    areaFactorMedical = objMedInNet.SpouseAgeGenderFactor;
                    ovaagFactorCost = ageGenderOV * objMedInNet.AreaFactor;//B45
                    ovTotCost = ovaagFactorCost * misCons * medicalUtilOV;//B52
                    ovDeductibleShare = (ageGenderOV * medicalUtilOV) / (areaFactorMedical * objMedInNet.SpouseMedUtil) * misContotPCP * objMedInNet.SpouseMedValofDeductible; //B51
                    status = inputObj.SpouseStatus * inputObj.NewSpouseMedStatusCode;
                }
                else
                {
                    medicalUtilOV = UtilizationCommon.CalculateUtilization(Constants.ColOVCost, objInputXML.Children.ChildMedicalUtil);//G5
                    ageGenderOV = CommonDAL.CalculateAgeGender(Constants.ColOVFactorNormalized, 'C');
                    areaFactorMedical = objMedInNet.ChildAgeGenderFactor;
                    ovaagFactorCost = ageGenderOV * objMedInNet.AreaFactor;//B45
                    ovTotCost = ovaagFactorCost * misCons * medicalUtilOV * inputObj.ChildStatus;//B52
                    ovDeductibleShare = (ageGenderOV * medicalUtilOV) / (areaFactorMedical * objMedInNet.ChildMedUtil) * misContotPCP * objMedInNet.ChildMedValofDeductible; //B51
                    status = inputObj.ChildStatus * inputObj.NewChildMedStatusCode;
                }

                if (deductible.ToUpper() == "1A" || deductible.ToUpper() == "1B")
                {
                    if (objMedInNet.BeginningOOP > 0)
                        ovCopayCost = Math.Max((1 - (ovDeductibleShare / ovTotCost)) * ovCopayAmount, 0) * medicalUtilOV * ovaagFactor * status;
                    else
                        ovCopayCost = 0;
                }
                else
                    ovCopayCost = ovCopayAmount * medicalUtilOV * ovaagFactor * status;
                return ovCopayCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateSplstCopayCost()
        {
            try
            {
                decimal splstCopayCost;
                splstCopayCost = objMedInNet.EmpTotSplstCopayCost + objMedInNet.SpouseTotSplstCopayCost + objMedInNet.ChildTotSplstCopayCost;
                return splstCopayCost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateSepecialistCopayCost(CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical, string individual)
        {
            try
            {
                decimal totalSplstCopayCost, status = 0, miscConsAvgSpCost, miscConsOVAAG, splstShareofDeductible;
                string Inpzipcode;
                decimal SplstAAGFactor = 0, MedicalUtilSp = 0;
                decimal AreaFactorSp = 0;
                miscConsAvgSpCost = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColAverageCostSpc);
                miscConsOVAAG = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColOVAAGAdjFactor);
                Inpzipcode = Convert.ToString(objInputXML.Zipcode);
                Inpzipcode = Inpzipcode.PadLeft(5, '0');
                Inpzipcode = Inpzipcode.Substring(0, 3);
                int zipCode;
                int.TryParse(Inpzipcode, out zipCode);
                objMedInNet.SpCopayAreaFactor = CommonDAL.CalculateAreaFactor(Constants.ColSpecialistFactor, zipCode);
                objMedInNet.EmpAgeGenderFactorSpCoPay = CommonDAL.CalculateAgeGender(Constants.ColSpecialist, Convert.ToChar(objInputXML.Employee.EmpGender), objInputXML.Employee.EmpAge);
                objMedInNet.SpouseAgeGenderFactorSpCoPay = objInputXML.Spouse.SpouseAge!=0?CommonDAL.CalculateAgeGender(Constants.ColSpecialist, Convert.ToChar(objInputXML.Spouse.SpouseGender), objInputXML.Spouse.SpouseAge):0;
                objMedInNet.ChildAgeGenderFactorSpCoPay = CommonDAL.CalculateAgeGender(Constants.ColSpecialist, 'C');

                switch (individual)
                {
                    case "Employee":
                        status = inputObj.NewEmpMedStatusCode;
                        MedicalUtilSp = UtilizationCommon.CalculateUtilization(Constants.ColSpecialistCost, objInputXML.Employee.EmpMedicalUtil);//C6
                        SplstAAGFactor = objMedInNet.EmpMedicalAAGFactorSpCoPay = objMedInNet.SpCopayAreaFactor * objMedInNet.EmpAgeGenderFactorSpCoPay * miscConsOVAAG;
                        AreaFactorSp = objMedInNet.EmpAgeGenderCostFactorSpCoPay = CommonDAL.CalculateAgeGender(Constants.ColSpecialistNormalized, Convert.ToChar(objInputXML.Employee.EmpGender), objInputXML.Employee.EmpAge);//B59
                        SplstTotCostProjection = CalculateSplstTotCostProjection(AreaFactorSp, miscConsAvgSpCost, MedicalUtilSp, individual);
                        break;
                    case "Spouse":
                        status = inputObj.NewSpouseMedStatusCode;
                        MedicalUtilSp = UtilizationCommon.CalculateUtilization(Constants.ColSpecialistCost, objInputXML.Spouse.SpouseMedicalUtil);//E6
                        SplstAAGFactor = objMedInNet.SpouseMedicalAAGFactorSpCoPay = objMedInNet.SpCopayAreaFactor * objMedInNet.SpouseAgeGenderFactorSpCoPay * miscConsOVAAG;
                        AreaFactorSp = objMedInNet.SpouseAgeGenderCostFactorSpCoPay = CommonDAL.CalculateAgeGender(Constants.ColSpecialistNormalized, Convert.ToChar(objInputXML.Spouse.SpouseGender), objInputXML.Spouse.SpouseAge);//D59
                        SplstTotCostProjection = CalculateSplstTotCostProjection(AreaFactorSp, miscConsAvgSpCost, MedicalUtilSp, individual);
                        break;
                    case "Child":
                        status = inputObj.NewChildMedStatusCode * inputObj.ChildStatus;
                        MedicalUtilSp = UtilizationCommon.CalculateUtilization(Constants.ColSpecialistCost, objInputXML.Children.ChildMedicalUtil);//C6
                        SplstAAGFactor = objMedInNet.ChildMedicalAAGFactorSpCoPay = objMedInNet.SpCopayAreaFactor * objMedInNet.ChildAgeGenderFactorSpCoPay * miscConsOVAAG;
                        AreaFactorSp = objMedInNet.ChildAgeGenderCostFactorSpCoPay = CommonDAL.CalculateAgeGender(Constants.ColSpecialistNormalized, 'C');//F59
                        SplstTotCostProjection = CalculateSplstTotCostProjection(AreaFactorSp, miscConsAvgSpCost, MedicalUtilSp, individual, inputObj.ChildStatus);
                        break;
                }

                splstShareofDeductible = CalculateSplstShareofDeductible(AreaFactorSp, objInputXML, inputObj, objPlanMedical, individual, MedicalUtilSp);
                objMedInNet.CoPayAmountSpCoPay = (objPlanMedical.SpecialistCopay + (1 - objPlanMedical.SpecialistCoinsurance) * miscConsAvgSpCost);//B62
                if (objPlanMedical.CPDTableDeductible.ToUpper() == "1A" || objPlanMedical.CPDTableDeductible.ToUpper() == "1B")
                {
                    if (objMedInNet.BeginningOOP > 0)
                        totalSplstCopayCost = Math.Max((1 - (splstShareofDeductible / SplstTotCostProjection)) * objMedInNet.CoPayAmountSpCoPay, 0);
                    else
                        totalSplstCopayCost = 0;
                }
                else
                    totalSplstCopayCost = objMedInNet.CoPayAmountSpCoPay;
                return totalSplstCopayCost * SplstAAGFactor * MedicalUtilSp * status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateSplstShareofDeductible(decimal areaFactorSp, CostEstimate objInputXML, CommonEntities inputObj, PlanDesignMedical objPlanMedical, string individual, decimal medicalUtilSp)
        {
            try
            {
                decimal splstShareofDeductible = 0;
                decimal miscConsAvgSpTotalCost = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColSpcofTotalCost);
                switch (individual)
                {
                    case "Employee":
                        splstShareofDeductible = ((areaFactorSp * medicalUtilSp) / (objMedInNet.EmpAgeGenderFactor * objMedInNet.EmpMedUtil)) * miscConsAvgSpTotalCost * objMedInNet.EmpMedValofDeductible;
                        break;
                    case "Spouse":
                        splstShareofDeductible = ((areaFactorSp * medicalUtilSp) / (objMedInNet.SpouseAgeGenderFactor * objMedInNet.SpouseMedUtil)) * miscConsAvgSpTotalCost * objMedInNet.SpouseMedValofDeductible;
                        break;
                    case "Child":
                        splstShareofDeductible = ((areaFactorSp * medicalUtilSp) / (objMedInNet.ChildAgeGenderFactor * objMedInNet.ChildMedUtil)) * miscConsAvgSpTotalCost * objMedInNet.ChildMedValofDeductible;
                        break;
                }
                return splstShareofDeductible;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateSplstTotCostProjection(decimal areaFactorSp, decimal misCons, decimal medicalUtilSp, string individual, int childStatus = 0)
        {
            try
            {
                if (individual == "Child")
                    SplstTotCostProjection = (objMedInNet.AreaFactor * areaFactorSp) * misCons * medicalUtilSp * childStatus;
                else
                    SplstTotCostProjection = (objMedInNet.AreaFactor * areaFactorSp) * misCons * medicalUtilSp;
                return SplstTotCostProjection;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void CalculateAdjRxNoScripts(CostEstimate objInputXML, PlanDesignMedical objPlanMedical)
        {
            try
            {
                decimal rxNoOfScriptsAdj, adjRXNoOfScripts, adjRXNoOfScriptsOut, ageGenderRxUtil, miscCons, avgScriptCost, misConstRx, misConstRxMAC, misConstRxAWP;
                miscCons = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColRxAAGAdjFactor);
                misConstRx = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColAvgCostRx);
                misConstRxMAC = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColMacRx);
                misConstRxAWP = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColAwpRx);
                avgScriptCost = misConstRx * (1 - ((misConstRxMAC * objPlanMedical.RxMACDiscount) + (misConstRxAWP * objPlanMedical.RxAWPDiscount))); //B77,D77,F77
                decimal PDRGeneric, PDRForm, PDRNonForm, PDMGeneric, PDMForm, PDMNonForm;
                PDRGeneric = objPlanMedical.RxCopayRetailGeneric != 0 ? 1 : 0;
                PDRForm = objPlanMedical.RxCopayRetailForm != 0 ? 1 : 0;
                PDRNonForm = objPlanMedical.RxCopayRetailNonForm != 0 ? 1 : 0;
                PDMGeneric = objPlanMedical.RxCopayMailGeneric != 0 ? 1 : 0;
                PDMForm = objPlanMedical.RxCopayMailForm != 0 ? 1 : 0;
                PDMNonForm = objPlanMedical.RxCopayMailNonForm != 0 ? 1 : 0;
                decimal PDUtil;

                if (objPlanMedical.CPDTableDeductible.ToUpper() == "1A" || objPlanMedical.CPDTableDeductible.ToUpper() == "16B")
                {
                    objPDEntities.EmpRxDeductibleTrigger = objMedInNet.EmpDeductibleReduction + objPlanMedical.RxIncludedInDeductibleAmt;//B91
                    objPDEntities.SpouseRxDeduxtibleTrigger = objMedInNet.SpouseDeductibleReduction + objPlanMedical.RxIncludedInDeductibleAmt;//D91
                    objPDEntities.ChildRxDeductibleTrigger = objMedInNet.ChildDeductibleReduction + objPlanMedical.RxIncludedInDeductibleAmt;//F91
                }
                else
                {
                    objPDEntities.EmpRxDeductibleTrigger = objPlanMedical.RxIncludedInDeductibleAmt; //B91
                    objPDEntities.SpouseRxDeduxtibleTrigger = objPlanMedical.RxIncludedInDeductibleAmt; //D91
                    objPDEntities.ChildRxDeductibleTrigger = objPlanMedical.RxIncludedInDeductibleAmt; //F91
                }
                objPDEntities.EmpPDUtil = UtilizationCommon.CalculateUtilization(Constants.ColRXCost, objInputXML.Employee.EmpDrugsUtil); //C7
                ageGenderRxUtil = CommonDAL.CalculateAgeGender(Constants.ColRxNumberofScripts, Convert.ToChar(objInputXML.Employee.EmpGender), objInputXML.Employee.EmpAge);
                adjRXNoOfScriptsOut = PDRGeneric * objPDEntities.RGeneric + PDRForm * objPDEntities.RForm + PDMNonForm * objPDEntities.RNonForm + PDMGeneric * objPDEntities.MGeneric + PDMForm * objPDEntities.MForm + PDMNonForm * objPDEntities.MNonForm; //C78
                PDUtil = objPDEntities.EmpPDUtil;
                rxNoOfScriptsAdj = ageGenderRxUtil * Rxareafactor * miscCons;//B76
                adjRXNoOfScripts = rxNoOfScriptsAdj * PDUtil * adjRXNoOfScriptsOut - objPDEntities.EmpRxDeductibleTrigger / avgScriptCost;
                objPDEntities.EmpAdjRxNoofScripts = Math.Max(adjRXNoOfScripts, 0);

                ageGenderRxUtil = objInputXML.Spouse.SpouseAge!=0 ?CommonDAL.CalculateAgeGender(Constants.ColRxNumberofScripts, Convert.ToChar(objInputXML.Spouse.SpouseGender), objInputXML.Spouse.SpouseAge):0;
                objPDEntities.SpousePDUtil = objInputXML.Spouse.SpouseAge!=0?UtilizationCommon.CalculateUtilization(Constants.ColRXCost, objInputXML.Spouse.SpouseDrugsUtil):0; //E7 
                adjRXNoOfScriptsOut = PDRGeneric * objPDEntities.RGeneric + PDRForm * objPDEntities.RForm + PDMNonForm * objPDEntities.RNonForm + PDMGeneric * objPDEntities.MGeneric + PDMForm * objPDEntities.MForm + PDMNonForm * objPDEntities.MNonForm; //E78
                PDUtil = objPDEntities.SpousePDUtil;
                rxNoOfScriptsAdj = ageGenderRxUtil * Rxareafactor * miscCons;//D76
                adjRXNoOfScripts = rxNoOfScriptsAdj * PDUtil * adjRXNoOfScriptsOut - objPDEntities.SpouseRxDeduxtibleTrigger / avgScriptCost;
                objPDEntities.SpouseAdjRxNoofScripts = Math.Max(adjRXNoOfScripts, 0);
                
                ageGenderRxUtil = CommonDAL.CalculateAgeGender(Constants.ColRxNumberofScripts, 'C');
                objPDEntities.ChildPDUtil = UtilizationCommon.CalculateUtilization(Constants.ColRXCost, objInputXML.Children.ChildDrugsUtil); //G7
                adjRXNoOfScriptsOut = PDRGeneric * objPDEntities.RGeneric + PDRForm * objPDEntities.RForm + PDMNonForm * objPDEntities.RNonForm + PDMGeneric * objPDEntities.MGeneric + PDMForm * objPDEntities.MForm + PDMNonForm * objPDEntities.MNonForm; //G78
                PDUtil = objPDEntities.ChildPDUtil;
                rxNoOfScriptsAdj = ageGenderRxUtil * Rxareafactor * miscCons;//F76
                adjRXNoOfScripts = rxNoOfScriptsAdj * PDUtil * adjRXNoOfScriptsOut - objPDEntities.ChildRxDeductibleTrigger / avgScriptCost;
                objPDEntities.ChildAdjRxNoofScripts = Math.Max(adjRXNoOfScripts, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateDeductibleAdjFactor(CostEstimate objInputXML, string inividual)
        {
            try
            {

                string inpzipcode = Convert.ToString(objInputXML.Zipcode);
                decimal miscconst;
                inpzipcode = inpzipcode.PadLeft(5, '0');
                inpzipcode = inpzipcode.Substring(0, 3);
                int zipCode;
                int.TryParse(inpzipcode, out zipCode);

                Rxareafactor = CommonDAL.CalculateAreaFactor(Constants.ColRxFactor, zipCode);//B73
                objMedInNet.AreaFactor = CommonDAL.CalculateAreaFactor(Constants.ColMedicalFactor, zipCode);//B17,//C17,//E17
                miscconst = CommonDAL.CalculateMiscCostsAndFactors(Constants.ColRxofTotalCost);

                if (inividual == "Employee")
                {
                    objPDEntities.EmpMedUtil = objMedInNet.EmpMedUtil = UtilizationCommon.CalculateUtilization(Constants.ColMedicalCost, objInputXML.Employee.EmpMedicalUtil);//C4
                    objPDEntities.EmpRxAgeGender = CommonDAL.CalculateAgeGender(Constants.ColRxFactorNormalized, Convert.ToChar(objInputXML.Employee.EmpGender), objInputXML.Employee.EmpAge);//B74
                    objMedInNet.EmpAgeGenderFactor = CommonDAL.CalculateAgeGender(Constants.ColMedicalFactor, Convert.ToChar(objInputXML.Employee.EmpGender), objInputXML.Employee.EmpAge);//B18
                    objMedInNet.EmpMedicalAAGFactor = objMedInNet.AreaFactor * objMedInNet.EmpAgeGenderFactor;//B19
                    objPDEntities.EmpPDUtil = UtilizationCommon.CalculateUtilization(Constants.ColRXCost, objInputXML.Employee.EmpDrugsUtil);//C7
                    objPDEntities.EmpDeductibleAdjFactor = ((objPDEntities.EmpPDUtil * objPDEntities.EmpRxAgeGender * Rxareafactor) / (objMedInNet.EmpMedicalAAGFactor * objMedInNet.EmpMedUtil)) * miscconst;
                    return objPDEntities.EmpDeductibleAdjFactor;
                }
                else if (inividual == "Spouse")
                {
                    objPDEntities.SpouseMedUtil = objMedInNet.SpouseMedUtil = UtilizationCommon.CalculateUtilization(Constants.ColMedicalCost, objInputXML.Spouse.SpouseMedicalUtil);//E4
                    objPDEntities.SpouseRxAgeGender = CommonDAL.CalculateAgeGender(Constants.ColRxFactorNormalized, Convert.ToChar(objInputXML.Spouse.SpouseGender), objInputXML.Spouse.SpouseAge);//D74
                    objMedInNet.SpouseAgeGenderFactor = CommonDAL.CalculateAgeGender(Constants.ColMedicalFactor, Convert.ToChar(objInputXML.Spouse.SpouseGender), objInputXML.Spouse.SpouseAge);//D18
                    objMedInNet.SpouseMedicalAAGFactor = objMedInNet.AreaFactor * objMedInNet.SpouseAgeGenderFactor;//D19
                    objPDEntities.SpousePDUtil = UtilizationCommon.CalculateUtilization(Constants.ColRXCost, objInputXML.Spouse.SpouseDrugsUtil);//E7
                    objPDEntities.SpouseDeductibleAdjFactor = ((objPDEntities.SpousePDUtil * objPDEntities.SpouseRxAgeGender * Rxareafactor) / (objMedInNet.SpouseMedicalAAGFactor * objMedInNet.SpouseMedUtil)) * miscconst;
                    return objPDEntities.SpouseDeductibleAdjFactor;
                }
                else
                {
                    objPDEntities.ChildMedUtil = objMedInNet.ChildMedUtil = UtilizationCommon.CalculateUtilization(Constants.ColMedicalCost, objInputXML.Children.ChildMedicalUtil);//G4
                    objPDEntities.ChildRxAgeGender = CommonDAL.CalculateAgeGender(Constants.ColRxFactorNormalized, 'C');//F74
                    objMedInNet.ChildAgeGenderFactor = CommonDAL.CalculateAgeGender(Constants.ColMedicalFactor, 'C');//F18 
                    objMedInNet.ChildMedicalAAGFactor = objMedInNet.AreaFactor * objMedInNet.ChildAgeGenderFactor;//F19
                    objPDEntities.ChildPDUtil = UtilizationCommon.CalculateUtilization(Constants.ColRXCost, objInputXML.Children.ChildDrugsUtil);//G7
                    objPDEntities.ChildDeductibleAdjFactor = ((objPDEntities.ChildPDUtil * objPDEntities.ChildRxAgeGender * Rxareafactor) / (objMedInNet.ChildMedicalAAGFactor * objMedInNet.ChildMedUtil)) * miscconst;
                    return objPDEntities.ChildDeductibleAdjFactor;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateOOPMax(PlanDesignMedical objPlanMedical, string individual, int childStatus = 0)
        {
            try
            {
                if (individual == "Employee")
                {
                    if (objPlanMedical.CPDTableOOPMax.ToUpper() == "1A")
                    {
                        if ((objPlanMedical.CPDTableDeductible.ToUpper() == "1A") || (objPlanMedical.CPDTableDeductible.ToUpper() == "16B"))
                            objPDEntities.EmpRxOOPMax = objPDEntities.EmpDeductibleAdjFactor * objMedInNet.BeginningOOP;
                        else
                            objPDEntities.EmpRxOOPMax = Math.Max((objMedInNet.Deductible + objMedInNet.BeginningOOP) * objPDEntities.EmpDeductibleAdjFactor - objPDEntities.EmpRxDeductibleTrigger - objMedInNet.EmpMedValofDeductible, 0);
                    }
                    else
                    {
                        if ((objPlanMedical.CPDTableDeductible.ToUpper() == "1A") || (objPlanMedical.CPDTableDeductible.ToUpper() == "16B"))
                            objPDEntities.EmpRxOOPMax = Math.Max(objPlanMedical.RxOOPMax - objMedInNet.EmpDeductibleReduction, 0);
                        else
                            objPDEntities.EmpRxOOPMax = objPlanMedical.RxOOPMax;

                    }
                    return objPDEntities.EmpRxOOPMax;
                }
                if (individual == "Spouse")
                {
                    if (objPlanMedical.CPDTableOOPMax.ToUpper() == "1A")
                    {
                        if ((objPlanMedical.CPDTableDeductible.ToUpper() == "1A") || (objPlanMedical.CPDTableDeductible.ToUpper() == "16B"))
                            objPDEntities.SpouseRxOOPMax = objPDEntities.SpouseDeductibleAdjFactor * objMedInNet.BeginningOOP;
                        else
                            objPDEntities.SpouseRxOOPMax = Math.Max((objMedInNet.Deductible + objMedInNet.BeginningOOP) * objPDEntities.SpouseDeductibleAdjFactor - objPDEntities.SpouseRxDeduxtibleTrigger - objMedInNet.SpouseMedValofDeductible, 0);
                    }
                    else
                    {
                        if ((objPlanMedical.CPDTableDeductible.ToUpper() == "1A") || (objPlanMedical.CPDTableDeductible.ToUpper() == "16B"))
                            objPDEntities.SpouseRxOOPMax = Math.Max(objPlanMedical.RxOOPMax - objMedInNet.SpouseDeductibleReduction, 0);
                        else
                            objPDEntities.SpouseRxOOPMax = objPlanMedical.RxOOPMax;

                    }
                    return objPDEntities.SpouseRxOOPMax;
                }
                else
                    if (objPlanMedical.CPDTableOOPMax.ToUpper() == "1A")
                    {
                        if ((objPlanMedical.CPDTableDeductible.ToUpper() == "1A") || (objPlanMedical.CPDTableDeductible.ToUpper() == "16B"))
                            objPDEntities.ChildRxOOPMax = objPDEntities.ChildDeductibleAdjFactor * objMedInNet.BeginningOOP;
                        else
                            objPDEntities.ChildRxOOPMax = Math.Max((objMedInNet.Deductible + objMedInNet.BeginningOOP) * objPDEntities.ChildDeductibleAdjFactor - objPDEntities.ChildRxDeductibleTrigger - (objMedInNet.ChildMedValofDeductible / childStatus), 0);
                    }
                    else
                    {
                        if ((objPlanMedical.CPDTableDeductible.ToUpper() == "1A") || (objPlanMedical.CPDTableDeductible.ToUpper() == "16B"))
                            objPDEntities.ChildRxOOPMax = Math.Max(objPlanMedical.RxOOPMax - objMedInNet.ChildDeductibleReduction, 0);
                        else
                            objPDEntities.ChildRxOOPMax = objPlanMedical.RxOOPMax;

                    }
                return objPDEntities.ChildRxOOPMax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateRxOOPTrigger(decimal oopMaxCopay, decimal deductibleTrigger, PlanDesignMedical objDrugEntity, string individual)
        {
            decimal stepl, stepll, sumCoinsurance;
            try
            {
                decimal retailGenericCoinsurance = 1 - objDrugEntity.RxCoInsuranceRetailGeneric;
                decimal retailFormularyCoinsurance = 1 - objDrugEntity.RxCoInsuranceRetailForm;
                decimal retailNonFormularyCoinsurance = 1 - objDrugEntity.RxCoInsuranceRetailNonForm;
                decimal mailGenericCoinsurance = 1 - objDrugEntity.RxCoInsuranceMailGeneric;
                decimal mailFormularyCoinsurance = 1 - objDrugEntity.RxCoInsuranceMailForm;
                decimal mailNonFormularyCoinsurance = 1 - objDrugEntity.RxCoInsuranceMailNonForm;

                retailGenericCoinsurance = retailGenericCoinsurance * objPDEntities.RGeneric;
                retailFormularyCoinsurance = retailFormularyCoinsurance * objPDEntities.RForm;
                retailNonFormularyCoinsurance = retailNonFormularyCoinsurance * objPDEntities.RNonForm;
                mailGenericCoinsurance = mailGenericCoinsurance * objPDEntities.MGeneric;
                mailFormularyCoinsurance = mailFormularyCoinsurance * objPDEntities.MForm;
                mailNonFormularyCoinsurance = mailNonFormularyCoinsurance * objPDEntities.MNonForm;
                sumCoinsurance = retailGenericCoinsurance + retailFormularyCoinsurance + retailNonFormularyCoinsurance + mailGenericCoinsurance + mailFormularyCoinsurance + mailNonFormularyCoinsurance;
                if (sumCoinsurance == 0)
                {
                    RxoopTrigger = deductibleTrigger;
                }
                else
                {
                    stepl = oopMaxCopay / sumCoinsurance;
                    stepll = stepl + deductibleTrigger;
                    RxoopTrigger = stepll;
                }

                return RxoopTrigger;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateAdjCPDDeductibleValue(int status, int medstatus, PlanDesignMedical objPlanMedical, string individual)
        {
            try
            {
                string cpdTier = "R";
                decimal RG, RF, RNF, MG, MF, MNF;
                decimal divfactorRG, divfactorRF, divfactorRNF, divfactorMG, divfactorMF, divfactorMNF, deductibleValue;
                switch (individual)
                {
                    case "Employee":
                        divfactorRG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.RGenericRatio * miscconConservFact;
                        divfactorRF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.RFormRatio * miscconConservFact;
                        divfactorRNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.RNonFormRatio * miscconConservFact;
                        divfactorMG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.MGenericRatio * miscconConservFact;
                        divfactorMF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.MFormRatio * miscconConservFact;
                        divfactorMNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.MNonFormRatio * miscconConservFact;

                        RG = CalculateAdjCPDDeductible(cpdTier, divfactorRG, objPDEntities.EmpRxDeductibleTrigger, status);
                        RF = CalculateAdjCPDDeductible(cpdTier, divfactorRF, objPDEntities.EmpRxDeductibleTrigger, status);
                        RNF = CalculateAdjCPDDeductible(cpdTier, divfactorRNF, objPDEntities.EmpRxDeductibleTrigger, status);
                        MG = CalculateAdjCPDDeductible(cpdTier, divfactorMG, objPDEntities.EmpRxDeductibleTrigger, status);
                        MF = CalculateAdjCPDDeductible(cpdTier, divfactorMF, objPDEntities.EmpRxDeductibleTrigger, status);
                        MNF = CalculateAdjCPDDeductible(cpdTier, divfactorMNF, objPDEntities.EmpRxDeductibleTrigger, status);
                        deductibleValue = ((RG * objPDEntities.RGeneric) + (RF * objPDEntities.RForm) + (RNF * objPDEntities.RNonForm) + (MF * objPDEntities.MForm) + (MG * objPDEntities.MGeneric) + (MNF * objPDEntities.MNonForm)) * medstatus;
                        break;
                    case "Spouse":
                        divfactorRG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.RGenericRatio * miscconConservFact;
                        divfactorRF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.RFormRatio * miscconConservFact;
                        divfactorRNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.RNonFormRatio * miscconConservFact;
                        divfactorMG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.MGenericRatio * miscconConservFact;
                        divfactorMF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.MFormRatio * miscconConservFact;
                        divfactorMNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.MNonFormRatio * miscconConservFact;

                        RG = CalculateAdjCPDDeductible(cpdTier, divfactorRG, objPDEntities.SpouseRxDeduxtibleTrigger, status);
                        RF = CalculateAdjCPDDeductible(cpdTier, divfactorRF, objPDEntities.SpouseRxDeduxtibleTrigger, status);
                        RNF = CalculateAdjCPDDeductible(cpdTier, divfactorRNF, objPDEntities.SpouseRxDeduxtibleTrigger, status);
                        MG = CalculateAdjCPDDeductible(cpdTier, divfactorMG, objPDEntities.SpouseRxDeduxtibleTrigger, status);
                        MF = CalculateAdjCPDDeductible(cpdTier, divfactorMF, objPDEntities.SpouseRxDeduxtibleTrigger, status);
                        MNF = CalculateAdjCPDDeductible(cpdTier, divfactorMNF, objPDEntities.SpouseRxDeduxtibleTrigger, status);
                        deductibleValue = ((RG * objPDEntities.RGeneric) + (RF * objPDEntities.RForm) + (RNF * objPDEntities.RNonForm) + (MF * objPDEntities.MForm) + (MG * objPDEntities.MGeneric) + (MNF * objPDEntities.MNonForm)) * medstatus;
                        break;
                    case "Child":

                        divfactorRG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.RGenericRatio * miscconConservFact;
                        divfactorRF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.RFormRatio * miscconConservFact;
                        divfactorRNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.RNonFormRatio * miscconConservFact;
                        divfactorMG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.MGenericRatio * miscconConservFact;
                        divfactorMF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.MFormRatio * miscconConservFact;
                        divfactorMNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.MNonFormRatio * miscconConservFact;

                        RG = CalculateAdjCPDDeductible(cpdTier, divfactorRG, objPDEntities.ChildRxDeductibleTrigger, status);
                        RF = CalculateAdjCPDDeductible(cpdTier, divfactorRF, objPDEntities.ChildRxDeductibleTrigger, status);
                        RNF = CalculateAdjCPDDeductible(cpdTier, divfactorRNF, objPDEntities.ChildRxDeductibleTrigger, status);
                        MG = CalculateAdjCPDDeductible(cpdTier, divfactorMG, objPDEntities.ChildRxDeductibleTrigger, status);
                        MF = CalculateAdjCPDDeductible(cpdTier, divfactorMF, objPDEntities.ChildRxDeductibleTrigger, status);
                        MNF = CalculateAdjCPDDeductible(cpdTier, divfactorMNF, objPDEntities.ChildRxDeductibleTrigger, status);
                        deductibleValue = ((RG * objPDEntities.RGeneric) + (RF * objPDEntities.RForm) + (RNF * objPDEntities.RNonForm) + (MF * objPDEntities.MForm) + (MG * objPDEntities.MGeneric) + (MNF * objPDEntities.MNonForm)) * medstatus;
                        break;
                    default:
                        deductibleValue = 0;
                        break;
                }
                return deductibleValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculateAdjCPDDeductible(string cpdTier, decimal divfactor, decimal deductibleTrigger, int status)
        {
            try
            {
                decimal cumAmtlukupone, cumAmtlukuptwo, cumFreqlukupone, cumFreqlukuptwo, ValDeductible;
                cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColTotalCummulativeAmt) * divfactor;
                cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibleTrigger / divfactor, Resources.Constants.ColTotalCummulativeAmt) * divfactor;
                cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibleTrigger / divfactor, Resources.Constants.ColCumulativeFrequency) * deductibleTrigger;
                cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, 0, Resources.Constants.ColCumulativeFrequency) * divfactor;
                ValDeductible = (1 * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo * 0)) * status;
                return ValDeductible;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private decimal CalculateAdjCPDCoinsuranceOOPMax(string cpdTier, decimal divfactor, decimal oopMAXRx, decimal deductibleTrigger, int status, decimal rxOOPTrigger)
        {
            try
            {
                decimal cumAmtlukupone, cumAmtlukuptwo, cumFreqlukupone, cumFreqlukuptwo, CoinsuranceOOPMax;
                cumAmtlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, deductibleTrigger / divfactor, Resources.Constants.ColTotalCummulativeAmt) * divfactor;
                cumAmtlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, rxOOPTrigger / divfactor, Resources.Constants.ColTotalCummulativeAmt) * divfactor;
                cumFreqlukupone = OOPDAL.GetAnnClaimAmount(cpdTier, rxOOPTrigger / divfactor, Resources.Constants.ColCumulativeFrequency) * rxOOPTrigger;
                cumFreqlukuptwo = OOPDAL.GetAnnClaimAmount(cpdTier, deductibleTrigger / divfactor, Resources.Constants.ColCumulativeFrequency) * deductibleTrigger;
                CoinsuranceOOPMax = (oopMAXRx * (cumAmtlukupone - cumAmtlukuptwo + cumFreqlukupone - cumFreqlukuptwo)) * status;
                return CoinsuranceOOPMax;
            }
            catch (Exception ex)
            { throw ex; }
        }

        private decimal CalculateRxCoinsuranceOOPMaxValue(int status, int medStatus, PlanDesignMedical objPlanMedical, string individual)
        {
            try
            {
                string cpdTier = "R";
                decimal empRG, empRF, empRNF, empMG, empMF, empMNF;
                decimal spouseRG, spouseRF, spouseRNF, spouseMG, spouseMF, spouseMNF;
                decimal childRG, childRF, childRNF, childMG, childMF, childMNF;
                decimal divfactorRG, divfactorRF, divfactorRNF, divfactorMG, divfactorMF, divfactorMNF;
                decimal rgCoinsurance, rfCoinsurance, rnfCoinsurance, mgCoinsurance, mfCoinsurance, mnfCoinsurance, rxEffectiveCopay;
                decimal oopMaxEmp, oopMaxSpouse, oopMaxChild;
                rgCoinsurance = 1 - objPlanMedical.RxCoInsuranceRetailGeneric;
                rfCoinsurance = 1 - objPlanMedical.RxCoInsuranceRetailForm;
                rnfCoinsurance = 1 - objPlanMedical.RxCoInsuranceRetailNonForm;
                mgCoinsurance = 1 - objPlanMedical.RxCoInsuranceMailGeneric;
                mfCoinsurance = 1 - objPlanMedical.RxCoInsuranceMailForm;
                mnfCoinsurance = 1 - objPlanMedical.RxCoInsuranceMailNonForm;

                rxEffectiveCopay = (objPlanMedical.RxCopayRetailGeneric * objPDEntities.RGeneric) + (objPlanMedical.RxCopayRetailForm * objPDEntities.RForm) + (objPlanMedical.RxCopayRetailNonForm * objPDEntities.RNonForm);
                rxEffectiveCopay = (rxEffectiveCopay + (objPlanMedical.RxCopayMailGeneric * objPDEntities.MGeneric) + (objPlanMedical.RxCopayMailForm * objPDEntities.MForm) + (objPlanMedical.RxCopayMailNonForm * objPDEntities.MNonForm)) * medStatus;

                if (individual == "Employee")
                {
                    oopMaxEmp = CalculateOOPMax(objPlanMedical, "Employee");//B98
                    objPDEntities.EmpRxOOPTrigger = CalculateRxOOPTrigger(Math.Max(oopMaxEmp - objPDEntities.EmpTotalRxCopay, 0), objPDEntities.EmpRxDeductibleTrigger, objPlanMedical, "Employee");

                    divfactorRG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.RGenericRatio * miscconConservFact;
                    divfactorRF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.RFormRatio * miscconConservFact;
                    divfactorRNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.RNonFormRatio * miscconConservFact;
                    divfactorMG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.MGenericRatio * miscconConservFact;
                    divfactorMF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.MFormRatio * miscconConservFact;
                    divfactorMNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.EmpRxAgeGender) * objPDEntities.EmpPDUtil * objPDEntities.MNonFormRatio * miscconConservFact;

                    empRG = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRG, rgCoinsurance, objPDEntities.EmpRxDeductibleTrigger, status, objPDEntities.EmpRxOOPTrigger);
                    empRF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRF, rfCoinsurance, objPDEntities.EmpRxDeductibleTrigger, status, objPDEntities.EmpRxOOPTrigger);
                    empRNF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRNF, rnfCoinsurance, objPDEntities.EmpRxDeductibleTrigger, status, objPDEntities.EmpRxOOPTrigger);
                    empMG = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMG, mgCoinsurance, objPDEntities.EmpRxDeductibleTrigger, status, objPDEntities.EmpRxOOPTrigger);
                    empMF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMF, mfCoinsurance, objPDEntities.EmpRxDeductibleTrigger, status, objPDEntities.EmpRxOOPTrigger);
                    empMNF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMNF, mnfCoinsurance, objPDEntities.EmpRxDeductibleTrigger, status, objPDEntities.EmpRxOOPTrigger);
                    objPDEntities.EmpRxCoinOOPMax = ((empRG * objPDEntities.RGeneric) + (empRF * objPDEntities.RForm) + (empRNF * objPDEntities.RNonForm) + (empMF * objPDEntities.MForm) + (empMG * objPDEntities.MGeneric) + (empMNF * objPDEntities.MNonForm)) * medStatus;
                    return objPDEntities.EmpRxCoinOOPMax;
                }
                else if (individual == "Spouse")
                {
                    oopMaxSpouse = CalculateOOPMax(objPlanMedical, "Spouse");//D98
                    objPDEntities.SpouseRxOOPTrigger = CalculateRxOOPTrigger(Math.Max(oopMaxSpouse - objPDEntities.SpouseTotalRxCopay, 0), objPDEntities.SpouseRxDeduxtibleTrigger, objPlanMedical, "Spouse");

                    divfactorRG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.RGenericRatio * miscconConservFact;
                    divfactorRF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.RFormRatio * miscconConservFact;
                    divfactorRNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.RNonFormRatio * miscconConservFact;
                    divfactorMG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.MGenericRatio * miscconConservFact;
                    divfactorMF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.MFormRatio * miscconConservFact;
                    divfactorMNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.SpouseRxAgeGender) * objPDEntities.SpousePDUtil * objPDEntities.MNonFormRatio * miscconConservFact;

                    spouseRG = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRG, rgCoinsurance, objPDEntities.SpouseRxDeduxtibleTrigger, status, objPDEntities.SpouseRxOOPTrigger);
                    spouseRF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRF, rfCoinsurance, objPDEntities.SpouseRxDeduxtibleTrigger, status, objPDEntities.SpouseRxOOPTrigger);
                    spouseRNF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRNF, rnfCoinsurance, objPDEntities.SpouseRxDeduxtibleTrigger, status, objPDEntities.SpouseRxOOPTrigger);
                    spouseMG = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMG, mgCoinsurance, objPDEntities.SpouseRxDeduxtibleTrigger, status, objPDEntities.SpouseRxOOPTrigger);
                    spouseMF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMF, mfCoinsurance, objPDEntities.SpouseRxDeduxtibleTrigger, status, objPDEntities.SpouseRxOOPTrigger);
                    spouseMNF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMNF, mnfCoinsurance, objPDEntities.SpouseRxDeduxtibleTrigger, status, objPDEntities.SpouseRxOOPTrigger);
                    objPDEntities.SpouseRxCoinOOPMax = ((spouseRG * objPDEntities.RGeneric) + (spouseRF * objPDEntities.RForm) + (spouseRNF * objPDEntities.RNonForm) + (spouseMF * objPDEntities.MForm) + (spouseMG * objPDEntities.MGeneric) + (spouseMNF * objPDEntities.MNonForm)) * medStatus;
                    return objPDEntities.SpouseRxCoinOOPMax;
                }
                else
                    oopMaxChild = CalculateOOPMax(objPlanMedical, "Child", status);//F98
                objPDEntities.ChildRxOOPTrigger = CalculateRxOOPTrigger(Math.Max(oopMaxChild - objPDEntities.ChildTotalRxCopay / status, 0), objPDEntities.ChildRxDeductibleTrigger, objPlanMedical, "Child");

                divfactorRG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.RGenericRatio * miscconConservFact;
                divfactorRF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.RFormRatio * miscconConservFact;
                divfactorRNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.RNonFormRatio * miscconConservFact;
                divfactorMG = (1 - objPlanMedical.RxMACDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.MGenericRatio * miscconConservFact;
                divfactorMF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.MFormRatio * miscconConservFact;
                divfactorMNF = (1 - objPlanMedical.RxAWPDiscount) * (Rxareafactor * objPDEntities.ChildRxAgeGender) * objPDEntities.ChildPDUtil * objPDEntities.MNonFormRatio * miscconConservFact;

                childRG = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRG, rgCoinsurance, objPDEntities.ChildRxDeductibleTrigger, status, objPDEntities.ChildRxOOPTrigger);
                childRF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRF, rfCoinsurance, objPDEntities.ChildRxDeductibleTrigger, status, objPDEntities.ChildRxOOPTrigger);
                childRNF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorRNF, rnfCoinsurance, objPDEntities.ChildRxDeductibleTrigger, status, objPDEntities.ChildRxOOPTrigger);
                childMG = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMG, mgCoinsurance, objPDEntities.ChildRxDeductibleTrigger, status, objPDEntities.ChildRxOOPTrigger);
                childMF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMF, mfCoinsurance, objPDEntities.ChildRxDeductibleTrigger, status, objPDEntities.ChildRxOOPTrigger);
                childMNF = CalculateAdjCPDCoinsuranceOOPMax(cpdTier, divfactorMNF, mnfCoinsurance, objPDEntities.ChildRxDeductibleTrigger, status, objPDEntities.ChildRxOOPTrigger);
                objPDEntities.ChildRxCoinOOPMax = ((childRG * objPDEntities.RGeneric) + (childRF * objPDEntities.RForm) + (childRNF * objPDEntities.RNonForm) + (childMF * objPDEntities.MForm) + (childMG * objPDEntities.MGeneric) + (childMNF * objPDEntities.MNonForm)) * medStatus;
                return objPDEntities.ChildRxCoinOOPMax;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
