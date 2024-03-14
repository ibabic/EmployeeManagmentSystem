using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class BranchRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Branch>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var branch = await appDbContext.Branchs.FindAsync(id);
            if (branch is null) return NotFound();

            appDbContext.Branchs.Remove(branch);
            await Commit();
            return Success();
        }

        public async Task<List<Branch>> GetAll() => await appDbContext.Branchs.AsNoTracking().Include(d => d.Department).ToListAsync();

        public async Task<Branch> GetById(int id) => await appDbContext.Branchs.FindAsync(id);

        public async Task<GeneralResponse> Insert(Branch item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Branch already added");
            appDbContext.Branchs.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Branch item)
        {
            var branch = await appDbContext.Branchs.FindAsync(item.Id);
            if (branch is null) return NotFound();
            branch.Name = item.Name;
            branch.DepartmentId = item.DepartmentId;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry Branch not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Branchs.FirstOrDefaultAsync(d => d.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
