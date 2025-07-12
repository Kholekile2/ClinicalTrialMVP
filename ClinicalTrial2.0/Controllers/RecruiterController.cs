using ClinicalTrial2._0.Models;
using ClinicalTrial2._0.Models.ViewModels;
using ClinicalTrial2._0.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrial2._0.Controllers
{
    [Authorize(Roles = "Recruiter")]
    public class RecruiterController : Controller
    {
        private readonly ITrialService _trialService;
        private readonly IReferenceDataService _referenceDataService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecruiterController(
            ITrialService trialService,
            IReferenceDataService referenceDataService,
            UserManager<ApplicationUser> userManager)
        {
            _trialService = trialService;
            _referenceDataService = referenceDataService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var trials = await _trialService.GetTrialsByCreatorAsync(userId);
            return View(trials);
        }

        [HttpGet]
        public async Task<IActionResult> CreateTrial()
        {
            var model = new CreateTrialViewModel
            {
                Locations = await _referenceDataService.GetAllLocationsAsync(),
                Diseases = await _referenceDataService.GetAllDiseasesAsync(),
                Treatments = await _referenceDataService.GetAllTreatmentsAsync(),
                TrialStartDate = DateTime.Today,
                TrialEndDate = DateTime.Today.AddMonths(6)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrial(CreateTrialViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Validate trial dates
                if (model.TrialEndDate <= model.TrialStartDate)
                {
                    ModelState.AddModelError("TrialEndDate", "Trial end date must be after start date.");
                }
                else
                {
                    var result = await _trialService.CreateTrialAsync(model, userId);

                    if (result)
                    {
                        TempData["SuccessMessage"] = "Trial created successfully!";
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("NCTNumber", "A trial with this NCT Number already exists.");
                    }
                }
            }

            // Reload dropdown data
            model.Locations = await _referenceDataService.GetAllLocationsAsync();
            model.Diseases = await _referenceDataService.GetAllDiseasesAsync();
            model.Treatments = await _referenceDataService.GetAllTreatmentsAsync();

            return View(model);
        }

        public async Task<IActionResult> TrialDetails(int id)
        {
            var trial = await _trialService.GetTrialByIdAsync(id);
            if (trial == null)
            {
                return NotFound();
            }

            // Ensure the current user is the creator of this trial
            var userId = _userManager.GetUserId(User);
            if (trial.CreatedByUserId != userId)
            {
                return Forbid();
            }

            return View(trial);
        }

        [HttpGet]
        public async Task<IActionResult> ManageEnrollments(int id)
        {
            if (!User.IsInRole("Recruiter"))
                return RedirectToAction("AccessDenied", "Account");

            var trial = await _trialService.GetTrialByIdAsync(id);
            if (trial == null)
                return NotFound();

            // Verify the recruiter owns this trial
            var user = await _userManager.GetUserAsync(User);
            if (trial.CreatedByUserId != user.Id)
                return Forbid();

            var enrollments = await _trialService.GetTrialEnrollmentsAsync(id);
            
            ViewBag.Trial = trial;
            return View(enrollments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEnrollmentStatus(int enrollmentId, string status, int trialId, string notes = null)
        {
            if (!User.IsInRole("Recruiter"))
                return RedirectToAction("AccessDenied", "Account");

            try
            {
                await _trialService.UpdateEnrollmentStatusAsync(enrollmentId, status, notes);
                TempData["SuccessMessage"] = $"Enrollment status updated to {status} successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update enrollment status: " + ex.Message;
            }

            return RedirectToAction("ManageEnrollments", new { id = trialId });
        }
    }
}
