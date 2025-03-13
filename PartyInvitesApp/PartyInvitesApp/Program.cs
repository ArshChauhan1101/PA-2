using Microsoft.EntityFrameworkCore;
using PartyInvitesApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseRouting();

#pragma warning disable 
app.UseEndpoints(endpoints =>
{
    // Ensure routes are correctly mapped
    endpoints.MapControllerRoute(
        name: "invitationResponse",
        pattern: "invitation/{partyId}/{guestEmail}",
        defaults: new { controller = "Invitation", action = "Response" });
});
#pragma warning restore 

app.Run();
