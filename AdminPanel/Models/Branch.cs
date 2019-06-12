using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required]
        public int BankId { get; set; }
        public string BankName_VW { get; set; }

        public int BranchId { get; set; }

        [Required]
        public string BranchTitle { get; set; }

        [Required]
        public string BranchAddress { get; set; }

        [Required]
        public string ContactNo { get; set; }

        [Required]
        public string BranchManager { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }

        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
