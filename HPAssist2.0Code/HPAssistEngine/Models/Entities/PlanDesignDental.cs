using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HPAssistEngine.Models.Entities
{
    public class PlanDesignDental
    {
        public int  DentalID { get; set; }
        public int  ModelID { get; set; }
        public string PlanName { get; set; }
        public decimal ClassIDeductible { get; set; }
        public decimal  ClassIIDeductible { get; set; }
        public decimal  ClassIIIDeductible { get; set; }
        public decimal ClassICoinsurance { get; set; }
        public decimal ClassIICoinsurance { get; set; }
        public decimal ClassIIICoinsurance { get; set; }
        public decimal  AnnualMax { get; set; }
        public decimal OrthoMax { get; set; }
        public decimal  OutNetClassIDeductible { get; set; }
        public decimal OutNetClassIIDeductible { get; set; }
        public decimal OutNetClassIIIDeductible { get; set; }
        public decimal OutNetClassICoinsurance { get; set; }
        public decimal OutNetClassIICoinsurance { get; set; }
        public decimal OutNetClassIIICoinsurance { get; set; }
        public decimal  OutNetAnnualMax { get; set; }
        public decimal OutNetOrthoMax { get; set; }
        public decimal NetworkUtil { get; set; }
        public decimal InNetDiscount { get; set; }
        public decimal OutNetDiscount { get; set; }

    }
}