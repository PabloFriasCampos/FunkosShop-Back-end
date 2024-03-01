using FunkosShopBack_end.Models;
using FunkosShopBack_end.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<ListaProductosPorCategoria> Get()
        {
            return _dbContext.Productos.GroupBy(p => p.Categoria)
                .Select(g => new ListaProductosPorCategoria
                {
                    Categoria = g.Key,
                    Productos = g.ToList()
                });
        }

        [HttpGet("{id}")]
        public ActionResult<Producto> DetalleProducto(int id)
        {
            Producto producto = _dbContext.Productos.Find(id);
            return producto == null ? NotFound() : producto;
        }
    }
}
