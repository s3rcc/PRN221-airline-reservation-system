using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using BussinessObjects;

namespace PRN___Final_Project.Pages.CRUD.TierManager
{
    public class TierManageModel : PageModel
    {
        private readonly ITierService _tierService;

        public TierManageModel(ITierService tierService)
        {
            _tierService = tierService;
            Tiers = new List<Tier>();
            Tier = new Tier();
        }

        [BindProperty]
        public Tier Tier { get; set; }
        public IEnumerable<Tier> Tiers { get; set; }

        public async Task OnGetAsync()
        {
            Tiers = await _tierService.GetAllTiersAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await Console.Out.WriteLineAsync("\n\n\n");
            // Check if the model state is valid


            if (Tier.TierId == 0)
            {
                await _tierService.AddTierAsync(Tier);
            }
            else
            {
                await Console.Out.WriteLineAsync("\n\n\n On Update id:" + Tier.TierId);

                await _tierService.UpdateTierAsync(Tier);

            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _tierService.DeleteTierAsync(id);
            return RedirectToPage();
        }
    }
}
