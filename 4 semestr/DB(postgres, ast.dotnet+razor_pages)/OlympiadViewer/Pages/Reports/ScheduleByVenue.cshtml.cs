using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Models;
using OlympiadViewer.Models.Views;
using OlympiadViewer.Services;

namespace OlympiadViewer.Pages.Reports
{
    public class ScheduleByVenueModel : PageModel
    {
        private readonly OlympiadContext _context;

        private readonly ExportService _exportService;

        public ScheduleByVenueModel(
            OlympiadContext context,
            ExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        public IList<ScheduleByVenueView> ReportRows { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly? SelectedDate { get; set; }

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
                        "ScheduleByVenue");

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ScheduleByVenue.xlsx");
        }

        private IQueryable<ScheduleByVenueView>
            BuildQuery()
        {
            IQueryable<ScheduleByVenueView> query =
                _context.ScheduleByVenueView
                    .AsQueryable();

            // FILTER

            if (SelectedDate.HasValue)
            {
                query = query.Where(x =>
                    x.StartDate ==
                    SelectedDate.Value);
            }

            // SORT

            query = DynamicQueryService
                .ApplySorting(
                    query,
                    SortColumn,
                    SortDirection);

            return query;
        }
    }
}