using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Diagnostics.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExceptionLogger _googleExceptionLogger;
        public HomeController(ILogger<HomeController> logger, [FromServices] IExceptionLogger exceptionLogger )
        {
            _logger = logger;
            _googleExceptionLogger = exceptionLogger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Accessed the Index method");

            try
            {
                throw new Exception("Demonstrating error reporting");
            }
            catch (Exception ex)
            {
                _googleExceptionLogger.Log(ex);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult LogIn()
        {
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
