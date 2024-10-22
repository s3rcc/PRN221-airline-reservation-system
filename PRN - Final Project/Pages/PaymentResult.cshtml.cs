using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class PaymentResultModel : PageModel
    {

            [BindProperty(SupportsGet = true)]
            public string Status { get; set; }

            [BindProperty(SupportsGet = true)]
            public string Message { get; set; }

            public void OnGet()
            {
                if (string.IsNullOrEmpty(Status))
                {
                    Status = "failed";
                    Message = "Kh�ng c� th�ng tin k?t qu? thanh to�n.";
                }
            }
        }
    }
