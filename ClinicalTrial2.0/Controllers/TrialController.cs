using ClinicalTrial2._0.Models;
using ClinicalTrial2._0.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrial2._0.Controllers
{
    [Authorize(Roles = "Participant")]
    public class TrialController : Controller
    {
        private readonly ITrialService _trialService;
        private readonly IReferenceDataService _referenceDataService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrialController(
            ITrialService trialService,
            IReferenceDataService referenceDataService,
            UserManager<ApplicationUser> userManager)
        {
            _trialService = trialService;
            _referenceDataService = referenceDataService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, int? diseaseId, int? locationId)
        {
            IEnumerable<Trial> trials;

            if (!string.IsNullOrEmpty(search))
            {
                trials = await _trialService.SearchTrialsAsync(search);
            }
            else if (diseaseId.HasValue)
            {
                trials = await _trialService.GetTrialsByDiseaseAsync(diseaseId.Value);
            }
            else if (locationId.HasValue)
            {
                trials = await _trialService.GetTrialsByLocationAsync(locationId.Value);
            }
            else
            {
                trials = await _trialService.GetActiveTrialsAsync();
            }

            ViewBag.Diseases = await _referenceDataService.GetAllDiseasesAsync();
            ViewBag.Locations = await _referenceDataService.GetAllLocationsAsync();
            ViewBag.Search = search;
            ViewBag.SelectedDiseaseId = diseaseId;
            ViewBag.SelectedLocationId = locationId;

            return View(trials);
        }

        public async Task<IActionResult> Details(int id)
        {
            var trial = await _trialService.GetTrialByIdAsync(id);
            if (trial == null)
            {
                return NotFound();
            }

            return View(trial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _trialService.EnrollParticipantAsync(userId, id);

            if (result)
            {
                TempData["SuccessMessage"] = "Enrollment request submitted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "You are already enrolled in this trial or enrollment failed.";
            }

            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> MyEnrollments()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var enrollments = await _trialService.GetParticipantEnrollmentsAsync(userId);
            return View(enrollments);
        }
    }
}
