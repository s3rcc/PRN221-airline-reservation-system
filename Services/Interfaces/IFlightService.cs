using BussinessObjects;

namespace Services.Interfaces
{
    public interface IFlightService
    {
        Task<IEnumerable<Flight>> GetAllFlightsAsync();
        Task<IEnumerable<Flight>> GetAllAvailableFlightsAsync();
        Task<Flight> GetFlightByIdAsync(int id);
        Task<string> AddFlightAsync(Flight flight);
        Task<string> UpdateFlightAsync(Flight flight);
        Task<string> DeleteFlightAsync(int id);
        Task<IEnumerable<Flight>> FilterFlightsAsync(int originId, int destinationId, DateTime departureTime);
        Task<IEnumerable<Flight>> GetFlightsByYear(int year);
        Task<IEnumerable<Flight>> GetFlightsByMonth(DateTime startDate, DateTime endDate);
        Task<int> GetTotalFlight();
        Task<bool> IsCanDelete(int id);
    }
}
