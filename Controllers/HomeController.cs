using ImageClassificationProject.Data;
using ImageClassificationProject.Models;
using ImageClassificationProject.Models.ViewModels;
using ImageClassificationProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageClassificationProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IClassificationService _classificationService; 

        // IStorageService ve IClassificationService'i Dependency Injection ile alıyoruz
        public HomeController(ApplicationDbContext context, 
                              IStorageService storageService,
                              IClassificationService classificationService)
        {
            _context = context;
            _storageService = storageService;
            _classificationService = classificationService;
        }

        // --- Main Page (Upload Form and Classification) ---
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ClassificationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ClassificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                string blobUri = string.Empty;
                string uploadedFileName = model.ImageFile.FileName;
                string classificationResult = string.Empty;
                double confidenceScore = 0.0;

                try
                {
                    // 1. UPLOAD TO AZURE BLOB STORAGE
                    // Ensures the file is stored in Azure before classification.
                    blobUri = await _storageService.UploadFileAsync(model.ImageFile);
                    
                    // 2. CLASSIFY IMAGE USING AZURE COMPUTER VISION
                    // Calls the external Azure service using the Blob URI.
                    (classificationResult, confidenceScore) = await _classificationService.ClassifyImageAsync(blobUri);

                    // 3. SAVE RECORD TO DATABASE
                    var historyEntry = new ClassificationHistory
                    {
                        FileName = uploadedFileName,
                        BlobUri = blobUri, 
                        ClassificationResult = classificationResult,
                        ConfidenceScore = confidenceScore,
                        Timestamp = DateTime.Now
                    };
                    
                    await _context.ClassificationHistories.AddAsync(historyEntry);
                    await _context.SaveChangesAsync();

                    // Update the ViewModel with the actual result
                    model.ResultMessage = $"Image successfully uploaded and classified! Result: {classificationResult} (Confidence: {confidenceScore:P0})."; 
                }
                catch (Exception ex)
                {
                    // Comprehensive error handling
                    model.ResultMessage = $"Error occurred during the process: {ex.Message}"; 
                    ModelState.AddModelError(string.Empty, model.ResultMessage);
                    Console.WriteLine($"Full Process Error: {ex.ToString()}"); // Console output remains Turkish/for debugging
                }

                return View(model); 
            }
            
            // If the model is not valid
            model.ResultMessage = "Please ensure you have selected a valid image file."; 
            return View(model);
        }

        // --- History Page ---
        [HttpGet]
        public async Task<IActionResult> History()
        {
            try
            {
                // Retrieve all history records from the database, ordered by newest first
                var history = await _context.ClassificationHistories
                                            .OrderByDescending(h => h.Timestamp)
                                            .ToListAsync();
                
                return View(history);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while loading history records: " + ex.Message; 
                return View(new List<ClassificationHistory>());
            }
        }
    }
}