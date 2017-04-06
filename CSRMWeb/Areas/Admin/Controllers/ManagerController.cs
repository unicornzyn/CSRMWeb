using CSRMDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSRMWeb.Areas.Admin.Controllers
{
    [Filter.AdminAuthoorize]
    public class ManagerController : Controller
    {
        //
        // GET: /Admin/Manager/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 会议简介
        /// </summary>
        /// <returns></returns>
        public ActionResult hyjj()
        {
            return View();
        }
        public JsonResult hyjjPost()
        {
            var areaid = Convert.ToInt32(Request.Form["selarea"]);
            var txtinfo = Request.Form["txtinfo"];

            var db = new DBConnection();
            var o = db.huiyijianjie.FirstOrDefault(a => a.areaid == areaid);
            if (o == null)
            {
                o = new huiyijianjie();
                o.areaid = areaid;
                o.info = txtinfo;
                o.addtime = DateTime.Now;
                db.huiyijianjie.Add(o);
            }
            else
            {
                o.info = txtinfo;
                o.addtime = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult hyjjGet(int areaid)
        {
            var db = new DBConnection();
            var o = db.huiyijianjie.FirstOrDefault(a => a.areaid == areaid);
            if (o == null)
            {
                o = new huiyijianjie();
            }

            return Json(new { Result = 1, Msg = "获取成功！", Data = o }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 会议日程
        /// </summary>
        /// <returns></returns>
        public ActionResult hyrc()
        {
            return View();
        }
        public JsonResult hyrcPost()
        {
            var areaid = Convert.ToInt32(Request.Form["selarea"]);
            var txtdate = Request.Form["txtdate"];
            var txttime = Request.Form["txttime"];
            var areaname = GetAreaName(areaid);

            var db = new DBConnection();
            var o = db.huiyirichen.FirstOrDefault(a => a.areaid == areaid);
            if (o == null)
            {
                o = new huiyirichen();
                o.areaid = areaid;
                o.date = txtdate;
                o.time = txttime;
                o.areastr = areaname;
                o.addtime = DateTime.Now;
                db.huiyirichen.Add(o);
            }
            else
            {
                o.date = txtdate;
                o.time = txttime;
                o.addtime = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult hyrcGet(int areaid)
        {
            var db = new DBConnection();
            var o = db.huiyirichen.FirstOrDefault(a => a.areaid == areaid);
            IList<huiyirichen_c> list;
            if (o == null)
            {
                o = new huiyirichen();
                list = new List<huiyirichen_c>();
            }
            else
            {
                list = db.huiyirichen_c.Where(a => a.pid == o.id).ToList();
            }

            return Json(new { Result = 1, Msg = "获取成功！", Data = new { o = o, list = list } }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult hyrcDel(int id)
        {
            var db = new DBConnection();
            var o = db.huiyirichen_c.FirstOrDefault(a => a.id == id);
            if (o != null)
            {
                db.huiyirichen_c.Remove(o);
                db.SaveChanges();
            }
            return Json(new { Result = 1, Msg = "删除成功！" });
        }
        public JsonResult hyrcAddSave()
        {
            var pid = Convert.ToInt32(Request.Form["pid"]);
            var cid = Convert.ToInt32(Request.Form["cid"]);
            var txt1 = Request.Form["txt1"];
            var txt2 = Request.Form["txt2"];
            var txt3 = Request.Form["txt3"];

            var db = new DBConnection();

            if (cid > 0)
            {
                var o = db.huiyirichen_c.FirstOrDefault(a => a.id == cid);
                if (o != null)
                {
                    o.time = txt1;
                    o.title = txt2;
                    o.author = txt3;
                    o.addtime = DateTime.Now;
                }
            }
            else
            {
                var o = new huiyirichen_c();
                o.pid = pid;
                o.time = txt1;
                o.title = txt2;
                o.author = txt3;
                o.addtime = DateTime.Now;
                db.huiyirichen_c.Add(o);
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 会议讲义
        /// </summary>
        /// <returns></returns>
        public ActionResult hyjy()
        {
            return View();
        }
        public JsonResult hyjyPost()
        {
            var areaid = Convert.ToInt32(Request.Form["selarea"]);
            var txtid = Convert.ToInt32(Request.Form["txtid"]);
            var txtname = Request.Form["txtname"];
            var txtunit = Request.Form["txtunit"];
            var txtauthor = Request.Form["txtauthor"];
            var txtimg = Request.Form["txtimg"];

            var db = new DBConnection();
            if (txtid == 0)
            {
                var o = new huiyijiangyi();
                o.areaid = areaid;
                o.name = txtname;
                o.unit = txtunit;
                o.author = txtauthor;
                o.pptname = txtimg;
                o.addtime = DateTime.Now;
                db.huiyijiangyi.Add(o);
            }
            else
            {
                var o = db.huiyijiangyi.FirstOrDefault(a => a.id == txtid);
                if (o != null)
                {
                    o.name = txtname;
                    o.unit = txtunit;
                    o.author = txtauthor;
                    o.pptname = txtimg;
                    o.addtime = DateTime.Now;
                }
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult hyjyGet(int areaid)
        {
            var db = new DBConnection();
            List<huiyijiangyi> list = db.huiyijiangyi.Where(a => a.areaid == areaid).ToList();

            return Json(new { Result = 1, Msg = "获取成功！", Data = list }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult hyjyDel(int id)
        {
            var db = new DBConnection();
            var o = db.huiyijiangyi.FirstOrDefault(a => a.id == id);
            if (o != null)
            {
                db.huiyijiangyi.Remove(o);
                db.SaveChanges();
            }
            return Json(new { Result = 1, Msg = "删除成功！" });
        }
        /// <summary>
        /// 讲者介绍
        /// </summary>
        /// <returns></returns>
        public ActionResult jzjs()
        {
            return View();
        }
        public JsonResult jzjsPost()
        {
            var areaid = Convert.ToInt32(Request.Form["selarea"]);
            var txtid = Convert.ToInt32(Request.Form["txtid"]);
            var txtname = Request.Form["txtname"];
            var txtunit = Request.Form["txtunit"];
            var txtinfo = Request.Form["txtinfo"];
            var txtimg = Request.Form["txtimg"];

            var db = new DBConnection();
            if (txtid == 0)
            {
                var o = new jiangzhejieshao();
                o.areaid = areaid;
                o.name = txtname;
                o.unitname = txtunit;
                o.info = txtinfo;
                o.img = txtimg;
                o.addtime = DateTime.Now;
                db.jiangzhejieshao.Add(o);
            }
            else
            {
                var o = db.jiangzhejieshao.FirstOrDefault(a => a.id == txtid);
                if (o != null)
                {
                    o.name = txtname;
                    o.unitname = txtunit;
                    o.info = txtinfo;
                    o.img = txtimg;
                    o.addtime = DateTime.Now;
                }
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jzjsGet(int areaid)
        {
            var db = new DBConnection();
            List<jiangzhejieshao> list = db.jiangzhejieshao.Where(a => a.areaid == areaid).ToList();

            return Json(new { Result = 1, Msg = "获取成功！", Data = list }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jzjsDel(int id)
        {
            var db = new DBConnection();
            var o = db.jiangzhejieshao.FirstOrDefault(a => a.id == id);
            if (o != null)
            {
                db.jiangzhejieshao.Remove(o);
                db.SaveChanges();
            }
            return Json(new { Result = 1, Msg = "删除成功！" });
        }
        /// <summary>
        /// 指南解读
        /// </summary>
        /// <returns></returns>
        public ActionResult znjd()
        {
            return View();
        }
        public JsonResult znjdPost()
        {
            var areaid = Convert.ToInt32(Request.Form["selarea"]);
            var txttitle = Request.Form["txttitle"];
            var txtinfo = Request.Form["txtinfo"];

            var db = new DBConnection();
            var o = db.zhinan.FirstOrDefault(a => a.areaid == areaid);
            if (o == null)
            {
                o = new zhinan();
                o.areaid = areaid;
                o.title = txttitle;
                o.content = txtinfo;
                o.addtime = DateTime.Now;
                db.zhinan.Add(o);
            }
            else
            {
                o.title = txttitle;
                o.content = txtinfo;
                o.addtime = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult znjdGet(int areaid)
        {
            var db = new DBConnection();
            var o = db.zhinan.FirstOrDefault(a => a.areaid == areaid);
            if (o == null)
            {
                o = new zhinan();
            }

            return Json(new { Result = 1, Msg = "获取成功！", Data = o }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 酒店交通
        /// </summary>
        /// <returns></returns>
        public ActionResult jdjt()
        {
            return View();
        }
        public JsonResult jdjtPost()
        {
            var areaid = Convert.ToInt32(Request.Form["selarea"]);
            var txtimg = Request.Form["txtimg"];
            
            var db = new DBConnection();
            var o = db.jiaotong.FirstOrDefault(a => a.areaid == areaid);
            if (o == null)
            {
                o = new jiaotong();
                o.areaid = areaid;
                o.img = txtimg;
                o.addtime = DateTime.Now;
                db.jiaotong.Add(o);
            }
            else
            {
                o.img = txtimg;
                o.addtime = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jdjtGet(int areaid)
        {
            var db = new DBConnection();
            var o = db.jiaotong.FirstOrDefault(a => a.areaid == areaid);
            IList<jiaotong_c> list;
            if (o == null)
            {
                o = new jiaotong();
                list = new List<jiaotong_c>();
            }
            else
            {
                list = db.jiaotong_c.Where(a => a.pid == o.id).ToList();
            }

            return Json(new { Result = 1, Msg = "获取成功！", Data = new { o = o, list = list } }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jdjtDel(int id)
        {
            var db = new DBConnection();
            var o = db.jiaotong_c.FirstOrDefault(a => a.id == id);
            if (o != null)
            {
                db.jiaotong_c.Remove(o);
                db.SaveChanges();
            }
            return Json(new { Result = 1, Msg = "删除成功！" });
        }
        public JsonResult jdjtAddSave()
        {
            var pid = Convert.ToInt32(Request.Form["pid"]);
            var cid = Convert.ToInt32(Request.Form["cid"]);
            var txt1 = Request.Form["txt1"];
            var txtinfo = Request.Form["txtinfo"];            

            var db = new DBConnection();

            if (cid > 0)
            {
                var o = db.jiaotong_c.FirstOrDefault(a => a.id == cid);
                if (o != null)
                {
                    o.title = txt1;
                    o.content = txtinfo;
                    o.addtime = DateTime.Now;
                }
            }
            else
            {
                var o = new jiaotong_c();
                o.pid = pid;
                o.title = txt1;
                o.content = txtinfo;
                o.addtime = DateTime.Now;
                db.jiaotong_c.Add(o);
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 精彩瞬间
        /// </summary>
        /// <returns></returns>
        public ActionResult jcsj()
        {
            return View();
        }
        public JsonResult jcsjPost()
        {
            var areaid = Convert.ToInt32(Request.Form["selarea"]);
            var txtid = Convert.ToInt32(Request.Form["txtid"]);
            var txtimg = Request.Form["txtimg"];

            var db = new DBConnection();
            if (txtid == 0)
            {
                var o = new shipin();
                o.areaid = areaid;                
                o.img = txtimg;
                o.addtime = DateTime.Now;
                db.shipin.Add(o);
            }
            else
            {
                var o = db.shipin.FirstOrDefault(a => a.id == txtid);
                if (o != null)
                {                    
                    o.img = txtimg;
                    o.addtime = DateTime.Now;
                }
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jcsjGet(int areaid)
        {
            var db = new DBConnection();
            List<shipin> list = db.shipin.Where(a => a.areaid == areaid).ToList();

            return Json(new { Result = 1, Msg = "获取成功！", Data = list }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult jcsjDel(int id)
        {
            var db = new DBConnection();
            var o = db.shipin.FirstOrDefault(a => a.id == id);
            if (o != null)
            {
                db.shipin.Remove(o);
                db.SaveChanges();
            }
            return Json(new { Result = 1, Msg = "删除成功！" });
        }
        /// <summary>
        /// 联系我们
        /// </summary>
        /// <returns></returns>
        public ActionResult lxwm()
        {
            return View();
        }
        public JsonResult lxwmPost()
        {
            var areaid = Convert.ToInt32(Request.Form["selarea"]);
            var txttitle = Request.Form["txttitle"];
            var txtinfo = Request.Form["txtinfo"];
            var txtemail = Request.Form["txtemail"];
            var db = new DBConnection();
            var o = db.lianxiwomen.FirstOrDefault(a => a.areaid == areaid);
            if (o == null)
            {
                o = new lianxiwomen();
                o.areaid = areaid;
                o.title = txttitle;
                o.content = txtinfo;
                o.email = txtemail;
                o.addtime = DateTime.Now;
                db.lianxiwomen.Add(o);
            }
            else
            {
                o.title = txttitle;
                o.content = txtinfo;
                o.email = txtemail;
                o.addtime = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new { Result = 1, Msg = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult lxwmGet(int areaid)
        {
            var db = new DBConnection();
            var o = db.lianxiwomen.FirstOrDefault(a => a.areaid == areaid);
            if (o == null)
            {
                o = new lianxiwomen();
            }

            return Json(new { Result = 1, Msg = "获取成功！", Data = o }, JsonRequestBehavior.AllowGet);
        }

        public string upload()
        {
            var file = Request.Files[0];
            var filename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
            file.SaveAs(Server.MapPath("~/upload/") + filename);
            return filename;
        }

        private string GetAreaName(int areaid)
        {
            string name = "";
            switch (areaid)
            {
                case 1: name = "北京"; break;
                case 2: name = "上海"; break;
                case 3: name = "太原"; break;
                case 4: name = "福州"; break;
                case 5: name = "昆明"; break;
                default:
                    break;
            }
            return name;
        }
    }
}
