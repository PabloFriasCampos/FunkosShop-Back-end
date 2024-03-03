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
using Microsoft.AspNetCore.Authorization;


namespace FunkosShopBack_end.Controllers
{
    [Authorize(Roles = "ADMIN,USUARIO")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private DBContext _dbContext;
        // Se obtiene por inyecciom los parametros preestablecidos para crear los token
        private readonly TokenValidationParameters _tokenParameters;

        public UsuariosController(DBContext dbContext, IOptionsMonitor<JwtBearerOptions> jwtOptions)
        {
            _dbContext = dbContext;
            _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme)
                .TokenValidationParameters;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            bool encontrado = false;
            UsuarioDTO usuarioDTO = new UsuarioDTO();
            Usuario usuario = _dbContext.Usuarios.Find(id);
           if(usuario != null)
            {
                usuarioDTO.UsuarioID = usuario.UsuarioID;
                usuarioDTO.NombreUsuario = usuario.NombreUsuario;
                usuarioDTO.Direccion = usuario.Direccion;
                usuarioDTO.Correo = usuario.Correo;
                usuarioDTO.Rol = usuario.Rol;
                encontrado = true;
            }
            return encontrado ? Ok(usuarioDTO) : BadRequest();
        }
        
        [AllowAnonymous]
        [HttpPost("signup")]
        public IActionResult RegistrarUsuario([FromBody] UsuarioDTO usuario)
        {

            bool resultado = _dbContext.RegistrarUsuario(new Usuario
            {
                NombreUsuario = usuario.NombreUsuario,
                Direccion = usuario.Direccion,
                Correo = usuario.Correo.ToLower(),
                Contrasena = PasswordHelper.Hash(usuario.Contrasena),
                Rol = "USUARIO",
            });
            return resultado ? Ok() : BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult IniciarSesion([FromBody] UsuarioDTO usuarioDTO)
        {
            Usuario usuario = _dbContext.AutenticarUsuario(usuarioDTO.Correo.ToLower(), PasswordHelper.Hash(usuarioDTO.Contrasena));

            if(usuario != null)
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // Aqui se anade los datos para autorizar al usuario
                    Claims = new Dictionary<string, object>
                    {
                        { "id", usuario.UsuarioID },
                        { ClaimTypes.Role, usuario.Rol}
                    },
                    // Aqui indicamos cuando cu�ndo caduca el token
                    Expires = DateTime.UtcNow.AddDays(30),
                    // Aqui especificamos nuestra clave y el algoritmo de firmado
                    SigningCredentials = new SigningCredentials(
                        _tokenParameters.IssuerSigningKey,
                        SecurityAlgorithms.HmacSha256Signature)
                };
                // Creamos el token y se lo devolvemos al usuario loggeado
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(tokenHandler.WriteToken(token));
            }
            return Unauthorized();
        }
        
        [HttpPut("modifyUser/{id}")]
        public IActionResult ModifyUser([FromBody] UsuarioDTO usuario, int id)
        {
            bool modificado = _dbContext.ModificarUsuario(usuario, id);
            return modificado ? Ok() : BadRequest();
        }
    }
}
