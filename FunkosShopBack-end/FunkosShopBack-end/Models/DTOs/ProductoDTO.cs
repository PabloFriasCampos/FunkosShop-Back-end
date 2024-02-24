namespace FunkosShopBack_end.Models.DTOs
{
    public class ProductoDTO
    {
        public string NombreProducto { get; set; }
        public decimal PrecioEUR { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int Stock { get; set; }
    }
}
