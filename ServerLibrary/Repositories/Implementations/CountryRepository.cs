using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class CountryRepository(AppDbContext appDbContext, ILogger<CountryRepository> logger) : IGenericRepositoryInterface<Country>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var methodName = nameof(DeleteById);
            logger.LogInformation($"[{methodName}] Attempting to delete country with ID: {id}");

            var country = await appDbContext.Countries.FindAsync(id);
            if (country is null)
            {
                logger.LogError($"[{methodName}] Country with ID {id} not found");
                return NotFound();
            }

            appDbContext.Countries.Remove(country);
            await Commit();

            logger.LogInformation($"[{methodName}] Country with ID {id} successfully deleted");
            return Success();
        }

        public async Task<List<Country>> GetAll()
        {
            var methodName = nameof(GetAll);
            logger.LogInformation($"[{methodName}] Attempting to retrieve all countries");

            var countries = await appDbContext.Countries.ToListAsync();

            if (countries == null || countries.Count == 0)
            {
                logger.LogWarning($"[{methodName}] No countries found");
            }
            else
            {
                logger.LogInformation($"[{methodName}] Successfully retrieved all countries");
            }

            return countries;
        }

        public async Task<Country> GetById(int id)
        {
            var methodName = nameof(GetById);
            logger.LogInformation($"[{methodName}] Attempting to retrieve country with ID: {id}");

            var country = await appDbContext.Countries.FindAsync(id);

            if (country == null)
            {
                logger.LogWarning($"[{methodName}] Country with ID {id} not found");
            }
            else
            {
                logger.LogInformation($"[{methodName}] Successfully retrieved country with ID: {id}");
            }

            return country;
        }

        public async Task<GeneralResponse> Insert(Country item)
        {
            var methodName = nameof(Insert);
            logger.LogInformation($"[{methodName}] Attempting to insert country with name: {item.Name}");

            if (!await CheckName(item.Name!))
            {
                logger.LogError($"[{methodName}] Country with name '{item.Name}' already added");
                return new GeneralResponse(false, "Country already added");
            }

            appDbContext.Countries.Add(item);
            await Commit();

            logger.LogInformation($"[{methodName}] Country with name '{item.Name}' successfully inserted");
            return Success();
        }

        public async Task<GeneralResponse> Update(Country item)
        {
            var methodName = nameof(Update);
            logger.LogInformation($"[{methodName}] Attempting to update country with ID: {item.Id}");

            var country = await appDbContext.Countries.FindAsync(item.Id);
            if (country is null)
            {
                logger.LogError($"[{methodName}] Country with ID {item.Id} not found");
                return NotFound();
            }

            country.Name = item.Name;
            await Commit();

            logger.LogInformation($"[{methodName}] Country with ID {item.Id} successfully updated");
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry Country not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Countries.FirstOrDefaultAsync(d => d.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
