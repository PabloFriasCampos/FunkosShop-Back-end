﻿using FunkosShopBack_end.Models;
using Microsoft.EntityFrameworkCore;

namespace FunkosShopBack_end
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

        public bool AutenticarUsuario(string nombreUsuario, string contrasena)
        {
            return Usuarios.Any(usuario => usuario.NombreUsuario == nombreUsuario && usuario.Contrasena == contrasena);
        }


        public void RegistraListaProductoCarrito(ListaProductosCarrito listaProductosCarrito)
        {
            ListaProductosCarrito.Add(listaProductosCarrito);
            SaveChanges();
        }
        
    }
}
