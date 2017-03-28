using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.Reflection;
namespace CSRMUtils
{
    public class ProductCache
    {
        
        public static void Add(string CacheName, object CacheValue, int Minute)
        {
            if (CacheValue == null)
            {
                return;
            }
            string CacheAllName = CacheName;
            if (CacheName.Length > 50)
            {
                CacheName = St.GetMd5(CacheName);
            }

            System.Web.HttpContext.Current.Cache.Insert(CacheName, CacheValue, null, DateTime.Now.AddMinutes(Minute), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public static void Add(string CacheName, object CacheValue)
        {
            Add(CacheName, CacheValue, 120);
        }
       
        public static void Add(string cacheName, string dependencies, string[] dependencyKey, string dependencyFileUrl, object source)
        {

            CacheDependency cacheDependency = null;
            switch (dependencies)
            {
                case "":
                    cacheDependency = null;
                    break;
                case "key":
                    cacheDependency = new CacheDependency(null, dependencyKey);
                    break;
                case "file":
                    cacheDependency = new CacheDependency(dependencyFileUrl);
                    break;

            }
            System.Web.HttpContext.Current.Cache.Insert(cacheName, source, cacheDependency);
        }

        public static Object Get(string cacheName)
        {

            if (cacheName.Length > 50)
            {
                cacheName = St.GetMd5(cacheName);
            }
            return (Object)HttpContext.Current.Cache[cacheName];            
        }
    }

}
