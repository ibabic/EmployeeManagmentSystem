using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class BranchRepository(AppDbContext appDbContext, ILogger<BranchRepository> logger) : IGenericRepositoryInterface<Branch>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var methodName = nameof(DeleteById);
            logger.LogInformation($"[{methodName}] Attempting to delete branch with ID: {id}");

            var branch = await appDbContext.Branchs.FindAsync(id);
            if (branch is null)
            {
                logger.LogError($"[{methodName}] Branch with ID {id} not found");
                return NotFound();
            }

            appDbContext.Branchs.Remove(branch);
            await Commit();

            logger.LogInformation($"[{methodName}] Branch with ID {id} successfully deleted");
            return Success();
        }

        public async Task<List<Branch>> GetAll()
        {
            var methodName = nameof(GetAll);
            logger.LogInformation($"[{methodName}] Attempting to retrieve all branches");

            var branches = await appDbContext.Branchs.AsNoTracking().Include(d => d.Department).ToListAsync();

            logger.LogInformation($"[{methodName}] Successfully retrieved all branches");

            return branches;
        }

        public async Task<Branch> GetById(int id)
        {
            var methodName = nameof(GetById);
            logger.LogInformation($"[{methodName}] Attempting to retrieve branch with ID: {id}");

            var branch = await appDbContext.Branchs.FindAsync(id);

            if (branch == null)
            {
                logger.LogError($"[{methodName}] Branch with ID {id} not found");
            }
            else
            {
                logger.LogInformation($"[{methodName}] Successfully retrieved branch with ID: {id}");
            }

            return branch;
        }

        public async Task<GeneralResponse> Insert(Branch item)
        {
            var methodName = nameof(Insert);
            logger.LogInformation($"[{methodName}] Attempting to insert branch: {item.Name}");

            if (!await CheckName(item.Name!))
            {
                logger.LogError($"[{methodName}] Branch '{item.Name}' already exists");
                return new GeneralResponse(false, "Branch already added");
            }

            appDbContext.Branchs.Add(item);
            await Commit();

            logger.LogInformation($"[{methodName}] Branch '{item.Name}' successfully inserted");
            return Success();
        }

        public async Task<GeneralResponse> Update(Branch item)
        {
            var methodName = nameof(Update);
            logger.LogInformation($"[{methodName}] Attempting to update branch with ID: {item.Id}");

            var branch = await appDbContext.Branchs.FindAsync(item.Id);
            if (branch is null)
            {
                logger.LogError($"[{methodName}] Branch with ID {item.Id} not found");
                return NotFound();
            }

            branch.Name = item.Name;
            branch.DepartmentId = item.DepartmentId;
            await Commit();

            logger.LogInformation($"[{methodName}] Branch with ID {item.Id} successfully updated");
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
