using Microsoft.EntityFrameworkCore;
using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using COMP2139_assign01.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using COMP2139_assign01.Middleware;


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(),
        "logs/log-.json",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .WriteTo.File("logs/log-.txt",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog to the services collection
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<CartService>();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Add Identity with default token providers
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
        // Configure password requirements
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        
        // Configure user requirements
        options.User.RequireUniqueEmail = true;
        
        // Configure email confirmation
        options.SignIn.RequireConfirmedEmail = true;
        
        // Configure lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

<<<<<<< HEAD
    // Add DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
=======
//Authorization policies
builder.Services.AddAuthorization(options => {
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));
});

var app = builder.Build();
>>>>>>> 060479397516ee85824607d4f92fac7e29805831

    // Uncomment and implement email sender
    // builder.Services.AddTransient<IEmailSender, EmailSender>();

    // Authorization policies
    builder.Services.AddAuthorization(options => {
        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?statusCode={0}");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging(options =>
    {
        // Customize the message template
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        
        // Emit debug-level events instead of the defaults
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
        
        // Attach additional properties to the request completion event
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
            
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
            }
        };
    });

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    
    
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    app.UseAuthentication(); // Add this line - it was missing!
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
<<<<<<< HEAD
catch (Exception ex)
=======
//builder.Services.AddTransient<IEmailSender, EmailSender>();


if (app.Environment.IsDevelopment())
>>>>>>> 060479397516ee85824607d4f92fac7e29805831
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
<<<<<<< HEAD
    Log.CloseAndFlush();
}
=======
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?statusCode={0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
>>>>>>> 060479397516ee85824607d4f92fac7e29805831
