using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Photo
    {
        [Key]
        public string Id { get; set; }

        public int? ArticleId { get; set; }

        public bool isProfilePhoto { get; set; }

        public virtual Article? Article { get; set; }
    }
}
