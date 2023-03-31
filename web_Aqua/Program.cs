using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using web_Aqua.Context;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<db_aquaponicsContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("db_aquaponics")))  ;
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/Home/Login";
    option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
});
builder.Services.AddAuthentication()
	.AddGoogle(googleOptions =>
	{
		// Đọc thông tin Authentication:Google từ appsettings.json
		IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

		// Thiết lập ClientID và ClientSecret để truy cập API google
		googleOptions.ClientId = googleAuthNSection["ClientId"];
		googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
		// Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
		googleOptions.CallbackPath = "/google";

	});


builder.Services.AddSession(option =>
{
    option.Cookie.Name = "cart";
    option.IdleTimeout = TimeSpan.FromMinutes(120);
});

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
app.UseSession();
app.UseRouting();
app.UseAuthentication();


app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");

	

});



app.Run();

