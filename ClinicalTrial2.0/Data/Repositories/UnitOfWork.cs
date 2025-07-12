using ClinicalTrial2._0.Models;

namespace ClinicalTrial2._0.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITrialRepository? _trials;
        private IRepository<Location>? _locations;
        private IRepository<Disease>? _diseases;
        private IRepository<Treatment>? _treatments;
        private IRepository<Enrollment>? _enrollments;
        private IRepository<TrialTreatment>? _trialTreatments;
        private IRepository<TrialTranslation>? _trialTranslations;
        private IRepository<ChatMessage>? _chatMessages;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITrialRepository Trials => _trials ??= new TrialRepository(_context);
        public IRepository<Location> Locations => _locations ??= new Repository<Location>(_context);
        public IRepository<Disease> Diseases => _diseases ??= new Repository<Disease>(_context);
        public IRepository<Treatment> Treatments => _treatments ??= new Repository<Treatment>(_context);
        public IRepository<Enrollment> Enrollments => _enrollments ??= new Repository<Enrollment>(_context);
        public IRepository<TrialTreatment> TrialTreatments => _trialTreatments ??= new Repository<TrialTreatment>(_context);
        public IRepository<TrialTranslation> TrialTranslations => _trialTranslations ??= new Repository<TrialTranslation>(_context);
        public IRepository<ChatMessage> ChatMessages => _chatMessages ??= new Repository<ChatMessage>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
