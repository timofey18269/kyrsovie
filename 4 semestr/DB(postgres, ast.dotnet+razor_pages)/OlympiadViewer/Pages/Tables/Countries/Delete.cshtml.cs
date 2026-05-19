using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Countries
{
    public class DeleteModel : PageModel
    {
        private readonly OlympiadContext _context;

        public DeleteModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Country Country { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Country = await _context.Countries.FindAsync(id);

            if (Country == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var country =
                await _context.Countries.FindAsync(id);

            if (country != null)
            {
                _context.Countries.Remove(country);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}