using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPlayground.Pages
{
    public class RazorPageModel : PageModel
    {
        public int Input { get; private set; }

        public void OnGet()
        {
            Input = int.Parse(Request.Query["input"]!);
        }
    }
}
