using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CSRMUtils
{
    public class St
    {
        public static string GetLoginUrl()
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["rooturl"] + "Home/Index";
            OpenAuthWX wb = new OpenAuthWX(System.Configuration.ConfigurationManager.AppSettings["wxappid"], System.Configuration.ConfigurationManager.AppSettings["wxappsecret"], 2);
            wb.RedirectUri = url;
            return wb.FirstPartUrl();
        }
        public static string GetMd5(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToLower();
        }
        public static string GetHTML(string url)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.Referer = url;
                httpWebRequest.Method = "GET";
                httpWebRequest.UserAgent = "User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                return html.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string GetFirstMatch(string text, string regex)
        {
            return GetMatch(text, regex, 1);
        }
        public static string GetMatch(string text, string regex, int index)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            Regex reg = new Regex(regex);
            MatchCollection mc = reg.Matches(text);
            if (mc.Count > 0 && mc[0].Groups.Count > index)
            {
                return mc[0].Groups[index].Value;
            }
            return "";
        }
    
    }
}
