using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FunkosShopBack_end.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private DBContext _dbContext;

        public UsuarioController(DBContext dbContext)
        {
            _dbContext = dbContext;
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
                Contrasena = datosUsuario.GetProperty("Contrasena").GetString(),
                Rol = "USUARIO",
            });
        }

        [HttpPost("login")]
        public bool IniciarSesion([FromBody] JsonElement datosUsuario)
        {

            return _dbContext.AutenticarUsuario(datosUsuario.GetProperty("NombreUsuario").GetString(), datosUsuario.GetProperty("Contrasena").GetString());

        }

    }
}
