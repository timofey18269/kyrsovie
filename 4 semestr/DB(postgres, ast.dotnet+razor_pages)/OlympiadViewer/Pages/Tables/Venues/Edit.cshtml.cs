using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Venues
{
    public class EditModel : PageModel
    {
        private readonly OlympiadContext _context;

        public EditModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Venue Venue { get; set; }

        [BindProperty]
        public List<int> SelectedSports { get; set; }
            = new();

        public SelectList Sports { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Venue = await _context.Venues
                .FirstOrDefaultAsync(v =>
                    v.VenueId == id);

            if (Venue == null)
                return NotFound();

            SelectedSports =
                Venue.PossibleSports?.ToList()
                ?? new List<int>();

            await LoadListsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return Page();
            }

            Venue.PossibleSports =
                SelectedSports.ToArray();

            _context.Venues.Update(Venue);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task LoadListsAsync()
        {
            Sports = new SelectList(
                await _context.Sports.ToListAsync(),
                "SportId",
                "SportName");
        }
    }
}