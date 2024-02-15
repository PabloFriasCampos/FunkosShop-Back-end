using FunkosShopBack_end.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Recursos;

namespace FunkosShopBack_end.Models
{
    public class DBContext : DbContext
    {
        private const string DATABASE_PATH = "funkos.db";

        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ProductoCarrito> ListaProductosCarrito { get; set; }
        public DbSet<ProductoPedido> ListaProductosPedido { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            options.UseSqlite($"DataSource={baseDir}{DATABASE_PATH}");
        }

        public bool RegistrarUsuario(Usuario usuario)
        {
            bool guardado = false;
            Carrito carrito = new Carrito();
            carrito.Usuario = usuario;
            if(Usuarios.FirstOrDefault(usuarioin => usuarioin.Correo == usuario.Correo 
            || usuarioin.NombreUsuario == usuario.NombreUsuario) == null)
            {
                Usuarios.Add(usuario);
                Carritos.Add(carrito);
                SaveChanges();
                guardado = true;
            }

            return guardado;
        }

        public void RegistrarProducto(Producto producto)
        {
            Productos.Add(producto);
            SaveChanges();
        }

        public int AutenticarUsuario(string correo, string contrasena)
        {
            Usuario usuario = Usuarios.FirstOrDefault(usuario => usuario.Correo == correo && usuario.Contrasena == contrasena);

            if(usuario != null)
            {
                return usuario.UsuarioID;
            }

            return -1;
        }

        public void RegistraListaProductoCarrito(ProductoCarrito productoCarrito)
        {
            ListaProductosCarrito.Add(productoCarrito);
            SaveChanges();
        }

        public void modificarCantidad(int productoID, int carritoID, int cantidad)
        {
            var listaProducto = ListaProductosCarrito.First(p => p.Producto.ProductoID == productoID && p.Carrito.CarritoID == carritoID);
            listaProducto.CantidadProducto += cantidad;
            
            if (listaProducto.CantidadProducto==0)
            {
                ListaProductosCarrito.Remove(listaProducto);
            }
            else
            {
                listaProducto.TotalProductoEUR = listaProducto.CantidadProducto * listaProducto.Producto.PrecioEUR;
            }
            
            SaveChanges();
        }

        public bool productoYaEnCarrito(int productoID, int carritoID)
        {
            var listaExiste = ListaProductosCarrito.Where(p => p.Producto.ProductoID == productoID && p.Carrito.CarritoID == carritoID).ToList();
            
            return listaExiste.IsNullOrEmpty();
        }

    }
}
