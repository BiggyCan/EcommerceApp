using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres."
        )]
        [Display(Name = "Nombre completo")]
        public string FullName { get; set; } = string.Empty;


        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
        [StringLength(
            150,
            ErrorMessage = "El correo electrónico es demasiado largo."
        )]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "La contraseña debe tener como mínimo 6 caracteres."
        )]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debes confirmar tu contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare(
            nameof(Password),
            ErrorMessage = "Las contraseñas no coinciden."
        )]
        public string ConfirmPassword { get; set; } = string.Empty;


        [StringLength(
            200,
            ErrorMessage = "La dirección no puede superar los 200 caracteres."
        )]
        [Display(Name = "Dirección")]
        public string? Address { get; set; }
    }
}