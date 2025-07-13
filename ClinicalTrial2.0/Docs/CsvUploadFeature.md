# CSV Upload Feature Documentation

## Overview

The CSV Upload Feature allows Recruiters to bulk import clinical trial data from CSV files into the Clinical Trial Access web application. This feature significantly streamlines the process of adding multiple trials by processing structured CSV data and automatically generating translations for multiple languages.

## How It Works

### 1. **Architecture Components**

#### **Presentation Layer (UI)**
- **Controller**: `RecruiterController.UploadCsv()` methods
- **View**: `Views/Recruiter/UploadCsv.cshtml`
- **View Model**: `CsvUploadViewModel` and `CsvUploadResultViewModel`

#### **Business Logic Layer (BLL)**
- **Service Interface**: `ICsvTrialService`
- **Service Implementation**: `CsvTrialService`
- **Translation Service**: `ITranslationService` and `TranslationService`
- **Utilities**: `ExtractNumber` utility class

#### **Data Access Layer (DAL)**
- **Models**: `Trial`, `Location`, `Disease`, `Treatment`, `TrialTreatment`, `TrialTranslation`
- **DTO**: `TrialUploadDto` for CSV data representation
- **EF Core Context**: `ApplicationDbContext`

### 2. **CSV Processing Pipeline**

#### **Step 1: File Validation**
- File type validation (only .csv files accepted)
- File size limit enforcement (10MB maximum)
- Content-Type verification

#### **Step 2: CSV Parsing**
- Uses **TinyCsvParser** library for robust CSV processing
- Maps CSV columns to `TrialUploadDto` properties using `TrialCsvMapping`
- Handles quoted strings and proper tokenization

#### **Step 3: Data Processing**
For each valid CSV row:

1. **Duplicate Check**: Verifies NCT Number uniqueness
2. **Disease Processing**: Creates or reuses existing diseases
3. **Location Processing**: Parses comma-separated location data and creates or reuses locations
4. **Trial Creation**: Creates main trial entity with extracted data
5. **Treatment Processing**: Processes pipe-separated treatment data and establishes many-to-many relationships
6. **Translation Generation**: Automatically creates translations for Xhosa (xh), Zulu (zu), and Afrikaans (af)

#### **Step 4: Database Persistence**
- Uses EF Core transactions for data integrity
- Maintains proper foreign key relationships
- Handles errors gracefully with rollback capability

### 3. **CSV File Format Specification**

The CSV file must follow this exact column structure:

| Column | Field Name | Description | Example | Required |
|--------|------------|-------------|---------|----------|
| 0 | NCT Number | Unique trial identifier | NCT12345678 | Yes |
| 1 | Trial Name | Name of the clinical trial | "COVID-19 Vaccine Study" | Yes |
| 2 | URL | Trial information URL | https://clinicaltrials.gov/... | No |
| 3 | Status | Current trial status | "Recruiting" | No |
| 4 | Description | Detailed trial description | "Study to evaluate safety..." | No |
| 5 | Condition | Medical condition/disease | "COVID-19" | Yes |
| 6 | Treatment | Treatments (pipe-separated) | "Drug:Vaccine\|Therapy:Placebo" | No |
| 7 | Sex | Target gender | "All", "Male", "Female" | No |
| 8 | Age Range | Target age range | "18-65 Years" | No |
| 9 | Trial Phase | Phase information | "Phase 2" | No |
| 10 | Start Date | Trial start date | 2024-01-01 | Yes |
| 11 | End Date | Trial end date | 2024-12-31 | Yes |
| 12 | Location | Location (comma-separated) | "123 Main St,Cape Town,Western Cape,8001,South Africa" | Yes |

#### **Special Format Rules:**

- **Treatment Format**: `Type1:Name1|Type2:Name2`
  - Use pipe (|) to separate multiple treatments
  - Use colon (:) to separate treatment type and name
  - Example: `Drug:Metformin|Therapy:Lifestyle Intervention`

- **Location Format**: `Street,City,Province,PostalCode,Country`
  - All components separated by commas
  - Example: `123 Main Street,Cape Town,Western Cape,8001,South Africa`

- **Date Format**: YYYY-MM-DD or MM/DD/YYYY
  - System automatically parses common date formats

## Services and Models Used

### **Core Services**

#### **CsvTrialService**
```csharp
public interface ICsvTrialService
{
    Task<CsvUploadResult> UploadTrialsFromCsvAsync(Stream csvStream, string userId);
    Task<List<TrialUploadDto>> ParseCsvAsync(Stream csvStream);
}
```

**Responsibilities:**
- CSV file parsing and validation
- Data transformation and mapping
- Database transaction management
- Error handling and reporting

#### **TranslationService**
```csharp
public interface ITranslationService
{
    Task<string?> TranslateTextAsync(string inputText, string targetLanguage);
}
```

