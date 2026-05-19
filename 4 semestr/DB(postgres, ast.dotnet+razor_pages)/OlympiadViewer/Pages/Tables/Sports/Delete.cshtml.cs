using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Sports
{
    public class DeleteModel : PageModel
    {
        private readonly OlympiadContext _context;

        public DeleteModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Sport Sport { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Sport = await _context.Sports.FindAsync(id);

            if (Sport == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var sport =
                await _context.Sports.FindAsync(id);

            if (sport != null)
            {
                _context.Sports.Remove(sport);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}