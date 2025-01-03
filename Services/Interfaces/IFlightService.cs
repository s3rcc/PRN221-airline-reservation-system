﻿using BussinessObjects;

namespace Services.Interfaces
{
    public interface IFlightService
    {
        Task<IEnumerable<Flight>> GetAllFlightsAsync();
        Task<IEnumerable<Flight>> GetAllFlightsWithPagitationAsync(int pageIndex, int pageSize);
        Task<IEnumerable<Flight>> GetAllAvailableFlightsAsync();
        Task<Flight> GetFlightByIdAsync(int id);
        Task<Flight> GetReturnFlightByIdAsync(int? id);
        Task<string> AddFlightAsync(Flight flight);
        Task<string> UpdateFlightAsync(Flight flight);
        Task<string> DeleteFlightAsync(int id);
        Task<IEnumerable<Flight>> FilterFlightsAsync(int originId, int destinationId, DateTime departureTime, int totalPassengers);
        Task<IEnumerable<Flight>> GetFlightsByYear(int year);
        Task<IEnumerable<Flight>> GetFlightsByMonth(DateTime startDate, DateTime endDate);
        Task<int> GetTotalFlight();
        Task<bool> IsCanDelete(int id);
        Task<IEnumerable<AirPlane>> GetAirPlanesFromUnavailableFlightsAsync();
        Task<IEnumerable<Flight>> GetAllFLightWithRealTimeCondition();
    }
}
