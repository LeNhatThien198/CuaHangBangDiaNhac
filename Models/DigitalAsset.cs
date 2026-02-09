using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBangDiaNhac.Models
{
    public enum AssetType
    {
        [Display(Name = "Preview - Nhạc xem trước")]
        Preview,
        [Display(Name = "Full Album - Album đầy đủ")]
        FullAlbum,
        [Display(Name = "Artwork - Tác phẩm nghệ thuật")]
        Artwork
    }

    public enum AudioQuality
    {
        [Display(Name = "128 kbps")]
        Low,
        [Display(Name = "256 kbps")]
        Medium,
        [Display(Name = "320 kbps")]
        High,
        [Display(Name = "Lossless")]
        Lossless,
        [Display(Name = "Flac")]
        Flac
    }

    public class DigitalAsset
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Loại tài sản số")]
        public AssetType AssetType { get; set; }

        [Required(ErrorMessage = "URL tệp không được để trống")]
        [Url(ErrorMessage = "URL không hợp lệ")]
        [Display(Name = "URL tệp")]
        public string FileUrl { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "Kích thước phải ≥ 0")]
        [Display(Name = "Kích thước tệp (bytes)")]
        public long FileSize { get; set; }

        [Display(Name = "Chất lượng âm thanh")]
        public AudioQuality? AudioQuality { get; set; }

        [Display(Name = "Có watermark")]
        public bool HasWatermark { get; set; } = false;

        [Display(Name = "Hạn token")]
        public DateTime? TokenExpiry { get; set; }

        [Required]
        [Display(Name = "Phiên bản phát hành")]
        public int ReleaseVersionId { get; set; }

        [ForeignKey("ReleaseVersionId")]
        public ReleaseVersion? ReleaseVersion { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
