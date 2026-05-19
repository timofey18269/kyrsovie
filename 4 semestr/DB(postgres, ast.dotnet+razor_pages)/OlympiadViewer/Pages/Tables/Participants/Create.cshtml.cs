using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Participants
{
    public class CreateModel : PageModel
    {
        private readonly OlympiadContext _context;

        public CreateModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Participant Participant { get; set; }

        public SelectList Countries { get; set; }

        public SelectList Sports { get; set; }

        public async Task OnGetAsync()
        {
            Countries = new SelectList(
                await _context.Countries.ToListAsync(),
                "CountryCode",
                "Name");

            Sports = new SelectList(
                await _context.Sports.ToListAsync(),
                "SportId",
                "SportName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return Page();
            }

            _context.Participants.Add(Participant);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task LoadListsAsync()
        {
            Countries = new SelectList(
                await _context.Countries.ToListAsync(),
                "CountryCode",
                "Name");

            Sports = new SelectList(
                await _context.Sports.ToListAsync(),
                "SportId",
                "SportName");
        }
    }
}