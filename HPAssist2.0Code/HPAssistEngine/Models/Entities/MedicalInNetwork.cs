using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPCalculation
{
    class MedicalInNetwork
    {
        public decimal AreaFactor { get; set; }
        public decimal SpCopayAreaFactor { get; set; }

        public decimal EmpAgeGenderFactor { get; set; }
        public decimal SpouseAgeGenderFactor { get; set; }
        public decimal ChildAgeGenderFactor { get; set; }

        public decimal EmpAgeGenderFactorSpCoPay { get; set; }
        public decimal SpouseAgeGenderFactorSpCoPay { get; set; }
        public decimal ChildAgeGenderFactorSpCoPay { get; set; }

        public decimal EmpAgeGenderCostFactorSpCoPay { get; set; }
        public decimal SpouseAgeGenderCostFactorSpCoPay { get; set; }
        public decimal ChildAgeGenderCostFactorSpCoPay { get; set; }

        public decimal EmpMedicalAAGFactor { get; set; }
        public decimal SpouseMedicalAAGFactor { get; set; }
        public decimal ChildMedicalAAGFactor { get; set; }

        public decimal EmpMedicalAAGFactorSpCoPay { get; set; }
        public decimal SpouseMedicalAAGFactorSpCoPay { get; set; }
        public decimal ChildMedicalAAGFactorSpCoPay { get; set; }

        public decimal EmpMedicalAAGCostFactorSpCoPay { get; set; }
        public decimal SpouseMedicalAAGCostFactorSpCoPay { get; set; }
        public decimal ChildMedicalAAGCostFactorSpCoPay { get; set; }

        public decimal Deductible { get; set; }
        public decimal EmpDeductibleReduction { get; set; }
        public decimal SpouseDeductibleReduction { get; set; }
        public decimal ChildDeductibleReduction { get; set; }

        public decimal EmpDeductibleTrigger { get; set; }
        public decimal SpouseDeductibleTrigger { get; set; }
        public decimal ChildDeductibleTrigger { get; set; }

        public decimal MemberCoinsurance { get; set; }
        public decimal BeginningOOP { get; set; }
        public decimal CoPayAmountSpCoPay { get; set; }

        public decimal EmpTotSplstCopayCost { get; set; }
        public decimal SpouseTotSplstCopayCost { get; set; }
        public decimal ChildTotSplstCopayCost { get; set; }

        public decimal EmpOOPReduction { get; set; }
        public decimal SpouseOOPReduction { get; set; }
        public decimal ChildOOPReduction { get; set; }

        public decimal EmpMedUtil { get; set; }
        public decimal SpouseMedUtil { get; set; }
        public decimal ChildMedUtil { get; set; }

        public decimal EmpAdjustedOOP { get; set; }
        public decimal SpouseAdjustedOOP { get; set; }
        public decimal ChildAdjustedOOP { get; set; }

        public decimal EmpOOPTrigger { get; set; }
        public decimal SpouseOOPTrigger { get; set; }
        public decimal ChildOOPTrigger { get; set; }

        public decimal EmpMedValofDeductible { get; set; }
        public decimal SpouseMedValofDeductible { get; set; }
        public decimal ChildMedValofDeductible { get; set; }

        public decimal EmpCoinOOPMax { get; set; }
        public decimal SpouseCoinOOPMax { get; set; }
        public decimal ChildCoinOOPMax { get; set; }
    }
}
