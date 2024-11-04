using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BussinessObjects;

namespace PRN___Final_Project.Pages.CRUD.LocationManager;

public class LocationManagementModel : PageModel
{
    private readonly ILocationService _locationService;
    public LocationManagementModel(ILocationService locationService)
    {
        _locationService = locationService;
        Locations = new List<Location>();
        Location = new Location();
    }

    [BindProperty]
    public Location Location { get; set; }
    public IEnumerable<Location> Locations { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!(User.IsInRole("Staff") || User.IsInRole("Admin")))
        {
            return RedirectToPage("/Errors/404");
        }
        Locations = await _locationService.GetAllLocationsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        Console.WriteLine($"Location Name: {Location.LocationName}");

        if (Location.LocationID == 0)
        {
            await _locationService.AddLocationAsync(Location);
        }
        else
        {
            await _locationService.UpdateLocationAsync(Location);
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _locationService.DeleteLocationAsync(id);
        return RedirectToPage();
    }
}
