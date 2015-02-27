
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace HPAssistEngine.Resources
{
    public class Constants
    {

        public const string HPAssistRequestFileName = "HPAssistUploadRequest";
        public const string HPAssistCompareCostRequestFile = "HPAssistCompareCostRequest";
        public const string ExceptionCaught = "EC000";
        public const string MemberDataFileName = "MemberData";

        public const string PrescDrug = "PrescriptionDrugs";
        public const string ColRxFactor = "Rxfactor";
        public const string ColRxNumberofScripts = "RxNumberofScripts";
        public const string ColRxAAGAdjFactor = "RxAAGAdjFactor";
        public const string ColPrescriptions = "Prescriptions";
        public const string ColRxUtilization = "RxUtilization";

        public const string Dental = "DentalFillings";
        public const string ColOrthoCostFactor = "OrthoCostFactor";
        public const string ColDentalFillingsFactor = "DentalFillingsFactor";
        public const string ColDentalFillings = "DentalFillings";

        public const string Vision = "Correctivelenses";
        public const string ColVisionCostFactor = "VisionCostFactor";
        public const string ColVisionUtilFactor = "VisionUtilFactor";
        public const string ColCorrectiveLenses = "CorrectiveLenses";
        public const string ColVisionAAGAdjFactor = "VisionAAGAdjFactor";

        public const string OfficeVisits = "OfficeVisits";
        public const string SpecialistVisits = "SpecialistVisits";
        public const string ColOVFactor = "OVFactor";
        public const string ColMedicalFactor = "MedicalFactor";
        public const string ColDentalFactor = "DentalFactor";

        public const string ColOVAAGAdjFactor = "OVAAGAdjFactor";
        public const string ColUtilisationOV = "OVUtilization";
        public const string ColChildGender = "C";
        public const string ColOVCopay = "OVCopay";

        public const string ColSpecialistFactor = "SpecialistFactor";
        public const string ColSpecialist = "Specialist";
        public const string ColSpecialistUtil = "SpecialistUtilization";
        public const string ColSpecialistCopay = "SpecialistCopay";

        public const string ColOrthoClaimAmt = "OrthoClaimAmt";
        public const string ColDentalOrthoFactor = "DentalOrthoFactor";
        public const string ColDentalweightAdult = "Adult";
        public const string ColDentalweightChild = "Child";
        public const string ColDentalAgeGenClassl = "DentalClassIFactor";
        public const string ColDentalAgeGenClassll = "DentalClassIIFactor";
        public const string ColDentalAgeGenClasslll = "DentalClassIIIFactor";
        public const string ColTotalAnnualClaimAmt = "TotalAnnualClaimAmount";
        public const string ColCumulativeFrequency = "CumulativeFrequency";
        public const string ColTotalCummulativeAmt = "CumulativeAnnualClaimAmount";
        public const string ColTierTypeA = "A";
        public const string ColTierTypeC = "C";
        public const string ColVisionClaimAmount = "VisionClaimAmt";
        public const string ColTierTypeR = "R";
        public const string ColMiscCostRx = "Rx%ofTotalCost";
        public const string ColSpecialistCost = "SpecialistCost";
        public const string ColSpecialistNormalized = "SpecialistNormalized";
        public const string ColAverageCostSpc = "AverageCostSpc";
        public const string ColSpcofTotalCost = "Spc%ofTotalCost";
        public const string ColRxofTotalCost = "Rx%ofTotalCost";
        public const string ColRXCost = "rxCost";
        public const string ColOVCost = "OVCost";
        public const string ColMacRx = "RxMAC%";
        public const string ColAwpRx = "RxAWP%";
        public const string ColMedicalCost = "MedicalCost";
        public const string ColAverageCostPCP = "AverageCostPCP";
        public const string ColRxFactorNormalized = "RxFactorNormalized";
        public const string ColOVFactorNormalized = "OVFactorNormalized";
        public const string ColAvgCostRx = "AverageCostRx";
        public const string ColRxBaseAdjFactor = "RxBaseAdjFactor";
        public const string ColPCPTotalCost = "PCP%ofTotalCost";
        public const string ColConservative_Factor = "Conservative_Factor";

        public const string NotEligible = "Not Eligible For Premium Subsidy";
        public const string Eligible = "Eligible For Premium Subsidy";
        public const string MedicalIdNotAvail = "MedicalId Is Not Available";
        public const string MedicalIdNotSel = "Medical Plan Is Not Selected";
        public const string DentalIdNotAvail="DentalId Is Not Available";
        public const string DentalIdNotSel = "Dental Plan Is Not Selected";
        public const string VisionIdNotAvail = "VisionId Is Not Available";
        public const string VisionIdNotSel = "Vision Plan Is Not Selected";
        public const string ExchangePlanforModelNotExist ="Exchange Plan does not exist for this Model with AccessCode ";
        public const string ExchangePlanforZipCodeNotExist="Exchange Plan Does not Exist for this ZipCode:";
        public const string msgTransIDnotPresent = "Requested Transaction ID does not exist In Database";

        public const string Regular = "Regular";
        public const string Compare = "Compare";

    }
}