using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment content is mandatory!")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? ArticleId { get; set; }

        public int Rating { get; set; }
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual Article? Article { get; set; }
    }
}
