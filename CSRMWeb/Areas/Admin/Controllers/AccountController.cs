using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSRMWeb.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Admin/Account/

        public ActionResult SignIn()
        {
            ViewBag.err = Request.QueryString["err"];
            return View();
        }

        public ActionResult SignInPost(string inputEmail, string inputPassword)
        {
            if (inputEmail == "admin" && inputPassword == "1")
            {
                return RedirectToAction("Index", "Manager");
            }
            else
            {
                return RedirectToAction("SignIn", new { err = 2 });
            }
        }
    }
}
