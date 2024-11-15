using BussinessObjects;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Services
{
    public class PilotService : IPilotService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PilotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> AddPilotAsync(Pilot pilot)
        {
            try
            {
                if (pilot == null)
                {
                    throw new ArgumentNullException(nameof(pilot));
                }

                var rs = await ValidateItem(pilot);

                if (rs == null)
                {
                    await _unitOfWork.Repository<Pilot>().AddAsync(pilot);
                    await _unitOfWork.SaveChangeAsync();
                }

                return rs;
            }
            catch (Exception ex)
            {
                return ("An error occurred while adding the pilot.");
            }
        }

        public async Task<string> DeletePilotAsync(int id)
        {
            try
            {

                var pilot = await _unitOfWork.Repository<Pilot>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Pilot not found.");

                var rs = await ValidatePilotDeleteAsync(pilot);

                if(rs != null)
                    return rs;


                //var flights = await _unitOfWork.Repository<Flight>().FindAsync(x => x.Pilot.PilotId.Equals(id)) ?? throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BADREQUEST, "There are flight with this pilot");
                _unitOfWork.Repository<Pilot>().DeleteAsync(pilot);
                await _unitOfWork.SaveChangeAsync();

                return null;
            }
            catch
            {
                return "Pilot is working, try again next time!";
            }
        }

        public async Task<IEnumerable<Pilot>> GetAllAvailablePilotsAsync() 
        {
            try
            {
                return await _unitOfWork.Repository<Pilot>().FindAsync(predicate: p => p.Status == true);
            }
            catch
            {
                throw new Exception("An error occurred while retrieving available pilots.");
            }
        }

        public async Task<IEnumerable<Pilot>> GetAllPilotsAsync()
        {
            try
            {
                return await _unitOfWork.Repository<Pilot>().GetAllAsync();
            }
            catch
            {
                throw new Exception("An error occurred while retrieving pilots.");
            }
        }

        public async Task<Pilot> GetPilotByIdAsync(int id)
        {
            try
            {
                var pilot = await _unitOfWork.Repository<Pilot>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Pilot not found.");
                return pilot;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the pilot.");
            } 
        }

		public async Task<int> GetTotalPilot()
		{
			try
			{
				var pilot = await _unitOfWork.Repository<Pilot>().GetAllAsync();
				return pilot.Count();
			}
			catch
			{
				throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting total pilot");
			}
		}

        public async Task SetPilotStatus(Pilot pilot, bool status)
        {
            pilot.Status = status;
            await UpdatePilotAsync(pilot);
        }

		public async Task<string> UpdatePilotAsync(Pilot pilot)
        {
            try
            {
                if (pilot == null) throw new ArgumentNullException(nameof(pilot));

                var rs = await ValidateItem(pilot);

                if (rs == null)
                {
                    await _unitOfWork.Repository<Pilot>().UpdateAsync(pilot);
                    await _unitOfWork.SaveChangeAsync();
                }

                return rs;
            }
            catch
            {
                return ("An error occurred while updating the pilot.");
            }
        }

        private async Task<string> ValidateItem(Pilot item)
        {
    //        var existingItem = await _unitOfWork.Repository<Pilot>()
    //.FindAsync(x => x.PilotName == item.PilotName.Trim());
    //        if (existingItem.Any())
    //        {
    //            return "Pilot name must be unique.";
    //        }

            return null;
        }

        public async Task<string?> ValidatePilotDeleteAsync(Pilot pilot)
        {
            var flights = await _unitOfWork.Repository<Flight>()
                .FindAsync(f => f.PilotId == pilot.PilotId);

            if (pilot == null)
                return "Pilot not found.";

            DateTime currentDateTime = DateTime.Now;

            foreach (var flight in flights)
            {
                if (flight.DepartureDateTime <= currentDateTime && flight.ArrivalDateTime >= currentDateTime)
                {
                    string flightDetails = $"Flight ID: {flight.FlightId}, Departure: {flight.DepartureDateTime.ToString("dd/MM/yyyy hh:mm tt")}, Arrival: {flight.ArrivalDateTime.ToString("dd/MM/yyyy hh:mm tt")}";
                    return $"Pilot ID {pilot.PilotId} Name {pilot.PilotName}: Currently flying on {flightDetails}. Cannot delete while flight is in progress.";
                }
            }

            return null;
        }




        public async Task<List<PilotVM>> GetAllPilotWithStatusDes()
        {
            var pilots = await _unitOfWork.Repository<Pilot>().GetAllAsync();
            var pilotVMList = new List<PilotVM>();

            foreach (var pilot in pilots)
            {
                var pilotVM = new PilotVM
                {
                    PilotId = pilot.PilotId,
                    PilotName = pilot.PilotName,
                    Status = pilot.Status
                };

                var flights = await _unitOfWork.Repository<Flight>()
                    .FindAsync(f => f.PilotId == pilot.PilotId);

                DateTime currentDateTime = DateTime.Now;
                var activeFlight = flights.FirstOrDefault(f => f.DepartureDateTime <= currentDateTime && f.ArrivalDateTime >= currentDateTime);

                if (activeFlight != null)
                {
                    pilotVM.StatusDes = $"Flying on Flight ID {activeFlight.FlightId}, Departure: {activeFlight.DepartureDateTime:dd/MM/yyyy hh:mm tt}, Arrival: {activeFlight.ArrivalDateTime:dd/MM/yyyy hh:mm tt}";
                }
                else
                {
                    var lastFlight = flights
                        .Where(f => f.ArrivalDateTime <= currentDateTime)
                        .OrderByDescending(f => f.ArrivalDateTime)
                        .FirstOrDefault();




                    if (lastFlight != null)
                    {

                        var desLoc = await _unitOfWork.Repository<Location>().GetByIdAsync(lastFlight.DestinationID);

                        TimeSpan timeDifference = currentDateTime - lastFlight.ArrivalDateTime;
                        if (timeDifference.TotalHours < 9)
                        {
                            pilotVM.StatusDes = $"Resting. Requires 9 hours rest if starting from {desLoc.LocationName}. Only {timeDifference.TotalHours:F0} hours have passed since last flight.";
                        }
                        else if (timeDifference.TotalHours < 24)
                        {
                            pilotVM.StatusDes = $"Requires 24 hours if not starting from a {desLoc.LocationName}. Only {timeDifference.TotalHours:F0} hours have passed since last flight.";
                        }
                        else
                            pilotVM.StatusDes = "Available";
                    }
                    else
                        pilotVM.StatusDes = "Available";
                }

                pilotVMList.Add(pilotVM);
            }

            return pilotVMList;
        }

    }



}
