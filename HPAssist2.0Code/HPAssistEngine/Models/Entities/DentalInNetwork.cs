using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPCalculation
{
    class DentalInNetwork
    {
        #region Old
        //public decimal AreaFactor { get; set; }
        //public decimal AgeGenderFactorClass1 { get; set; }
        //public decimal AgeGenderFactorClass2 { get; set; }
        //public decimal AgeGenderFactorClass3 { get; set; }
        //public decimal DentalAAGFactorClass1 { get; set; }
        //public decimal DentalAAGFactorClass2 { get; set; }
        //public decimal DentalAAGFactorClass3 { get; set; }
        //public decimal AAGFactorAvg { get; set; }       
        //public decimal DeductibleTriggerAvg { get; set; }
        //public decimal MemberCoinsuranceAvg { get; set; }
        //public decimal AnnualMaxTrigger { get; set; }       
        //public decimal DeductibleValue { get; set; }
        //public decimal ConinsuranceValue { get; set; }
        //public decimal AnnualMaxValue { get; set; }
        //public decimal DentalCosts { get; set; }
        //public decimal OrthoAreaFactor { get; set; }
        //public decimal OrthoAgeGenderFactor { get; set; }
        //public decimal OrthoAAGFactor { get; set; }
        //public decimal OrthoCosts { get; set; }
        //public decimal OrthoApplyCoinsAnnualBenefitMax { get; set; }
        #endregion

        public decimal empInAAGFactorAverage { get; set; }
        public decimal spouseInAAGFactorAverage { get; set; }
        public decimal childInAAGFactorAverage { get; set; }

        public decimal empIndeductibleTrigger { get; set; }
        public decimal spouseIndeductibleTrigger { get; set; }
        public decimal childIndeductibleTrigger { get; set; }

        public decimal empInMemCoinAvg { get; set; }
        public decimal spouseInMemCoinAvg { get; set; }
        public decimal childInMemCoinAvg { get; set; }

        public decimal empInannualMaxTrigger { get; set; }
        public decimal spouseInannualMaxTrigger { get; set; }
        public decimal childInannualMaxTrigger { get; set; }

        public decimal empInValueofDeductible { get; set; }
        public decimal spouseInValueofDeductible { get; set; }
        public decimal childInValueofDeductible { get; set; }

        public decimal empInValueofCoin { get; set; }
        public decimal spouseInValueofCoin { get; set; }
        public decimal childInValueofCoin { get; set; }

        public decimal empInvalaboveAnnualMax { get; set; }
        public decimal spouseInvalaboveAnnualMax { get; set; }
        public decimal childInvalaboveAnnualMax { get; set; }

        public decimal empInDentalCost { get; set; }
        public decimal spouseInDentalCost { get; set; }
        public decimal childInDentalCost { get; set; }

        public decimal empInAreaFactor { get; set; }
        public decimal spouseInAreaFactor { get; set; }
        public decimal childInAreaFactor { get; set; }

        public decimal empInageGenderFactor { get; set; }
        public decimal spouseInageGenderFactor { get; set; }
        public decimal childInageGenderFactor { get; set; }

        public decimal empInorthodontiaAAGFactor { get; set; }
        public decimal spouseInorthodontiaAAGFactor { get; set; }
        public decimal childInorthodontiaAAGFactor { get; set; }

        public decimal empInOrthodontiaCosts { get; set; }
        public decimal spouseInOrthodontiaCosts { get; set; }
        public decimal childInOrthodontiaCosts { get; set; }

        public decimal empInapplyCoinsAnnualBenefitMax { get; set; }
        public decimal spouseInapplyCoinsAnnualBenefitMax { get; set; }
        public decimal childInapplyCoinsAnnualBenefitMax { get; set; }

        public decimal totalDentalCost { get; set; }
        public decimal totalAnnualBenefitMax { get; set; }
        public decimal totalCoverageDentalCost { get; set; }
    }
}
