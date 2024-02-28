using FunkosShopBack_end.Models.Entities;
using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FunkosShopBack_end.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FunkosShopBack_end.Controllers
{
    [Authorize(Roles = "USUARIO,ADMIN")]
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
        public async Task<CarritoDTO> DetalleCarritoAsync(int idCarrito)
        {
            Carrito carrito = await _dbContext.Carritos.Include(carrito => carrito.ListaProductosCarrito).ThenInclude(listaProductos => listaProductos.Producto)
                .FirstOrDefaultAsync(carrito => carrito.CarritoID == idCarrito);

            double totalCarrito = 0;

            foreach(ProductoCarrito producto in carrito.ListaProductosCarrito)
            {
                totalCarrito += producto.TotalProductoEUR;
            }

            CarritoDTO carritoDTO = new CarritoDTO
            {
                TotalCarritoEUR = totalCarrito,
                ListaProductosCarrito = carrito.ListaProductosCarrito
            };

            return carritoDTO;
        }

        [HttpGet("total/{idCarrito}")]
        public async Task<int> TotalProductosCarrito(int idCarrito)
        {
            Carrito carrito = await _dbContext.Carritos.Include(carrito => carrito.ListaProductosCarrito).ThenInclude(listaProductos => listaProductos.Producto)
                .FirstOrDefaultAsync(carrito => carrito.CarritoID == idCarrito);

            int totalCarrito = 0;

            foreach (ProductoCarrito producto in carrito.ListaProductosCarrito)
            {
                totalCarrito += producto.CantidadProducto;
            }

            return totalCarrito;
        }
    }
}