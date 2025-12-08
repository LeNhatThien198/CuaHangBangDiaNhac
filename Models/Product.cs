using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBangDiaNhac.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [MaxLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Danh sách bài hát")]
        public string? Tracklist { get; set; }

        [Required]
        [Precision(18, 2)]
        [Display(Name = "Giá bán")]
        public decimal Price { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Giá khuyến mãi")]
        public decimal? PromotionPrice { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Giá vốn")]
        public decimal Cost { get; set; }

        [Display(Name = "Hiển thị")]
        public bool IsPublished { get; set; } = true;

        [Required]
        [Display(Name = "Tồn kho")]
        public int Quantity { get; set; }
        public bool IsUsed { get; set; } = false;

        [Display(Name = "Cho phép đặt trước")]
        public bool IsPreOrder { get; set; } = false;

        [Display(Name = "Ngày phát hành / Hàng về dự kiến")]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Nước sản xuất")]
        [MaxLength(50)]
        public string? Country { get; set; }

        [Display(Name = "Năm phát hành")]
        public int? ReleaseYear { get; set; }

        [Display(Name = "Tình trạng đĩa")]
        [MaxLength(50)]
        public string? Condition { get; set; }

        [Display(Name = "Ngày nhập")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Định dạng")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; } 
        [Display(Name = "Hãng đĩa")]
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        [Display(Name = "Nghệ sĩ")]
        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        [Display(Name = "Thể loại")]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        [Display(Name = "Phong cách")]
        public int? StyleId { get; set; }
        public Style? Style { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        [NotMapped]
        public string DisplayName
        {
            get
            {
                return Category != null ? $"{Name} - {Category.Name}" : Name;
            }
        }
    }
}