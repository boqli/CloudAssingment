using Common;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class CreditController : Controller
    {
        private IFireStoreDataAccess firestore;
        private ICacheRepository cacheRepo;

        public CreditController(IFireStoreDataAccess _firestore, ICacheRepository _cache)
        {
            firestore = _firestore;
            cacheRepo = _cache;
        }

        [Authorize]
        public IActionResult Index()
        {

            //return View(cacheRepo.getCreditCost2());
            return View(cacheRepo.getCredits());
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(Credit credit1)
        {
            IEnumerable<Credit> credits = cacheRepo.getCredits();

            foreach (var credit in credits)
            {
               
               credit.credit10 = credit1.credit10;
               credit.credit20 = credit1.credit20;
               credit.credit30 = credit1.credit30;
               cacheRepo.updateCredit(credit);
                
            }

            return RedirectToAction("Index", "Credit");
        }


        [Authorize]
        public IActionResult Register(Credit credit)
        {

            return View();

        }

        //[Route("Buy/{credit:int}")]
        [Authorize]
        public async Task<IActionResult> Buy(int credit)
        {

            var cost = cacheRepo.buyCredit(credit);
            firestore.purchase(credit, cost, User.Claims.ElementAt(4).Value);
            return RedirectToAction("Index", "Users");
            //
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Buy2( IFormCollection form)
        {
            var num = Int32.Parse(form["credits"].ToString());
            firestore.purchase(num, 0, User.Claims.ElementAt(4).Value);
            return RedirectToAction("Index", "Users");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(Credit credit)
        {
            cacheRepo.addCredit(credit);
            return RedirectToAction("Index", "Credit");
        }

    }
}
