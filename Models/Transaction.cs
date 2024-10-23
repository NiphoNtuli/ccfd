using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFD.Models
{
    // Transaction model
    public class Transaction
    {
        public int Id { get; set; }
        public int TransId { get; set; }
        public string MerchantId { get; set; }
        public string Location { get; set; }
        public double TotalAmount { get; set; }
        public DateTime Date { get; set; }
        public string CustomerId { get; set; }
        public string AuthToken { get; set; }
        public string Status { get; set; }
    }
}