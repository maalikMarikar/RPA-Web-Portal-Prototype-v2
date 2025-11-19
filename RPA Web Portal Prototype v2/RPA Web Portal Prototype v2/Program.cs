using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RPA_Web_Portal_Prototype_v2.Model;
using RPA_Web_Portal_Prototype_v2.Repositories;
using RPA_Web_Portal_Prototype_v2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<PasswordHasher<User>>();
builder.Services.AddScoped<ThePasswordHasher>();
builder.Services.AddScoped<MySqlRepository>();

//jwt//

var jwtKey = builder.Configuration["Jwt:Key"]??"";
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKeyEncrypted = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;

    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = jwtKeyEncrypted,
        
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = con =>
        {
            if (con.Request.Cookies.ContainsKey("jwt"))
            {
                con.Token = con.Request.Cookies["jwt"];
            }

            return Task.CompletedTask;
        }
    };

});
//jwt//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=UserLogin}/{id?}")
    .WithStaticAssets();


app.Run();