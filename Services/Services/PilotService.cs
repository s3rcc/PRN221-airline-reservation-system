﻿using BussinessObjects;
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
                var pilot = await _unitOfWork.Repository<Pilot>().GetByIdAsync(id) ?? throw new KeyNotFoundException("Role not found.");
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

    }
}
