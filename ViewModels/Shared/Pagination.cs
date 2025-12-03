namespace CuaHangBangDiaNhac.ViewModels.Shared
{
    public class Pagination
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasOtherPage => TotalPages > 1;
    }
}
