using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CCFD.Models
{
    // Report model
    public class ReportViewModel
    {
        [Display(Name = "Full Report")]
        public bool FullReport { get; set; }
        [Display(Name = "Fraud Report")]
        public bool FraudReport { get; set; }
        [Display(Name = "Clean Report")]
        public bool CleanReport { get; set; }
    }
}