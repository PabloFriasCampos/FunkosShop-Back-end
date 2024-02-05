using Microsoft.EntityFrameworkCore;

namespace FunkosShopBack_end.Models.Entities
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        public DateTime FechaPedido { get; set; }
        public double TotalPedidoEUR { get; set; }
        public double TotalPedidoETH { get; set; }
        public bool Pagado { get; set; }
        public string HashTransaccion { get; set; }
        public string WalletCliente { get; set; }
        public ICollection<ListaProductosPedido> Productos { get; set; }
        public Usuario Usuario { get; set; }

    }
}