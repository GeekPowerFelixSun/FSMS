using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSMS.Web.Areas.DemoManage.Controllers
{
    [Area("DemoManage")]
    public class IconController : Controller
    {
        public IActionResult FontAwesome()
        {
            return View();
        }
    }
}
