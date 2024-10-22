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
                    Message = "Không có thông tin k?t qu? thanh toán.";
                }
            }
        }
    }
