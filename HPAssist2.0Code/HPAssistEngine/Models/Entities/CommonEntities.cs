using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPAssistEngine.Models.Entities
{
    public class CommonEntities
    {
        int empAge = 0, spouseAge = 0;
        string empGender = "", spouseGender = "";
        int empMedPlanSelection = 0, spouseMedPlanSelection = 0, childMedPlanSelection = 0;
        int empDentPlanSelection = 0, spouseDentPlanSelection = 0, childDentPlanSelection = 0;
        int empVisPlanSelection = 0, SpouseVisPlanSelection = 0, childVisPlanSelection = 0;
        int childCount = 0;

        public int EEStatus { get; set; }
        public int SpouseStatus { get; set; }
        public int ChildStatus { get; set; }

        public int NewEmpMedStatusCode { get; set; }
        public int NewSpouseMedStatusCode { get; set; }
        public int NewChildMedStatusCode { get; set; }

        public int NewEmpDentalStatusCode { get; set; }
        public int NewSpouseDentalStatusCode { get; set; }
        public int NewChildDentalStatusCode { get; set; }

        public int NewEmpVisionStatusCode { get; set; }
        public int NewSpouseVisionStatusCode { get; set; }
        public int NewChildVisionStatusCode { get; set; }

        public int NewEmpMedUtilizationOVFactorCosts { get; set; }
        public int NewSpouseMedUtilizationOVFactorCosts { get; set; }
        public int NewChildMedUtilizationOVFactorCosts { get; set; }

        public int NewEmpMedUtilizationNotOVFactorCosts { get; set; }
        public int NewSpouseMedUtilizationNotOVFactorCosts { get; set; }
        public int NewChildMedUtilizationNotOVFactorCosts { get; set; }

        public int NewEmpMedUtilizationSPFactorCosts { get; set; }
        public int NewSpouseMedUtilizationSPFactorCosts { get; set; }
        public int NewChildMedUtilizationSPFactorCosts { get; set; }

        public int NewEmpPrescriptionDrugUtilizationCosts { get; set; }
        public int NewSpousePrescriptionDrugUtilizationCosts { get; set; }
        public int NewChildPrescriptionDrugUtilizationCosts { get; set; }

        public int NewEmpMedUtilizationOVFactorUtil { get; set; }
        public int NewSpouseMedUtilizationOVFactorUtil { get; set; }
        public int NewChildMedUtilizationOVFactorUtil { get; set; }

        public int NewEmpMedUtilizationSPFactorUtil { get; set; }
        public int NewSpouseMedUtilizationSPFactorUtil { get; set; }
        public int NewChildMedUtilizationSPFactorUtil { get; set; }

        public int NewEmpPrescriptionDrugUtilizationUtil { get; set; }
        public int NewSpousePrescriptionDrugUtilizationUtil { get; set; }
        public int NewChildPrescriptionDrugUtilizationUtil { get; set; }

        public CommonEntities(CostEstimate Obj)
        {
            this.empAge = Obj.Employee.EmpAge;
            this.spouseAge = Obj.Spouse.SpouseAge;
            this.spouseGender = Obj.Spouse.SpouseGender;
            this.empGender = Convert.ToString(Obj.Employee.EmpGender);
            this.empMedPlanSelection = Obj.Employee.EmpMedicalPlanExist;
            this.spouseMedPlanSelection = Obj.Spouse.SpouseMedicalPlanExist;
            this.childMedPlanSelection = Obj.Children.ChildMedicalPlanExist;
            this.empDentPlanSelection = Obj.Employee.EmpDentalPlanExist;
            this.spouseDentPlanSelection = Obj.Spouse.SpouseDentalPlanExist;
            this.childDentPlanSelection = Obj.Children.ChildDentalPlanExist;
            this.childCount = Obj.Children.NoOfChildren;
            this.empVisPlanSelection = Obj.Employee.EmpVisionPlanExist;
            this.SpouseVisPlanSelection = Obj.Spouse.SpouseVisionPlanExist;
            this.childVisPlanSelection = Obj.Children.ChildVisionPlanExist;

            EEStatus = GetEEStatus();
            SpouseStatus = GetSpouseStatus();
            ChildStatus = GetChildStatus();

            NewEmpMedStatusCode = GetNewMedStatusCodeForEmp();
            NewSpouseMedStatusCode = GetNewMedStatusCodeForSpouse();
            NewChildMedStatusCode = GetNewMedStatusCodeForChild();

            NewEmpDentalStatusCode = GetNewDentStatusCodeForEmp();
            NewSpouseDentalStatusCode = GetNewDentStatusCodeForSpouse();
            NewChildDentalStatusCode = GetNewDentStatusCodeForChild();

            NewEmpVisionStatusCode = GetNewVisStatusCodeForEmp();
            NewSpouseVisionStatusCode = GetNewVisStatusCodeForSpouse();
            NewChildVisionStatusCode = GetNEwVisStatusCodeForChild();
        }
        private int GetEEStatus()
        {
            if (empAge < 1 || string.IsNullOrEmpty(empGender))
                return 0;
            else
                return 1;
        }
        private int GetSpouseStatus()
        {
            if (spouseAge == 0)
                return 0;
            else
                return 1;
        }
        private int GetChildStatus()
        {
            if (childCount < 1)
                return 0;
            else
                return childCount;
        }
        private int GetNewMedStatusCodeForEmp()
        {
            if (EEStatus == 0 || empMedPlanSelection == 0)
                return 0;
            else
                return 1;
        }
        private int GetNewMedStatusCodeForSpouse()
        {
            if (SpouseStatus == 0 || spouseMedPlanSelection == 0)
                return 0;
            else
                return 1;
        }
        private int GetNewMedStatusCodeForChild()
        {
            if (ChildStatus == 0 || childMedPlanSelection == 0)
                return 0;
            else
                return 1;
        }
        private int GetNewDentStatusCodeForEmp()
        {
            if (EEStatus == 0 || empDentPlanSelection == 0)
                return 0;
            else
                return 1;
        }
        private int GetNewDentStatusCodeForSpouse()
        {
            if (EEStatus == 0 || spouseDentPlanSelection == 0)
                return 0;
            else
                return 1;
        }
        private int GetNewDentStatusCodeForChild()
        {
            if (ChildStatus == 0 || childDentPlanSelection == 0)
                return 0;
            else
                return 1;


        }
        private int GetNewVisStatusCodeForEmp()
        {
            if (EEStatus == 0 || empVisPlanSelection == 0)
                return 0;
            else
                return 1;
        }
        private int GetNewVisStatusCodeForSpouse()
        {
            if (SpouseStatus == 0 || SpouseVisPlanSelection == 0)
                return 0;
            else
                return 1;
        }
        private int GetNEwVisStatusCodeForChild()
        {
            if (ChildStatus == 0 || childVisPlanSelection == 0)
                return 0;
            else
                return 1;
        }

    }
}
