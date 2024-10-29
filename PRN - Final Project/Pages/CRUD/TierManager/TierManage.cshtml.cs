using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using BussinessObjects;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;

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

            StatusMessage = Noti.GetMsg();
            IsSuccess = Noti.IsSuccess;

        }

        [BindProperty]
        public Tier Tier { get; set; }
        public IEnumerable<Tier> Tiers { get; set; }

        public string StatusMessage { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;


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


                try
                {
                    await _tierService.UpdateTierAsync(Tier);
                    Noti.SetSuccess("Update tier success!");
                }
                catch (Exception ex)
                {
                    Noti.SetFail($"Error: {ex.Message}");
                }

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
