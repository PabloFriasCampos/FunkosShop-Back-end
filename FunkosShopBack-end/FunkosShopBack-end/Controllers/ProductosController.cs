using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FunkosShopBack_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private DBContext _dbContext;

        public ProductosController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Producto> Get()
        {
            return _dbContext.Productos;
        }

        [HttpGet("{id}")]
        public ActionResult<Producto> DetalleProducto(int id)
        {
            Producto producto = _dbContext.Productos.Find(id);
            return producto == null ? NotFound() : producto;
        }

        [HttpPost]
        public void CrearProducto([FromBody] JsonElement datosProductos)
        {
            _dbContext.RegistrarProducto(new Producto
            {
                NombreProducto = "Sora",
                PrecioEUR = 3,
                Descripcion = "Kingdom Hearts",
                Categoria = "Anime",
                Stock = 4
            });
        }
    }
}
