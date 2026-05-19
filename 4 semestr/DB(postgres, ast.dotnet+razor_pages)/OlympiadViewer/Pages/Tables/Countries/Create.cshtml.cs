using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Countries
{
    public class CreateModel : PageModel
    {
        private readonly OlympiadContext _context;

        public CreateModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Country Country { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Countries.Add(Country);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}