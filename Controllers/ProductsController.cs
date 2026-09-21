using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // TIENDA / CATÁLOGO
        // ==========================================

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string? search = null,
            string? category = null)
        {
            search =
                string.IsNullOrWhiteSpace(search)
                    ? null
                    : search.Trim();

            category =
                string.IsNullOrWhiteSpace(category)
                    ? null
                    : category.Trim();


            var isSearch =
                !string.IsNullOrWhiteSpace(search)
                ||
                !string.IsNullOrWhiteSpace(category);


            // ======================================
            // CONSULTA BASE
            // ======================================

            var catalogQuery =
                _context.Products
                    .AsNoTracking();


            // ======================================
            // CARGAR CATÁLOGO COMPLETO
            // ======================================

            var allProducts =
                await catalogQuery
                    .OrderByDescending(
                        p => p.CreatedAt
                    )
                    .ToListAsync();


            // ======================================
            // CONSULTA DE RESULTADOS
            // ======================================

            IQueryable<Product> query =
                _context.Products
                    .AsNoTracking();


            // ======================================
            // BUSCADOR SIN DIFERENCIAR
            // MAYÚSCULAS Y MINÚSCULAS
            // ======================================

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchPattern =
                    $"%{search}%";


                query =
                    query.Where(
                        p =>

                            EF.Functions.ILike(
                                p.Name,
                                searchPattern
                            )

                            ||

                            EF.Functions.ILike(
                                p.Description,
                                searchPattern
                            )

                            ||

                            (
                                p.Category != null
                                &&
                                EF.Functions.ILike(
                                    p.Category,
                                    searchPattern
                                )
                            )
                    );
            }


            // ======================================
            // FILTRO POR CATEGORÍA
            // ======================================

            if (!string.IsNullOrWhiteSpace(category))
            {
                query =
                    query.Where(
                        p =>
                            p.Category != null
                            &&
                            EF.Functions.ILike(
                                p.Category,
                                category
                            )
                    );
            }


            // ======================================
            // RESULTADOS
            // ======================================

            List<Product> products;


            if (isSearch)
            {
                products =
                    await query
                        .OrderByDescending(
                            p => p.CreatedAt
                        )
                        .ToListAsync();
            }
            else
            {
                products =
                    allProducts;
            }


            // ======================================
            // CATEGORÍAS DISPONIBLES
            // ======================================

            var categories =
                allProducts
                    .Where(
                        p =>
                            !string.IsNullOrWhiteSpace(
                                p.Category
                            )
                    )
                    .Select(
                        p => p.Category!
                    )
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase
                    )
                    .OrderBy(
                        c => c
                    )
                    .ToList();


            // ======================================
            // PRODUCTOS RELACIONADOS
            //
            // No se muestran como resultados de
            // búsqueda. Se usarán en la sección
            // "También te puede gustar".
            // ======================================

            var resultIds =
                products
                    .Select(
                        p => p.Id
                    )
                    .ToHashSet();


            var resultCategories =
                products
                    .Where(
                        p =>
                            !string.IsNullOrWhiteSpace(
                                p.Category
                            )
                    )
                    .Select(
                        p => p.Category!
                    )
                    .ToHashSet(
                        StringComparer.OrdinalIgnoreCase
                    );


            var relatedProducts =
                allProducts
                    .Where(
                        p =>
                            !resultIds.Contains(
                                p.Id
                            )
                    )
                    .OrderByDescending(
                        p =>
                            !string.IsNullOrWhiteSpace(
                                p.Category
                            )
                            &&
                            resultCategories.Contains(
                                p.Category
                            )
                    )
                    .ThenByDescending(
                        p => p.CreatedAt
                    )
                    .Take(12)
                    .ToList();


            // ======================================
            // INFORMACIÓN PARA LAS VISTAS
            // ======================================

            ViewBag.Search =
                search;

            ViewBag.Category =
                category;

            ViewBag.Categories =
                categories;

            ViewBag.AllProducts =
                allProducts;

            ViewBag.RelatedProducts =
                relatedProducts;

            ViewBag.SearchResultCount =
                products.Count;

            ViewBag.TotalCatalogProducts =
                allProducts.Count;


            return View(
                products
            );
        }


        // ==========================================
        // DETALLES
        // ==========================================

        [AllowAnonymous]
        public async Task<IActionResult> Details(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var product =
                await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        p => p.Id == id
                    );


            if (product == null)
            {
                return NotFound();
            }


            return View(
                product
            );
        }


        // ==========================================
        // CREAR - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // ==========================================
        // CREAR - POST
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    product
                );
            }


            product.CreatedAt =
                DateTime.UtcNow;


            _context.Products.Add(
                product
            );


            await _context.SaveChangesAsync();


            return RedirectToAction(
                "Products",
                "Admin"
            );
        }


        // ==========================================
        // EDITAR - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var product =
                await _context.Products
                    .FindAsync(
                        id
                    );


            if (product == null)
            {
                return NotFound();
            }


            return View(
                product
            );
        }


        // ==========================================
        // EDITAR - POST
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }


            if (!ModelState.IsValid)
            {
                return View(
                    product
                );
            }


            try
            {
                var existingProduct =
                    await _context.Products
                        .FindAsync(
                            id
                        );


                if (existingProduct == null)
                {
                    return NotFound();
                }


                existingProduct.Name =
                    product.Name;

                existingProduct.Description =
                    product.Description;

                existingProduct.Price =
                    product.Price;

                existingProduct.Stock =
                    product.Stock;

                existingProduct.ImageUrl =
                    product.ImageUrl;

                existingProduct.Category =
                    product.Category;

                existingProduct.UpdatedAt =
                    DateTime.UtcNow;


                await _context
                    .SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists =
                    await _context.Products
                        .AnyAsync(
                            p =>
                                p.Id
                                ==
                                product.Id
                        );


                if (!exists)
                {
                    return NotFound();
                }


                throw;
            }


            return RedirectToAction(
                "Products",
                "Admin"
            );
        }


        // ==========================================
        // ELIMINAR - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var product =
                await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        p => p.Id == id
                    );


            if (product == null)
            {
                return NotFound();
            }


            return View(
                product
            );
        }


        // ==========================================
        // ELIMINAR - POST
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var product =
                await _context.Products
                    .FindAsync(
                        id
                    );


            if (product != null)
            {
                _context.Products.Remove(
                    product
                );


                await _context
                    .SaveChangesAsync();
            }


            return RedirectToAction(
                "Products",
                "Admin"
            );
        }
    }
}