using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Results
{
    public class EditModel : PageModel
    {
        private readonly OlympiadContext _context;

        public EditModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Result Result { get; set; }

        public SelectList Sports { get; set; }

        public SelectList Participants { get; set; }

        public SelectList Starts { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Result = await _context.Results
                .FirstOrDefaultAsync(r =>
                    r.ResultId == id);

            if (Result == null)
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

            _context.Results.Update(Result);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task LoadListsAsync()
        {
            Sports = new SelectList(
                await _context.Sports.ToListAsync(),
                "SportId",
                "SportName");

            Participants = new SelectList(
                await _context.Participants.ToListAsync(),
                "ParticipantId",
                "FullName");

            Starts = new SelectList(
                await _context.EventSchedules.ToListAsync(),
                "StartId",
                "StartId");
        }
    }
}