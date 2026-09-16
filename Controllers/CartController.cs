using System.Text.Json;
using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private const string CartSessionKey = "BigGameCart";

        public CartController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var cart = GetCart();

            ViewBag.TotalItems =
                cart.Sum(item => item.Quantity);

            ViewBag.Total =
                cart.Sum(item => item.Subtotal);

            return View(cart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id)
        {
            var product =
                await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                TempData["CartError"] =
                    "El producto no existe.";

                return RedirectToAction(
                    "Index",
                    "Products"
                );
            }


            if (product.Stock <= 0)
            {
                TempData["CartError"] =
                    "Este producto está agotado.";

                return RedirectToAction(
                    "Details",
                    "Products",
                    new { id = product.Id }
                );
            }


            var cart = GetCart();

            var item =
                cart.FirstOrDefault(
                    item => item.ProductId == product.Id
                );


            if (item == null)
            {
                cart.Add(
                    new CartItem
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        ImageUrl = product.ImageUrl,
                        Category = product.Category,
                        Price = product.Price,
                        Quantity = 1,
                        Stock = product.Stock
                    }
                );
            }
            else
            {
                if (item.Quantity >= product.Stock)
                {
                    TempData["CartError"] =
                        $"Solo existen {product.Stock} unidades disponibles.";

                    return RedirectToAction(
                        nameof(Index)
                    );
                }

                item.Quantity++;

                item.Price = product.Price;
                item.Stock = product.Stock;
                item.ImageUrl = product.ImageUrl;
                item.Category = product.Category;
                item.Name = product.Name;
            }


            SaveCart(cart);


            TempData["CartSuccess"] =
                $"{product.Name} fue agregado al carrito.";


            return RedirectToAction(
                nameof(Index)
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Increase(int id)
        {
            var product =
                await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                TempData["CartError"] =
                    "El producto ya no existe.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            var cart = GetCart();

            var item =
                cart.FirstOrDefault(
                    item => item.ProductId == id
                );


            if (item == null)
            {
                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (item.Quantity >= product.Stock)
            {
                TempData["CartError"] =
                    $"No puedes agregar más. Stock disponible: {product.Stock}.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            item.Quantity++;

            item.Stock = product.Stock;
            item.Price = product.Price;

            SaveCart(cart);


            return RedirectToAction(
                nameof(Index)
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decrease(int id)
        {
            var cart = GetCart();

            var item =
                cart.FirstOrDefault(
                    item => item.ProductId == id
                );


            if (item == null)
            {
                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                cart.Remove(item);
            }


            SaveCart(cart);


            return RedirectToAction(
                nameof(Index)
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();

            var item =
                cart.FirstOrDefault(
                    item => item.ProductId == id
                );


            if (item != null)
            {
                cart.Remove(item);

                SaveCart(cart);

                TempData["CartSuccess"] =
                    "Producto eliminado del carrito.";
            }


            return RedirectToAction(
                nameof(Index)
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(
                CartSessionKey
            );


            TempData["CartSuccess"] =
                "El carrito fue vaciado.";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // ==========================================
        // FINALIZAR COMPRA
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();


            if (!cart.Any())
            {
                TempData["CartError"] =
                    "Tu carrito está vacío.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            var user =
                await _userManager.GetUserAsync(User);


            if (user == null)
            {
                return Challenge();
            }


            var productIds =
                cart
                    .Select(item => item.ProductId)
                    .Distinct()
                    .ToList();


            var products =
                await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();


            foreach (var item in cart)
            {
                var product =
                    products.FirstOrDefault(
                        p => p.Id == item.ProductId
                    );


                if (product == null)
                {
                    TempData["CartError"] =
                        $"El producto {item.Name} ya no está disponible.";

                    return RedirectToAction(
                        nameof(Index)
                    );
                }


                if (product.Stock < item.Quantity)
                {
                    TempData["CartError"] =
                        $"No existe suficiente stock de {product.Name}. " +
                        $"Disponibles: {product.Stock}.";

                    return RedirectToAction(
                        nameof(Index)
                    );
                }
            }


            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                var order =
                    new Order
                    {
                        UserId = user.Id,

                        CustomerName =
                            string.IsNullOrWhiteSpace(user.FullName)
                                ? user.Email ?? "Cliente BigGame"
                                : user.FullName,

                        CustomerEmail =
                            user.Email ?? string.Empty,

                        ShippingAddress =
                            user.Address,

                        Status = "Confirmado",

                        CreatedAt =
                            DateTime.UtcNow
                    };


                decimal total = 0;


                foreach (var cartItem in cart)
                {
                    var product =
                        products.First(
                            p => p.Id == cartItem.ProductId
                        );


                    var subtotal =
                        product.Price
                        *
                        cartItem.Quantity;


                    total += subtotal;


                    order.Items.Add(
                        new OrderItem
                        {
                            ProductId =
                                product.Id,

                            ProductName =
                                product.Name,

                            UnitPrice =
                                product.Price,

                            Quantity =
                                cartItem.Quantity
                        }
                    );


                    product.Stock -=
                        cartItem.Quantity;

                    product.UpdatedAt =
                        DateTime.UtcNow;
                }


                order.TotalAmount =
                    total;


                _context.Orders.Add(order);


                await _context.SaveChangesAsync();


                await transaction.CommitAsync();


                HttpContext.Session.Remove(
                    CartSessionKey
                );


                return RedirectToAction(
                    nameof(Success),
                    new { id = order.Id }
                );
            }
            catch
            {
                await transaction.RollbackAsync();


                TempData["CartError"] =
                    "No se pudo completar la compra. Intenta nuevamente.";


                return RedirectToAction(
                    nameof(Index)
                );
            }
        }


        // ==========================================
        // COMPRA REALIZADA
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);


            if (user == null)
            {
                return Challenge();
            }


            var order =
                await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(
                        o =>
                            o.Id == id
                            &&
                            o.UserId == user.Id
                    );


            if (order == null)
            {
                return NotFound();
            }


            return View(order);
        }


        private List<CartItem> GetCart()
        {
            var cartJson =
                HttpContext.Session
                    .GetString(CartSessionKey);


            if (string.IsNullOrWhiteSpace(cartJson))
            {
                return new List<CartItem>();
            }


            try
            {
                return JsonSerializer
                    .Deserialize<List<CartItem>>(
                        cartJson
                    )
                    ?? new List<CartItem>();
            }
            catch
            {
                return new List<CartItem>();
            }
        }


        private void SaveCart(
            List<CartItem> cart)
        {
            var cartJson =
                JsonSerializer.Serialize(cart);


            HttpContext.Session.SetString(
                CartSessionKey,
                cartJson
            );
        }
    }
}