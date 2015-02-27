#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
#endregion


namespace HPAssistEngine.Models.Entities
{
    public class Request
    {
        public CostEstimate CostEstimate { get; set; }
    }

    public class Employee
    {
        public int EmpAge { get; set; }
        public string EmpGender { get; set; }
        public int EmpMedicalPlanExist { get; set; }
        public int EmpDentalPlanExist { get; set; }
        public int EmpVisionPlanExist { get; set; }
        public int EmpOrthoDontic { get; set; }
        public int EmpCorrectivelenses { get; set; }
        public int EmpMedicalUtil { get; set; }
        public int EmpDrugsUtil { get; set; }
        public decimal HouseHoldIncome { get; set; }
    }

    public class Children
    {
        public int NoOfChildren { get; set; }
        public int ChildMedicalPlanExist { get; set; }
        public int ChildDentalPlanExist { get; set; }
        public int ChildVisionPlanExist { get; set; }
        public int ChildOrthoDontic { get; set; }
        public int ChildCorrectivelenses { get; set; }
        public int ChildMedicalUtil { get; set; }
        public int ChildDrugsUtil { get; set; }
    }

    public class Spouse
    {
        public int SpouseAge { get; set; }
        public string SpouseGender { get; set; }
        public int SpouseMedicalPlanExist { get; set; }
        public int SpouseDentalPlanExist { get; set; }
        public int SpouseVisionPlanExist { get; set; }
        public int SpouseOrthoDontic { get; set; }
        public int SpouseCorrectivelenses { get; set; }
        public int SpouseMedicalUtil { get; set; }
        public int SpouseDrugsUtil { get; set; }
    }

    public class PlanIds
    {
        public int MedicalId { get; set; }
        public int DentalId { get; set; }
        public int VisionId { get; set; }
    }

    public class CostEstimate
    {
        public PlanIds PlanIds { get; set; }
        public int Zipcode { get; set; }
        public Employee Employee { get; set; }
        public Spouse Spouse { get; set; }
        public Children Children { get; set; }
        public string DeductionBand { get; set; }
        public string SpouseSurcharge { get; set; }
        public string WellnessPlan { get; set; }
        public string TobaccoUse { get; set; }
    }

}