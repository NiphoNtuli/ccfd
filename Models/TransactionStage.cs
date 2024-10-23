using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFD.Models
{
    // API transaction stage receiver
    public class TransactionStage
    {
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string Items { get; set; }
        public string Location { get; set; }
        public string TotalAmount { get; set; }
        public string Date { get; set; }
        public string Customer { get; set; }
        public string AuthToken { get; set; }
        public string Status { get; set; }
    }
}