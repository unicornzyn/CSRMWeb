using CSRMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CSRMWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() 
        {
            var wx = new OpenAuthWX(System.Configuration.ConfigurationManager.AppSettings["wxappid"], System.Configuration.ConfigurationManager.AppSettings["wxappsecret"], 2);
            
            var token = wx.SecondPart();
            ViewBag.openid = token.openid;
            return View();
        }      
    }
}
