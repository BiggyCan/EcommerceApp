using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// BASE DE DATOS
// ==========================================

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection"
    );

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la conexión DefaultConnection."
    );
}

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseNpgsql(connectionString)
);


// ==========================================
// IDENTITY
// ==========================================

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(
        options =>
        {
            options.Password.RequiredLength = 3;

            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;

            options.Password.RequiredUniqueChars = 1;

            options.User.RequireUniqueEmail = true;
        }
    )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// ==========================================
// CONFIGURACIÓN DE COOKIES
// ==========================================

builder.Services.ConfigureApplicationCookie(
    options =>
    {
        options.LoginPath =
            "/Account/Login";

        options.AccessDeniedPath =
            "/Account/Login";

        options.ExpireTimeSpan =
            TimeSpan.FromHours(8);

        options.SlidingExpiration =
            true;
    }
);


// ==========================================
// SESIÓN PARA EL CARRITO
// ==========================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(
    options =>
    {
        options.IdleTimeout =
            TimeSpan.FromHours(8);

        options.Cookie.HttpOnly =
            true;

        options.Cookie.IsEssential =
            true;
    }
);


// ==========================================
// MVC
// ==========================================

builder.Services.AddControllersWithViews();


var app = builder.Build();


// ==========================================
// MANEJO DE ERRORES
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Home/Error"
    );

    app.UseHsts();
}


// ==========================================
// MIDDLEWARE
// ==========================================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();


// ==========================================
// RUTA PRINCIPAL
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Products}/{action=Index}/{id?}"
);


// ==========================================
// CREAR ROLES, USUARIOS Y PRODUCTOS INICIALES
// ==========================================

using (var scope = app.Services.CreateScope())
{
    var services =
        scope.ServiceProvider;


    var context =
        services.GetRequiredService<ApplicationDbContext>();

    var userManager =
        services.GetRequiredService<
            UserManager<ApplicationUser>
        >();

    var roleManager =
        services.GetRequiredService<
            RoleManager<IdentityRole>
        >();


    // ======================================
    // APLICAR MIGRACIONES PENDIENTES
    // ======================================

    await context.Database.MigrateAsync();


    // ======================================
    // CREAR ROLES
    // ======================================

    string[] roles =
    {
        "Admin",
        "User"
    };


    foreach (var role in roles)
    {
        var roleExists =
            await roleManager
                .RoleExistsAsync(role);


        if (!roleExists)
        {
            await roleManager
                .CreateAsync(
                    new IdentityRole(role)
                );
        }
    }


    // ======================================
    // FUNCIÓN PARA CREAR USUARIOS
    // ======================================

    async Task CreateUserIfNotExists(
        string email,
        string password,
        string fullName,
        string address,
        string role)
    {
        var user =
            await userManager
                .FindByEmailAsync(email);


        if (user == null)
        {
            user =
                new ApplicationUser
                {
                    UserName =
                        email,

                    Email =
                        email,

                    FullName =
                        fullName,

                    Address =
                        address,

                    EmailConfirmed =
                        true,

                    CreatedAt =
                        DateTime.UtcNow
                };


            var result =
                await userManager
                    .CreateAsync(
                        user,
                        password
                    );


            if (result.Succeeded)
            {
                await userManager
                    .AddToRoleAsync(
                        user,
                        role
                    );
            }
        }
        else
        {
            var currentRoles =
                await userManager
                    .GetRolesAsync(user);


            if (!currentRoles.Contains(role))
            {
                await userManager
                    .AddToRoleAsync(
                        user,
                        role
                    );
            }
        }
    }


    // ======================================
    // USUARIOS DE PRUEBA
    // ======================================

    await CreateUserIfNotExists(
        "juan@gmail.com",
        "123",
        "Juan Pérez",
        "Cochabamba",
        "User"
    );


    await CreateUserIfNotExists(
        "maria@gmail.com",
        "123",
        "María López",
        "Cochabamba",
        "User"
    );


    await CreateUserIfNotExists(
        "admin@gmail.com",
        "123",
        "Administrador BigGame",
        "Cochabamba",
        "Admin"
    );


    // ======================================
    // CARGAR CATÁLOGO INICIAL
    // ======================================

    await ProductSeeder.SeedAsync(context);
}


// ==========================================
// INICIAR APLICACIÓN
// ==========================================

app.Run();