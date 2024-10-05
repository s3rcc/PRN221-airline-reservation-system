using BussinessObjects;
using Repositories.Interface;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PilotService : IPilotService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PilotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddPilotAsync(Pilot pilot)
        {
            try
            {
                if (pilot == null)
                {
                    throw new ArgumentNullException(nameof(pilot));
                }

                await _unitOfWork.Repository<Pilot>().AddAsync(pilot);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while adding the role.");
            }
        }

        public async Task DeletePilotAsync(int id)
        {
            try
            {
                var pilot = await _unitOfWork.Repository<Pilot>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Role not found.");
                _unitOfWork.Repository<Pilot>().DeleteAsync(pilot);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while deleting the pilot.");
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

        public async Task UpdatePilotAsync(Pilot pilot)
        {
            try
            {
                if (pilot == null) throw new ArgumentNullException(nameof(pilot));

                await _unitOfWork.Repository<Pilot>().UpdateAsync(pilot);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while updating the pilot.");
            };
        }
    }
}
