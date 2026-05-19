using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Sports
{
    public class EditModel : PageModel
    {
        private readonly OlympiadContext _context;

        public EditModel(OlympiadContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Sports.Update(Sport);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}