using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BussinessObjects;

namespace PRN___Final_Project.Pages.CRUD.AirPlaneManager;

public class AirPlaneManagementModel : PageModel
{
    private readonly IAirPlaneService _airPlaneService;

    public AirPlaneManagementModel(IAirPlaneService airPlaneService)
    {
        _airPlaneService = airPlaneService;
        AirPlanes = new List<AirPlane>();
        AirPlane = new AirPlane();
    }

    [BindProperty]
    public AirPlane AirPlane { get; set; }
    public IEnumerable<AirPlane> AirPlanes { get; set; }

    public async Task OnGetAsync()
    {
        AirPlanes = await _airPlaneService.GetAllAirPlanesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (AirPlane.PlaneId == 0)
        {
            await _airPlaneService.AddAirPlaneAsync(AirPlane);
        }
        else
        {
            await _airPlaneService.UpdateAirPlaneAsync(AirPlane);
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _airPlaneService.DeleteAirPlaneAsync(id);
        return RedirectToPage();
    }
}
