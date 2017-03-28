using CSRMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSRMDAL;


namespace CSRMWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var wx = new OpenAuthWX(System.Configuration.ConfigurationManager.AppSettings["wxappid"], System.Configuration.ConfigurationManager.AppSettings["wxappsecret"], 2);

            var token = wx.SecondPart();

            Session["openid"] = token.openid;            

            return RedirectToAction("zaixianbaoming", "Home");
        }

        public ActionResult zaixianbaoming()
        {
            if (Session["openid"] == null)
            {
                return Redirect(St.GetLoginUrl());                
            }
            else
            {
                var openid = Session["openid"].ToString();
                DBConnection db = new DBConnection();
                
                if (db.users.Where(a => a.openid == openid ).Count() > 0)
                {
                    ViewBag.isreg = 1;
                }
                else
                {
                    ViewBag.isreg = 0;
                }

                WXShare wx = WXShareTool.ShareModel(Request.Url.ToString());

                ViewBag.appid = System.Configuration.ConfigurationManager.AppSettings["wxappid"];
                ViewBag.timestamp = wx.timestamp;
                ViewBag.nonceStr = wx.nonceStr;
                ViewBag.signature = wx.signature;

                ViewBag.provinces = Newtonsoft.Json.JsonConvert.SerializeObject(db.province.ToList());
                ViewBag.citys = Newtonsoft.Json.JsonConvert.SerializeObject(db.city.ToList());
                return View();
            }
        }
        [HttpPost]
        public JsonResult baoming(int areaid, int pid,string pname, int cid,string cname, string name, string tel, string work, string dept, int age, string tech)
        {
            if (Session["openid"] == null)
            {
                return Json(new { result = 0, msg = St.GetLoginUrl() }, JsonRequestBehavior.DenyGet);
            }
            DBConnection db = new DBConnection();   
            string openid = Session["openid"].ToString();
            if (db.users.Where(a => a.openid == openid).Count() == 0) 
            {
                var o = db.users.Create();               
                o.openid = openid;
                o.areaid = areaid;
                o.provinceid = pid;
                o.provincename = pname;
                o.cityid = cid;
                o.cityname = cname;
                o.name = name;
                o.telnum = tel;
                o.workaddr = work;
                o.department = dept;
                o.age = age;
                o.techtitle = tech;
                o.addtime = DateTime.Now;
                db.users.Add(o);
                db.SaveChanges();
            }

            return Json(new { result = 1 }, JsonRequestBehavior.DenyGet);
        }
    }
}
