using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBangDiaNhac.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Precision(18, 2)]
        [Display(Name = "Phí vận chuyển")]
        public decimal ShippingFee { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Giảm giá")]
        public decimal Discount { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Số tiền đã cọc")]
        public decimal DepositAmount { get; set; } = 0;

        public string? UserId { get; set; }
        public User? User { get; set; }

        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddress { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        [NotMapped]
        [Display(Name = "Tổng tiền")]
        public decimal Total
        {
            get
            {
                decimal itemsTotal = 0;
                foreach (var i in Items) itemsTotal += i.UnitPrice * i.Quantity;
                return itemsTotal + ShippingFee - Discount;
            }
        }

        [Display(Name = "Phương thức thanh toán")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "Đơn cọc (Pre-order)")]
        public bool IsPreOrderDeposit { get; set; } = false;
    }

    public enum OrderStatus
    {
        [Display(Name = "Chờ xử lý")] Pending,
        [Display(Name = "Đang xử lý")] Processing,
        [Display(Name = "Đang vận chuyển")] Shipped,
        [Display(Name = "Đã giao hàng")] Delivered,
        [Display(Name = "Đã huỷ")] Cancelled,
        [Display(Name = "Đã trả hàng")] Returned
    }

    public enum PaymentMethod
    {
        [Display(Name = "Thanh toán khi nhận hàng (COD)")] COD,
        [Display(Name = "Chuyển khoản / Đặt cọc")] Deposit
    }
}