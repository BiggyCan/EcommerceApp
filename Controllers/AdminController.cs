using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var totalProducts =
                await _context.Products.CountAsync();

            var totalUsers =
                await _userManager.Users.CountAsync();

            var lowStock =
                await _context.Products
                    .CountAsync(p =>
                        p.Stock > 0 &&
                        p.Stock <= 5);

            var outOfStock =
                await _context.Products
                    .CountAsync(p =>
                        p.Stock == 0);

            var totalStock =
                await _context.Products
                    .SumAsync(p =>
                        (int?)p.Stock)
                    ?? 0;

            var inventoryValue =
                await _context.Products
                    .SumAsync(p =>
                        (decimal?)(p.Price * p.Stock))
                    ?? 0;


            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.LowStock = lowStock;
            ViewBag.OutOfStock = outOfStock;
            ViewBag.TotalStock = totalStock;
            ViewBag.InventoryValue = inventoryValue;


            var recentProducts =
                await _context.Products
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .ToListAsync();


            return View(recentProducts);
        }


        public async Task<IActionResult> Users()
        {
            var users =
                await _userManager.Users
                    .OrderBy(u => u.FullName)
                    .ThenBy(u => u.Email)
                    .ToListAsync();


            var userRoles =
                new Dictionary<string, IList<string>>();


            foreach (var user in users)
            {
                userRoles[user.Id] =
                    await _userManager
                        .GetRolesAsync(user);
            }


            ViewBag.UserRoles = userRoles;


            return View(users);
        }


        public async Task<IActionResult> Products(
            string? search = null,
            string? category = null)
        {
            var query =
                _context.Products
                    .AsQueryable();


            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Description.Contains(search));
            }


            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p =>
                    p.Category == category);
            }


            var products =
                await query
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();


            var categories =
                await _context.Products
                    .Where(p =>
                        p.Category != null &&
                        p.Category != "")
                    .Select(p => p.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();


            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Categories = categories;


            return View(products);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(
            string id)
        {
            var user =
                await _userManager
                    .FindByIdAsync(id);


            if (user == null)
            {
                return NotFound();
            }


            var currentRoles =
                await _userManager
                    .GetRolesAsync(user);


            if (currentRoles.Any())
            {
                await _userManager
                    .RemoveFromRolesAsync(
                        user,
                        currentRoles
                    );
            }


            await _userManager
                .AddToRoleAsync(
                    user,
                    "Admin"
                );


            TempData["SuccessMessage"] =
                $"{user.Email} ahora es administrador.";


            return RedirectToAction(
                nameof(Users)
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeUser(
            string id)
        {
            var user =
                await _userManager
                    .FindByIdAsync(id);


            if (user == null)
            {
                return NotFound();
            }


            if (user.Id ==
                _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] =
                    "No puedes quitar tu propio rol de administrador.";

                return RedirectToAction(
                    nameof(Users)
                );
            }


            var currentRoles =
                await _userManager
                    .GetRolesAsync(user);


            if (currentRoles.Any())
            {
                await _userManager
                    .RemoveFromRolesAsync(
                        user,
                        currentRoles
                    );
            }


            await _userManager
                .AddToRoleAsync(
                    user,
                    "User"
                );


            TempData["SuccessMessage"] =
                $"{user.Email} ahora es usuario normal.";


            return RedirectToAction(
                nameof(Users)
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(
            string id)
        {
            var user =
                await _userManager
                    .FindByIdAsync(id);


            if (user == null)
            {
                return NotFound();
            }


            if (user.Id ==
                _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] =
                    "No puedes eliminar tu propia cuenta de administrador.";

                return RedirectToAction(
                    nameof(Users)
                );
            }


            var result =
                await _userManager
                    .DeleteAsync(user);


            if (result.Succeeded)
            {
                TempData["SuccessMessage"] =
                    "Usuario eliminado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "No se pudo eliminar el usuario.";
            }


            return RedirectToAction(
                nameof(Users)
            );
        }


        public IActionResult CreateProduct()
        {
            return RedirectToAction(
                "Create",
                "Products"
            );
        }
    }
}