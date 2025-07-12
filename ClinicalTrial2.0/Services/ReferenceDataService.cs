using ClinicalTrial2._0.Data.Repositories;
using ClinicalTrial2._0.Models;

namespace ClinicalTrial2._0.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReferenceDataService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            return await _unitOfWork.Locations.GetAllAsync();
        }

        public async Task<IEnumerable<Disease>> GetAllDiseasesAsync()
        {
            return await _unitOfWork.Diseases.GetAllAsync();
        }

        public async Task<IEnumerable<Treatment>> GetAllTreatmentsAsync()
        {
            return await _unitOfWork.Treatments.GetAllAsync();
        }

        public async Task<Location?> GetLocationByIdAsync(int id)
        {
            return await _unitOfWork.Locations.GetByIdAsync(id);
        }

        public async Task<Disease?> GetDiseaseByIdAsync(int id)
        {
            return await _unitOfWork.Diseases.GetByIdAsync(id);
        }

        public async Task<Treatment?> GetTreatmentByIdAsync(int id)
        {
            return await _unitOfWork.Treatments.GetByIdAsync(id);
        }
    }
}
