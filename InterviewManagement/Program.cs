using InterviewManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<InterviewManagementContext>();
InterviewManagementContext interviewManagementContext = new InterviewManagementContext();


// Add cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Index";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        option.SlidingExpiration = true;
    });
//policy
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    option.AddPolicy("Interviewer", policy => policy.RequireRole("Interviewer"));
    option.AddPolicy("Recruiter", policy => policy.RequireRole("Recruiter"));
    option.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
    option.AddPolicy("Employee", policy => policy.RequireRole("Admin","Interviewer","Recruiter","Manager"));
    option.AddPolicy("Offer", policy => policy.RequireRole("Admin", "Recruiter", "Manager"));
    option.AddPolicy("Job", policy => policy.RequireRole("Admin", "Interviewer", "Recruiter", "Manager"));
});
// Register the EmailService as a transient service
builder.Services.AddTransient<EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{//
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add session middleware

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
