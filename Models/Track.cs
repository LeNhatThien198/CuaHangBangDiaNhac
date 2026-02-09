using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBangDiaNhac.Models
{
    public class Track
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số thứ tự phải lớn hơn 0")]
        [Display(Name = "Số thứ tự bài hát")]
        public int TrackNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bài hát")]
        [MaxLength(200, ErrorMessage = "Tên bài hát tối đa 200 ký tự")]
        [Display(Name = "Tên bài hát")]
        public string Title { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thời lượng phải ≥ 0")]
        [Display(Name = "Thời lượng (giây)")]
        public int Duration { get; set; }

        [Url(ErrorMessage = "URL preview không hợp lệ")]
        [Display(Name = "URL preview")]
        public string? PreviewUrl { get; set; }

        [Display(Name = "Phiên bản phát hành")]
        public int ReleaseVersionId { get; set; }

        [ForeignKey("ReleaseVersionId")]
        public ReleaseVersion? ReleaseVersion { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
