using FunkosShopBack_end.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FunkosShopBack_end.Models
{
    public class DBContext : DbContext
    {
        private const string DATABASE_PATH = "funkos.db";

        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ListaProductosCarrito> ListaProductosCarrito { get; set; }
        public DbSet<ListaProductosPedido> ListaProductosPedido { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            options.UseSqlite($"DataSource={baseDir}{DATABASE_PATH}");
        }

        public void RegistrarUsuario(Usuario usuario)
        {
            Usuarios.Add(usuario);

            Carrito carrito = new Carrito
            {
                Usuario = usuario
            };
            
            Carritos.Add(carrito);
            SaveChanges();
        }

        public void RegistrarProducto(Producto producto)
        {
            Productos.Add(producto);
            SaveChanges();
        }

        public bool AutenticarUsuario(string correo, string contrasena)
        {
            return Usuarios.Any(usuario => usuario.Correo == correo && usuario.Contrasena == contrasena);
        }


        public void RegistraListaProductoCarrito(ListaProductosCarrito listaProductosCarrito)
        {
            ListaProductosCarrito.Add(listaProductosCarrito);
            SaveChanges();
        }

        public void modificarCantidad(int productoID, int carritoID, int cantidad)
        {
            var listaProducto = ListaProductosCarrito.First(p => p.Producto.ProductoId == productoID && p.Carrito.CarritoID == carritoID);

            listaProducto.CantidadProducto += cantidad;
            listaProducto.TotalProductoEUR = listaProducto.CantidadProducto * listaProducto.Producto.PrecioEUR;

            SaveChanges();
        }

        public bool compruebaExiste(int productoID, int carritoID)
        {
            var listaExiste = ListaProductosCarrito.Where(p => p.Producto.ProductoId == productoID && p.Carrito.CarritoID == carritoID).ToList();

            return listaExiste.IsNullOrEmpty();
        }

    }
}
