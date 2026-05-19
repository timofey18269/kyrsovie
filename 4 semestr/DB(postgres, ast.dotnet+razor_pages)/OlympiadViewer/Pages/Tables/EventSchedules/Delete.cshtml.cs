using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.EventSchedules
{
    public class DeleteModel : PageModel
    {
        private readonly OlympiadContext _context;

        public DeleteModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EventSchedule EventSchedule { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            EventSchedule =
                await _context.EventSchedules
                    .Include(e => e.Sport)
                    .Include(e => e.Venue)
                    .FirstOrDefaultAsync(e =>
                        e.StartId == id);

            if (EventSchedule == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var schedule =
                await _context.EventSchedules
                    .FindAsync(id);

            if (schedule != null)
            {
                _context.EventSchedules
                    .Remove(schedule);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}