using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface ILocationsService
    {
        Task<Location> AddLocation(Location location);
        Task<Location> GetLocationById(int id);
        Task<IEnumerable<Location>> GetLocations();
        Task RemoveLocation(int id);
        Task<Location> UpdateLocation(JObject updates, int id);
    }
}