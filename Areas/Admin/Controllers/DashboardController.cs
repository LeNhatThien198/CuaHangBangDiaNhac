using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")] 
    [Authorize(Roles = "Admin,Staff")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
