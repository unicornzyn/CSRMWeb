using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSRMWeb.Filter
{
    public class AdminAuthoorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.Session == null || httpContext.Session["adminid"] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext != null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult() { Data = new { Result = 0, Msg = "登录超时，请重新登录！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Admin/Account/SignIn?ReturnUrl=" + filterContext.HttpContext.Request.RawUrl);
                }

            }

        }
    }
}