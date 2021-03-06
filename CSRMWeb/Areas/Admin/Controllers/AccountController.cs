﻿using System;
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
            Session["returnurl"] = Request.QueryString["ReturnUrl"];
            ViewBag.err = Request.QueryString["err"];
            return View();
        }

        public ActionResult SignInPost(string inputEmail, string inputPassword)
        {
            if (inputEmail == System.Configuration.ConfigurationManager.AppSettings["adminuser"] && inputPassword == System.Configuration.ConfigurationManager.AppSettings["adminpwd"])
            {
                Session["adminid"] = 1;
                if (Session["returnurl"] == null)
                {
                    return RedirectToAction("hyjj", "Manager");
                }
                else
                {
                    return Redirect(Session["returnurl"].ToString());
                }
            }
            else
            {
                return RedirectToAction("SignIn", new { err = 2, ReturnUrl = Session["returnurl"].ToString() });
            }
        }
    }
}
