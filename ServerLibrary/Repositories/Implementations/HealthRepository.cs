using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class HealthRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<MedicalLeave>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.MedicalLeaves.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.MedicalLeaves.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<MedicalLeave>> GetAll() => await appDbContext.MedicalLeaves.AsNoTracking().ToListAsync();

        public async Task<MedicalLeave> GetById(int id) => await appDbContext.MedicalLeaves.FirstOrDefaultAsync(eid => eid.EmployeeId == id);

        public async Task<GeneralResponse> Insert(MedicalLeave item)
        {
            appDbContext.MedicalLeaves.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(MedicalLeave item)
        {
            var obj = await appDbContext.MedicalLeaves
                .FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();
            obj.MedicalRecomendation = item.MedicalRecomendation;
            obj.MedicalDiagnose = item.MedicalDiagnose;
            obj.Date = item.Date;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry data not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}

