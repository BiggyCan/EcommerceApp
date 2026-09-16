using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(150)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string CustomerEmail { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ShippingAddress { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Confirmado";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}