using Microsoft.EntityFrameworkCore;

namespace FunkosShopBack_end.Models.Entities
{
    [Index(nameof(Correo), IsUnique = true)]
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }

        public ICollection<Pedido> Pedidos { get; set; }
    }
}