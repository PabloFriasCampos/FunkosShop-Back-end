using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Mvc;

namespace FunkosShopBack_end.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestingController : ControllerBase
    {
        private DBContext _dbContext;

        public TestingController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Usuario> Get() 
        {
            return _dbContext.Usuarios;
        }

        [HttpPost]
        public void CreateUser()
        {
            _dbContext.Usuarios.Add(new Usuario
            {
                NombreUsuario="Pablo",
                Direccion="DireccionTest",
                Correo="@test",
                Contrasena="1234",
                Rol="ADMIN"
            });
            _dbContext.SaveChanges();
        }
    }
}
