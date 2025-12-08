using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels
{
    public class ProductListVM
    {
        public int Id { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; } 

        [Display(Name = "Giá bán")]
        public decimal Price { get; set; }

        [Display(Name = "Giá khuyến mãi")]
        public decimal? PromotionPrice { get; set; } 

        [Display(Name = "Giá vốn")]
        public decimal Cost { get; set; }

        [Display(Name = "Tồn kho")]
        public int Quantity { get; set; }


        [Display(Name = "Nghệ sĩ")]
        public string ArtistName { get; set; }

        [Display(Name = "Định dạng")]
        public string CategoryName { get; set; } 

        [Display(Name = "Nhà phát hành")]
        public string BrandName { get; set; }    

        [Display(Name = "Thể loại")]
        public string GenreName { get; set; }    

        [Display(Name = "Phong cách")]
        public string StyleName { get; set; }    

        public bool IsUsed { get; set; }      
        public bool IsPreOrder { get; set; }  
        public bool IsPublished { get; set; } 
    }
}