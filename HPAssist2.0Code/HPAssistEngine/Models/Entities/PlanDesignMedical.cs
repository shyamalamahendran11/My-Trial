using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HPAssistEngine.Models.Entities
{
    public class PlanDesignMedical
    {
        public decimal Deductible { get; set; }
        public decimal FamilyDeductible { get; set; }
        public decimal FamilyOOPMax { get; set; }
        public decimal MemberCoinsurance { get; set; }
        public decimal OOPMax { get; set; }
        public decimal OVCopay { get; set; }
        public decimal OVCoinsurance { get; set; }
        public decimal SpecialistCopay { get; set; }
        public decimal SpecialistCoinsurance { get; set; }
        public decimal InNetDiscount { get; set; }
        public decimal OutNetDiscount { get; set; }
        public decimal OutNetDeductible { get; set; }
        public decimal OutNetFamilyDeductible { get; set; }
        public decimal OutNetFamilyOOPMax { get; set; }
        public decimal OutNetMemberCoinsurance { get; set; }
        public decimal OutNetOOPMax { get; set; }
        public decimal NetworkUtil { get; set; }
        public decimal RxCopayRetailGeneric { get; set; }
        public decimal RxCopayRetailForm { get; set; }
        public decimal RxCopayRetailNonForm { get; set; }
        public decimal RxCopayMailGeneric { get; set; }
        public decimal RxCopayMailForm { get; set; }
        public decimal RxCopayMailNonForm { get; set; }
        public string CPDTableDeductible { get; set; }
        public string CPDTableOOPMax { get; set; }
        public decimal RxCoInsuranceRetailGeneric { get; set; }
        public decimal RxCoInsuranceRetailForm { get; set; }
        public decimal RxCoInsuranceRetailNonForm { get; set; }
        public decimal RxCopayMaxRetailGeneric { get; set; }
        public decimal RxCopayMinRetailGeneric { get; set; }
        public decimal RxCopayMaxRetailForm { get; set; }
        public decimal RxCopayMinRetailForm { get; set; }
        public decimal RxCopayMaxRetailNonForm { get; set; }
        public decimal RxCopayMinRetailNonForm { get; set; }
        public decimal RxCopayMaxMailGeneric { get; set; }
        public decimal RxCopayMinMailGeneric { get; set; }
        public decimal RxCopayMaxMailForm { get; set; }
        public decimal RxCopayMinMailForm { get; set; }
        public decimal RxCopayMaxMailNonForm { get; set; }
        public decimal RxCopayMinMailNonForm { get; set; }
        public decimal RxIncludedInDeductibleAmt { get; set; }
        public string CPDTableOutNetwork { get; set; }
        public decimal RxOOPMax { get; set; }
        public decimal RxMACDiscount { get; set; }
        public decimal RxAWPDiscount { get; set; }
        public decimal RxCoInsuranceMailGeneric { get; set; }
        public decimal RxCoInsuranceMailForm { get; set; }
        public decimal RxCoInsuranceMailNonForm { get; set; }
    }

}
