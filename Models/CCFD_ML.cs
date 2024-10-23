using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CCFD.Models
{
    // Compute transaction legitimacy
    public class CCFD_ML
    {
        public string currentUser { get; set; }
        public string currentLocation { get; set; }
        public string currentItem { get; set; }
        private List<string> prevLocations { get; set; }
        private string prevLocation { get; set; }
        private List<string> prevItems { get; set; }
        private List<int> prevTransactionIds { get; set; }

        public CCFD_ML() { }

        // Check location matches with previous successful transaction 
        public bool CurrentToPrevLocationMatch()
        {
            using (var ccfdEntities = new CCFDEntities())
            {
                prevLocation = ccfdEntities.transactions
                    .Where(w => w.CustomerId == currentUser && (w.Status == "APPROVED" || w.Status == "AUTH_APPROVED"))
                    .OrderByDescending(o => o.Id)
                    .Select(s => s.Location)
                    .FirstOrDefault();
            }

            if (prevLocation == currentLocation) {
                return true;
            }

            return false;
        }

        // Check current location if ever visited before
        public bool LocationEverVisited()
        {
            using (var ccfdEntities = new CCFDEntities())
            {
                prevLocations = ccfdEntities.transactions
                    .Where(w => w.CustomerId == currentUser && (w.Status == "APPROVED" || w.Status == "AUTH_APPROVED"))
                    .Select(s => s.Location)
                    .ToList();
            }

            if (prevLocations.Any(currentLocation.Contains)) {
                return true;
            }

            return false;
        }

        // Check items if ever bought before, user spending habit
        public bool ItemEverBought()
        {
            using (var ccfdEntities = new CCFDEntities())
            {
                prevTransactionIds = ccfdEntities.transactions
                    .Where(w => w.CustomerId == currentUser && (w.Status == "APPROVED" || w.Status == "AUTH_APPROVED"))
                    .Select(s => s.TransId)
                    .ToList();

                // Get all previous successful transaction item(s)
                prevItems = ccfdEntities.items
                    .Where(w => prevTransactionIds.Contains(w.Item_TransId))
                    .Select(s => s.Name)
                    .ToList();
            }

            foreach (var item in prevItems)
            {
                if (currentItem == item) {
                    return true;
                }
            }

            return false;
        }
    }
}