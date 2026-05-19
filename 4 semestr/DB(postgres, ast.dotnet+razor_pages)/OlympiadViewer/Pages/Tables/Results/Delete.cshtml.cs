using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Results
{
    public class DeleteModel : PageModel
    {
        private readonly OlympiadContext _context;

        public DeleteModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Result Result { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Result = await _context.Results
                .Include(r => r.Participant)
                .Include(r => r.Sport)
                .FirstOrDefaultAsync(r =>
                    r.ResultId == id);

            if (Result == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var result =
                await _context.Results
                    .FindAsync(id);

            if (result != null)
            {
                _context.Results.Remove(result);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}