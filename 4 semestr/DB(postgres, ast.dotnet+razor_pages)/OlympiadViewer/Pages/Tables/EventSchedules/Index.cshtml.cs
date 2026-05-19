using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Tables.EventSchedules
{
    public class IndexModel : PageModel
    {
        private readonly OlympiadContext _context;

        public IndexModel(OlympiadContext context)
        {
            _context = context;
        }

        public IList<EventSchedule> EventSchedules { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly? ToDate { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<EventSchedule> query =
                _context.EventSchedules
                    .Include(e => e.Sport)
                    .Include(e => e.Venue);

            // DATE FILTER

            if (FromDate.HasValue)
            {
                
                query = query.Where(e =>
                    e.StartDate >= FromDate.Value);
            }

            if (ToDate.HasValue)
            {
                query = query.Where(e =>
                    e.StartDate <= ToDate.Value);
            }

            // SORT

            query = DynamicQueryService
                .ApplySorting(
                    query,
                    SortColumn,
                    SortDirection);

            EventSchedules =
                await query.ToListAsync();
        }
    }
}