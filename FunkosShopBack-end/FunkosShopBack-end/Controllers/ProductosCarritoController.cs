using FunkosShopBack_end.Models.Entities;
using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Mvc;

namespace FunkosShopBack_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosCarritoController : ControllerBase
    {

        private DBContext _dbContext;

        public ProductosCarritoController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("{productoID}/{carritoID}/{cantidadProducto}")]
        public void AgregaProductoACarrito(int productoID, int carritoID, int cantidadProducto)
        {

            Carrito carrito = _dbContext.Carritos.Find(carritoID);

            Producto productoToAgregar = _dbContext.Productos.Find(productoID);

            if (_dbContext.productoYaEnCarrito(productoID, carritoID))
            {
                ProductoCarrito productoCarrito = new ProductoCarrito
                {
                    CarritoID = carrito.CarritoID,
                    ProductoID = productoToAgregar.ProductoID,
                    CantidadProducto = cantidadProducto,
                    TotalProductoEUR = (double)(cantidadProducto * productoToAgregar.PrecioEUR)
                };

                _dbContext.RegistraListaProductoCarrito(productoCarrito);

                carrito.ListaProductosCarrito.Add(productoCarrito);
                _dbContext.SaveChanges();

            }
            else
            {
                _dbContext.modificarCantidad(productoID, carritoID, cantidadProducto);
            }
        }
    }
}
