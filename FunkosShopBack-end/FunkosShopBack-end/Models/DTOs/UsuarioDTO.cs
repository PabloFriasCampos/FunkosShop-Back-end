namespace FunkosShopBack_end.Models.DTOs
{
    public class UsuarioDTO
    {
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
    }
}
