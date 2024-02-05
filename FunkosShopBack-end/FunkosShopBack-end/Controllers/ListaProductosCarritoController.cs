using FunkosShopBack_end.Models.Entities;
using FunkosShopBack_end.Models;
using FunkosShopBack_end.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace FunkosShopBack_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListaProductosCarritoController : ControllerBase
    {

        private DBContext _dbContext;

        public ListaProductosCarritoController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*[HttpGet]
        public IEnumerable<ListaProductosCarrito> muestraTodos()
        {

            return _dbContext.ListaProductosCarrito;
        }*/

        [HttpPost("{productoID}/{carritoID}/{cantidadProducto}")]
        public void AgregaListaProductoCarrito(int productoID, int carritoID, int cantidadProducto)
        {

            var carrito = _dbContext.Carritos.Find(carritoID);

            var productoToAgregar = _dbContext.Productos.Find(productoID);

            if (_dbContext.compruebaExiste(productoID, carritoID))
            {
                _dbContext.RegistraListaProductoCarrito(new ListaProductosCarrito
                {
                    Carrito = carrito,
                    Producto = productoToAgregar,
                    CantidadProducto = cantidadProducto,
                    TotalProductoEUR = cantidadProducto * productoToAgregar.PrecioEUR
                });
            }
            else
            {
                _dbContext.modificarCantidad(productoID, carritoID, cantidadProducto);
            }
        }

       
        [HttpGet]
        public IEnumerable<ListaProductosCarrito> muestraTodo()
        {
            return _dbContext.ListaProductosCarrito;
        }
        
        
        
        /*
        [HttpGet("MuestraPorProductoID")]
        public IEnumerable<ListaProductosCarrito> NoExiste(int productoID, int carritoID)
        {
            var listaExiste = _dbContext.ListaProductosCarrito.Where(p=> p.Producto.ProductoId == productoID && p.Carrito.CarritoID==carritoID).ToList();

            return listaExiste;


        }*/




    }
}
