using EntryIt.Services;
using EntryIt.Data;
using Microsoft.Extensions.Logging;
using Blazor.Sonner.Extensions;
using QuestPDF.Infrastructure;

namespace EntryIt
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Configure QuestPDF license for PDF export functionality
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            // ✅ Register your AppDbContext for DI
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Services.AddSonner();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<ILoggerService, LoggerService>();
            builder.Services.AddScoped<IJournalService, JournalService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddScoped<IMoodService, MoodService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IStreakService, StreakService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
    		builder.Logging.AddDebug();
#endif
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            return app;
        }
    }
}
