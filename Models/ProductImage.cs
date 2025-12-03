using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Đường dẫn ảnh")]
        public string Url { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; } = false;

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}