using BussinessObjects;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddLocationAsync(Location location)
        {
            try
            {
                if (location == null)
                {
                    throw new ArgumentNullException(nameof(location));
                }

                await _unitOfWork.Repository<Location>().AddAsync(location);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while adding the location.");
            }
        }

        public async Task DeleteLocationAsync(int id)
        {
            try
            {
                var location = await _unitOfWork.Repository<Location>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Location not found.");

                var flights = await _unitOfWork.Repository<Flight>().FindAsync(f => (f.OriginID == location.LocationID || f.DestinationID == location.LocationID) && f.Status == true);

                if (flights.Any())
                {
                    throw new Exception("Cannot delete the location because there are flights related to it.");
                }

                _unitOfWork.Repository<Location>().DeleteAsync(location);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while deleting the location.");
            }
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            try
            {
                return await _unitOfWork.Repository<Location>().GetAllAsync();
            }
            catch
            {
                throw new Exception("An error occurred while retrieving locations.");
            }
        }

        public async Task<Location> GetLocationByIdAsync(int id)
        {
            try
            {
                var location = await _unitOfWork.Repository<Location>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Location not found.");
                return location;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the location.");
            }
        }

        public async Task UpdateLocationAsync(Location location)
        {
            try
            {
                if (location == null) throw new ArgumentNullException(nameof(location));

                await _unitOfWork.Repository<Location>().UpdateAsync(location);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while updating the location.");
            }
        }
    }
}
