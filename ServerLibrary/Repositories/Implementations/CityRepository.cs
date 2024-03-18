using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Interfaces;

namespace ServerLibrary.Repositories.Implementations
{
    public class CityRepository(AppDbContext appDbContext, ILogger<CityRepository> logger) : IGenericRepositoryInterface<City>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var methodName = nameof(DeleteById);
            logger.LogInformation($"[{methodName}] Attempting to delete city with ID: {id}");

            var city = await appDbContext.Cities.FindAsync(id);
            if (city is null)
            {
                logger.LogError($"[{methodName}] City with ID {id} not found");
                return NotFound();
            }

            appDbContext.Cities.Remove(city);
            await Commit();

            logger.LogInformation($"[{methodName}] City with ID {id} successfully deleted");
            return Success();
        }

        public async Task<List<City>> GetAll()
        {
            var methodName = nameof(GetAll);
            logger.LogInformation($"[{methodName}] Attempting to retrieve all cities");

            var cities = await appDbContext.Cities.AsNoTracking().Include(c => c.Country).ToListAsync();

            logger.LogInformation($"[{methodName}] Successfully retrieved all cities");

            return cities;
        }

        public async Task<City> GetById(int id)
        {
            var methodName = nameof(GetById);
            logger.LogInformation($"[{methodName}] Attempting to retrieve city with ID: {id}");

            var city = await appDbContext.Cities.FindAsync(id);

            if (city == null)
            {
                logger.LogError($"[{methodName}] City with ID {id} not found");
            }
            else
            {
                logger.LogInformation($"[{methodName}] Successfully retrieved city with ID: {id}");
            }

            return city;
        }

        public async Task<GeneralResponse> Insert(City item)
        {
            var methodName = nameof(Insert);
            logger.LogInformation($"[{methodName}] Attempting to insert city: {item.Name}");

            if (!await CheckName(item.Name!))
            {
                logger.LogError($"[{methodName}] City already exists: {item.Name}");
                return new GeneralResponse(false, "City already added");
            }

            appDbContext.Cities.Add(item);
            await Commit();

            logger.LogInformation($"[{methodName}] City {item.Name} successfully inserted");
            return Success();
        }

        public async Task<GeneralResponse> Update(City item)
        {
            var methodName = nameof(Update);
            logger.LogInformation($"[{methodName}] Attempting to update city with ID: {item.Id}");

            var city = await appDbContext.Cities.FindAsync(item.Id);
            if (city is null)
            {
                logger.LogError($"[{methodName}] City with ID {item.Id} not found");
                return NotFound();
            }

            city.Name = item.Name;
            city.CountryId = item.CountryId;

            await Commit();

            logger.LogInformation($"[{methodName}] City with ID {item.Id} successfully updated");
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry City not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Cities.FirstOrDefaultAsync(d => d.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
