using BussinessObjects;


namespace Services.Interfaces
{
    public interface ITierService
    {
        Task AddTierAsync(Tier tier);
        Task DeleteTierAsync(int id);
        Task<IEnumerable<Tier>> GetAllTiersAsync();
        Task<Tier> GetTierByIdAsync(int id);
        Task UpdateTierAsync(Tier tier);
    }
}