**Responsibilities:**
- Integration with Azure Cognitive Services Translator
- Multi-language support (Xhosa, Zulu, Afrikaans)
- Error handling for translation failures

### **Data Models**

#### **Primary Models**
- `Trial`: Main clinical trial entity
- `Location`: Geographic location data
- `Disease`: Medical conditions
- `Treatment`: Therapeutic interventions
- `TrialTreatment`: Many-to-many relationship between trials and treatments
- `TrialTranslation`: Multi-language translations

#### **Supporting Models**
- `TrialUploadDto`: Data transfer object for CSV parsing
- `CsvUploadResult`: Result container for upload operations
- `CsvUploadViewModel`: View model for UI interaction

### **Utilities**

#### **ExtractNumber**
```csharp
public static class ExtractNumber
{
    public static int GetNumber(string text);
}
```
- Extracts numeric values from text (e.g., "Phase 2" → 2)
- Used for parsing trial phase information

#### **TrialCsvMapping**
```csharp
public class TrialCsvMapping : CsvMapping<TrialUploadDto>
```
- Maps CSV columns to DTO properties
- Handles column ordering and data type conversion

## Error Handling

### **Validation Levels**

1. **File Level Validation**
   - File type verification
   - File size limits
   - Basic structure validation

2. **Data Level Validation**
   - Required field validation
   - Data type verification
   - Business rule enforcement

3. **Database Level Validation**
   - Duplicate prevention
   - Referential integrity
   - Transaction rollback on errors

### **Error Reporting**

- Detailed error messages for each failed trial
- Summary statistics (processed vs. imported)
- User-friendly error display in UI
- Graceful degradation for translation failures

## Usage Instructions

### **For Recruiters**

1. **Access the Upload Page**
   - Navigate to Recruiter Dashboard
   - Click "Upload CSV" button
   - Review format instructions

2. **Prepare CSV File**
   - Ensure proper column structure
   - Validate required fields
   - Check file size (max 10MB)

3. **Upload Process**
   - Select CSV file
   - Review validation messages
   - Monitor upload progress
   - Review results summary

4. **Review Results**
   - Check import statistics
   - Review any error messages
   - Verify imported trials in dashboard

### **For Developers**

#### **Configuration**
Add Azure Translator settings to `appsettings.json`:
```json
{
  "AzureTranslator": {
    "SubscriptionKey": "your-key-here",
    "Endpoint": "https://api.cognitive.microsofttranslator.com/"
  }
}
```

#### **Dependency Injection**
Services are registered in `Program.cs`:
```csharp
builder.Services.AddHttpClient<ITranslationService, TranslationService>();
builder.Services.AddScoped<ICsvTrialService, CsvTrialService>();
```

#### **Required Packages**
- `TinyCsvParser`: CSV parsing functionality
- `System.Text.Json`: JSON serialization for API calls

## Extension Points

### **Adding New Languages**
1. Update `_supportedLanguages` array in `CsvTrialService`
2. Ensure Azure Translator supports the language code
3. Update UI documentation if needed

### **Custom CSV Formats**
1. Create new mapping class inheriting from `CsvMapping<T>`
2. Implement custom DTO for different field structure
3. Register alternative service implementations

### **Additional Validation Rules**
1. Extend `CsvTrialService.ProcessTrialAsync()` method
2. Add business rule validation
3. Update error reporting mechanisms

### **Alternative Translation Providers**
1. Implement `ITranslationService` interface
2. Register alternative implementation in DI container
3. Update configuration as needed

## Testing Recommendations

### **Unit Tests**
- Test CSV parsing with various file formats
- Validate data transformation logic
- Mock translation service for isolated testing

### **Integration Tests**
- Test complete upload workflow
- Verify database transactions
- Test error handling scenarios

### **Sample CSV Data**
Create test files with:
- Valid data scenarios
- Invalid data scenarios
- Edge cases (empty fields, special characters)
- Large datasets for performance testing

## Performance Considerations

### **Optimization Strategies**
- Use EF Core batch operations for large datasets
- Implement async/await patterns throughout
- Consider pagination for very large CSV files
- Cache translation results for repeated content

### **Monitoring**
- Track upload success/failure rates
- Monitor translation API usage
- Log performance metrics for large files
- Alert on unusual error patterns

## Security Considerations

### **Input Validation**
- File type verification
- Content scanning for malicious data
- Size limits to prevent DoS attacks
- SQL injection prevention through parameterized queries

### **Authorization**
- Role-based access control (Recruiter only)
- User ownership verification for created trials
- Audit logging for data imports

### **Data Protection**
- Secure transmission of translation requests
- Proper handling of sensitive medical data
- Compliance with healthcare data regulations

---

*This documentation is current as of July 13, 2025. Update as the feature evolves.*
