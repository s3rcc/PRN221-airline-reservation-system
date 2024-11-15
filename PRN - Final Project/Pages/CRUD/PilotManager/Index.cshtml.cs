using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using System;

namespace PRN___Final_Project.Pages.CRUD.PilotManager
{
    public class PilotManagementModel : PageModel
    {
        private readonly IPilotService _pilotService;

        public PilotManagementModel(IPilotService pilotService)
        {
            _pilotService = pilotService;
            Pilots = new List<PilotVM>();
            Pilot = new Pilot();

            StatusMessage = Noti.GetMsg();
            IsSuccess = Noti.IsSuccess;
        }

        [BindProperty]
        public Pilot Pilot { get; set; }
        public IEnumerable<PilotVM> Pilots { get; set; }

        // New properties to track messages
        public string StatusMessage { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!(User.IsInRole("Staff") || User.IsInRole("Admin")))
            {
                return RedirectToPage("/Errors/404");
            }
            Pilots = await _pilotService.GetAllPilotWithStatusDes();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var rs = string.Empty;
            var action = string.Empty;

            try
            {
                if (Pilot.PilotId == 0)
                {
                    rs = await _pilotService.AddPilotAsync(Pilot);
                    action = "Create";
                }
                else
                {
                    rs = await _pilotService.UpdatePilotAsync(Pilot);
                    action = "Update";
                }

                Noti.SetByResult(action, "pilot", rs);
            }
            catch (Exception ex)
            {
                Noti.SetFail($"Error: {ex.Message}");
            }

            // Redirect to the same page with status messages
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var rs = await _pilotService.DeletePilotAsync(id);


            Noti.SetByResult("Delete", "pilot", rs);

            return RedirectToPage();
        }
    }
}
