using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.EventSchedules
{
    public class CreateModel : PageModel
    {
        private readonly OlympiadContext _context;

        public CreateModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EventSchedule EventSchedule { get; set; }

        public SelectList Sports { get; set; }

        public SelectList Venues { get; set; }

        public async Task OnGetAsync()
        {
            await LoadListsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return Page();
            }

            _context.EventSchedules
                .Add(EventSchedule);

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