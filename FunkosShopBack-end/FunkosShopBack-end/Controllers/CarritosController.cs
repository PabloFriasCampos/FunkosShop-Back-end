using FunkosShopBack_end.Models;
using FunkosShopBack_end.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FunkosShopBack_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritosController : ControllerBase
    {

        private DBContext _dbContext;

        public CarritosController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{idCarrito}")]   
        public ActionResult<Carrito> DetalleCarrito(int id)
        {
            Carrito carrito = _dbContext.Carritos.Find(id);
            return carrito == null ? NotFound() : carrito;
        }

        [HttpGet]
        public IEnumerable<Carrito> TodosCarritos()
        {
            return _dbContext.Carritos;
        }


       
        

        /*
        [HttpPost("agregaralcarrito")]
        public async Task<IActionResult> AgregarAlCarrito(int carritoId, int productoId, int cantidadProducto)
        {
            try
            {
                // Verificar si el carrito existe
                var carrito = await _dbContext.Carritos
                    .Include(c => c.Productos)
                    .FirstOrDefaultAsync(c => c.CarritoID == carritoId);

                if (carrito == null)
                {
                    return BadRequest("Carrito no encontrado");
                }

                // Verificar si el producto existe
                var producto = await _dbContext.Productos
                    .FirstOrDefaultAsync(p => p.ProductoId == productoId);

                if (producto == null)
                {
                    return BadRequest("Producto no encontrado");
                }

                // Crear un nuevo objeto ListaProductoCarrito
                var listaProductoCarrito = new ListaProductosCarrito
                {
                    Carrito = carrito,
                    Producto = producto,
                    CantidadProducto = cantidadProducto,
                    TotalProductoEUR = cantidadProducto * producto.PrecioEUR
                };

                // Actualizar el total del carrito
                carrito.TotalCarritoEUR += listaProductoCarrito.TotalProductoEUR;

                // Agregar el objeto ListaProductoCarrito al contexto y guardar los cambios en la base de datos
                _dbContext.ListaProductosCarrito.Add(listaProductoCarrito);
                await _dbContext.SaveChangesAsync();

                return Ok("Producto agregado al carrito exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }*/


    }
}
