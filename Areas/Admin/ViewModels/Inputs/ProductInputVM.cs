using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs
{
    public class ProductInputVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Display(Name = "Tên Album/Đĩa")]
        public string Name { get; set; }

        [Display(Name = "Mô tả chi tiết")]
        public string? Description { get; set; }

        [Display(Name = "Tracklist (Danh sách bài hát)")]
        public string? Tracklist { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        [Display(Name = "Giá bán")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Giá khuyến mãi")]
        public decimal? PromotionPrice { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Giá vốn")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(0, int.MaxValue)]
        [Display(Name = "Số lượng tồn kho")]
        public int Quantity { get; set; }

        [Display(Name = "Hiển thị sản phẩm")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Tình trạng đĩa")]
        [MaxLength(50)]
        public string? Condition { get; set; }

        [Display(Name = "Nước sản xuất")]
        [MaxLength(50)]
        public string? Country { get; set; }

        [Display(Name = "Năm phát hành")]
        public int? ReleaseYear { get; set; }

        [Display(Name = "Đĩa cũ")]
        public bool IsUsed { get; set; }

        [Display(Name = "Pre-order")]
        public bool IsPreOrder { get; set; }

        [Display(Name = "Ngày phát hành dự kiến")]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Ảnh bìa chính")]
        [ValidateNever]
        public IFormFile? CoverImage { get; set; }

        [Display(Name = "Ảnh chi tiết")]
        [ValidateNever]
        public List<IFormFile>? GalleryImages { get; set; }

        [ValidateNever]
        public string? CurrentCoverUrl { get; set; }

        [ValidateNever]
        public List<ProductImage>? CurrentGallery { get; set; } = new List<ProductImage>();


        [Required(ErrorMessage = "Vui lòng chọn nghệ sĩ")]
        [Display(Name = "Nghệ sĩ")]
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà phát hành")]
        [Display(Name = "Nhà phát hành")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn định dạng")]
        [Display(Name = "Định dạng")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        [Display(Name = "Thể loại chính")]
        public int GenreId { get; set; }

        [Display(Name = "Phong cách")]
        public int? StyleId { get; set; }
    }
}