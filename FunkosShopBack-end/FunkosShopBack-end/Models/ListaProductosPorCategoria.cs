using FunkosShopBack_end.Models.Entities;

namespace FunkosShopBack_end.Models
{
    public class ListaProductosPorCategoria
    {
        public string Categoria { get; set; }
        public IEnumerable<Producto> Productos { get; set; }
    }
}
