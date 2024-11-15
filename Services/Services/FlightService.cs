using BussinessObjects;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using Services.Interfaces;
using System.Linq.Expressions;

namespace Services.Services
{
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPilotService _iplotService;
        private readonly ILocationService _locationService;

        public FlightService(IUnitOfWork unitOfWork, IPilotService iplotService, ILocationService locationService)
        {
            _unitOfWork = unitOfWork;
            _iplotService = iplotService;
            _locationService = locationService;
        }

        public async Task<string> AddFlightAsync(Flight flight)
        {



            try
            {
                if (flight == null)
                {
                    throw new ArgumentNullException(nameof(flight));
                }

                var pilotValid = await ValidateFlightScheduleAsync(flight);
                if (pilotValid != null)
                    return pilotValid;


                var rs = await ValidateFlight(flight, true);
                //await _iplotService.SetPilotStatus(flight.Pilot, false);

                if (rs == null)
                {
                    var plane = await _unitOfWork.Repository<AirPlane>().GetByIdAsync(flight
                        .PlaneId);
                    flight.AvailableNormalSeat = plane.NormalSeatNumber;
                    flight.AvailableVipSeat = plane.VipSeatNumber;
                    await _unitOfWork.Repository<Flight>().AddAsync(flight);
                    await _unitOfWork.SaveChangeAsync();
                }

                return rs;
            }
            catch
            {
                return ("An error occurred while adding the airplane.");
            }
        }

        public async Task<bool> IsCanDelete(int id)
        {
            try
            {
                var flight = await _unitOfWork.Repository<Flight>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Flight not found.");
                _unitOfWork.Repository<Flight>().DeleteAsync(flight);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> DeleteFlightAsync(int id)
        {
            try
            {
                var flight = await _unitOfWork.Repository<Flight>().GetByIdAsync(id)
                    ?? throw new KeyNotFoundException("Flight not found.");

                DateTime currentDateTime = DateTime.Now;

                if (flight.DepartureDateTime <= currentDateTime && flight.ArrivalDateTime >= currentDateTime)
                    return "Cannot delete flight. The flight is currently in progress.";

                _unitOfWork.Repository<Flight>().DeleteAsync(flight);
                await _unitOfWork.SaveChangeAsync();

                return null;
            }
            catch
            {
                return "An error occurred while deleting the flight.";
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
            ],
                orderBy: x => x.OrderByDescending(x => x.DepartureDateTime));
            }
            catch
            {
                throw new Exception("An error occured while retrieving flights.");
            }
        }

