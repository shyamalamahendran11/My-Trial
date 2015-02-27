using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPCalculation
{
    class VisionInNetwork
    {
        public decimal empAreaFactor { get; set; }
        public decimal spouseAreaFactor { get; set; }
        public decimal childAreaFactor { get; set; }

        public decimal empAgeGenderFactor { get; set; }
        public decimal spouseAgeGenderFactor { get; set; }
        public decimal childAgeGenderFactor { get; set; }

        public decimal empLensesAAGFactor { get; set; }
        public decimal spouseLensesAAGFactor { get; set; }
        public decimal childLensesAAGFactor { get; set; }

        public decimal empAdjVisionCosts { get; set; }
        public decimal spouseAdjVisionCosts { get; set; }
        public decimal childAdjVisionCosts { get; set; }

        public decimal empAdjCopayLimits { get; set; }
        public decimal spouseCopayLimits { get; set; }
        public decimal childCopayLimits { get; set; }

        public decimal visionCost { get; set; }
    }
}
