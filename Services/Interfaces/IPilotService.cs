﻿using BussinessObjects;

namespace Services.Interfaces
{
    public interface IPilotService
    {
        Task<IEnumerable<Pilot>> GetAllPilotsAsync();
        Task<IEnumerable<Pilot>> GetAllAvailablePilotsAsync();
        Task<Pilot> GetPilotByIdAsync(int id);
        Task<string> AddPilotAsync(Pilot pilot);
        Task<string> UpdatePilotAsync(Pilot pilot);
        Task<string> DeletePilotAsync(int id);
        Task<int> GetTotalPilot();
        Task SetPilotStatus(Pilot pilot, bool status);
        Task<List<PilotVM>> GetAllPilotWithStatusDes();
    }
}
