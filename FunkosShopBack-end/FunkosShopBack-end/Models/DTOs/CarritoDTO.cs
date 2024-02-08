using FunkosShopBack_end.Models.Entities;

namespace FunkosShopBack_end.Models.DTOs
{
    public class CarritoDTO
    {
        public double TotalCarritoEUR { get; set; }

        public ICollection<ProductoCarrito> ListaProductosCarrito { get; set; }
    }
}
