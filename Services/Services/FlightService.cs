using BussinessObjects;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Services
{
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FlightService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddFlightAsync(Flight flight)
        {
            try
            {
                if (flight == null)
                {
                    throw new ArgumentNullException(nameof(flight));
                }
                var plane = await _unitOfWork.Repository<AirPlane>().GetByIdAsync(flight
                    .PlaneId);
                flight.AvailableNormalSeat = plane.NormalSeatNumber;
                flight.AvailableVipSeat = plane.VipSeatNumber;
                await _unitOfWork.Repository<Flight>().AddAsync(flight);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while adding the airplane.");
            }
        }

        public async Task DeleteFlightAsync(int id)
        {
            try
            {
                var flight = await _unitOfWork.Repository<Flight>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Flight not found.");
                _unitOfWork.Repository<Flight>().DeleteAsync(flight);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while deleting the flight.");
            }
        }

        public async Task<IEnumerable<Flight>> FilterFlightsAsync(int originId, int destinationId, DateTime departureDate, int totalPassengers)
        {
            try
            {
                return await _unitOfWork.Repository<Flight>().FindAsync(
                    f => f.OriginID == originId &&
                         f.DestinationID == destinationId &&
                         f.DepartureDateTime.Date == departureDate.Date &&
                 (f.AvailableNormalSeat >= totalPassengers || f.AvailableVipSeat >= totalPassengers),
                    includes:
                    [
                flight => flight.Plane,
                flight => flight.Pilot,
                flight => flight.Origin,
                flight => flight.Destination
                    ],
                    orderBy: flights => flights.OrderBy(f => f.DepartureDateTime)
                );
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while filtering flights.", ex);
            }
        }

        public async Task<IEnumerable<Flight>> GetAllAvailableFlightsAsync()
        {
            try
            {
                return await _unitOfWork.Repository<Flight>().FindAsync(f => f.Status, 
                includes:
            [
                flight => flight.Plane,
                flight => flight.Pilot,
                flight => flight.Origin,
                flight => flight.Destination
            ]);
            }
            catch
            {
                throw new Exception("An error occured while retrieving flights.");
            }
        }

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            try
            {
                return await _unitOfWork.Repository<Flight>().GetAllAsync(includes:
            [
                flight => flight.Plane,
                flight => flight.Pilot,
                flight => flight.Origin,
                flight => flight.Destination
            ]);
            }
            catch
            {
                throw new Exception("An error occured while retrieving flights.");
            }
        }

        public async Task<IEnumerable<Flight>> GetFlightsByMonth(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _unitOfWork.Repository<Flight>().FindAsync(x => x.DepartureDateTime >= startDate.Date && x.DepartureDateTime <= endDate.Date, includes:
                    [
                    flight => flight.Plane,
                flight => flight.Pilot,
                flight => flight.Origin,
                flight => flight.Destination
                    ]);
            }
            catch
            {
                throw new Exception("An error occured while retrieving flights.");
            }
        }

        public async Task<IEnumerable<Flight>> GetFlightsByYear(int year)
        {
            try
            {
                return await _unitOfWork.Repository<Flight>().FindAsync(x => x.DepartureDateTime.Year == year ,includes:
                    [
                    flight => flight.Plane,
                flight => flight.Pilot,
                flight => flight.Origin,
                flight => flight.Destination
                    ]);
            }
            catch
            {
                throw new Exception("An error occured while retrieving flights.");
            }
        }

        public async Task<Flight> GetFlightByIdAsync(int id)
        {
            try
            {
                var flight = await _unitOfWork.Repository<Flight>().GetByIdAsync(id, includes:
            [
                flight => flight.Plane,
                flight => flight.Pilot,
                flight => flight.Origin,
                flight => flight.Destination
            ]) ?? throw new KeyNotFoundException("Flight not found.");
                return flight;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the flight.");
            }
        }


        public async Task UpdateFlightAsync(Flight flight)
        {
            try
            {
                if (flight == null) throw new ArgumentNullException(nameof(flight));

                await _unitOfWork.Repository<Flight>().UpdateAsync(flight);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while updating the flight.");
            }
        }

		public async Task<int> GetTotalFlight()
		{
			try
			{
				var flight = await _unitOfWork.Repository<Flight>().GetAllAsync();
				return flight.Count();
			}
			catch
			{
				throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting total flight");
			}
		}

        public async Task<Flight> GetReturnFlightByIdAsync(int? id)
        {
            try
            {
                var flight = await _unitOfWork.Repository<Flight>().FirstOrDefaultAsync(x => x.FlightId == id, includes:
            [
                flight => flight.Plane,
                flight => flight.Pilot,
                flight => flight.Origin,
                flight => flight.Destination
            ]);
                return flight;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the flight.");
            }
        }
    }
}
