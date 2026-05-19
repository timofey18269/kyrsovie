using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;

namespace OlympiadViewer.Pages.Tables.Participants
{
    public class DeleteModel : PageModel
    {
        private readonly OlympiadContext _context;

        public DeleteModel(OlympiadContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Participant Participant { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Participant = await _context.Participants
                .Include(p => p.Country)
                .Include(p => p.Sport)
                .FirstOrDefaultAsync(p =>
                    p.ParticipantId == id);

            if (Participant == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var participant =
                await _context.Participants
                    .FindAsync(id);

            if (participant != null)
            {
                _context.Participants.Remove(participant);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}