using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

        [HttpGet]
        public IEnumerable<ListaProductosCarrito> muestraTodos()
        {

            return _dbContext.ListaProductosCarrito;
        }

        [HttpPost("{productoID}")]
        public void AgregaListaProductoCarrito(int productoID, int carritoID, int cantidadProducto)
        {

            var carrito = _dbContext.Carritos.Find(carritoID);

            var productoToAgregar = _dbContext.Productos.Find(productoID);

            _dbContext.RegistraListaProductoCarrito(new ListaProductosCarrito
            {
                Carrito = carrito,
                Producto = productoToAgregar,
                CantidadProducto = cantidadProducto,
                TotalProductoEUR = 66
            });
        }

       

    }
}
