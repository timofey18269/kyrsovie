using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Tables.Results
{
    public class IndexModel : PageModel
    {
        private readonly OlympiadContext _context;

        public IndexModel(OlympiadContext context)
        {
            _context = context;
        }

        public IList<Result> Results { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinPlace { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxPlace { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Result> query =
                _context.Results
                    .Include(r => r.Participant)
                    .Include(r => r.Sport)
                    .Include(r => r.EventSchedule);

            // SEARCH

            query = DynamicQueryService
                .ApplySearch(query, SearchTerm);

            // FILTER

            if (MinPlace.HasValue)
            {
                query = query.Where(r =>
                    r.Place >= MinPlace.Value);
            }

            if (MaxPlace.HasValue)
            {
                query = query.Where(r =>
                    r.Place <= MaxPlace.Value);
            }

            // SORT

            query = DynamicQueryService
                .ApplySorting(
                    query,
                    SortColumn,
                    SortDirection);

            Results = await query.ToListAsync();
        }
    }
}