using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSRMUtils
{
    public class WXShare
    {
        public string timestamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }
    }

    public class WXShareTool 
    {
        public static WXShare ShareModel(string url)
        {
            WXShare w = new WXShare();
            string jsapi_ticket = "";
            string key = "wxshare_jsapi_ticket";
            object obj = ProductCache.Get(key);
            if (obj == null)
            {
                string json = St.GetHTML(string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", System.Configuration.ConfigurationManager.AppSettings["wxappid"], System.Configuration.ConfigurationManager.AppSettings["wxappsecret"]));
                string access_token = St.GetFirstMatch(json, "access_token\":\"(\\S*?)\"");
                string json2 = St.GetHTML(string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", access_token));
                jsapi_ticket = St.GetFirstMatch(json2, "ticket\":\"(\\S*?)\"");
                ProductCache.Add(key, jsapi_ticket, 100);
            }
            else
            {
                jsapi_ticket = obj.ToString();
            }
            w.timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
            w.nonceStr = "wwwcsrmwebnet";
            string s = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsapi_ticket, w.nonceStr, w.timestamp, url);
            w.signature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "SHA1");
            return w;
        }
    }
}
