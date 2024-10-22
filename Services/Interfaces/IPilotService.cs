using BussinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPilotService
    {
        Task<IEnumerable<Pilot>> GetAllPilotsAsync();
        Task<IEnumerable<Pilot>> GetAllAvailablePilotsAsync();
        Task<Pilot> GetPilotByIdAsync(int id);
        Task AddPilotAsync(Pilot pilot);
        Task UpdatePilotAsync(Pilot pilot);
        Task DeletePilotAsync(int id);
        Task<int> GetTotalPilot();
    }
}
