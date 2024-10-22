using BussinessObjects;

namespace Services.Interfaces
{
    public interface IFlightService
    {
        Task<IEnumerable<Flight>> GetAllFlightsAsync();
        Task<IEnumerable<Flight>> GetAllAvailableFlightsAsync();
        Task<Flight> GetFlightByIdAsync(int id);
        Task AddFlightAsync(Flight flight);
        Task UpdateFlightAsync(Flight flight);
        Task DeleteFlightAsync(int id);
        Task<IEnumerable<Flight>> FilterFlightsAsync(int originId, int destinationId, DateTime departureTime, DateTime? returnDate);
        Task<IEnumerable<Flight>> FilterFlightsAsync(int originId, int destinationId, DateTime departureTime);
        Task<IEnumerable<Flight>> GetFlightsByYear(int year);
        Task<IEnumerable<Flight>> GetFlightsByMonth(DateTime startDate, DateTime endDate);
        Task<int> GetTotalFlight();
    }
}
