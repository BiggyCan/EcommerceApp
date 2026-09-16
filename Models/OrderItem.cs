using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [MaxLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal Subtotal
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }
    }
}