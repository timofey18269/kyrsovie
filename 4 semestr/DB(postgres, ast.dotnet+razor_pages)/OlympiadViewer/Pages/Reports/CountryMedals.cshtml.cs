using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Models.Views;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Reports
{
    public class CountryMedalsModel : PageModel
    {
        private readonly OlympiadContext _context;

        private readonly ExportService _exportService;

        public CountryMedalsModel(
            OlympiadContext context,
            ExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        public IList<CountryMedalsView> ReportRows { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinGold { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxGold { get; set; }

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
                        "CountryMedals");

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CountryMedals.xlsx");
        }

        private IQueryable<CountryMedalsView>
            BuildQuery()
        {
            IQueryable<CountryMedalsView> query =
                _context.CountryMedalsView
                    .AsQueryable();

            // SEARCH

            if (!string.IsNullOrWhiteSpace(
                SearchTerm))
            {
                query = query.Where(x =>
                    x.CountryName.Contains(
                        SearchTerm));
            }

            // FILTERS

            if (MinGold.HasValue)
            {
                query = query.Where(x =>
                    x.GoldMedals >=
                    MinGold.Value);
            }

            if (MaxGold.HasValue)
            {
                query = query.Where(x =>
                    x.GoldMedals <=
                    MaxGold.Value);
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