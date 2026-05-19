using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.EventSchedules
{
    public class EditModel : PageModel
    {
        private readonly OlympiadContext _context;

        public EditModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EventSchedule EventSchedule { get; set; }

        public SelectList Sports { get; set; }

        public SelectList Venues { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            EventSchedule =
                await _context.EventSchedules
                    .FirstOrDefaultAsync(e =>
                        e.StartId == id);

            if (EventSchedule == null)
                return NotFound();

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

            _context.EventSchedules
                .Update(EventSchedule);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task LoadListsAsync()
        {
            Sports = new SelectList(
                await _context.Sports.ToListAsync(),
                "SportId",
                "SportName");

            Venues = new SelectList(
                await _context.Venues.ToListAsync(),
                "VenueId",
                "Name");
        }
    }
}