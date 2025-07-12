namespace ClinicalTrial2._0.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITrialRepository Trials { get; }
        IRepository<Models.Location> Locations { get; }
        IRepository<Models.Disease> Diseases { get; }
        IRepository<Models.Treatment> Treatments { get; }
        IRepository<Models.Enrollment> Enrollments { get; }
        IRepository<Models.TrialTreatment> TrialTreatments { get; }
        IRepository<Models.TrialTranslation> TrialTranslations { get; }
        IRepository<Models.ChatMessage> ChatMessages { get; }
        Task<int> SaveChangesAsync();
    }
}
