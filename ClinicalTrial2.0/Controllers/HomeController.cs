using System.Diagnostics;
using ClinicalTrial2._0.Models;
using ClinicalTrial2._0.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrial2._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITrialService _trialService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ITrialService trialService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _trialService = trialService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // If user is authenticated, redirect based on role
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Recruiter"))
                    {
                        return RedirectToAction("Dashboard", "Recruiter");
                    }
                    else if (roles.Contains("Participant"))
                    {
                        return RedirectToAction("Index", "Trial");
                    }
                }
            }

            // Show featured trials for non-authenticated users
            var activeTrials = await _trialService.GetActiveTrialsAsync();
            ViewBag.FeaturedTrials = activeTrials.Take(6);

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
    }
}
