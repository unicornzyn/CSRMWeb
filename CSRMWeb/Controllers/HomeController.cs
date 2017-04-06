using CSRMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSRMDAL;
using CSRMWeb.Filter;


namespace CSRMWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
#if DEBUG
            ViewBag.isreg = 1;
            Session["openid"] = "1";
            Session["areaid"] = 1;
#else
            var wx = new OpenAuthWX(System.Configuration.ConfigurationManager.AppSettings["wxappid"], System.Configuration.ConfigurationManager.AppSettings["wxappsecret"], 2);

            var token = wx.SecondPart();

            if (token == null)
            {
                return Redirect(St.GetLoginUrl());
            }
            var openid = token.openid;

            Session["openid"] = token.openid;


            DBConnection db = new DBConnection();
            var o = db.users.FirstOrDefault(a => a.openid == openid);
            if (o != null)
            {
                Session["areaid"] = o.areaid;
                ViewBag.isreg = 1;
            }
            else
            {
                ViewBag.isreg = 0;
            }

#endif
            

            


            return View();
        }
        [CustAuthorizeAttribute()]
        public ActionResult huiyijianjie() 
        {
            var areaid = Convert.ToInt32(Session["areaid"]);
            DBConnection db = new DBConnection();
            var o = db.huiyijianjie.FirstOrDefault(a => a.areaid == areaid);

            return View(o);
        }
        [CustAuthorizeAttribute()]
        public ActionResult huiyijiangyi()
        {
            var areaid = Convert.ToInt32(Session["areaid"]);
            DBConnection db = new DBConnection();
            var list = db.huiyijiangyi.Where(a => a.areaid == areaid).ToList();

            return View(list);
        }
        [CustAuthorizeAttribute()]
        public ActionResult huiyirichen()
        {
            var areaid = Convert.ToInt32(Session["areaid"]);
            DBConnection db = new DBConnection();
            CSRMDAL.Model.huiyiricheng_ext x = new CSRMDAL.Model.huiyiricheng_ext();
            x.o = db.huiyirichen.FirstOrDefault(a => a.areaid == areaid);
            x.list = db.huiyirichen_c.Where(a => a.pid == x.o.id).ToList();
            return View(x);
        }
        [CustAuthorizeAttribute()]
        public ActionResult jiangzhejieshao()
        {
            var areaid = Convert.ToInt32(Session["areaid"]);
            DBConnection db = new DBConnection();
            var list = db.jiangzhejieshao.Where(a => a.areaid == areaid).ToList();

            return View(list);
        }
        [CustAuthorizeAttribute()]
        public ActionResult jiaotong()
        {
            var areaid = Convert.ToInt32(Session["areaid"]);
            DBConnection db = new DBConnection();
            CSRMDAL.Model.jiaotong_ext x = new CSRMDAL.Model.jiaotong_ext();
            x.o = db.jiaotong.FirstOrDefault(a => a.areaid == areaid);
            x.list = db.jiaotong_c.Where(a => a.pid == x.o.id).ToList();
            return View(x);
        }
        [CustAuthorizeAttribute()]
        public ActionResult lianxiwomen()
        {
            var areaid = Convert.ToInt32(Session["areaid"]);
            DBConnection db = new DBConnection();
            var o = db.lianxiwomen.FirstOrDefault(a => a.areaid == areaid);

            return View(o);
        }
        [CustAuthorizeAttribute()]
        public ActionResult shipin()
        {
            var areaid = Convert.ToInt32(Session["areaid"]);
            DBConnection db = new DBConnection();

            var list = db.shipin.Where(a => a.areaid == areaid).OrderBy(a => Guid.NewGuid()).ToList();
            if (list.Count() > 0)
            {
                ViewBag.img = "/upload/" + list.First().img;
                list.Remove(list.First());
            }
            else
            {
                ViewBag.img = "";
            }
            return View(list);
        }
        [CustAuthorizeAttribute()]
        public ActionResult zhinan()
        {
            var areaid = Convert.ToInt32(Session["areaid"]);
            DBConnection db = new DBConnection();
            var o = db.zhinan.FirstOrDefault(a => a.areaid == areaid);

            return View(o);
        }
        [CustAuthorizeAttribute()]
        public ActionResult zaixianbaoming()
        {
            var openid = Session["openid"].ToString();
            DBConnection db = new DBConnection();

            if (db.users.Where(a => a.openid == openid).Count() > 0)
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
                return Json(new { result = 1 }, JsonRequestBehavior.DenyGet);
            }
            else 
            {
                return Json(new { result = 2 }, JsonRequestBehavior.DenyGet);
            }

            
        }
    }
}
