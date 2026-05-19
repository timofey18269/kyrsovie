using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Models.Views;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Reports
{
    public class AverageAgeBySportModel : PageModel
    {
        private readonly OlympiadContext _context;

        private readonly ExportService _exportService;

        public AverageAgeBySportModel(
            OlympiadContext context,
            ExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        public IList<AverageAgeView> ReportRows { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? MinAge { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? MaxAge { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; }

        public async Task OnGetAsync()
        {
            ReportRows =
                await BuildQuery()
                    .ToListAsync();
        }

        public async Task<IActionResult>
            OnPostExportAsync()
        {
            var data =
                await BuildQuery()
                    .ToListAsync();

            var table =
                _exportService
                    .ConvertToDataTable(data);

            var bytes =
                _exportService
                    .ExportToExcel(
                        table,
                        "AverageAgeBySport");

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AverageAgeBySport.xlsx");
        }

        private IQueryable<AverageAgeView>
            BuildQuery()
        {
            IQueryable<AverageAgeView> query =
                _context.AverageAgeView
                    .AsQueryable();

            // SEARCH

            if (!string.IsNullOrWhiteSpace(
                SearchTerm))
            {
                query = query.Where(x =>
                    x.SportName.Contains(
                        SearchTerm));
            }

            // FILTERS

            if (MinAge.HasValue)
            {
                query = query.Where(x =>
                    x.AverageAge >=
                     ((int)MinAge.Value));
            }

            if (MaxAge.HasValue)
            {
                query = query.Where(x =>
                    x.AverageAge <=
                    ((int)MaxAge.Value));
            }

            // SORTING

            query = DynamicQueryService
                .ApplySorting(
                    query,
                    SortColumn,
                    SortDirection);

            return query;
        }
    }
}