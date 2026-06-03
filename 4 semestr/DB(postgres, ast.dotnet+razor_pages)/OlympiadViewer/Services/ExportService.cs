using ClosedXML.Excel;
using System.Data;

namespace OlympiadViewer.Services
{
    public class ExportService
    {
        public byte[] ExportToExcel(DataTable table,string sheetName = "Report")
        {
            using var workbook = new XLWorkbook();

            var worksheet =
                workbook.Worksheets.Add(sheetName);

            // HEADERS

            for (int col = 0;col < table.Columns.Count;col++)
            {
                worksheet.Cell(1, col + 1).Value =table.Columns[col].ColumnName;

                worksheet.Cell(1, col + 1).Style.Font.Bold = true;
            }

            // DATA

            for (int row = 0;row < table.Rows.Count;row++)
            {
                for (int col = 0;col < table.Columns.Count;col++)
                {
                    worksheet.Cell( row + 2,col + 1).Value = table.Rows[row][col]?.ToString();
                }
            }

            // AUTO SIZE

            worksheet.Columns().AdjustToContents();
            using var stream =new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        public DataTable ConvertToDataTable<T>(IEnumerable<T> data)
        {
            DataTable table = new();

            var properties = typeof(T).GetProperties();

            // COLUMNS

            foreach (var property in properties)
            {
                Type columnType = Nullable.GetUnderlyingType( property.PropertyType) ?? property.PropertyType;

                table.Columns.Add(property.Name, columnType);
            }

            // ROWS

            foreach (var item in data)
            {
                var values = new object[properties.Length];

                for (int i = 0;  i < properties.Length; i++)
                {
                    values[i] = properties[i]  .GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(values);
            }

            return table;
        }
    }
}