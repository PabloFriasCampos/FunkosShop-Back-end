namespace FunkosShopBack_end.Models.Entities
{
    public class ListaProductosPedido
    {
        public int ListaProductosPedidoID { get; set; }
        public Producto Producto { get; set; }
        public Pedido Pedido { get; set; }
        public int CantidadProducto { get; set; }
        public double TotalProductoEUR { get; set; }
    }
}
