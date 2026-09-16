using EcommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Data
{
    public static class ProductSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            var existingNames =
                await context.Products
                    .Select(p => p.Name)
                    .ToListAsync();

            var existingProducts =
                new HashSet<string>(
                    existingNames,
                    StringComparer.OrdinalIgnoreCase
                );

            var now = DateTime.UtcNow;


            var products =
                new List<Product>
                {
                    // ==========================================
                    // POKÉMON TCG
                    // ==========================================

                    new Product
                    {
                        Name = "Pokémon TCG 30th Celebration Pack",
                        Description = "Sobre de cartas Pokémon de la colección 30th Celebration.",
                        Price = 69.00m,
                        Stock = 20,
                        Category = "Trading Cards",
                        ImageUrl = "https://media.gamestop.com/i/gamestop/pkm-30th-pack.jpeg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Pokémon TCG Mega Evolution Delta Reign Pack",
                        Description = "Sobre de cartas Pokémon Mega Evolution Delta Reign.",
                        Price = 69.00m,
                        Stock = 18,
                        Category = "Trading Cards",
                        ImageUrl = "https://media.gamestop.com/i/gamestop/pkm-deltareign-pack.jpeg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Pokémon TCG Mega Evolution Pitch Black Pack",
                        Description = "Sobre de cartas Pokémon Mega Evolution Pitch Black.",
                        Price = 69.00m,
                        Stock = 18,
                        Category = "Trading Cards",
                        ImageUrl = "https://media.gamestop.com/i/gamestop/pkm-pitchblack-pack_1.jpeg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Pokémon TCG Mega Evolution Chaos Rising Pack",
                        Description = "Sobre de cartas Pokémon Mega Evolution Chaos Rising.",
                        Price = 69.00m,
                        Stock = 18,
                        Category = "Trading Cards",
                        ImageUrl = "https://media.gamestop.com/i/gamestop/pkm-chaosrising-pack_1.jpeg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Pokémon TCG Mega Evolution Perfect Order Pack",
                        Description = "Sobre de cartas Pokémon Mega Evolution Perfect Order.",
                        Price = 69.00m,
                        Stock = 18,
                        Category = "Trading Cards",
                        ImageUrl = "https://media.gamestop.com/i/gamestop/pkm-perfectorder-pack_1.jpeg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },


                    // ==========================================
                    // FEATURED PRODUCTS
                    // ==========================================

                    new Product
                    {
                        Name = "Onimusha Way of the Sword Steel Book Edition - PlayStation 5",
                        Description = "Edición Steel Book para PlayStation 5.",
                        Price = 519.00m,
                        Stock = 8,
                        Category = "Videojuegos",
                        ImageUrl = "/images/products/onimusha-ps5.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "The Blood of Dawnwalker - Xbox Series X",
                        Description = "RPG de acción para Xbox Series X.",
                        Price = 489.00m,
                        Stock = 10,
                        Category = "Videojuegos",
                        ImageUrl = "/images/products/blood-of-dawnwalker.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Yu-Gi-Oh! Magnificent Monsters Booster Box",
                        Description = "Caja de sobres coleccionables Yu-Gi-Oh!.",
                        Price = 249.00m,
                        Stock = 12,
                        Category = "Trading Cards",
                        ImageUrl = "/images/products/yugioh-magnificent-monsters.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Pokémon TCG Destined Rivals Elite Trainer Box",
                        Description = "Elite Trainer Box de Pokémon TCG Destined Rivals.",
                        Price = 849.00m,
                        Stock = 7,
                        Category = "Trading Cards",
                        ImageUrl = "/images/products/pokemon-destined-rivals.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "NBA 2K27 - Xbox Series X",
                        Description = "Videojuego de baloncesto para Xbox Series X.",
                        Price = 489.00m,
                        Stock = 9,
                        Category = "Videojuegos",
                        ImageUrl = "/images/products/nba2k27-xbox.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },


                    // ==========================================
                    // WHAT EVERYONE'S PLAYING
                    // ==========================================

                    new Product
                    {
                        Name = "The Legend of Zelda: Ocarina of Time - Nintendo Switch 2",
                        Description = "Aventura para Nintendo Switch 2.",
                        Price = 489.00m,
                        Stock = 13,
                        Category = "Videojuegos",
                        ImageUrl = "/images/products/zelda-ocarina-switch2.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Final Fantasy VII Rebirth Deluxe Edition - PlayStation 5",
                        Description = "Edición Deluxe para PlayStation 5.",
                        Price = 699.00m,
                        Stock = 6,
                        Category = "Videojuegos",
                        ImageUrl = "/images/products/final-fantasy-vii.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Marvel's Wolverine - PlayStation 5",
                        Description = "Juego de acción de Marvel para PlayStation 5.",
                        Price = 489.00m,
                        Stock = 15,
                        Category = "Videojuegos",
                        ImageUrl = "/images/products/wolverine-ps5.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Grand Theft Auto VI - PlayStation 5",
                        Description = "Grand Theft Auto VI para PlayStation 5.",
                        Price = 559.00m,
                        Stock = 15,
                        Category = "Videojuegos",
                        ImageUrl = "/images/products/gta6-ps5.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Star Wars Zero Company - PC",
                        Description = "Juego de estrategia de Star Wars para PC.",
                        Price = 349.00m,
                        Stock = 11,
                        Category = "Videojuegos",
                        ImageUrl = "/images/products/star-wars-zero-company.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },


                    // ==========================================
                    // PLAYSTATION
                    // ==========================================

                    new Product
                    {
                        Name = "PlayStation 5 Slim",
                        Description = "Consola PlayStation 5 Slim.",
                        Price = 4299.00m,
                        Stock = 5,
                        Category = "Consolas",
                        ImageUrl = "/images/products/ps5-slim.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Sony DualSense Wireless Controller - White",
                        Description = "Control inalámbrico DualSense para PlayStation 5.",
                        Price = 589.00m,
                        Stock = 18,
                        Category = "Accesorios",
                        ImageUrl = "/images/products/dualsense-white.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Sony DualSense Wireless Controller - Nova Pink",
                        Description = "Control DualSense color Nova Pink para PlayStation 5.",
                        Price = 599.00m,
                        Stock = 12,
                        Category = "Accesorios",
                        ImageUrl = "/images/products/dualsense-pink.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Sony DualSense Edge Wireless Controller",
                        Description = "Control inalámbrico profesional para PlayStation 5.",
                        Price = 1399.00m,
                        Stock = 5,
                        Category = "Accesorios",
                        ImageUrl = "/images/products/dualsense-edge.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },


                    // ==========================================
                    // XBOX
                    // ==========================================

                    new Product
                    {
                        Name = "Xbox Series X",
                        Description = "Consola Xbox Series X.",
                        Price = 4199.00m,
                        Stock = 5,
                        Category = "Consolas",
                        ImageUrl = "/images/products/xbox-series-x.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Microsoft Xbox Wireless Controller",
                        Description = "Control inalámbrico para Xbox Series X|S y PC.",
                        Price = 559.00m,
                        Stock = 16,
                        Category = "Accesorios",
                        ImageUrl = "/images/products/xbox-controller.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Turtle Beach Stealth 700 Gen 3 Wireless Headset",
                        Description = "Headset inalámbrico para Xbox y PC.",
                        Price = 1049.00m,
                        Stock = 8,
                        Category = "Accesorios",
                        ImageUrl = "/images/products/turtle-beach-stealth-700.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },


                    // ==========================================
                    // NINTENDO
                    // ==========================================

                    new Product
                    {
                        Name = "Nintendo Switch OLED",
                        Description = "Consola Nintendo Switch con pantalla OLED.",
                        Price = 2999.00m,
                        Stock = 7,
                        Category = "Consolas",
                        ImageUrl = "/images/products/switch-oled.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Nintendo Switch 2",
                        Description = "Consola Nintendo Switch de nueva generación.",
                        Price = 3799.00m,
                        Stock = 6,
                        Category = "Consolas",
                        ImageUrl = "/images/products/switch2.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Nintendo Switch 2 Pro Controller",
                        Description = "Control Pro para Nintendo Switch 2.",
                        Price = 699.00m,
                        Stock = 10,
                        Category = "Accesorios",
                        ImageUrl = "/images/products/switch2-pro-controller.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },


                    // ==========================================
                    // PC GAMING
                    // ==========================================

                    new Product
                    {
                        Name = "Gaming Keyboard RGB",
                        Description = "Teclado mecánico RGB para gaming.",
                        Price = 449.00m,
                        Stock = 12,
                        Category = "PC Gaming",
                        ImageUrl = "/images/products/gaming-keyboard.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Gaming Headset 7.1",
                        Description = "Auriculares gaming con sonido envolvente.",
                        Price = 399.00m,
                        Stock = 14,
                        Category = "PC Gaming",
                        ImageUrl = "/images/products/gaming-headset.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Portable Gaming SSD 1TB",
                        Description = "SSD portátil de 1TB para PC y consolas.",
                        Price = 629.00m,
                        Stock = 9,
                        Category = "PC Gaming",
                        ImageUrl = "/images/products/portable-ssd.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },


                    // ==========================================
                    // COLLECTIBLES
                    // ==========================================

                    new Product
                    {
                        Name = "Super7 Invincible Thragg Deluxe Action Figure",
                        Description = "Figura de acción coleccionable de Invincible.",
                        Price = 219.00m,
                        Stock = 8,
                        Category = "Coleccionables",
                        ImageUrl = "/images/products/invincible-thragg.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Hasbro Marvel Legends Series Punisher",
                        Description = "Figura coleccionable Marvel Legends.",
                        Price = 209.00m,
                        Stock = 10,
                        Category = "Coleccionables",
                        ImageUrl = "/images/products/marvel-punisher.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Funko POP! Toy Story Woody and Buzz",
                        Description = "Pack coleccionable Funko POP! de Toy Story.",
                        Price = 179.00m,
                        Stock = 9,
                        Category = "Coleccionables",
                        ImageUrl = "/images/products/funko-woody-buzz.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "The Amazing Digital Circus Pomni Plush",
                        Description = "Peluche coleccionable de Pomni.",
                        Price = 279.00m,
                        Stock = 11,
                        Category = "Coleccionables",
                        ImageUrl = "/images/products/pomni-plush.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },


                    // ==========================================
                    // DIGITAL STORE
                    // ==========================================

                    new Product
                    {
                        Name = "PlayStation Store Gift Card",
                        Description = "Tarjeta digital para PlayStation Store.",
                        Price = 350.00m,
                        Stock = 40,
                        Category = "Tienda digital",
                        ImageUrl = "/images/products/playstation-gift-card.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Xbox Gift Card",
                        Description = "Tarjeta digital para Xbox Store.",
                        Price = 350.00m,
                        Stock = 40,
                        Category = "Tienda digital",
                        ImageUrl = "/images/products/xbox-gift-card.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    },

                    new Product
                    {
                        Name = "Nintendo eShop Gift Card",
                        Description = "Tarjeta digital para Nintendo eShop.",
                        Price = 350.00m,
                        Stock = 40,
                        Category = "Tienda digital",
                        ImageUrl = "/images/products/nintendo-gift-card.jpg",
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                };


            var newProducts =
                products
                    .Where(
                        product =>
                            !existingProducts.Contains(
                                product.Name
                            )
                    )
                    .ToList();


            if (!newProducts.Any())
            {
                return;
            }


            await context.Products
                .AddRangeAsync(
                    newProducts
                );


            await context.SaveChangesAsync();
        }
    }
}