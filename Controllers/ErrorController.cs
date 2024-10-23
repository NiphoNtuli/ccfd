using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CCFD.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        // Handle view not found error 404
        public ViewResult ViewNotFoundPage()
        {
            Response.BufferOutput = true;
            Response.StatusCode = 404;
            Response.Headers.Remove("Server");
            return View();
        }

        // Handle internal server error 500
        public ViewResult ViewInternalServerErrorPage()
        {
            Response.BufferOutput = true;
            Response.StatusCode = 500;
            Response.Headers.Remove("Server");
            return View();
        }

        // Handle permission error 403
        public ViewResult ViewPermissionErrorPage()
        {
            Response.BufferOutput = true;
            Response.StatusCode = 403;
            Response.Headers.Remove("Server");
            return View();
        }
    }
}