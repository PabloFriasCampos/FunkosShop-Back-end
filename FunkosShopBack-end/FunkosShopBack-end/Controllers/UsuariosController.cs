using FunkosShopBack_end.Resources;
using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;


namespace FunkosShopBack_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private DBContext _dbContext;
        // Se obtiene por inyecci�n los par�metros preestablecidos para crear los token
        private readonly TokenValidationParameters _tokenParameters;

        public UsuariosController(DBContext dbContext, IOptionsMonitor<JwtBearerOptions> jwtOptions)
        {
            _dbContext = dbContext;
            _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme)
                .TokenValidationParameters;
        }

        [HttpGet]
        public IEnumerable<Usuario> Get()
        {
            return _dbContext.Usuarios;
        }

        [HttpPost("signup")]
        public void RegistrarUsuario([FromBody] JsonElement datosUsuario)
        {

            _dbContext.RegistrarUsuario(new Usuario
            {
                NombreUsuario = datosUsuario.GetProperty("NombreUsuario").GetString(),
                Direccion = datosUsuario.GetProperty("Direccion").GetString(),
                Correo = datosUsuario.GetProperty("Correo").GetString(),
                Contrasena = PasswordHelper.Hash(datosUsuario.GetProperty("Contrasena").GetString()),
                Rol = "USUARIO",
            });
        }

        [HttpPost("login")]
        public IActionResult IniciarSesion([FromBody] JsonElement datosUsuario)
        {

            if(_dbContext.AutenticarUsuario(datosUsuario.GetProperty("NombreUsuario").GetString(), datosUsuario.GetProperty("Contrasena").GetString()))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // Aqui se a�ade los datos para autorizar al usuario
                    Claims = new Dictionary<string, object>
                    {
                        { "id", Guid.NewGuid().ToString() },
                        { ClaimTypes.Role, "USUARIO" }
                    },
                    // Aqu� indicamos cuando cu�ndo caduca el token
                    Expires = DateTime.UtcNow.AddDays(30),
                    // Aqu� especificamos nuestra clave y el algoritmo de firmado
                    SigningCredentials = new SigningCredentials(
                        _tokenParameters.IssuerSigningKey,
                        SecurityAlgorithms.HmacSha256Signature)
                };

                // Creamos el token y se lo devolvemos al usuario logeado
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                string stringToken = tokenHandler.WriteToken(token);

                return Ok(stringToken);
            }

            // Si el usuario no existe, lo indicamos
            return Unauthorized("Este usuario no existe");

        }

    }
}
