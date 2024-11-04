using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BussinessObjects;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;

namespace PRN___Final_Project.Pages.CRUD.AirPlaneManager;

public class AirPlaneManagementModel : PageModel
{
    private readonly IAirPlaneService _airPlaneService;

    public AirPlaneManagementModel(IAirPlaneService airPlaneService)
    {
        _airPlaneService = airPlaneService;
        AirPlanes = new List<AirPlane>();
        AirPlane = new AirPlane();

        StatusMessage = Noti.GetMsg();
        IsSuccess = Noti.IsSuccess;

    }

    [BindProperty]
    public AirPlane AirPlane { get; set; }
    public IEnumerable<AirPlane> AirPlanes { get; set; }

    public string StatusMessage { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = true;


    public async Task OnGetAsync()
    {
        AirPlanes = await _airPlaneService.GetAllAirPlanesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {

        try
        {
            if (!ModelState.IsValid)
            {
                Noti.SetFail($"Some thing wrong here!");
                return Page();
            }

            if (AirPlane.PlaneId == 0)
            {
                await _airPlaneService.AddAirPlaneAsync(AirPlane);
                Noti.SetSuccess("Create plane success!");
            }
            else
            {
                await _airPlaneService.UpdateAirPlaneAsync(AirPlane);
                Noti.SetSuccess("Update plane success!");
            }
        }
        catch (Exception ex)
        {
            Noti.SetFail($"Error: {ex.Message}");

        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {

        var rs = await _airPlaneService.DeleteAirPlaneAsync(id);

        if (rs == null)
            Noti.SetSuccess("Airplane deleted successfully.");
        else
            Noti.SetFail(rs);

        return RedirectToPage();
    }
}
