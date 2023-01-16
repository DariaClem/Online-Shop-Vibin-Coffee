using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is mandatory!")]
        public string CategoryName { get; set; }

        public virtual ICollection<Article>? Articles { get; set; }
    }
}
