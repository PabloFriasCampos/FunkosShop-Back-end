namespace FunkosShopBack_end.Models.Entities
{
    public class ListaProductosCarrito
    {
        public int ListaProductosCarritoID { get; set; }
        public Carrito Carrito { get; set; }
        public Producto Producto { get; set; }
        public int CantidadProducto { get; set; }
        public double TotalProductoEUR { get; set; }

    }
}
