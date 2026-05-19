using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Tables.Venues
{
    public class IndexModel : PageModel
    {
        private readonly OlympiadContext _context;

        public IndexModel(OlympiadContext context)
        {
            _context = context;
        }

        public IList<Venue> Venues { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Venue> query =
                _context.Venues;

            query = DynamicQueryService
                .ApplySearch(query, SearchTerm);

            query = DynamicQueryService
                .ApplySorting(
                    query,
                    SortColumn,
                    SortDirection);

            Venues = await query.ToListAsync();
        }
    }
}