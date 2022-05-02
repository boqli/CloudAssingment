using Common;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class AdminController : Controller
    {
        private ICacheRepository cacheRepo;

        public AdminController(ICacheRepository _cacheRepo)
        {
            cacheRepo = _cacheRepo;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuItem menuItem)
        {
            //validate
            cacheRepo.AddMenu(menuItem);
            return View();
        }

        public IActionResult List()
        {
            return View(cacheRepo.GetMenus());
        }

        
       




    }
}
