using FunkosShopBack_end.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FunkosShopBack_end.Models.DTOs;
using FunkosShopBack_end.Models.Entities;
using FunkosShopBack_end.Models;


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
        public void RegistrarUsuario([FromBody] UsuarioDTO usuarioDTO)
        {

            _dbContext.RegistrarUsuario(new Usuario
            {
                NombreUsuario = usuarioDTO.NombreUsuario,
                Direccion = usuarioDTO.Direccion,
                Correo = usuarioDTO.Correo,
                Contrasena = PasswordHelper.Hash(usuarioDTO.Contrasena),
                Rol = "USUARIO",
            });
        }

        [HttpPost("login")]
        public IActionResult IniciarSesion([FromBody] UsuarioDTO usuarioDTO)
        {

            if(_dbContext.AutenticarUsuario(usuarioDTO.Correo, PasswordHelper.Hash(usuarioDTO.Contrasena)))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // Aqui se anade los datos para autorizar al usuario
                    Claims = new Dictionary<string, object>
                    {
                        { "id", Guid.NewGuid().ToString() },
                        { ClaimTypes.Role, "USUARIO" }
                    },
                    // Aqui indicamos cuando cu�ndo caduca el token
                    Expires = DateTime.UtcNow.AddDays(30),
                    // Aqui especificamos nuestra clave y el algoritmo de firmado
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
