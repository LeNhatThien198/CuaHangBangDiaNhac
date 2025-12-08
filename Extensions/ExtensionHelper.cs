using System.Globalization;

namespace CuaHangBangDiaNhac.Extensions
{
    public static class ExtensionHelper
    {
        public static string ToVnd(this decimal price)
        {
            if (price <= 0) return "Liên hệ";

            var culture = CultureInfo.GetCultureInfo("vi-VN");

            return price.ToString("#,##0", culture.NumberFormat) + " VNĐ";
        }
    }
}