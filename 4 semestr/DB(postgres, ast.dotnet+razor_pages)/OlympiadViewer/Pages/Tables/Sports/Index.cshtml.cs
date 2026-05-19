using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Tables.Sports
{
    public class IndexModel : PageModel
    {
        private readonly OlympiadContext _context;

        public IndexModel(OlympiadContext context)
        {
            _context = context;
        }

        public IList<Sport> Sports { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Sport> query =
                _context.Sports;

            query = DynamicQueryService
                .ApplySearch(query, SearchTerm);

            query = DynamicQueryService
                .ApplySorting(
                    query,
                    SortColumn,
                    SortDirection);

            Sports = await query.ToListAsync();
        }
    }
}