
using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;


namespace FunkosShopBack_end
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            // Configuramos para que el directorio de trabajo sea donde está el ejecutable
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                BearerFormat = "JWT",
                                Name = "Authorization",
                                Description = "qwertyuiopasdfijasengiouasengpizjfshgs510g65sd0h6g2x0s6xyv0bdt16ghjklñzxcvbnm",
                                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                                Scheme = JwtBearerDefaults.AuthenticationScheme
                            });
                options.OperationFilter<SecurityRequirementsOperationFilter>(true, JwtBearerDefaults.AuthenticationScheme);
            }) ;
            
                
            

            builder.Services.AddScoped<DBContext>();
            builder.Services.AddTransient<DBSeeder>();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
                });
            }

            builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    // Por seguridad guardamos la clave privada en variables de entorno
                    string key = Environment.GetEnvironmentVariable("JWT_KEY");

                    options.TokenValidationParameters = new TokenValidationParameters() {
                        // Que se valide o no el emisor del token
                        ValidateIssuer = false,
                        // Que se valide para quién o para que propósito está destinado el token
                        ValidateAudience = false,
                        // Indicamos la clave
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

            var app = builder.Build();

            /*using (IServiceScope scope = app.Services.CreateScope())
            {
                DBContext dbContext = scope.ServiceProvider.GetService<DBContext>();
                dbContext.Database.EnsureCreated();
            }*/

            // Si no está creada la base de datos y la creamos y rellenamos 
            using (IServiceScope scope = app.Services.CreateScope())
            {
                DBSeeder dbSeeder = scope.ServiceProvider.GetService<DBSeeder>();
                await dbSeeder.SeedAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseCors();
            }

            app.UseHttpsRedirection();

            // Habilita la autenticación
            app.UseAuthentication();
            // Habilita la autorización
            app.UseAuthorization();

            // Habilitamos el uso de archivos estáticos
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();

            
        }
    }
}
