using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml.Linq;
using CSRMUtils;

namespace CSRMWeb.Controllers
{
    public class WXController : Controller
    {
        //help: https://mp.weixin.qq.com/wiki
        //
        // GET: /Home/

        public string api()
        {
            if (Request.HttpMethod == "GET")
            {
                string signature = Request.QueryString["signature"];
                string timestamp = Request.QueryString["timestamp"];
                string nonce = Request.QueryString["nonce"];
                string echostr = Request.QueryString["echostr"];

                string signstr = "";
                int i = Tencent.WXBizMsgCrypt.GenarateSinature(System.Configuration.ConfigurationManager.AppSettings["wxtoken"], timestamp, nonce, ref signstr);

                if (signature == signstr)
                {
                    return echostr;
                }
                else
                {
                    return "";
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                StreamReader reader = new StreamReader(Request.InputStream);
                string xmlData = reader.ReadToEnd();
                //log(xmlData);
                try
                {
                    XDocument xml = XDocument.Parse(xmlData);
                    if (xml.Element("xml").Element("MsgType").Value == "event")
                    {
                        if (xml.Element("xml").Element("Event").Value == "subscribe")
                        {
                            return Subscribe(xml.Element("xml").Element("FromUserName").Value, xml.Element("xml").Element("ToUserName").Value);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    log(ex.Message);
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }

                return "success";
            }
            return "success";
        }

        public string Subscribe(string toUser, string fromUser)
        {
            return CreateImageAndText(toUser, fromUser, "2017年CSRM规范指南中国行", "请点击图片，进入平台在线报名及了解会议信息", System.Configuration.ConfigurationManager.AppSettings["rooturl"] + "Images/banner.jpg", St.GetLoginUrl());
        }
        //生成图文消息
        private string CreateImageAndText(string toUser, string fromUser, string title, string description, string picurl, string url)
        {
            string xml = string.Format(@"<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[news]]></MsgType>
<ArticleCount>1</ArticleCount>
<Articles>
<item>
<Title><![CDATA[{3}]]></Title> 
<Description><![CDATA[{4}]]></Description>
<PicUrl><![CDATA[{5}]]></PicUrl>
<Url><![CDATA[{6}]]></Url>
</item>
</Articles>
</xml>", toUser, fromUser, GetTimeInt(), title, description, picurl, url);
            return xml;
        }
        private long GetTimeInt()
        {
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(DateTime.Now - start).TotalSeconds;
        }
        private void log(string s)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Server.MapPath("~/log.txt"), true))
            {
                sw.WriteLine(s);
                sw.Close();
            }
        }        
    }
}
