namespace FunkosShopBack_end.Models.Entities
{
    public class Pedido
    {
        public int PedidoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaPedido { get; set; }
        public double TotalPedidoEUR { get; set; }
        public double TotalPedidoETH { get; set; }
        public string Value { get; set; } 
        public string WalletCliente { get; set; }
        public string HashTransaccion { get; set; }
        public bool Pagado { get; set; }


        public Usuario Usuario { get; set; }
        public ICollection<ProductoPedido> ListaProductosPedido { get; set; }

    }
}