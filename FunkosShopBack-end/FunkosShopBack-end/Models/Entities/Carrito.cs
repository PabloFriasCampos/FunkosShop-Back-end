namespace FunkosShopBack_end.Models.Entities
{
    public class Carrito
    {
        public int CarritoID { get; set; }
        public int UsuarioID { get; set; }
        public double TotalCarritoEUR { get; set; }

        public ICollection<ProductoCarrito> ListaProductosCarrito { get; set; } = [];
        public Usuario Usuario { get; set; }
    }
}