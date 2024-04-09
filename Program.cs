using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Services;
using WypozyczeniaAPI.Prof;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DBContextConnection") ?? throw new InvalidOperationException("Connection string 'DBContextConnection' not found.");

builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DBContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IPojazdService, PojazdService>();
builder.Services.AddScoped<IRezerwacjaService, RezerwacjaService>();
builder.Services.AddScoped<IWypozyczenieService, WypozyczenieService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddAutoMapper(typeof(WypozyczalniaProfil));

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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DBContext>();
    context.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User", "Employee" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<User>>();

    string email = "admin@admin.com";
    string passwd = "zaq1@WSX";

    if(await userManager.FindByEmailAsync(email) == null)
    {
        var user = new User();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;

        user.Imie = "Adam";
        user.Nazwisko = "Adamski";
        user.CVC = "123";
        user.NumerKarty = "123";

        var id = user.Id;

        await userManager.CreateAsync(user, passwd);
        await userManager.AddToRoleAsync(user, "Admin");
    }

    email = "Tomasz@Test.com";
    passwd = "zaq1@WSX";
    //imie: Tomasz Nazwisko: Testowy 
    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new User();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;

        user.Imie = "Tomasz";
        user.Nazwisko = "Test";
        user.CVC = "123";
        user.NumerKarty = "123";

        await userManager.CreateAsync(user, passwd);
        await userManager.AddToRoleAsync(user, "User");
    }
    /*
    email = "Piotrek@Praco.com";
    var user1 = await userManager.FindByEmailAsync(email);
    await userManager.AddToRoleAsync(user1, "Employee");
    */

    email = "Piotrek@Praco.com";
    passwd = "zaq1@WSX";
    //imie: Piotrek Nazwisko: Pracowity 
    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new User();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;

        user.Imie = "Piotrek";
        user.Nazwisko = "Praco";
        user.CVC = "123";
        user.NumerKarty = "123";

        await userManager.CreateAsync(user, passwd);
        await userManager.AddToRoleAsync(user, "Employee");
    }

}

app.Run();
