using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? CouponId { get; set; }

        public DateTime Date { get; set; }

        public virtual Coupon? Coupon { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}
