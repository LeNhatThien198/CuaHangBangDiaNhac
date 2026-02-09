using CuaHangBangDiaNhac.Cli;
using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Data.Seeders;
using CuaHangBangDiaNhac.ServicesRegisters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using CuaHangBangDiaNhac.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthServices(builder.Configuration);

builder.Services.AddRepositories();
builder.Services.AddBusinessServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        
        await DataSeeder.SeedDataAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (args.Length > 0)
{
    await CliHandler.Handle(args, app);
    return; 
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Must be before Authentication

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapHub<CuaHangBangDiaNhac.Hubs.TicketHub>("/ticketHub");
app.MapHub<CuaHangBangDiaNhac.Hubs.AuditLogHub>("/auditHub");
app.MapHub<CuaHangBangDiaNhac.Hubs.NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();