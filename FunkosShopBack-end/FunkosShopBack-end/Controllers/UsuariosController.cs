using FunkosShopBack_end.Resources;
using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace FunkosShopBack_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private DBContext _dbContext;

        public UsuariosController(DBContext dbContext)
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
                Contrasena = PasswordHelper.Hash(datosUsuario.GetProperty("Contrasena").GetString()),
                Rol = "USUARIO",
            });
        }

        [HttpPost("login")]
        public bool IniciarSesion([FromBody] JsonElement datosUsuario)
        {
            return _dbContext.AutenticarUsuario(datosUsuario.GetProperty("NombreUsuario").GetString(), PasswordHelper.Hash(datosUsuario.GetProperty("Contrasena").GetString()));

        }

    }
}
