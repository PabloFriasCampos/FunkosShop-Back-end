﻿using FunkosShopBack_end.Models.Entities;

namespace FunkosShopBack_end.Models
{
    public class DBSeeder
    {

        private readonly DBContext _dbContext;

        public DBSeeder(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedAsync()
        {
            bool created = await _dbContext.Database.EnsureCreatedAsync();

            if (created)
            {
                await SeedImagesAsync();
            }

            _dbContext.SaveChanges();
        }

        private async Task SeedImagesAsync()
        {
            Producto[] productos =
            [
                new Producto() { NombreProducto = "IRON MAN", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "MARVEL", Stock = 10 },
                new Producto() { NombreProducto = "DEADPOOL", PrecioEUR = 16.45, Descripcion = "Funko pop", Categoria = "MARVEL", Stock = 10 },
                new Producto() { NombreProducto = "NICK FURY", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "MARVEL", Stock = 10 },
                new Producto() { NombreProducto = "DR. STRANGE", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "MARVEL", Stock = 10 },
                new Producto() { NombreProducto = "LORD VOLDEMORT", PrecioEUR = 16.45, Descripcion = "Funko pop", Categoria = "HARRY POTTER", Stock = 10 },
                new Producto() { NombreProducto = "HARRY POTTER", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "HARRY POTTER", Stock = 10 },
                new Producto() { NombreProducto = "DOBBY", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "HARRY POTTER", Stock = 10 },
                new Producto() { NombreProducto = "FAWKES", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "HARRY POTTER", Stock = 10 },
                new Producto() { NombreProducto = "DARTH VADER", PrecioEUR = 14.45, Descripcion = "Funko pop", Categoria = "STAR WARS", Stock = 10 },
                new Producto() { NombreProducto = "HAN SOLO", PrecioEUR = 11.45, Descripcion = "Funko pop", Categoria = "STAR WARS", Stock = 10 },
                new Producto() { NombreProducto = "DARTH MAUL", PrecioEUR = 39.45, Descripcion = "Funko pop", Categoria = "STAR WARS", Stock = 10 },
                new Producto() { NombreProducto = "C3PO", PrecioEUR = 27.45, Descripcion = "Funko pop", Categoria = "STAR WARS", Stock = 10 },
                new Producto() { NombreProducto = "MICHAEL JORDAN", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "DEPORTISTAS", Stock = 10 },
                new Producto() { NombreProducto = "LEWIS HAMILTON", PrecioEUR = 16.45, Descripcion = "Funko pop", Categoria = "DEPORTISTAS", Stock = 10 },
                new Producto() { NombreProducto = "JHON CENA", PrecioEUR = 14.45, Descripcion = "Funko pop", Categoria = "DEPORTISTAS", Stock = 10 },
                new Producto() { NombreProducto = "LIONEL MESSI", PrecioEUR = 22.45, Descripcion = "Funko pop", Categoria = "DEPORTISTAS", Stock = 10 },
                new Producto() { NombreProducto = "GOKU", PrecioEUR = 18.45, Descripcion = "Funko pop", Categoria = "ANIME", Stock = 10 },
                new Producto() { NombreProducto = "BLACK ASTA", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "ANIME", Stock = 10 },
                new Producto() { NombreProducto = "SORA", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "ANIME", Stock = 10 },
                new Producto() { NombreProducto = "LUFFY", PrecioEUR = 13.45, Descripcion = "Funko pop", Categoria = "ANIME", Stock = 10 }
            ];

            await _dbContext.Productos.AddRangeAsync(productos);
        }

    }
}