        public async Task<IEnumerable<AirPlane>> GetAirPlanesFromUnavailableFlightsAsync()
        {
            try
            {
                var unavailableFlights = await _unitOfWork.Repository<Flight>().FindAsync(f => f.Status == false, includes:
                    [
                        flight => flight.Plane,
                        flight => flight.Pilot,
                        flight => flight.Origin,
                        flight => flight.Destination
                    ]);

                var airplanes = unavailableFlights
                    .Select(flight => flight.Plane)
                    .Distinct()
                    .ToList();

                return airplanes;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving airplanes from unavailable flights.");
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
            ],
            orderBy: x => x.OrderByDescending(x => x.DepartureDateTime)
            );
            }
            catch
            {
                throw new Exception("An error occured while retrieving flights.");
            }
        }

        public async Task<IEnumerable<Flight>> GetAllFlightsWithPagitationAsync(int pageIndex, int pageSize)
        {
            var query = await _unitOfWork.Repository<Flight>()
                .GetAllAsync(orderBy: x => x.OrderByDescending(x => x.DepartureDateTime));

            // Apply pagination using Skip and Take
            var paginatedResults = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            // Apply real-time condition on the paginated flights
            foreach (var flight in paginatedResults)
            {
                if (DateTime.Now > flight.ArrivalDateTime)
                {
                    await SetUnAvailableStatusForFlight(flight);
                }
            }


            return paginatedResults;
        }


        public async Task<IEnumerable<Flight>> GetAllFLightWithRealTimeCondition()
        {
            var flights = await GetAllFlightsAsync();

            foreach (var flight in flights)
            {
                if (DateTime.Now > flight.ArrivalDateTime)
                    await SetUnAvailableStatusForFlight(flight);
            }
            return flights;
        }


        public async Task SetUnAvailableStatusForFlight(Flight flight)
        {
            flight.Status = false;
            await _iplotService.SetPilotStatus(flight.Pilot, true);
            await UpdateFlightAsync(flight);
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
                return await _unitOfWork.Repository<Flight>().FindAsync(x => x.DepartureDateTime.Year == year, includes:
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

        public async Task<string> UpdateFlightAsync(Flight flight)
        {
            try
            {
                if (flight == null) throw new ArgumentNullException(nameof(flight));
                var rs = await ValidateFlight(flight, false);

                if (rs == null)
                {
                    var plane = await _unitOfWork.Repository<AirPlane>().GetByIdAsync(flight
                    .PlaneId);
                    if (flight.AvailableNormalSeat == null && flight.AvailableVipSeat == null)
                    {
                        flight.AvailableNormalSeat = plane.NormalSeatNumber;
                        flight.AvailableVipSeat = plane.VipSeatNumber;
                    }
                    await _unitOfWork.Repository<Flight>().UpdateAsync(flight);
                    await _unitOfWork.SaveChangeAsync();
                }

                return rs;
            }
            catch
            {
                return ("An error occurred while updating the flight.");
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

        private async Task<string> ValidateFlight(Flight flight, bool isCreate)
        {
            var flightExists = await _unitOfWork.Repository<Flight>()
                .AnyAsync(f => f.FlightNumber == flight.FlightNumber.Trim(), true);

            if (flightExists && isCreate)
                return "Flight number must be unique.";

            if (flight.OriginID == flight.DestinationID)
                return "Origin and Destination must be different.";

            if (flight.DepartureDateTime >= flight.ArrivalDateTime)
                return "Departure time must be earlier than arrival time.";

            if (flight.BasePrice <= 0)
                return "Base price must be greater than zero.";

            return null;
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


        public async Task<string?> ValidateFlightScheduleAsync(Flight flightCreate)
        {
            string errors = null;
            // Get pilot by id
            var pilot = await _unitOfWork.Repository<Pilot>().GetByIdAsync(flightCreate.PilotId);

            if (pilot == null)
            {
                return "Pilot not found.";
            }
            // Get location By id
            var oriLoc = await _unitOfWork.Repository<Location>().GetByIdAsync(flightCreate.OriginID);
            var oriLocName = oriLoc.LocationName;

            // Filter flight have the same pilot and the flight is previouus to the flight want to create
            var flights = await _unitOfWork.Repository<Flight>()
                .FindAsync(f => f.PilotId == flightCreate.PilotId && f.ArrivalDateTime <= flightCreate.DepartureDateTime);
            // The previous flight compare to create flight
            var flightLast = flights
                .OrderByDescending(f => f.ArrivalDateTime)
                .FirstOrDefault();

            if (flightLast == null)
                return null;

            // Check time between 2 flight 
            TimeSpan timeDifference = flightCreate.DepartureDateTime - flightLast.ArrivalDateTime;
            string formattedTimeDifference = $"{Math.Floor(timeDifference.TotalHours):F0} hours and {timeDifference.Minutes} minutes";

            string lastArrivalFormatted = flightLast.ArrivalDateTime.ToString("dd/MM/yyyy hh:mm tt");
            string createDepartureFormatted = flightCreate.DepartureDateTime.ToString("dd/MM/yyyy hh:mm tt");

            var desLoc = await _unitOfWork.Repository<Location>().GetByIdAsync(flightLast.DestinationID);
            var desLocName = desLoc.LocationName;

            var piName = pilot.PilotName;

            string? error = (flightLast.DestinationID == flightCreate.OriginID) switch
            {

                // Case where 9 hours are required between flights
                true when timeDifference.TotalHours < 9 =>
                    $"Pilot ID {pilot.PilotId} - Name {piName}: Requires 9 hours rest after a flight from {desLocName}. Last arrival was at {lastArrivalFormatted} (only {formattedTimeDifference} have passed).",

                false when timeDifference.TotalHours < 24 =>
                    $"Pilot ID {pilot.PilotId} - Name {piName}: Needs 24 hours for pilot to move if the flight did not start from {desLocName}. Last arrival was at {lastArrivalFormatted} (only {formattedTimeDifference} have passed).",


                _ => null
            };

            return error;
        }


    }
}
