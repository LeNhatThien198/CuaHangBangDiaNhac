using CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels
{
    public class GenreManagerVM
    {
        public List<Genre> Genres { get; set; } = new List<Genre>();

        public GenreInputVM NewGenre { get; set; } = new GenreInputVM();
        public StyleInputVM NewStyle { get; set; } = new StyleInputVM();
    }
}