using ACE_MVC.DataService;
using ACE_MVC.ChatHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // Add SignalR service

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor(); // Needed for accessing HttpContext

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5205); // Or your desired port
});
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
// Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Check if the connection string is valid
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("The connection string 'DefaultConnection' is missing.");
}

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
    });

// Register UserService with dependency injection
builder.Services.AddScoped<UserService>(provider => new UserService(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapHub<ChatHub>("/chatHub");
// Map the SignalR hub route

app.Run();