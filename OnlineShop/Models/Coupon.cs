using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Coupon
    {
        [Key]
        [Required(ErrorMessage = "Coupon code is mandatory!")]
        [MinLength(5, ErrorMessage = "Coupon code must have more than 5 characters")]
        [StringLength(10, ErrorMessage = "Coupon code can not have more than 10 characters!")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Coupon discount is mandatory!")]
        [Range(0, 100, ErrorMessage ="Coupon discount must be greater than 0 and less than 100!")]
        public int? Discount { get; set; }

    }
}
