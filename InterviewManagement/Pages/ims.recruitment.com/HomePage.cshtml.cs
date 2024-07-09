using InterviewManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace InterviewManagement.Pages.ims.recruitment.com
{
    [Authorize(Policy ="Employee")]
    public class HomePageModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
