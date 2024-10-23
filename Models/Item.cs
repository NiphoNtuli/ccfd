using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFD.Models
{
    // Item model
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Item_TransId { get; set; }
    }
}