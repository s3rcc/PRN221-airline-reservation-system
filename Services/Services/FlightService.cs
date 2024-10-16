﻿using BussinessObjects;
using Repositories.Interface;
using Services.Interfaces;
using System.Linq.Expressions;

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

        public async Task<IEnumerable<Flight>> FilterFlightsAsync(int originId, int destinationId, DateTime departureTime)
        {
            try
            {
                return await _unitOfWork.Repository<Flight>().FindAsync(
                    f => f.OriginID == originId &&
                         f.DestinationID == destinationId &&
                         f.DepartureDateTime.Date == departureTime.Date,
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
                return await _unitOfWork.Repository<Flight>().FindAsync(f => f.Status == true, includes:
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
    }
}
