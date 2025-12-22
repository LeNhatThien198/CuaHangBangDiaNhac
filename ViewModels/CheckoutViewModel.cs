using CuaHangBangDiaNhac.Models;
using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.ViewModels
{
    public class CheckoutViewModel
    {
        // User's available addresses for selection
        public List<Address> Addresses { get; set; } = new List<Address>();

        // Selected Address ID (bound on form post)
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn địa chỉ nhận hàng")]
        public int SelectedAddressId { get; set; }

        // Grouped Shipments
        public List<CheckoutShipmentViewModel> Shipments { get; set; } = new List<CheckoutShipmentViewModel>();

        // Logic Helpers
        public bool HasPreOrder => Shipments.Any(s => s.IsPreOrder);
        public List<int> SelectedIds { get; set; } = new List<int>();

        // Payment Method
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD"; // Default

        // Grand Totals
        public decimal TotalMerchandiseAmount => Shipments.Sum(s => s.MerchandiseAmount);
        public decimal TotalShippingFee => Shipments.Sum(s => s.ShippingFee);
        public decimal GrandTotal => TotalMerchandiseAmount + TotalShippingFee;
    }

    public class CheckoutShipmentViewModel
    {
        public int ShipmentId { get; set; } // 1, 2, 3... just for UI indexing
        public string Title { get; set; } // "Đơn hàng có sẵn" or "Đơn đặt trước (Dự kiến: dd/MM/yyyy)"
        public bool IsPreOrder { get; set; }
        public DateTime? ReleaseDate { get; set; }
        
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        public decimal MerchandiseAmount => Items.Sum(i => (i.PromotionPrice ?? i.Price) * i.Quantity);
        public decimal ShippingFee { get; set; } = 30000; // Fixed 30k per shipment
        public decimal TotalAmount => MerchandiseAmount + ShippingFee;
    }
}
