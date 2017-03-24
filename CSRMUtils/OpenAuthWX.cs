using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net.Json;

namespace CSRMUtils
{
    public class OpenAuthWX
    {
        private static string AppID = "";
        private static string AppSecret = "";
        private static int WXType = 0;
        private string RedirectAuthUrl = "";
        private string StrReturnUrl = "";
        private HttpResponse Response = null;
        private HttpRequest Request = null;
        /// <summary>
        /// 同意授权返回地址
        /// </summary>
        public string RedirectUri
        {
            get { return RedirectAuthUrl; }
            set { RedirectAuthUrl = value; }
        }
        /// <summary>
        ///  返回当前连接页面
        /// </summary>
        public string ReturnUrl
        {
            get { return StrReturnUrl; }
            set { StrReturnUrl = value; }
        }
        /// <summary>
        /// 配置APPID值与AppSecret值
        /// </summary>
        /// <param name="appid">APPID</param>
        /// <param name="appsecret">AppSecret</param>
        /// <param name="type">0:服务账号/1:开放平台帐号</param>
        public OpenAuthWX(string appid, string appsecret, int type)
        {
            AppID = appid;
            AppSecret = appsecret;
            WXType = type;
            Request = HttpContext.Current.Request;
            Response = HttpContext.Current.Response;
        }
        /// <summary>
        /// 用户同意授权，获取code
        /// </summary>
        public string FirstPartUrl()
        {
            string CallUrl = "";
            if (WXType == 0)
            {
                CallUrl = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&state={2}#wechat_redirect",
                    AppID, System.Web.HttpUtility.UrlEncode(RedirectUri), ReturnUrl == "" ? Util.GenerateRndNonce() : System.Web.HttpUtility.UrlEncode(ReturnUrl));
            }
            else
            {
                CallUrl = string.Format("https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect",
                    AppID, System.Web.HttpUtility.UrlEncode(RedirectUri), ReturnUrl == "" ? Util.GenerateRndNonce() : System.Web.HttpUtility.UrlEncode(ReturnUrl));
            }
            return CallUrl;
        }
        /// <summary>
        /// 通过code换取网页授权access_token
        /// </summary>
        public WXToken SecondPart()
        {
            string StrCode = Request["code"];
            WXToken at = null;
            try
            {
                string CallUrl = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
                    AppID, AppSecret, StrCode);
                string JsonData = Util.HttpGERServer(CallUrl);
                if (JsonData != "")
                {
                    JsonTextParser parser = new JsonTextParser();
                    JsonObject obj = parser.Parse(JsonData);
                    JsonUtility.GenerateIndentedJsonText = false;
                    JsonObjectCollection JsonObj = obj as JsonObjectCollection;
                    if (Util.IsJsonJsonObjection(JsonObj))
                    {
                        at = new WXToken();
                        foreach (JsonObject field in JsonObj)
                        {
                            switch (field.Name)
                            {
                                case "access_token":
                                    at.access_token = field.GetValue().ToString();
                                    break;
                                case "expires_in":
                                    at.expires_in = Convert.ToInt32(field.GetValue().ToString());
                                    break;
                                case "refresh_token":
                                    at.refresh_token = field.GetValue().ToString();
                                    break;
                                case "openid":
                                    at.openid = field.GetValue().ToString();
                                    break;
                                case "scope":
                                    at.scope = field.GetValue().ToString();
                                    break;
                                case "unionid":
                                    at.unionid = field.GetValue().ToString();
                                    break;
                                case "errcode":
                                    at.errcode = field.GetValue().ToString();
                                    break;
                                case "errmsg":
                                    at.errmsg = field.GetValue().ToString();
                                    break;
                            }
                        }
                        if (at.errcode != "" && at.errmsg != "")
                        {
                            at = null;
                        }
                    }
                }
            }
            catch (Exception msg)
            {
                at = null;
                Response.Write("<h3>程序：" + msg.TargetSite + "--微信error:" + at.errcode + "</h3>");
                Response.Write("<h3>程序msg:" + msg.Message + "--微信msg:" + at.errmsg + "</h3>");
                Response.End();
            }
            return at;
        }
        /// <summary>
        /// 刷新access_token接口（如果需要）
        /// </summary>
        /// <param name="access_token">填写通过access_token获取到的refresh_token参数 </param>
        /// <param name="refresh_token">填写为refresh_token </param>
        /// <returns>刷新后Refresh_Token对象</returns>
        public WXToken_Refresh ThirdPart(string access_token, string refresh_token)
        {
            WXToken_Refresh rt = null;
            try
            {
                string CallUrl = string.Format("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type={1}&refresh_token={2}",
                       AppID, refresh_token, access_token);
                string JsonData = Util.HttpGERServer(CallUrl);
                if (JsonData != "")
                {
                    JsonTextParser parser = new JsonTextParser();
                    JsonObject obj = parser.Parse(JsonData);
                    JsonUtility.GenerateIndentedJsonText = false;
                    JsonObjectCollection JsonObj = obj as JsonObjectCollection;
                    if (Util.IsJsonJsonObjection(JsonObj))
                    {
                        rt = new WXToken_Refresh();
                        foreach (JsonObject field in JsonObj)
                        {
                            switch (field.Name)
                            {
                                case "access_token":
                                    rt.access_token = field.GetValue().ToString();
                                    break;
                                case "expires_in":
                                    rt.expires_in = Convert.ToInt32(field.GetValue().ToString());
                                    break;
                                case "refresh_token":
                                    rt.refresh_token = field.GetValue().ToString();
                                    break;
                                case "openid":
                                    rt.openid = field.GetValue().ToString();
                                    break;
                                case "scope":
                                    rt.scope = field.GetValue().ToString();
                                    break;
                                case "errcode":
                                    rt.errcode = field.GetValue().ToString();
                                    break;
                                case "errmsg":
                                    rt.errmsg = field.GetValue().ToString();
                                    break;
                            }
                        }
                        if (rt.errcode != "" && rt.errmsg != "")
                        {
                            rt = null;
                        }
                    }
                }
            }
            catch (Exception msg)
            {
                rt = null;
                Response.Write("<h3>程序：" + msg.TargetSite + "--微信error:" + rt.errcode + "</h3>");
                Response.Write("<h3>程序msg:" + msg.Message + "--微信msg:" + rt.errmsg + "</h3>");
                Response.End();
            }
            return rt;
        }
        /// <summary>
        /// 拉取用户信息(需scope为 snsapi_userinfo)
        /// </summary>
        /// <param name="access_token">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同 </param>
        /// <param name="openid">用户的唯一标识 </param>
        /// <returns>返回WXUser对象</returns>
        public WXUser FourthPart(string access_token, string openid)
        {
            WXUser user = null;
            try
            {
                 string CallUrl = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN",
                        access_token, openid);
                string JsonData =Util.HttpGERServer(CallUrl);
                if (JsonData != "")
                {
                    JsonTextParser parser = new JsonTextParser();
                    JsonObject obj = parser.Parse(JsonData);
                    JsonUtility.GenerateIndentedJsonText = false;
                    JsonObjectCollection JsonObj = obj as JsonObjectCollection;
                    if (Util.IsJsonJsonObjection(JsonObj))
                    {
                        user = new WXUser();
                        foreach (JsonObject field in JsonObj)
                        {
                            switch (field.Name)
                            {
                                case "openid":
                                    user.openid = field.GetValue().ToString();
                                    break;
                                case "nickname":
                                    user.nickname = field.GetValue().ToString();
                                    break;
                                case "sex":
                                    int nSex = Convert.ToInt32(field.GetValue().ToString());
                                    string str = "未知 ";
                                    switch (nSex)
                                    {
                                        case 1:
                                            str = "男性";
                                            break;
                                        case 2:
                                            str = "女性";
                                            break;
                                    }
                                    user.sex = str;
                                    break;
                                case "province":
                                    user.province = field.GetValue().ToString();
                                    break;
                                case "city":
                                    user.province = field.GetValue().ToString();
                                    break;
                                case "country":
                                    user.country = field.GetValue().ToString();
                                    break;
                                case "headimgurl":
                                    user.headimgurl = field.GetValue().ToString();
                                    break;
                                case "privilege":
                                    user.privilege = field.GetValue().ToString();
                                    break;
                                case "unionid":
                                    user.unionid = field.GetValue().ToString();
                                    break;
                                case "errcode":
                                    user.errcode = field.GetValue().ToString();
                                    break;
                                case "errmsg":
                                    user.errmsg = field.GetValue().ToString();
                                    break;
                            }
                        }
                        if (user.errcode != "" && user.errmsg != "")
                        {
                            user = null;
                        }
                    }
                }
            }
            catch (Exception msg)
            {
                user = null;
                Response.Write("<h3>程序：" + msg.TargetSite + "--微信error:" + user.errcode + "</h3>");
                Response.Write("<h3>程序msg:" + msg.Message + "--微信msg:" + user.errmsg + "</h3>");
                Response.End();
            }
            return user;
        }
    }
    public class WXToken
    {
        private string strAccess_Token = "";
        private int strExpires_In = 0;
        private string strRefresh_Token = "";
        private string strOpenid = "";
        private string strScope = "";
        private string strUnionid = "";
        private string strErrCode = "";
        private string strErrMsg = "";
        public string access_token
        {
            get { return strAccess_Token; }
            set { strAccess_Token = value; }
        }
        public int expires_in
        {
            get { return strExpires_In; }
            set { strExpires_In = value; }
        }
        public string refresh_token
        {
            get { return strRefresh_Token; }
            set { strRefresh_Token = value; }
        }
        public string openid
        {
            get { return strOpenid; }
            set { strOpenid = value; }
        }
        public string scope
        {
            get { return strScope; }
            set { strScope = value; }
        }
        public string unionid
        {
            get { return strUnionid; }
            set { strUnionid = value; }
        }
        public string errcode
        {
            get { return strErrCode; }
            set { strErrCode = value; }
        }
        public string errmsg
        {
            get { return strErrMsg; }
            set { strErrMsg = value; }
        }
    }
    public class WXToken_Refresh
    {
        private string strAccess_Token = "";
        private int strExpires_In = 0;
        private string strRefresh_Token = "";
        private string strOpenid = "";
        private string strScope = "";
        private string strErrCode = "";
        private string strErrMsg = "";
        public string access_token
        {
            get { return strAccess_Token; }
            set { strAccess_Token = value; }
        }
        public int expires_in
        {
            get { return strExpires_In; }
            set { strExpires_In = value; }
        }
        public string refresh_token
        {
            get { return strRefresh_Token; }
            set { strRefresh_Token = value; }
        }
        public string openid
        {
            get { return strOpenid; }
            set { strOpenid = value; }
        }
        public string scope
        {
            get { return strScope; }
            set { strScope = value; }
        }
        public string errcode
        {
            get { return strErrCode; }
            set { strErrCode = value; }
        }
        public string errmsg
        {
            get { return strErrMsg; }
            set { strErrMsg = value; }
        }
    }
    public class WXUser
    {
        private string strOpenid = "";
        private string strNickName = "";
        private string strSex = "";
        private string strProvince = "";
        private string strCity = "";
        private string strCountry = "";
        private string strHeadImgUrl = "";
        private string strPrivilege = "";
        private string strUnionid = "";
        private string strErrCode = "";
        private string strErrMsg = "";
        public string openid
        {
            get { return strOpenid; }
            set { strOpenid = value; }
        }
        public string nickname
        {
            get { return strNickName; }
            set { strNickName = value; }
        }
        public string sex
        {
            get { return strSex; }
            set { strSex = value; }
        }
        public string province
        {
            get { return strProvince; }
            set { strProvince = value; }
        }
        public string city
        {
            get { return strCity; }
            set { strCity = value; }
        }
        public string country
        {
            get { return strCountry; }
            set { strCountry = value; }
        }
        public string headimgurl
        {
            get { return strHeadImgUrl; }
            set { strHeadImgUrl = value; }
        }
        /// <summary>
        /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom） 
        /// </summary>
        public string privilege
        {
            get { return strPrivilege; }
            set { strPrivilege = value; }
        }
        public string unionid
        {
            get { return strUnionid; }
            set { strUnionid = value; }
        }
        public string errcode
        {
            get { return strErrCode; }
            set { strErrCode = value; }
        }
        public string errmsg
        {
            get { return strErrMsg; }
            set { strErrMsg = value; }
        }
    }
    public class WXAuthACCESS
    {
        private string strErrCode = "";
        private string strErrMsg = "";
        public string errcode
        {
            get { return strErrCode; }
            set { strErrCode = value; }
        }
        public string errmsg
        {
            get { return strErrMsg; }
            set { strErrMsg = value; }
        }
    }
}
