namespace FunkosShopBack_end.Models.Entities
{
    public class ProductoCarrito
    {
        public int ProductoCarritoID { get; set; }
        public int CarritoID { get; set; }
        public int ProductoID { get; set; }
        public int CantidadProducto { get; set; }
        public double TotalProductoEUR { get; set; }

        public Carrito Carrito { get; set; }
        public Producto Producto { get; set; }

    }
}
