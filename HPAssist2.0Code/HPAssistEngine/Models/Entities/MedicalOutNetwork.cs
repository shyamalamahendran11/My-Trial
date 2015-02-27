using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPCalculation
{
    class MedicalOutNetwork
    {
        public decimal EmpOOPTrigger { get; set; }  //C28
        public decimal SpouseOOPTrigger { get; set; }//E28
        public decimal ChildOOPTrigger { get; set; }  //G28

        public decimal EmpCoinOOPMax { get; set; }//C31
        public decimal SpouseCoinOOPMax { get; set; }//E31
        public decimal ChildCoinOOPMax { get; set; }//G31

        public decimal MemberCoinsurance { get; set; }//C24

        public decimal EmpDeductibleTrigger { get; set; }//C23
        public decimal SpouseDeductibleTrigger { get; set; }//E23
        public decimal ChildDeductibleTrigger { get; set; }//G23

        public decimal EmpMedValofDeductible { get; set; }//C30
        public decimal SpouseMedValofDeductible { get; set; }//E30
        public decimal ChildMedValofDeductible { get; set; }//G30
    
    }
}
