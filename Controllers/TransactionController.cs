using CCFD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCFD.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        // GET: Transaction
        public ActionResult Index()
        {
            return View();
        }

        // Return transaction to authorize or no trnasaction for logged in user
        public ActionResult Authorize()
        {
            try
            {
                Transaction transaction = new Transaction(); // Initialize new transaction model

                // Load transaction model with recent transaction to evaluate
                using (var ccfdEntities = new CCFDEntities())
                {
                    transaction = ccfdEntities.transactions
                        .Where(w => w.CustomerId == User.Identity.Name && w.Status == "PENDING")
                        .OrderBy(o => o.Id).FirstOrDefault();
                }

                // Return no transaction if transaction model is null
                if (transaction == null) {
                    return RedirectToAction("Index", "Transaction");
                }

                return View(transaction);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Transaction receiver API
        [HttpPost]
        [AllowAnonymous]
        public string PreAuthorize(TransactionStage transactionStage)
        {
            try
            {
                // Resolve transaction stage to ccfd entities
                using (var ccfdEntities = new CCFDEntities())
                {
                    int _id = int.Parse(transactionStage.Id);
                    string _merchantId = transactionStage.MerchantId;
                    string _location = transactionStage.Location;
                    double _totalAmount = double.Parse(transactionStage.TotalAmount);
                    DateTime _date = DateTime.Parse(transactionStage.Date);
                    string _authToken = transactionStage.AuthToken;

                    Transaction transaction = new Transaction();
                    Customer customer = new Customer();

                    customer.AccountId = transactionStage.Customer.Substring(0, transactionStage.Customer.IndexOf('%'));
                    transactionStage.Customer = transactionStage.Customer.Substring(transactionStage.Customer.IndexOf('%') + 1);

                    customer.FullName = transactionStage.Customer.Substring(0, transactionStage.Customer.IndexOf('%'));
                    transactionStage.Customer = transactionStage.Customer.Substring(transactionStage.Customer.IndexOf('%') + 1);

                    customer.DOB = DateTime.Parse(transactionStage.Customer);

                    int itemsCount = transactionStage.Items.Count(c => c == '%') + 1;

                    Item item = new Item();
                    
                    // Loop through items and add to item model to be persisted against database
                    for (int i = 0; i < itemsCount; i++)
                    {
                        if (i!=itemsCount-1)
                        {
                            item.Name = transactionStage.Items.Substring(0, transactionStage.Items.IndexOf('%'));
                            transactionStage.Items = transactionStage.Items.Substring(transactionStage.Items.IndexOf('%') + 1);
                        }
                        else {
                            item.Name = transactionStage.Items;
                        }
                        
                        item.Item_TransId = _id;

                        ccfdEntities.items.Add(item);

                        item = new Item();
                    }

                    transaction.TransId = _id;
                    transaction.MerchantId = _merchantId;
                    transaction.Location = _location;
                    transaction.TotalAmount = _totalAmount;
                    transaction.Date = _date;
                    transaction.AuthToken = _authToken;
                    transaction.CustomerId = customer.AccountId;
                    transaction.Status = transactionStage.Status;

                    ccfdEntities.transactions.Add(transaction);

                    // Check if customer exist before adding new one
                    if (ccfdEntities.customers.Where(w => w.AccountId == customer.AccountId).Count() == 0) {
                        ccfdEntities.customers.Add(customer);
                    }

                    ccfdEntities.SaveChanges();
                }

                return "okay";
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult Approve()
        {
            try
            {
                // Check if transaction is valid using CCFD ML model
                if (IsValid())
                {
                    Transaction transaction = new Transaction();

                    // Mark transaction status approved if CCFD ML model return true
                    using (var ccfdEntities = new CCFDEntities())
                    {
                        transaction = ccfdEntities.transactions
                            .Where(w => w.CustomerId == User.Identity.Name && w.Status == "PENDING")
                            .OrderBy(o => o.Id).FirstOrDefault();

                        transaction.Status = "APPROVED";

                        ccfdEntities.transactions.Attach(transaction);
                        ccfdEntities.Entry(transaction).Property(p => p.Status).IsModified = true;
                        ccfdEntities.SaveChanges();
                    }

                    return View();
                }

                Session["AuthenticateStatus"] = "YES"; // Fraudulent transaction, set http base session variable authentication status to yes

                return RedirectToAction("Authenticate", "Transaction"); // Redirect to authentication
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Succesfully authenticated, redirected to auth aprrove
        public ActionResult AuthApprove(string uid)
        {
            try
            {
                if (uid == User.Identity.Name) // Check authenticated user id matches
                {
                    Transaction transaction = new Transaction();

                    using (var ccfdEntities = new CCFDEntities())
                    {
                        transaction = ccfdEntities.transactions
                            .Where(w => w.CustomerId == User.Identity.Name && w.Status == "PENDING")
                            .OrderBy(o => o.Id).FirstOrDefault();

                        transaction.Status = "AUTH_APPROVED";

                        ccfdEntities.transactions.Attach(transaction);
                        ccfdEntities.Entry(transaction).Property(p => p.Status).IsModified = true;
                        ccfdEntities.SaveChanges();
                    }

                    return View();
                }

                return RedirectToAction("Failed", "Transaction");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // User decline transaction
        public ActionResult Decline()
        {
            try
            {
                Transaction transaction = new Transaction();

                // Mark transaction status to decline
                using (var ccfdEntities = new CCFDEntities())
                {
                    transaction = ccfdEntities.transactions
                        .Where(w => w.CustomerId == User.Identity.Name && w.Status == "PENDING")
                        .OrderBy(o => o.Id).FirstOrDefault();

                    transaction.Status = "DECLINED";
                    
                    ccfdEntities.transactions.Attach(transaction);
                    ccfdEntities.Entry(transaction).Property(p => p.Status).IsModified = true;
                    ccfdEntities.SaveChanges();
                }

                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //[HttpPost]
        public ActionResult Authenticate()
        {
            try
            {
                // Return authentication view based on authentication status
                if (Session["AuthenticateStatus"] == "YES" || Session["AuthenticateStatus"] == "ENROLL") {
                    return View();
                }

                return RedirectToAction("Failed", "Transaction");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Failed transaction
        public ActionResult Failed()
        {
            try
            {
                Transaction transaction = new Transaction();

                using (var ccfdEntities = new CCFDEntities())
                {
                    transaction = ccfdEntities.transactions
                        .Where(w => w.CustomerId == User.Identity.Name && w.Status == "PENDING")
                        .OrderBy(o => o.Id).FirstOrDefault();

                    transaction.Status = "FAILED";

                    ccfdEntities.transactions.Attach(transaction);
                    ccfdEntities.Entry(transaction).Property(p => p.Status).IsModified = true;
                    ccfdEntities.SaveChanges();
                }

                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Assess transaction validity
        public bool IsValid()
        {
            try
            {
                Transaction transaction = new Transaction();
                List<Item> items = new List<Item>();

                using (var ccfdEntities = new CCFDEntities())
                {
                    // Auto-pass user's firt time transaction
                    if (ccfdEntities.transactions.Where(w => w.CustomerId == User.Identity.Name).Count() == 1)
                    {
                        ccfdEntities.Database.Connection.Close();
                        return true;
                    }

                    transaction = ccfdEntities.transactions
                        .Where(w => w.CustomerId == User.Identity.Name && w.Status == "PENDING")
                        .OrderBy(o => o.Id)
                        .FirstOrDefault();

                    items = ccfdEntities.items
                        .Where(w => w.Item_TransId == transaction.TransId)
                        .ToList();
                }

                CCFD_ML ccfd_ml = new CCFD_ML();
                int validScore = 0;
                int totalScore = 3 + items.Count();

                ccfd_ml.currentUser = User.Identity.Name;
                ccfd_ml.currentLocation = transaction.Location;

                // Check current transaction location match to previous location, score severity +2
                if (ccfd_ml.CurrentToPrevLocationMatch()) {
                    validScore = validScore + 2;
                }

                // Check current transaction location ever visited before, score severity +1
                if (ccfd_ml.LocationEverVisited()) {
                    validScore = validScore + 1;
                }

                // Check each item ever purchased before, score severity +1 per item
                foreach (var item in items)
                {
                    ccfd_ml.currentItem = item.Name;

                    if (ccfd_ml.ItemEverBought()) {
                        validScore = validScore + 1;
                    }
                }

                // If score severity is more than 49%, transaction is valid
                if ((validScore/totalScore) * 100 > 49) {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}