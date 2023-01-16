using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int? OrderId { get; set; } 
        public int? ArticleId { get; set; }
        public int ArticleCount { get; set; }

        public virtual Order? Order { get; set; }

        public virtual Article? Article { get; set;}
    }
}
