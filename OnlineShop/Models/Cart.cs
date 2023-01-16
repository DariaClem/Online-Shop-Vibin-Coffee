using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Cart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleId { get; set; }

        public int ArticleCount { get; set; }
        
        public string? UserId { get; set; }
        
        public virtual ApplicationUser? User { get; set; }
        
        public virtual Article? Article { get; set; }
    }
}
