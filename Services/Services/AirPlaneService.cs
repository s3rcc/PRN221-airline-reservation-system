using BussinessObjects;
using Repositories.Interface;
using Services.Interfaces;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Services.Services
{
	public class AirPlaneService : IAirPlaneService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AirPlaneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAirPlaneAsync(AirPlane airPlane)
        {
            try
            {
                if (airPlane == null)
                {
                    throw new ArgumentNullException(nameof(airPlane));
                }

                await _unitOfWork.Repository<AirPlane>().AddAsync(airPlane);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while adding the airplane.");
            }
        }

        public async Task DeleteAirPlaneAsync(int id)
        {
            try
            {
                var airPlane = await _unitOfWork.Repository<AirPlane>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Air plane not found.");
                _unitOfWork.Repository<AirPlane>().DeleteAsync(airPlane);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while deleting the air plane.");
            }
        }

        public async Task<IEnumerable<AirPlane>> GetAllAirPlanesAsync()
        {
            try
            {
                return await _unitOfWork.Repository<AirPlane>().GetAllAsync();
            }
            catch
            {
                throw new Exception("An error occurred while retrieving air planes.");
            }
        }

        public async Task<AirPlane> GetAirPlaneByIdAsync(int id)
        {
            try
            {
                var airPlane = await _unitOfWork.Repository<AirPlane>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Air plane not found.");
                return airPlane;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the air plane.");
            }
        }

        public async Task UpdateAirPlaneAsync(AirPlane airPlane)
        {
            try
            {
                if (airPlane == null) throw new ArgumentNullException(nameof(airPlane));

                await _unitOfWork.Repository<AirPlane>().UpdateAsync(airPlane);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while updating the air plane.");
            }
        }

		public async Task<int> GetTotalAirplane()
		{
            try
            {
                var airPlane = await _unitOfWork.Repository<AirPlane>().GetAllAsync();
                return airPlane.Count();
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting total ariplane");
            }
		}
	}
}
