using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Venues
{
    public class DeleteModel : PageModel
    {
        private readonly OlympiadContext _context;

        public DeleteModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Venue Venue { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Venue = await _context.Venues
                .FirstOrDefaultAsync(v =>
                    v.VenueId == id);

            if (Venue == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var venue =
                await _context.Venues.FindAsync(id);

            if (venue != null)
            {
                _context.Venues.Remove(venue);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}