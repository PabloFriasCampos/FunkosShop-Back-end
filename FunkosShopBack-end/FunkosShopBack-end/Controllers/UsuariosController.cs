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
using Microsoft.VisualBasic;


namespace FunkosShopBack_end.Controllers
{
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
        public UsuarioDTO Get(int id)
        {
            Usuario usuario = _dbContext.Usuarios.Find(id);
            UsuarioDTO usuarioDTO = new UsuarioDTO
            {
                NombreUsuario = usuario.NombreUsuario,
                Direccion = usuario.Direccion,
                Correo = usuario.Correo,
                Rol = usuario.Rol
            };
            return usuarioDTO;
        }

        [HttpGet]
        public ICollection<UsuarioDTO> GetUsers()
        {
            ICollection<Usuario> listaUsuarios = _dbContext.Usuarios.ToList();

            ICollection<UsuarioDTO> listaDTO = [];
            foreach (Usuario usuario in listaUsuarios)
            {
                UsuarioDTO usuarioDTO = new UsuarioDTO
                {
                    NombreUsuario = usuario.NombreUsuario,
                    Direccion = usuario.Direccion,
                    Correo = usuario.Correo,
                    Rol = usuario.Rol
                };
                listaDTO.Add(usuarioDTO);
            }
            return listaDTO;
        }

        [HttpPut("modifyUserRole/{id}")]
        public IActionResult ModifyUser([FromBody] string newRole, int id)
        {

            bool modificado = _dbContext.ModificarUsuario(new Usuario
            {
                UsuarioID = id,
                Rol = newRole
            });
            return modificado ? Ok("Usuario modificado") : BadRequest("No existe dicho usuario");
        }

        [HttpDelete("deleteUser/{id}")]
        public IActionResult DeleteUser(int id) 
        {
            bool borrado = false;
            Usuario usuarioDel = _dbContext.Usuarios.FirstOrDefault(u => u.UsuarioID == id);
            if (usuarioDel != null)
            {
                _dbContext.Remove(usuarioDel);
                _dbContext.SaveChanges();
                borrado = true;
            }

            return borrado ? Ok("Usuario borrado") : BadRequest("Fallo en la operación de borrado");
        }

        [HttpPost("signup")]
        public IActionResult RegistrarUsuario([FromBody] UsuarioDTO usuario)
        {

            bool resultado = _dbContext.RegistrarUsuario(new Usuario
            {
                NombreUsuario = usuario.NombreUsuario,
                Direccion = usuario.Direccion,
                Correo = usuario.Correo,
                Contrasena = PasswordHelper.Hash(usuario.Contrasena),
                Rol = "USUARIO",
            });
            return resultado ? Ok("Registro completado") : BadRequest("Ya hay un usuario con ese correo y nombre de usuario");
        }

        [HttpPost("login")]
        public IActionResult IniciarSesion([FromBody] UsuarioDTO usuarioDTO)
        {
            int usuarioId = _dbContext.AutenticarUsuario(usuarioDTO.Correo, PasswordHelper.Hash(usuarioDTO.Contrasena));

            if(usuarioId != -1)
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
                stringToken += ";" + usuarioId;

                
                return Ok(stringToken);
            }

            // Si el usuario no existe, lo indicamos
            return Unauthorized();

        }

    }
}
