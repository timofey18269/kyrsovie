using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Models.Views;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Reports
{
    public class AthleteResultsModel : PageModel
    {
        private readonly OlympiadContext _context;

        private readonly ExportService _exportService;

        public AthleteResultsModel(
            OlympiadContext context,
            ExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        public IList<AthleteResultsView> ReportRows { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinPlace { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxPlace { get; set; }

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
                        "AthleteResults");

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AthleteResults.xlsx");
        }

        private IQueryable<AthleteResultsView>
            BuildQuery()
        {
            IQueryable<AthleteResultsView> query =
                _context.AthleteResultsView
                    .AsQueryable();

            // SEARCH

            if (!string.IsNullOrWhiteSpace(
                SearchTerm))
            {
                query = query.Where(x =>
                    x.FullName.Contains(
                        SearchTerm)
                    ||
                    x.SportName.Contains(
                        SearchTerm));
            }

            // FILTERS

            if (MinPlace.HasValue)
            {
                query = query.Where(x =>
                    x.Place >=
                    MinPlace.Value);
            }

            if (MaxPlace.HasValue)
            {
                query = query.Where(x =>
                    x.Place <=
                    MaxPlace.Value);
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