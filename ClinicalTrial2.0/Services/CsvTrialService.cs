using ClinicalTrial2._0.Data;
using ClinicalTrial2._0.Models;
using ClinicalTrial2._0.Models.DTOs;
using ClinicalTrial2._0.Services.CsvMapping;
using ClinicalTrial2._0.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace ClinicalTrial2._0.Services
{
    /// <summary>
    /// Interface for CSV trial upload services
    /// </summary>
    public interface ICsvTrialService
    {
        Task<CsvUploadResult> UploadTrialsFromCsvAsync(Stream csvStream, string userId);
        Task<List<TrialUploadDto>> ParseCsvAsync(Stream csvStream);
    }

    /// <summary>
    /// Result of CSV upload operation
    /// </summary>
    public class CsvUploadResult
    {
        public bool Success { get; set; }
        public int TrialsProcessed { get; set; }
        public int TrialsImported { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Service for handling CSV trial data upload and processing
    /// Implements the business logic for importing trial data from CSV files
    /// </summary>
    public class CsvTrialService : ICsvTrialService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITranslationService _translationService;
        private readonly string[] _supportedLanguages = { "xh", "zu", "af" }; // Xhosa, Zulu, Afrikaans

        public CsvTrialService(ApplicationDbContext context, ITranslationService translationService)
        {
            _context = context;
            _translationService = translationService;
        }

        /// <summary>
        /// Main method to upload trials from CSV stream
        /// </summary>
        public async Task<CsvUploadResult> UploadTrialsFromCsvAsync(Stream csvStream, string userId)
        {
            var result = new CsvUploadResult();
            
            try
            {
                var trialData = await ParseCsvAsync(csvStream);
                result.TrialsProcessed = trialData.Count;

                foreach (var trialDto in trialData)
                {
                    try
                    {
                        await ProcessTrialAsync(trialDto, userId);
                        result.TrialsImported++;
                    }
                    catch (InvalidOperationException ex)
                    {
                        result.Errors.Add($"Trial {trialDto.NCTNumber}: {ex.Message}");
                    }
                    catch (ArgumentException ex)
                    {
                        result.Errors.Add($"Trial {trialDto.NCTNumber}: Invalid data format - {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Trial {trialDto.NCTNumber}: Unexpected error - {ex.Message}");
                    }
                }

                result.Success = result.Errors.Count == 0 || result.TrialsImported > 0;
                result.Message = result.Success 
                    ? $"Successfully imported {result.TrialsImported} out of {result.TrialsProcessed} trials."
                    : "Failed to import trials. Please check the errors.";

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"CSV parsing error: {ex.Message}");
                result.Message = "Failed to parse CSV file.";
            }

            return result;
        }

        /// <summary>
        /// Parses CSV stream into TrialUploadDto objects
        /// </summary>
        public async Task<List<TrialUploadDto>> ParseCsvAsync(Stream csvStream)
        {
            var trials = new List<TrialUploadDto>();

            var csvParserOptions = new CsvParserOptions(true, new QuotedStringTokenizer(','));
            
            // Try the clinical trials format first (actual format from clinicaltrials.gov)
            ICsvMapping<TrialUploadDto> csvMapper = new ClinicalTrialsCsvMapping();
            var csvParser = new CsvParser<TrialUploadDto>(csvParserOptions, csvMapper);

            var result = csvParser.ReadFromStream(csvStream, System.Text.Encoding.UTF8);

            foreach (var record in result)
            {
                if (record.IsValid)
                {
                    trials.Add(record.Result);
                }
            }

            // If no valid records found, try the original mapping format
            if (trials.Count == 0)
            {
                csvStream.Position = 0; // Reset stream position
                csvMapper = new TrialCsvMapping();
                csvParser = new CsvParser<TrialUploadDto>(csvParserOptions, csvMapper);
                result = csvParser.ReadFromStream(csvStream, System.Text.Encoding.UTF8);

                foreach (var record in result)
                {
                    if (record.IsValid)
                    {
                        trials.Add(record.Result);
                    }
                }
            }

            return await Task.FromResult(trials);
        }

        /// <summary>
        /// Processes a single trial from CSV data
        /// </summary>
        private async Task ProcessTrialAsync(TrialUploadDto trialDto, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if trial already exists
                var existingTrial = await _context.Trials
                    .FirstOrDefaultAsync(t => t.NCTNumber == trialDto.NCTNumber);

                if (existingTrial != null)
                {
                    throw new InvalidOperationException($"Trial with NCT Number {trialDto.NCTNumber} already exists");
                }

                // Process Disease
                var disease = await GetOrCreateDiseaseAsync(trialDto.Condition);

                // Process Location
                var location = await GetOrCreateLocationAsync(trialDto.Location);

                // Create Trial with data validation and truncation
                var trial = new Trial
                {
                    NCTNumber = TruncateString(trialDto.NCTNumber, 50),
                    TrialName = TruncateString(trialDto.TrialName, 200),
                    Description = TruncateString(trialDto.Description, 2000),
                    URL = trialDto.URL,
                    TrialPhase = ExtractNumber.GetNumber(trialDto.TrialPhase),
                    Sex = TruncateString(trialDto.Sex, 20),
                    AgeRange = TruncateString(trialDto.AgeRange, 50),
                    TrialStartDate = trialDto.TrialStartDate,
                    TrialEndDate = trialDto.TrialEndDate,
                    CreatedByUserId = userId,
                    DiseaseId = disease.DiseaseId,
                    LocationId = location.LocationId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Trials.Add(trial);
                await _context.SaveChangesAsync();

                // Process Treatments
                await ProcessTrialTreatmentsAsync(trial.TrialId, trialDto.Treatment);

                // Generate Translations
                await GenerateTranslationsAsync(trial);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Truncates string to specified maximum length
        /// </summary>
        private static string TruncateString(string? input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            
            return input.Length <= maxLength ? input : input.Substring(0, maxLength);
        }

        /// <summary>
        /// Gets existing disease or creates a new one
        /// </summary>
        private async Task<Disease> GetOrCreateDiseaseAsync(string condition)
        {
            var disease = await _context.Diseases
                .FirstOrDefaultAsync(d => d.DiseaseName == condition);

            if (disease == null)
            {
                disease = new Disease
                {
                    DiseaseName = condition,
                    Description = condition,
                    Category = "Clinical Trial"
                };

                _context.Diseases.Add(disease);
                await _context.SaveChangesAsync();
            }

            return disease;
        }

        /// <summary>
        /// Gets existing location or creates a new one
        /// Location format: "Street/Organization,City,Province,PostalCode,Country" or variations
        /// </summary>
        private async Task<Location> GetOrCreateLocationAsync(string locationString)
        {
            var locationParts = locationString.Split(',');
            
            if (locationParts.Length < 2)
            {
                throw new ArgumentException("Invalid location format. Expected at least: City,Province or Organization,City,Province,PostalCode,Country");
            }

            // Handle different location formats
            string streetAddress = "";
            string city = "";
            string province = "";
            string postalCode = "";
            string country = "South Africa"; // Default country

            if (locationParts.Length >= 5)
            {
                // Full format: "Street/Organization,City,Province,PostalCode,Country"
                streetAddress = locationParts[0].Trim();
                city = locationParts[1].Trim();
                province = locationParts[2].Trim();
                postalCode = locationParts[3].Trim();
                country = locationParts[4].Trim();
            }
            else if (locationParts.Length >= 4)
            {
                // Format: "Organization,City,Province,PostalCode"
                streetAddress = locationParts[0].Trim();
                city = locationParts[1].Trim();
                province = locationParts[2].Trim();
                postalCode = locationParts[3].Trim();
            }
            else if (locationParts.Length >= 3)
            {
                // Format: "Organization,City,Province"
                streetAddress = locationParts[0].Trim();
                city = locationParts[1].Trim();
                province = locationParts[2].Trim();
            }
            else
            {
                // Minimal format: "City,Province"
                city = locationParts[0].Trim();
                province = locationParts[1].Trim();
            }

            // Truncate fields to fit database constraints
            streetAddress = TruncateString(streetAddress, 200);
            city = TruncateString(city, 100);
            province = TruncateString(province, 100);
            postalCode = TruncateString(postalCode, 10);
            country = TruncateString(country, 100);

            // Check for existing location
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => 
                    l.Address == streetAddress &&
                    l.City == city &&
                    l.Province == province &&
                    l.PostalCode == postalCode &&
                    l.Country == country);

            if (location == null)
            {
                location = new Location
                {
                    Address = streetAddress,
                    City = city,
                    Province = province,
                    PostalCode = postalCode,
                    Country = country
                };

                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
            }

            return location;
        }

        /// <summary>
        /// Processes trial treatments from pipe-separated string
        /// Treatment format: "Type1:Name1|Type2:Name2" or "BIOLOGICAL: Name1|BIOLOGICAL: Name2"
        /// </summary>
        private async Task ProcessTrialTreatmentsAsync(int trialId, string treatmentString)
        {
            if (string.IsNullOrEmpty(treatmentString))
                return;

            var treatments = treatmentString.Split('|');

            foreach (var treatmentItem in treatments)
            {
                var treatmentParts = treatmentItem.Split(':');
                if (treatmentParts.Length < 2)
                    continue;

                var treatmentType = treatmentParts[0].Trim();
                var treatmentName = string.Join(":", treatmentParts.Skip(1)).Trim(); // Handle names with colons

                // Truncate to fit database constraints
                treatmentType = TruncateString(treatmentType, 50);
                treatmentName = TruncateString(treatmentName, 100);

                // Get or create treatment
                var treatment = await _context.Treatments
                    .FirstOrDefaultAsync(t => t.TreatmentName == treatmentName && t.TreatmentType == treatmentType);

                if (treatment == null)
                {
                    treatment = new Treatment
                    {
                        TreatmentName = treatmentName,
                        TreatmentType = treatmentType,
                        Description = TruncateString($"{treatmentType}: {treatmentName}", 500)
                    };

                    _context.Treatments.Add(treatment);
                    await _context.SaveChangesAsync();
                }

                // Check if relationship already exists
                var existingRelation = await _context.TrialTreatments
                    .FirstOrDefaultAsync(tt => tt.TrialId == trialId && tt.TreatmentId == treatment.TreatmentId);

                if (existingRelation == null)
                {
                    // Create trial-treatment relationship
                    var trialTreatment = new TrialTreatment
                    {
                        TrialId = trialId,
                        TreatmentId = treatment.TreatmentId
                    };

                    _context.TrialTreatments.Add(trialTreatment);
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Generates translations for trial name and description
        /// </summary>
        private async Task GenerateTranslationsAsync(Trial trial)
        {
            foreach (var languageCode in _supportedLanguages)
            {
                try
                {
                    var translatedName = await _translationService.TranslateTextAsync(trial.TrialName, languageCode);
                    var translatedDescription = !string.IsNullOrEmpty(trial.Description) 
                        ? await _translationService.TranslateTextAsync(trial.Description, languageCode)
                        : null;

                    if (!string.IsNullOrEmpty(translatedName))
                    {
                        var translation = new TrialTranslation
                        {
                            TrialId = trial.TrialId,
                            LanguageCode = languageCode,
                            TranslatedTrialName = translatedName,
                            TranslatedDescription = translatedDescription,
                            CreatedDate = DateTime.UtcNow
                        };

                        _context.TrialTranslations.Add(translation);
                    }
                }
                catch (Exception ex)
                {
                    // Log translation error but don't fail the entire process
                    Console.WriteLine($"Translation error for language {languageCode}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
