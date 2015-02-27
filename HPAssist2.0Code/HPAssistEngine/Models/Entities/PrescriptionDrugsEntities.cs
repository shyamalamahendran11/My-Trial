using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HPAssistEngine.Models.Entities
{
    public class PrescriptionDrugsEntities
    {
        public decimal EmpPDUtil { get; set; }
        public decimal SpousePDUtil { get; set; }
        public decimal ChildPDUtil { get; set; }

        public decimal EmpMedUtil { get; set; }
        public decimal SpouseMedUtil { get; set; }
        public decimal ChildMedUtil { get; set; }

        public decimal EmpMedAAGFactor { get; set; }
        public decimal SpouseMedAAGFactor { get; set; }
        public decimal ChildMedAAGFactor { get; set; }

        public decimal EmpAdjRxNoofScripts { get; set; }
        public decimal SpouseAdjRxNoofScripts { get; set; }
        public decimal ChildAdjRxNoofScripts { get; set; }

        public decimal MedBeginOOP { get; set; }
        public decimal TotalCost { get; set; }

        public decimal EmpRxValofDeductible { get; set; }
        public decimal SpouseRxValofDeductible { get; set; }
        public decimal ChildRxValofDeductible { get; set; }

        public decimal EmpRxCoinOOPMax { get; set; }
        public decimal SpouseRxCoinOOPMax { get; set; }
        public decimal ChildRxCoinOOPMax { get; set; }

        public decimal TotalRxCoinOOPMax { get; set; }

        public decimal EmpRxOOPMax { get; set; }
        public decimal SpouseRxOOPMax { get; set; }
        public decimal ChildRxOOPMax { get; set; }

        public decimal EmpTotalRxCopay { get; set; }
        public decimal SpouseTotalRxCopay { get; set; }
        public decimal ChildTotalRxCopay { get; set; }

        public decimal EmpRxDeductibleTrigger { get; set; }
        public decimal SpouseRxDeduxtibleTrigger { get; set; }
        public decimal ChildRxDeductibleTrigger { get; set; }

        public decimal EmpRxOOPTrigger { get; set; }
        public decimal SpouseRxOOPTrigger { get; set; }
        public decimal ChildRxOOPTrigger { get; set; }

        public decimal RGenericRatio { get; set; }
        public decimal RFormRatio { get; set; }
        public decimal RNonFormRatio { get; set; }
        public decimal MGenericRatio { get; set; }
        public decimal MFormRatio { get; set; }
        public decimal MNonFormRatio { get; set; }

        public decimal RGeneric { get; set; }
        public decimal RForm { get; set; }
        public decimal RNonForm { get; set; }
        public decimal MGeneric { get; set; }
        public decimal MForm { get; set; }
        public decimal MNonForm { get; set; }

        public decimal EmpRxAgeGender { get; set; }
        public decimal SpouseRxAgeGender { get; set; }
        public decimal ChildRxAgeGender { get; set; }

        public decimal EmpOVTotCopayCost { get; set; }
        public decimal SpouseOVTotCopayCost { get; set; }
        public decimal ChildOVTotCopayCost { get; set; }

        public decimal EmpDeductibleAdjFactor { get; set; }
        public decimal SpouseDeductibleAdjFactor { get; set; }
        public decimal ChildDeductibleAdjFactor { get; set; }

    }
}