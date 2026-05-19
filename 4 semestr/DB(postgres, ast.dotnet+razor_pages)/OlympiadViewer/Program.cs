using Microsoft.EntityFrameworkCore;
using OlympiadViewer.Data;
using OlympiadViewer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OlympiadContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});
builder.Services.AddScoped<ExportService>();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();