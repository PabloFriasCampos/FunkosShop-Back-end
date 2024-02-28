using FunkosShopBack_end.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FunkosShopBack_end.Resources;
using FunkosShopBack_end.Models.DTOs;
using System.Data;


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
        public bool ModifyUserRole(string newRole, int id)
        {
            bool modificado = false;
            Usuario usuarioMod = Usuarios.FirstOrDefault(u => u.UsuarioID == id);
            if (usuarioMod != null)
            {
                usuarioMod.Rol = newRole;
                SaveChanges();
                modificado = true;
            }

            return modificado;
        }
        public bool ModificarUsuario(UsuarioDTO usuario, int id)
        {
            bool modificado = false;
            Usuario usuarioMod = Usuarios.FirstOrDefault(u => u.UsuarioID == id);
            if (usuarioMod != null && !verificarEmail(usuario.Correo) && usuario.Contrasena.Length>0)
            {
                usuarioMod.NombreUsuario = usuario.NombreUsuario;
                usuarioMod.Direccion = usuario.Direccion;
                usuarioMod.Contrasena = PasswordHelper.Hash(usuario.Contrasena);
                usuarioMod.Correo = usuario.Correo;
                SaveChanges();
                modificado = true;
            }
            else if(usuarioMod != null && !verificarEmail(usuario.Correo) && usuario.Contrasena.Length==0)
            {
                usuarioMod.NombreUsuario = usuario.NombreUsuario;
                usuarioMod.Direccion = usuario.Direccion;
                usuarioMod.Correo = usuario.Correo;
                SaveChanges();
                modificado = true;
            } else if (usuarioMod != null && usuarioMod.Correo == usuario.Correo && usuario.Contrasena.Length==0)
            {
                usuarioMod.NombreUsuario = usuario.NombreUsuario;
                usuarioMod.Direccion = usuario.Direccion;
                SaveChanges();
                modificado = true;
            }
            else if (usuarioMod != null && usuarioMod.Correo == usuario.Correo && usuario.Contrasena.Length>0)
            {
                usuarioMod.NombreUsuario = usuario.NombreUsuario;
                usuarioMod.Direccion = usuario.Direccion;
                usuarioMod.Contrasena = PasswordHelper.Hash(usuario.Contrasena);
                SaveChanges();
                modificado = true;
            }


            return modificado;
        }

        public bool verificarEmail(string correo)
        {
            bool encontrado = true;
            if((Usuarios.FirstOrDefault(u=> u.Correo == correo) == null))
            {
                encontrado = false;
            }
           
            return encontrado;
        }

        public bool ModificarProducto(Producto producto, int id)
        {
            bool modificado = false;
            Producto productoMod = Productos.FirstOrDefault(p => p.ProductoID == id);
            if (productoMod != null)
            {
                productoMod.NombreProducto = producto.NombreProducto;
                productoMod.PrecioEUR = producto.PrecioEUR;
                productoMod.Descripcion = producto.Descripcion;
                productoMod.Categoria = producto.Categoria;
                productoMod.Stock = producto.Stock;
                SaveChanges();
                modificado = true;
            }

            return modificado;
        }
       
        public bool RegistrarUsuario(Usuario usuario)
        {
            bool guardado = false;
           
            if (Usuarios.FirstOrDefault(usuarioin => usuarioin.Correo == usuario.Correo) == null)
            {
                Carrito carrito = new Carrito();
                carrito.Usuario = usuario;
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

        public Usuario AutenticarUsuario(string correo, string contrasena)
        {
            Usuario usuario = Usuarios.FirstOrDefault(usuario => usuario.Correo == correo && usuario.Contrasena == contrasena);

            if (usuario != null)
            {
                return usuario;
            }

            return usuario;
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

            if (listaProducto.CantidadProducto == 0)
            {
                ListaProductosCarrito.Remove(listaProducto);
            }
            else
            {
                listaProducto.TotalProductoEUR = (double)(listaProducto.CantidadProducto * listaProducto.Producto.PrecioEUR);
            }

            
            SaveChanges();
        }
        
        public int productoYaEnCarrito(int productoID, int carritoID)
        {
            List<ProductoCarrito> listaExiste = ListaProductosCarrito.Where(p => p.Producto.ProductoID == productoID && p.Carrito.CarritoID == carritoID).ToList();
            listaExiste.Count();
            return listaExiste.Count();
        }
        

       async public void enviaEmail(int idPedido)
        {
            Pedido pedido = Pedidos
                                .Include(p => p.ListaProductosPedido)
                                    .ThenInclude(l => l.Producto)
                                .First(p => p.PedidoID == idPedido);

            Usuario usuario = Usuarios.Find(pedido.UsuarioID);

            string solofecha = pedido.FechaPedido.ToString().Split(" ")[0];
            string html = 
                """
                                <!DOCTYPE html>
                <html lang="es">

                <head>
                  <meta charset="UTF-8" />
                  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                  <title>Tabla de Productos</title>
                  <style>
                    body {
                      font-family: Arial, sans-serif;
                      margin: 20px;
                      background-color: #f8f8f8;
                      color: #333;
                    }

                    label {
                      display: block;
                      margin-bottom: 5px;
                    }

                    input {
                      width: 100%;
                      padding: 8px;
                      margin-bottom: 15px;
                      box-sizing: border-box;
                      border: 1px solid #ccc;
                      border-radius: 4px;
                    }

                    input[type="date"] {
                      width: calc(100% - 16px);
                      /* Adjust for padding */
                    }

                    table {
                      width: 100%;
                      border-collapse: collapse;
                      margin-top: 20px;
                      background-color: #fff;
                    }

                    table,
                    th,
                    td {
                      border: 1px solid #ddd;
                    }

                    th,
                    td {
                      padding: 10px;
                      text-align: left;
                    }

                    img {
                      max-width: 50px;
                      max-height: 50px;
                    }

                    .total-row {
                      font-weight: bold;
                    }

                    .total-row td {
                      background-color: #f2f2f2;
                    }
                  </style>
                </head>

                <body>
                  <!-- Agrega el campo para la fecha y el ID del pedido aquí -->
                  <div>
                    <label for="fecha">Fecha: 
                """ 
                    + solofecha +
                """ 
                </label>
                    <label for="idPedido">ID del Pedido:
                """ 
                    + idPedido +
                """
                    </label>
                  </div>

                  <table>
                    <thead>
                      <tr>
                        <th>Imagen</th>
                        <th>Nombre del Producto</th>
                        <th>Cantidad</th>
                        <th>Total en Euros</th>
                      </tr>
                    </thead>
                    <tbody>
                """;
            string productos = "";
            foreach (ProductoPedido prodComprado in pedido.ListaProductosPedido) 
            {
                productos += $"""
                      <tr>
                        <td><img src="https://localhost:7281/images/{prodComprado.ProductoID}.png" alt="Imagen del Producto"/></td>
                        <td> {prodComprado.Producto.NombreProducto} </td>
                        <td> {prodComprado.CantidadProducto} </td>
                        <td> {prodComprado.CantidadProducto * prodComprado.Producto.PrecioEUR} </td>
                      </tr> 
                      """;

            }

            html += productos;

            html +=
               $"""
                                 </tbody>
                    <tfoot>
                      <tr class="total-row">
                        <td colspan="3">Total en Euros:</td>
                        <td id="totalEuros">{pedido.TotalPedidoEUR}</td>
                      </tr>
                      <tr class="total-row">
                        <td colspan="3">Total en ETH:</td>
                        <td id="totalETH">{pedido.TotalPedidoETH}</td>
                      </tr>
                    </tfoot>
                  </table>
                </body>

                </html>
                """;

            await EmailService.SendMessageAsync(usuario.Correo, "Factura pedido", html, true);
            
        }
    }
}

    

