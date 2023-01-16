using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is mandatory!")]
        [StringLength(100, ErrorMessage = "Title can not have more than 100 characters!")]
        [MinLength(5, ErrorMessage = "Title must have more than 5 characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Content is mandatory!")]
        public string? Content { get; set; }
        
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Category is mandatory!")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Price is mandatory!")]
        public float? Price { get; set; }

        [Required(ErrorMessage = "Weight is mandatory!")]
        public int? Weight { get; set; }

        public bool Accepted { get; set; }

        public int? Rating { get; set; }
        
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Cart>? Carts { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<Photo>? Photos { get; set; }
        
        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }

    }
}
