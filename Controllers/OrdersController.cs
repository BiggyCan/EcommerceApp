using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // ==========================================
        // PEDIDOS DEL USUARIO ACTUAL
        // ==========================================

        public async Task<IActionResult> MyOrders()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var orders =
                await _context.Orders
                    .Include(o => o.Items)
                    .Where(o => o.UserId == user.Id)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

            return View(orders);
        }


        // ==========================================
        // DETALLE DE UN PEDIDO DEL USUARIO
        // ==========================================

        public async Task<IActionResult> Details(int id)
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
                            (
                                o.UserId == user.Id
                                ||
                                User.IsInRole("Admin")
                            )
                    );

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // ==========================================
        // TODAS LAS VENTAS - SOLO ADMIN
        // ==========================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(
            string? search,
            string? status)
        {
            var query =
                _context.Orders
                    .Include(o => o.Items)
                    .AsQueryable();


            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query =
                    query.Where(o =>
                        EF.Functions.ILike(
                            o.CustomerName,
                            $"%{search}%"
                        )
                        ||
                        EF.Functions.ILike(
                            o.CustomerEmail,
                            $"%{search}%"
                        )
                    );
            }


            if (!string.IsNullOrWhiteSpace(status))
            {
                query =
                    query.Where(
                        o => o.Status == status
                    );
            }


            var orders =
                await query
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();


            ViewBag.Search =
                search ?? string.Empty;

            ViewBag.Status =
                status ?? string.Empty;

            ViewBag.TotalOrders =
                await _context.Orders.CountAsync();

            ViewBag.TotalSales =
                await _context.Orders
                    .SumAsync(o => (decimal?)o.TotalAmount)
                ?? 0;

            ViewBag.ConfirmedOrders =
                await _context.Orders
                    .CountAsync(
                        o => o.Status == "Confirmado"
                    );


            return View(orders);
        }
    }
}