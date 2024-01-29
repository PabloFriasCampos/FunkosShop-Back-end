namespace FunkosShopBack_end.Models
{
    public class Carrito
    {
        public int CarritoID { get; set; }
        public Usuario Usuario { get; set; }
        public double TotalCarritoEUR { get; set; }
        public ICollection<ListaProductosCarrito> Productos { get; set; }
    }
}