using BookSearchSystem.Application.Interfaces;
using BookSearchSystem.Application.Services;
using BookSearchSystem.Domain.Interfaces;
using BookSearchSystem.Infrastructure.Configuration;
using BookSearchSystem.Infrastructure.Data;
using BookSearchSystem.Infrastructure.ExternalServices;
using BookSearchSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// API Configuration
builder.Services.Configure<OpenLibraryApiSettings>(
    builder.Configuration.GetSection(OpenLibraryApiSettings.SectionName));

// HttpClient for external API
builder.Services.AddHttpClient<IBookSearchService, OpenLibraryService>();

// Repository Services
builder.Services.AddScoped<ISearchHistoryRepository, SearchHistoryRepository>();

// Application Services
builder.Services.AddScoped<IBookSearchApplicationService, BookSearchApplicationService>();
builder.Services.AddScoped<ISearchHistoryApplicationService, SearchHistoryApplicationService>();

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=BookSearch}/{action=Index}/{id?}");


app.Run();
