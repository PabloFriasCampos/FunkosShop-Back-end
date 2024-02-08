using FunkosShopBack_end.Models.Entities;
using FunkosShopBack_end.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FunkosShopBack_end.Models.DTOs;

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
        public async Task<CarritoDTO> DetalleCarritoAsync(int idCarrito)
        {
            Carrito carrito = await _dbContext.Carritos.Include(carrito => carrito.ListaProductosCarrito).ThenInclude(listaProductos => listaProductos.Producto)
                .FirstOrDefaultAsync(carrito => carrito.CarritoID == idCarrito);

            CarritoDTO carritoDTO = new CarritoDTO
            {
                TotalCarritoEUR = carrito.TotalCarritoEUR,
                ListaProductosCarrito = carrito.ListaProductosCarrito
            };

            return carritoDTO;
        }
    }
}