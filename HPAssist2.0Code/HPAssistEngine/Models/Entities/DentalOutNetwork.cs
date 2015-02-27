using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPCalculation
{
    class DentalOutNetwork
    {
        #region old
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
        //public decimal TotalBaseDentalCosts { get; set; }
        //public decimal ApplyNoCoverageDentalCosts { get; set; }   
        #endregion

        public decimal empOutAAGFactorAverage { get; set; }
        public decimal spouseOutAAGFactorAverage { get; set; }
        public decimal childOutAAGFactorAverage { get; set; }

        public decimal empOutdeductibleTrigger { get; set; }
        public decimal spouseOutdeductibleTrigger { get; set; }
        public decimal childOutdeductibleTrigger { get; set; }

        public decimal empOutMemCoinAvg { get; set; }
        public decimal spouseOutMemCoinAvg { get; set; }
        public decimal childOutMemCoinAvg { get; set; }

        public decimal empOutannualMaxTrigger { get; set; }
        public decimal spouseOutannualMaxTrigger { get; set; }
        public decimal childOutannualMaxTrigger { get; set; }

        public decimal empOutValueofDeductible { get; set; }
        public decimal spouseOutValueofDeductible { get; set; }
        public decimal childOutValueofDeductible { get; set; }

        public decimal empOutValueofCoin { get; set; }
        public decimal spouseOutValueofCoin { get; set; }
        public decimal childOutValueofCoin { get; set; }

        public decimal empOutvalaboveAnnualMax { get; set; }
        public decimal spouseOutvalaboveAnnualMax { get; set; }
        public decimal childOutvalaboveAnnualMax { get; set; }

        public decimal empOutDentalCost { get; set; }
        public decimal spouseOutDentalCost { get; set; }
        public decimal childOutDentalCost { get; set; }

        public decimal empOutTotBaseDentalCost { get; set; }
        public decimal spouseOutTotBaseDentalCost { get; set; }
        public decimal childOutTotBaseDentalCost { get; set; }

        public decimal empOutApplyNoCovDentalCosts { get; set; }
        public decimal spouseOutApplyNoCovDentalCosts { get; set; }
        public decimal childOutApplyNoCovDentalCosts { get; set; }

    }
}
