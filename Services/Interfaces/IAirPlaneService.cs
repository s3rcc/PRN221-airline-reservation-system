using BussinessObjects;

namespace Services.Interfaces
{
    public interface IAirPlaneService
    {
        Task<IEnumerable<AirPlane>> GetAllAirPlanesAsync();
        Task<AirPlane> GetAirPlaneByIdAsync(int id);
        Task AddAirPlaneAsync(AirPlane airPlane);
        Task UpdateAirPlaneAsync(AirPlane airPlane);
        Task<string> DeleteAirPlaneAsync(int id);
        Task<int> GetTotalAirplane(); 
    }
}
