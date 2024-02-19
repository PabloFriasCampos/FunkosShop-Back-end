namespace FunkosShopBack_end.Models.Entities
{
    public class ProductoPedido
    {
        public int ProductoPedidoID { get; set; }
        public int ProductoID { get; set; }
        public int PedidoID { get; set; }
        public int CantidadProducto { get; set; }
        public decimal TotalProductoEUR { get; set; }

        public Producto Producto { get; set; }
        public Pedido Pedido { get; set; }
    }
}
