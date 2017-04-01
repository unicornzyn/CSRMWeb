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
            var areaid = Request.Form["selarea"];
            return Json(new { Result = 1, Msg = "保存成功！"}, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 会议日程
        /// </summary>
        /// <returns></returns>
        public ActionResult hyrc()
        {
            return View();
        }
        /// <summary>
        /// 会议讲义
        /// </summary>
        /// <returns></returns>
        public ActionResult hyjy()
        {
            return View();
        }
        /// <summary>
        /// 讲者介绍
        /// </summary>
        /// <returns></returns>
        public ActionResult jzjs()
        {
            return View();
        }
        /// <summary>
        /// 指南解读
        /// </summary>
        /// <returns></returns>
        public ActionResult znjd()
        {
            return View();
        }
        /// <summary>
        /// 酒店交通
        /// </summary>
        /// <returns></returns>
        public ActionResult jdjt()
        {
            return View();
        }
        /// <summary>
        /// 精彩瞬间
        /// </summary>
        /// <returns></returns>
        public ActionResult jcsj()
        {
            return View();
        }
        /// <summary>
        /// 联系我们
        /// </summary>
        /// <returns></returns>
        public ActionResult lxwm()
        {
            return View();
        }


    }
}
