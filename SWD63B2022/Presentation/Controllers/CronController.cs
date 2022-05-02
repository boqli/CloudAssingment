using DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class CronController : Controller
    {
        private IFireStoreDataAccess fireStore;
        private readonly ILogger<CronController> _logger;
        public CronController(ILogger<CronController> logger, IFireStoreDataAccess _fireStore)
        {
            _logger = logger;
            fireStore = _fireStore;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Logging this once a day - delete");
            fireStore.deleteBucketFirestoreFiles("boqlicloudprog", User.Claims.ElementAt(4).Value);
            return Ok("Done");
        }
    }
}
