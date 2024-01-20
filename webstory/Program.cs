using System.Text;
using Azure.Core;
using DataAccess.AutoMapper;
using DataAccess.Data;
using DataAccess.Middleware;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.Services;
using DataAccess.Services.IServices;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Utility;

var builder = WebApplication.CreateBuilder(args);

// Auto Mapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add services to the container.
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IChineseBookRepository, ChineseBookRepository>();
builder.Services.AddScoped<IChineseBookService, ChineseBookService>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<IChapterService, ChapterService>();

builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IGenreService, GenreService>();

builder.Services.AddScoped<IGenreBookRepository, GenreBookRepository>();

builder.Services.AddScoped<ICrawlingService, CrawlingService>();

builder.Services.AddScoped<IBookTagRepository, BookTagRepository>();
builder.Services.AddScoped<IBookBookTagRepository, BookBookTagRepository>();

builder.Services.AddScoped<IBookReadingRepository, BookReadingRepository>();
builder.Services.AddScoped<IBookReadingService, BookReadingService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();


// sql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}, ServiceLifetime.Scoped);


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebBook API", Version = "v1" });

    // Cấu hình Swagger để sử dụng Bearer Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Định nghĩa chính sách ủy quyền
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(SD.ADMIN, policy => policy.RequireRole(SD.ADMIN));
    options.AddPolicy(SD.MOD, policy => policy.RequireRole(SD.MOD));
    options.AddPolicy(SD.CONVERT, policy => policy.RequireRole(SD.CONVERT));
    options.AddPolicy(SD.AUTHOR, policy => policy.RequireRole(SD.AUTHOR));
    options.AddPolicy(SD.USER, policy => policy.RequireRole(SD.USER));
});

// Views book
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "BookViews";
});

// encoding
builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
});

// https
//builder.Services.AddHttpsRedirection(options =>
//{
//    options.HttpsPort = 443; // Port mặc định cho HTTPS
//});

// CRFS
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});

//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add(new ValidateAntiForgeryTokenAttribute());
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebBook API V1");

        // Cấu hình Swagger UI để sử dụng Bearer Authentication
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Swagger UI - WebBook API";
        c.EnableDeepLinking();
        c.DisplayOperationId();
        c.DefaultModelExpandDepth(0);
        c.DefaultModelRendering(ModelRendering.Model);
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
        c.DocExpansion(DocExpansion.None);
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
        c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete);
    });
}

// CRFS_TOKEN
//var antiforgery = app.Services.GetRequiredService<IAntiforgery>();

//app.Use((context, next) =>
//{
//    var requestPath = context.Request.Path.Value;

//    //if (string.Equals(requestPath, "/api/**", StringComparison.OrdinalIgnoreCase)
//    //    || string.Equals(requestPath, "/index.html", StringComparison.OrdinalIgnoreCase))
//    //{
//    var tokenSet = antiforgery.GetAndStoreTokens(context);
//    context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!,
//        new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.None });
//    //}

//    return next(context);
//});

//app.Use(async (context, next) =>
//{
//    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

//    var tokens = antiforgery.GetAndStoreTokens(context);

//    // Lưu Request token vào session
//    context.Session.SetString("XSRF-TOKEN", tokens.RequestToken!);

//    await next();
//});

//app.Use(async (context, next) =>
//{
//    if (context.Request.Headers.ContainsKey("X-App-Source"))
//    {
//        var antiforgery = context.RequestServices.GetService<IAntiforgery>();
//        var tokens = antiforgery.GetAndStoreTokens(context);
//        Console.WriteLine("Xin chao");
//        // Loại bỏ token từ yêu cầu
//        context.Request.Headers.Remove("X-CSRF-TOKEN");
//    }

//    await next();
//});


// Thêm middleware chuyển hướng từ HTTP sang HTTPS
//app.Use(async (context, next) =>
//{
//    // Nếu không phải là request HTTPS
//    if (!context.Request.IsHttps)
//    {
//        var withHttps = "https://" + context.Request.Host + context.Request.Path;
//        context.Response.Redirect(withHttps);
//    }
//    else
//    {
//        // Nếu là request HTTPS, tiếp tục xử lý bình thường
//        await next();
//    }
//});

// cors
app.UseCors(builder => builder
    .WithOrigins("https://truyenmoi.click",
                    "https://www.truyenmoi.click",
                     "https://api-url.truyenmoi.click",
                      "https://api.truyenmoi.click",
                      "http://localhost:4200",
                      "http://127.0.0.1:8000",
                    "https://webbookangular-git-main-trongthuong96s-projects-80c5b89c.vercel.app")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

// Thêm middleware vào pipeline
app.UseMiddleware<ViewsCounterMiddleware>();
app.UseMiddleware<SignatureVerificationMiddleware>();
//app.UseHeaderCheck();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
