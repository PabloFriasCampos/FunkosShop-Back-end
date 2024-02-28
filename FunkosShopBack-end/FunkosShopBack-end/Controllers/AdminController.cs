using FunkosShopBack_end.Models;
using FunkosShopBack_end.Models.DTOs;
using FunkosShopBack_end.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FunkosShopBack_end.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private DBContext _dbContext;

        public AdminController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("newProduct")]
        public IActionResult NewProduct([FromBody] Producto producto)
        {
            bool resultado = false;
            _dbContext.Productos.Add(producto);
           int rows = _dbContext.SaveChanges();
            if(rows==1) { resultado = true; }
            
            return resultado ? Ok() : BadRequest();
        }

        
        [HttpGet("listProducts")]
        public ICollection<Producto> listProducts()
        {
            return _dbContext.Productos.ToList();
        }

        [HttpPut("modifyProduct/{id}")]
        public IActionResult modifyProduct([FromBody] Producto producto, int id)
        {
            bool resultado = _dbContext.ModificarProducto(producto, id);

            return resultado ? Ok() : BadRequest();
        }

        [HttpPut("modifyUserRole/{id}")]
        public IActionResult ModifyUser(int id, [FromBody] string newRole)
        {

            bool modificado = _dbContext.ModifyUserRole(newRole, id);
            return modificado ? Ok() : BadRequest();
        }
        [HttpGet("getUsers")]
        public ICollection<UsuarioDTO> GetUsers()
        {
            ICollection<Usuario> listaUsuarios = _dbContext.Usuarios.ToList();

            ICollection<UsuarioDTO> listaDTO = [];
            foreach (Usuario usuario in listaUsuarios)
            {
                UsuarioDTO usuarioDTO = new UsuarioDTO
                {
                    UsuarioID = usuario.UsuarioID,
                    NombreUsuario = usuario.NombreUsuario,
                    Direccion = usuario.Direccion,
                    Correo = usuario.Correo,
                    Rol = usuario.Rol
                };
                listaDTO.Add(usuarioDTO);
            }
            return listaDTO;
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

            return borrado ? Ok() : BadRequest();
        }

    }
}
