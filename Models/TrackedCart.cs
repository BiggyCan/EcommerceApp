using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Models
{
    public class TrackedCart
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public string UserId { get; set; }
            = string.Empty;


        public ApplicationUser? User { get; set; }


        [Required]
        [MaxLength(30)]
        public string Status { get; set; }
            = "Activo";


        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;


        public DateTime LastActivityAt { get; set; }
            = DateTime.UtcNow;


        public DateTime? ConvertedAt { get; set; }


        public int? ConvertedOrderId { get; set; }


        public Order? ConvertedOrder { get; set; }


        public int TotalItems { get; set; }


        public decimal TotalAmount { get; set; }


        public ICollection<TrackedCartItem> Items { get; set; }
            = new List<TrackedCartItem>();
    }


    public class TrackedCartItem
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public int TrackedCartId { get; set; }


        public TrackedCart? TrackedCart { get; set; }


        [Required]
        public int ProductId { get; set; }


        public Product? Product { get; set; }


        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }
            = string.Empty;


        [Required]
        public decimal UnitPrice { get; set; }


        [Required]
        public int Quantity { get; set; }


        [NotMapped]
        public decimal Subtotal =>
            UnitPrice * Quantity;
    }
}