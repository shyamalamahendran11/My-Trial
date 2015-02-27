using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HPAssistEngine.Models.Entities
{
    public class PlanDesignVision
    {
        public int  VisionID { get; set; }
        public int  ModelID { get; set; }
        public string PlanName { get; set; }
        public decimal  ExamCoPay { get; set; }
        public decimal  HardwareCoPay { get; set; }
    }
}