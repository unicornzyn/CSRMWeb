using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Net.Cache;
using System.Net;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using System.Net.Json;

namespace CSRMUtils
{
    public class Util
    {
        private static Random RndSeed = new Random();
        /// <summary>
        /// 随机产生数字
        /// </summary>
        /// <returns></returns>
        public static string GenerateRndNonce()
        {
            return (RndSeed.Next(1, 0x5f5e0ff).ToString("00000000") + RndSeed.Next(1, 0x5f5e0ff).ToString("00000000") + RndSeed.Next(1, 0x5f5e0ff).ToString("00000000") + RndSeed.Next(1, 0x5f5e0ff).ToString("00000000"));
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string DESEncryptMethod(string rs)
        {
            byte[] desKey = new byte[] { 0x64, 0x15, 0x38, 0x88, 0x91, 0x78, 0x08, 0x50 };
            byte[] desIV = new byte[] { 0x64, 0x15, 0x38, 0x88, 0x91, 0x78, 0x08, 0x50 };

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                byte[] inputByteArray = Encoding.Default.GetBytes(rs);
                //byte[] inputByteArray=Encoding.Unicode.GetBytes(rs);

                des.Key = desKey;  // ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = desIV;   //ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(),
                 CryptoStreamMode.Write);
                //Write the byte array into the crypto stream
                //(It will end up in the memory stream)
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                //Get the data back from the memory stream, and into a string
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    //Format as hex
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
                return ret.ToString();
            }
            catch
            {
                return rs;
            }
            finally
            {
                des = null;
            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string DESDecryptMethod(string rs)
        {
            byte[] desKey = new byte[] { 0x64, 0x15, 0x38, 0x88, 0x91, 0x78, 0x08, 0x50 };
            byte[] desIV = new byte[] { 0x64, 0x15, 0x38, 0x88, 0x91, 0x78, 0x08, 0x50 };

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                //Put the input string into the byte array
                byte[] inputByteArray = new byte[rs.Length / 2];
                for (int x = 0; x < rs.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(rs.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = desKey;   //ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = desIV;    //ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                //Flush the data through the crypto stream into the memory stream
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                //Get the decrypted data back from the memory stream
                StringBuilder ret = new StringBuilder();

                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return rs;
            }
            finally
            {
                des = null;
            }
        }
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name=”timeStamp”></param>
        /// <returns></returns>
        public static DateTime GetStampTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name=”time”></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        /// <summary>       
        /// 取得客户端真实IP。如果有代理则取第一个非内网地址       
        /// </summary>       
        public static string GeIPAddress()
        {
            string result = String.Empty;
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (result != null && result != String.Empty)
            {
                //可能有代理       
                if (result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式       

                    result = null;
                else
                {
                    if (result.IndexOf(",") != -1)
                    {
                        //有“,”，估计多个代理。取第一个不是内网的IP。       
                        result = result.Replace(" ", "").Replace("'", "");
                        string[] temparyip = result.Split(",;".ToCharArray());
                        for (int i = 0; i < temparyip.Length; i++)
                        {
                            if (IsIPAddress(temparyip[i])
                                && temparyip[i].Substring(0, 3) != "10."
                                && temparyip[i].Substring(0, 7) != "192.168"
                                && temparyip[i].Substring(0, 7) != "172.16.")
                            {
                                return temparyip[i];    //找到不是内网的地址       
                            }
                        }
                    }
                    else if (IsIPAddress(result)) //代理即是IP格式       
                        return result;

                    else
                        result = null;    //代理中的内容 非IP，取IP       
                }
            }
            string IpAddress = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (null == result || result == String.Empty)
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (result == null || result == String.Empty)
                result = HttpContext.Current.Request.UserHostAddress;
            return result;
        }
        /// <summary>      
        /// 判断是否是IP地址格式 0.0.0.0      
        /// </summary>      
        /// <param name="str1">待判断的IP地址</param>      
        /// <returns>true or false</returns>      
        public static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15)

                return false;

            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";

            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }
        #region 取子串函数
        /// <summary>
        /// 取子串函数
        /// </summary>
        /// <param name="strInput">字符串</param>
        /// <param name="length">需要截取的长度 中文按2个长度计算  非中文按一个长度计算</param>
        /// <returns></returns>
        public static string SubString(string strInput, int length)
        {
            if (String.IsNullOrEmpty(strInput)) return String.Empty;

            strInput = strInput.Trim();
            ASCIIEncoding ascii = new ASCIIEncoding();
            int intLength = 0;
            string strString = "";
            byte[] s = ascii.GetBytes(strInput);

            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    intLength += 2;
                else
                    intLength += 1;

                try
                {
                    strString += strInput.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (intLength >= length)
                {
                    break;
                }
            }
            return strString;
        }
        #endregion
        public static bool IsNUllAndEmpty(string name)
        {
            bool isGood = false;
            if (System.Web.HttpContext.Current.Request[name] != null)
            {
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request[name]))
                {
                    isGood = true;
                }
            }
            return isGood;
        }
        public static bool IsNUllAndEmptyNumber(string name)
        {
            bool isGood = false;
            if (System.Web.HttpContext.Current.Request[name] != null)
            {
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request[name]) && IsNumber(System.Web.HttpContext.Current.Request[name]))
                {
                    isGood = true;
                }
            }
            return isGood;
        }
        //判断是不是数字
        private static Regex _isNumber = new Regex("^[0-9]+$");

        public static bool IsNumber(string inputData)
        {
            if (inputData == null)
            {
                return false;
            }
            else
            {
                Match m = _isNumber.Match(inputData);
                return m.Success;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static string HttpPOSTServer(string url, string formData)
        {
            string html = string.Empty;
            System.Net.ServicePointManager.Expect100Continue = false;
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            CookieContainer cookieContainer = new CookieContainer();
            // 将提交的字符串数据转换成字节数组 
            byte[] postData = Encoding.UTF8.GetBytes(formData);

            HttpWebRequest httpWebRequest;
            httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.KeepAlive = false;
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.Referer = url;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = postData.Length;

            // 提交请求数据 
            System.IO.Stream outputStream = httpWebRequest.GetRequestStream();
            outputStream.Write(postData, 0, postData.Length);
            outputStream.Close();

            HttpWebResponse httpWebResponse;
            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
            }
            httpWebResponse.Close();
            html.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            return html;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGERServer(string url)
        {
            string html = string.Empty;
            System.Net.ServicePointManager.Expect100Continue = false;
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            CookieContainer cookieContainer = new CookieContainer();
            HttpWebRequest httpWebRequest;
            httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.KeepAlive = false;
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.Referer = url;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
            httpWebRequest.Method = "GET";

            HttpWebResponse httpWebResponse;
            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
            }
            httpWebResponse.Close();
            html.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            return html;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HttpGERServer(string url, Encoding encode)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (MemoryStream ms = new MemoryStream())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    int readc;
                    byte[] buffer = new byte[1024];
                    while ((readc = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, readc);
                    }
                }
                return encode.GetString(ms.ToArray());
            }
        }
        public static bool IsJsonJsonObjection(JsonObjectCollection json)
        {
            bool isGood = false;
            if (json != null && json != Convert.DBNull)
            {
                if (json.Count > 0)
                {
                    isGood = true;
                }
            }
            return isGood;
        }
        public static bool IsJsonArray(JsonArrayCollection json)
        {
            bool isGood = false;
            if (json != null && json != Convert.DBNull)
            {
                if (json.Count > 0)
                {
                    isGood = true;
                }
            }
            return isGood;
        }
        public static bool IsListJsonObject(List<JsonObject> list)
        {
            bool isGood = false;
            if (list != null && list != Convert.DBNull)
            {
                if (list.Count > 0)
                {
                    isGood = true;
                }
            }
            return isGood;
        }
        public static bool IsJsonObject(JsonObject json)
        {
            bool isGood = false;
            if (json != null && json != Convert.DBNull)
            {
                isGood = true;
            }
            return isGood;
        }
        public static bool IsJsonCollection(JsonCollection json)
        {
            bool isGood = false;
            if (json != null && json != Convert.DBNull)
            {
                if (json.Count > 0)
                {
                    isGood = true;
                }
            }
            return isGood;
        }
        /// <summary>
        /// 过滤字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GoodStr(string str)
        {
            string strRe = str;
            strRe = strRe.Replace("'", "");
            strRe = strRe.Replace("&", "");
            strRe = strRe.Replace(";", "");
            strRe = strRe.Replace("--", "");
            strRe = strRe.Replace("<", "《");
            strRe = strRe.Replace(">", "》");
            strRe = strRe.Replace("(", "（");
            strRe = strRe.Replace(")", "）");
            strRe = strRe.Replace("=", "");
            strRe = strRe.Replace("\"", "");
            strRe = strRe.Replace(",", "，");
            strRe = strRe.Trim();
            return strRe;
        }
        /// <summary>
        /// 字符串过滤
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlTextEncode(string str)
        {
            string StrFilter = str;
            StrFilter = StrFilter.Replace("&", "&amp;");
            StrFilter = StrFilter.Replace(">", "&gt;");
            StrFilter = StrFilter.Replace("<", "&lt;");
            StrFilter = StrFilter.Replace("\"", "&quot;");
            StrFilter = StrFilter.Replace("'", "&#39;");
            StrFilter = StrFilter.Replace("=", "&#61;");
            StrFilter = StrFilter.Replace("`", "&#96;");
            StrFilter = StrFilter.Replace("--", "");
            StrFilter = StrFilter.Replace("(", "（");
            StrFilter = StrFilter.Replace(")", "）");
            StrFilter = StrFilter.Replace(",", "，");
            StrFilter = StrFilter.Replace(@"\s", "");
            return StrFilter;
        }
        /// <summary>
        /// 字符串解过滤
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlTextDecode(string str)
        {
            string StrFilter = str;
            StrFilter = StrFilter.Replace("&amp;", "&");
            StrFilter = StrFilter.Replace("&gt;", ">");
            StrFilter = StrFilter.Replace("&lt;", "<");
            StrFilter = StrFilter.Replace("&quot;", "\"");
            StrFilter = StrFilter.Replace("&#39;", "'");
            StrFilter = StrFilter.Replace("&#96;", "`");
            StrFilter = StrFilter.Replace("&#61;", "=");
            StrFilter = StrFilter.Replace(";", "");
            StrFilter = StrFilter.Replace("--", "");
            StrFilter = StrFilter.Replace("（", "(");
            StrFilter = StrFilter.Replace("）", ")");
            StrFilter = StrFilter.Replace("，", ",");
            StrFilter = StrFilter.Replace(@"\s", "");
            return StrFilter;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlTextTrim(string str)
        {
            return Regex.Replace(str, @"(?:^[ \t\n\r\s]+)|(?:[ \t\n\r\s]+$)", "");
        }
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SHA1(string text) 
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] str1 = Encoding.UTF8.GetBytes(text);
            byte[] str2 = sha1.ComputeHash(str1);
            sha1.Clear();
            (sha1 as IDisposable).Dispose();
            return BitConverter.ToString(str2).Replace("-", string.Empty).ToLower();
        }
       
    }
}
