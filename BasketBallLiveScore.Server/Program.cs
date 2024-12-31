using Microsoft.EntityFrameworkCore;
using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Ajoute le service AuthService
builder.Services.AddScoped<AuthService>();

// Ajoute le service TeamService
builder.Services.AddScoped<TeamService>();

// Ajoute le service MatchService
builder.Services.AddScoped<MatchService>();

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Permet les requêtes de n'importe quelle origine
              .AllowAnyMethod()  // Permet toutes les méthodes HTTP (GET, POST, etc.)
              .AllowAnyHeader(); // Permet tous les en-têtes
    });
});

// Configuration du DbContext avec la chaîne de connexion
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Ajoute la configuration des contrôleurs
builder.Services.AddControllersWithViews();

// Services supplémentaires
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuration du pipeline de requêtes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Utilise la politique CORS "AllowAll"
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();

// Mappe les contrôleurs
app.MapControllers(); // Point d'entrée pour l'API

// Fallback pour une application SPA (comme Angular ou React)
app.MapFallbackToFile("/index.html");

app.Run();
