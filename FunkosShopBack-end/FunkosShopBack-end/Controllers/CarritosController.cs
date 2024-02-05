using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

      /*  [HttpPost("{idUser}")]
        public ActionResult<Carrito> CreaCarrito(int idUser)
        {
            Carrito carrito = new Carrito();

            carrito.Usuario = _dbContext.Usuarios.Where(p => p.UsuarioId == idUser).First();

            carrito.TotalCarritoEUR = 15;

            _dbContext.Carritos.Add(carrito);
            
            _dbContext.SaveChanges();
            return carrito == null ? NotFound() : carrito;
        }
      */
        /*

        [HttpGet("{id}")]
        public ActionResult<ListaProductosCarrito> DetalleListaProductoCarrito(int id)
        {
            ListaProductosCarrito lista = _dbContext.ListaProductosCarrito.Find(id);
            return lista == null ? NotFound() : lista;
        }*/



        [HttpPost("{productoID}")]
        public ActionResult<ListaProductosCarrito> AgregaListaProductoCarrito(int productoID, int carritoID, int cantidadProducto)
        {
            
            var carrito = _dbContext.Carritos.Find(carritoID);

            var productoToAgregar = _dbContext.Productos.Find(productoID);

            ListaProductosCarrito listaProductosCarrito = new ListaProductosCarrito();

            listaProductosCarrito.Carrito = carrito;

            listaProductosCarrito.Producto = productoToAgregar;

            listaProductosCarrito.CantidadProducto = 5;

            listaProductosCarrito.TotalProductoEUR = 10;

            _dbContext.ListaProductosCarrito.Add(listaProductosCarrito);

            _dbContext.SaveChanges();

            return listaProductosCarrito == null ? NotFound() : listaProductosCarrito;
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
