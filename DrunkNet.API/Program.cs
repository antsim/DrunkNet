using DrunkNet.API.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using DrunkNet.API.Extensions;
using DrunkNet.API.Models;
using DrunkNet.API.Request;
using DrunkNet.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

const string root = "/api";
var builder = WebApplication.CreateBuilder(args);
var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = domain;
        options.Audience = builder.Configuration["Auth0:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = builder.Configuration["Auth0:Audience"],
            ValidIssuer = domain
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:current_bac", policy =>
        policy.Requirements.Add(new HasScopeRequirement("read:current_bac", domain)));
    options.AddPolicy("read:profile", policy =>
        policy.Requirements.Add(new HasScopeRequirement("read:profile", domain)));
    options.AddPolicy("write:profile", policy =>
        policy.Requirements.Add(new HasScopeRequirement("write:profile", domain)));
    options.AddPolicy("write:drink", policy =>
        policy.Requirements.Add(new HasScopeRequirement("write:drink", domain)));
    options.AddPolicy("read:drinks", policy =>
        policy.Requirements.Add(new HasScopeRequirement("read:drinks", domain)));
    options.AddPolicy("read:toplist", policy =>
        policy.Requirements.Add(new HasScopeRequirement("read:toplist", domain)));
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IDrinkService, DrinkService>();
builder.Services.AddTransient<IClaimsTransformation, AuthEnhancerClaimsTransformation>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet($"{root}/bac", (ClaimsPrincipal user, [FromServices]IDrinkService drinkService) =>
    drinkService.GetBac(user.GetInternalUserId(), new List<Consumption>())).RequireAuthorization();

app.MapGet($"{root}/profile", (ClaimsPrincipal user, [FromServices]IUserService userService) => 
    userService.GetProfile(user.GetInternalUserId())).RequireAuthorization();

app.MapPost($"{root}/profile", ([FromBody]User user, ClaimsPrincipal principal, [FromServices]IUserService userService) => 
    userService.UpdateProfile(user, principal.GetInternalUserId())).RequireAuthorization();

app.MapPost($"{root}/drink", ([FromBody]DrinkRequest request, ClaimsPrincipal principal, [FromServices]IDrinkService drinkService) =>
    drinkService.AddDrink(request, principal.GetInternalUserId())).RequireAuthorization();

app.MapDelete($"{root}/drink/{{id:int}}", (ClaimsPrincipal principal, [FromServices]IDrinkService drinkService, int id) =>
    drinkService.DeleteDrink(id, principal.GetInternalUserId())).RequireAuthorization();

app.MapGet($"{root}/bachistory", (ClaimsPrincipal user, [FromServices] IDrinkService drinkService) =>
    drinkService.GetBacHistory(user.GetInternalUserId())).RequireAuthorization();

app.MapGet($"{root}/drinkList", (ClaimsPrincipal user, [FromServices] IDrinkService drinkService) =>
    drinkService.GetDrinkList(user.GetInternalUserId())).RequireAuthorization();

app.MapGet($"{root}/toplist", ([FromServices] IDrinkService drinkService) =>
    drinkService.GetTopList()).RequireAuthorization();

app.Run();