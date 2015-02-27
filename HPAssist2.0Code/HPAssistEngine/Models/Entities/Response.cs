#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
#endregion

namespace HPAssistEngine.Models
{
    public class Response
    {
        public List<Error> Errors { get; set; }
       
        public class Error
        {
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class CostEstimateResponse
        {
            public int TransactionID { get; set; }
            public string Status { get; set; }
            public List<Error> Errors { get; set; }
            public EmployeeCost EmployeeCost { get; set; }
            public UtilizationCoverage UtilizationCoverage { get; set; }
        }

        public class EmployeeCost
        {
            public Medical Medical { get; set; }
            public ExchangePlan ExchangePlan { get; set; }
            public Dental Dental { get; set; }
            public Vision Vision { get; set; }
            public decimal RangeLow { get; set; }
            public decimal RangeAverage { get; set; }
            public decimal RangeHigh { get; set; }
            public decimal TotalOOP { get; set; }
            public decimal TotalEmployeeCost { get; set; }
            public decimal ExRangeLow { get; set; }
            public decimal ExRangeAverage { get; set; }
            public decimal ExRangeHigh { get; set; }
            public decimal ExTotalOOP { get; set; }
            public decimal ExTotalEmployeeCost { get; set; }
            public List<Message> Messages { get; set; }
        }
       
        public class Message
        {
            public string MessageText { get; set; }
        }

        public class Medical
        {
            public decimal HospitalOrPhysician { get; set; }
            public decimal PrescriptionDrug { get; set; }
            public decimal Premium { get; set; }
            public decimal HRAContributions { get; set; }
            public decimal RangeLow { get; set; }
            public decimal RangeAverage { get; set; }
            public decimal RangeHigh { get; set; }
            public decimal TotalOOP { get; set; }
            public decimal TotalEmployeeCost { get; set; }
        }
        
        public class ExchangePlan
        {
            public decimal HospitalOrPhysician { get; set; }
            public decimal PrescriptionDrug { get; set; }
            public decimal Premium { get; set; }
            public decimal HRAContributions { get; set; }
            public decimal RangeLow { get; set; }
            public decimal RangeAverage { get; set; }
            public decimal RangeHigh { get; set; }
            public decimal TotalOOP { get; set; }
            public decimal TotalEmployeeCost { get; set; }
        }
        
        public class Dental
        {
            public decimal OOP { get; set; }
            public decimal Premium { get; set; }
        }
        
        public class Vision
        {
            public decimal OOP { get; set; }
            public decimal Premium { get; set; }
        }

        public class UtilizationCoverage
        {
            public EmpCoverage EmpCoverage { get; set; }
            public USAverage USAverage { get; set; }
        }

        public class USAverage
        {
            public USAverage() { PrescriptionDrug = 0; OfficeVisits = 0; SpecialistVisits = 0; DentalFillings = 0; CorrectiveLenses = 0; }
            public decimal PrescriptionDrug { get; set; }
            public decimal OfficeVisits { get; set; }
            public decimal SpecialistVisits { get; set; }
            public decimal DentalFillings { get; set; }
            public decimal CorrectiveLenses { get; set; }
        }

        public class EmpCoverage
        {
            public EmpCoverage() { PrescriptionDrug = 0; OfficeVisits = 0; SpecialistVisits = 0; DentalFillings = 0; CorrectiveLenses = 0; }
            public decimal PrescriptionDrug { get; set; }
            public decimal OfficeVisits { get; set; }
            public decimal SpecialistVisits { get; set; }
            public decimal DentalFillings { get; set; }
            public decimal CorrectiveLenses { get; set; }
        }

        public class CompareCostRequest
        {
            public int TransactionID { get; set; }
            public PlanIds PlanIds { get; set; }
        }

        public class PlanIds
        {
            public int MedicalId { get; set; }
            public int DentalId { get; set; }
            public int VisionId { get; set; }
        }
    }
}


