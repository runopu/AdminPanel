using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class Bank
    {

        public int Id { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string ContactNo { get; set; }


    }
}
