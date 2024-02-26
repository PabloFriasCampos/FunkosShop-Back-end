using FunkosShopBack_end.Models;
using FunkosShopBack_end.Models.DTOs;
using FunkosShopBack_end.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FunkosShopBack_end.Controllers
{
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
            ICollection<Producto> listaProductos = _dbContext.Productos.ToList();
            return listaProductos;
        }

        [HttpPut("modifyProduct{id}")]
        public IActionResult modifyProduct([FromBody] Producto producto, int id)
        {
            bool resultado = _dbContext.ModificarProducto(producto, id);

            return resultado ? Ok() : BadRequest();
        }

        [HttpPut("modifyUserRole/{id}")]
        public IActionResult ModifyUser([FromBody] string newRole, int id)
        {

            bool modificado = _dbContext.ModifyUserRole(newRole, id);
            return modificado ? Ok("Usuario modificado") : BadRequest("No existe dicho usuario");
        }

    }
}
