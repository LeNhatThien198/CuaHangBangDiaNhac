using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBangDiaNhac.Models
{
    public enum VersionType
    {
        [Display(Name = "Original - Phát hành gốc")]
        Original,
        [Display(Name = "Repress - Tái phát hành")]
        Repress,
        [Display(Name = "Reissue - Tái phát hành lại")]
        Reissue,
        [Display(Name = "Used - Hàng cũ")]
        Used
    }

    public enum ReleaseCondition
    {
        [Display(Name = "M (Mint)")]
        Mint,
        [Display(Name = "NM (Near Mint)")]
        NearMint,
        [Display(Name = "VG+ (Very Good Plus)")]
        VeryGoodPlus,
        [Display(Name = "VG (Very Good)")]
        VeryGood,
        [Display(Name = "G (Good)")]
        Good,
        [Display(Name = "F (Fair)")]
        Fair,
        [Display(Name = "P (Poor)")]
        Poor
    }

    public class ReleaseVersion
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Loại phiên bản")]
        public VersionType VersionType { get; set; }

        [MaxLength(50, ErrorMessage = "Barcode tối đa 50 ký tự")]
        [Display(Name = "Mã barcode")]
        public string? Barcode { get; set; }

        [Required]
        [Display(Name = "Ngày phát hành")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [Display(Name = "Tình trạng")]
        public ReleaseCondition Condition { get; set; }

        [MaxLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [Required]
        [Display(Name = "Sản phẩm")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public ICollection<Track> Tracks { get; set; } = new List<Track>();

        public DigitalAsset? DigitalAsset { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
