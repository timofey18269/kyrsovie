using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Results
{
    public class CreateModel : PageModel
    {
        private readonly OlympiadContext _context;

        public CreateModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Result Result { get; set; }

        public SelectList Sports { get; set; }

        public SelectList Participants { get; set; }

        public SelectList Starts { get; set; }

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

            _context.Results.Add(Result);

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