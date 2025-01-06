using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BasketBallLiveScore.Server.Hub;

var builder = WebApplication.CreateBuilder(args);

// Ajouter la configuration pour g�rer les cycles dans la s�rialisation JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;  // D�sactive les r�f�rences circulaires
    });

// Ajouter SignalR
builder.Services.AddSignalR();

// Ajouter Swagger et la configuration JWT pour Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
        });
});

// Configuration JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true, // identifier les parties
                ValidateAudience = true, // empecher les attaques par rejeu => un site ne peut pas rejouer un token sur un autre site
                ValidateLifetime = true, // limiter la dur�e de vie di token
                ValidateIssuerSigningKey = true, // demande l'utilisation de signature
                ValidIssuer = builder.Configuration["Jwt:Issuer"], // emetteur du token
                ValidAudience = builder.Configuration["Jwt:Audience"], // receveur du token
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
                ClockSkew = TimeSpan.Zero // par d�faut c'est 5 minutes. Il s'agit de l'�cart de temps autoris� entre l'heure du client et celle du serveur

            };
        });

// Ajouter les services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddScoped<FoulService>();
builder.Services.AddScoped<ScoreService>();
builder.Services.AddScoped<TimerService>();

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient",
        policy => policy.WithOrigins("https://localhost:4200") // L'URL de votre application Angular
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// Configuration du DbContext avec la cha�ne de connexion
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));



// Configuration des contr�leurs
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin","User")); // Permet l'acc�s aux deux r�les
});


var app = builder.Build();

// Ajout de l'authentification au pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MatchHub>("Hub/matchHub");

// Configuration du pipeline de requ�tes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Utiliser la politique CORS "AllowAngularDevClient"
app.UseCors("AllowAngularDevClient");

app.UseHttpsRedirection();

// Mappe les contr�leurs
app.MapControllers();

app.Run();
