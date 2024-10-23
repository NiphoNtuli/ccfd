using CCFD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCFD.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        // GET: Report
        // Load report selection view
        public ActionResult Index()
        {
            try
            {
                ReportViewModel reportViewModel = new ReportViewModel();

                reportViewModel.FullReport = false;
                reportViewModel.FraudReport = false;
                reportViewModel.CleanReport = false;

                return View(reportViewModel);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Generate selected report
        [HttpPost]
        public ActionResult Generate(ReportViewModel reportViewModel)
        {
            try
            {
                List<Transaction> transaction = new List<Transaction>();

                if (reportViewModel.FullReport&&!reportViewModel.FraudReport&&!reportViewModel.CleanReport) // Verify one (Full) report selected
                {
                    ViewBag.Message = "Full Transaction Data Report.";

                    using (var ccfdEntities = new CCFDEntities()) {
                        transaction = ccfdEntities.transactions.ToList();
                    }
                }
                else if (reportViewModel.FraudReport&&!reportViewModel.FullReport&&!reportViewModel.CleanReport) // Verify one (Fraud) report selected
                {
                    ViewBag.Message = "Fraud Transaction Data Report.";

                    using (var ccfdEntities = new CCFDEntities())
                    {
                        transaction = ccfdEntities.transactions
                            .Where(w => w.Status == "FAILED")
                            .ToList();
                    }
                }
                else if (reportViewModel.CleanReport&&!reportViewModel.FullReport&&!reportViewModel.FraudReport) // Verify one (Clean) report selected
                {
                    ViewBag.Message = "Passed Transaction Data Report.";

                    using (var ccfdEntities = new CCFDEntities())
                    {
                        transaction = ccfdEntities.transactions
                            .Where(w => w.Status == "APPROVED" || w.Status == "AUTH_APPROVED")
                            .ToList();
                    }
                }
                else { // Return to report select if non or multiple reports were selected
                    return RedirectToAction("Index", "Report");
                }

                return View(transaction);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}