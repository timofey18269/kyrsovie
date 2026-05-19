using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Tables.Participants
{
    public class IndexModel : PageModel
    {
        private readonly OlympiadContext _context;

        public IndexModel(OlympiadContext context)
        {
            _context = context;
        }

        public IList<Participant> Participants { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinAge { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxAge { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Participant> query =
                _context.Participants
                    .Include(p => p.Country)
                    .Include(p => p.Sport);

            // SEARCH

            query = DynamicQueryService
                .ApplySearch(query, SearchTerm);

            // FILTER AGE

            if (MinAge.HasValue)
            {
                query = query.Where(p =>
                    p.BirthDate <= DateOnly.FromDateTime(DateTime.Today.AddYears(-(int)MinAge.Value)));
            }

            if (MaxAge.HasValue)
            {
                query = query.Where(p =>
                    p.BirthDate >= DateOnly.FromDateTime(DateTime.Today.AddYears(-(int)MaxAge.Value)));
            }

            // SORT

            query = DynamicQueryService
                .ApplySorting(
                    query,
                    SortColumn,
                    SortDirection);

            Participants = await query.ToListAsync();
        }
    }
}