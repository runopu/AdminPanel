using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class ReportMapping
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string Bank_VW { get; set; }
        public int BranchId { get; set; }
        public string Branch_VW { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string ContactNo { get; set; }
        [Required]
        public string ReportTitle { get; set; }
        [Required]
        public string ReportUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
