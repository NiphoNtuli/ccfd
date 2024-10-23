using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFD.Models
{
    // Customer model
    public class Customer
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public DateTime DOB { get; set; }
    }
}