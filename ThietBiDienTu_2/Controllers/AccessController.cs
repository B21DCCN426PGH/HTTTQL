using Microsoft.AspNetCore.Mvc;
using ThietBiDienTu_2.Models;

namespace ThietBiDienTu_2.Controllers
{
    public class AccessController : Controller
    {
        ToolDbContext _context;

        public AccessController(ToolDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
           
            if (HttpContext.Session.GetInt32("UserName") == null)
            {
                return View();
                //If not login
            }
            else
            {
                if (HttpContext.Session.GetInt32("TypeAccount") == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                }
                //If has already login
            }
        }

        [HttpPost]
        public IActionResult Login(Taikhoan user)
        {
            
            if (HttpContext.Session.GetInt32("UserName") == null)
            {
                var u = _context.Taikhoans
                    .Where(x => x.Matk.Equals(user.Matk) && x.Matkhau.Equals(user.Matkhau))
                    .FirstOrDefault();
                //Check user and password

                if (u != null)
                {
                    HttpContext.Session.SetInt32("UserName", u.Matk);
                    HttpContext.Session.SetInt32("TypeAccount", (int)u.Loaitk);

                    if (HttpContext.Session.GetInt32("TypeAccount") == 0)
                    {

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                    }

                }
            }
            ModelState.AddModelError("Matkhau", "Tài khoản hoặc mật khẩu không hợp lệ");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login", "Access");
        }


    }
}
