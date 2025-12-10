namespace CuaHangBangDiaNhac.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal TotalAmount => Items.Sum(i => i.Total);
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; } // Thumbnail URL
        public string ArtistName { get; set; }
        public int ArtistId { get; set; }
        public bool IsPreOrder { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; } // For validation

        public bool IsOutOfStock => MaxStock <= 0;
        public decimal DisplayPrice => PromotionPrice ?? Price;
        public decimal Total => DisplayPrice * Quantity;
    }
}
