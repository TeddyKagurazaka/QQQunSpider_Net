using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace qqQunSupport
{

    public class qqQunModule
    {
        readonly CookieContainer cookies = new CookieContainer();
        public qqQunModule(authModule authInfo)
        {
            cookies.Add(new Cookie("uin", authInfo.uin,"/","qq.com"));
            cookies.Add(new Cookie("skey", authInfo.skey, "/", "qq.com"));
        }
        public string RequestQun_Str(string keyword,string page = "0",string wantnum = "20")
        {
            try
            {
                return makeRequest_Plain(keyword, page, wantnum);
            }
            catch
            {
                return null;
            }
        }
        public List<Dictionary<string, object>> RequestQun(string keyword,string page = "0",string wantnum = "20")
        {
            try
            {
                var result = makeRequest(keyword, page, wantnum);
                if (!result.ContainsKey("group_list"))
                {
                    if (result["ec"].ToString() != "0")
                    {
                        return new List<Dictionary<string, object>>
                    { new Dictionary<string, object> { { "error","反爬虫"},{"code","-1" } } ,result};
                    }
                    else
                    {
                        return new List<Dictionary<string, object>>
                    { new Dictionary<string, object> { { "error","无结果"},{"code","0" } } ,result};
                    }


                }
                var groupList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(result["group_list"].ToString());
                return groupList;
            }
            catch
            {
                return null;
            }
        }
        string makeRequest_Plain(string keyword, string page = "0", string wantnum = "20")
        {
            var reqStr = "https://qun.qq.com/cgi-bin/group_search/group_search" +
                "?keyword={0}&page={1}&wantnum={2}";

            HttpWebRequest request = WebRequest.Create(string.Format(reqStr, System.Web.HttpUtility.UrlEncode(keyword), page, wantnum)) as HttpWebRequest;
            request.CookieContainer = cookies;
            request.Accept = "*/*";
            request.UserAgent = "Mozilla/5.0 (Linux; Android 9; MI 5s Build/PQ2A.190305.002; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/66.0.3359.126 MQQBrowser/6.2 TBS/044704 Mobile Safari/537.36 V1_AND_SQ_7.7.6_899_GM_D QQ/7.7.6.3680 NetType/WIFI WebP/0.3.0 Pixel/1080";
            string respStr = "";
            using (var resp = request.GetResponse())
            {
                using (var respReader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    respStr = respReader.ReadToEnd();
                }
            }
            return respStr;
        }
        Dictionary<string, object> makeRequest(string keyword,string page = "0",string wantnum = "20")
        {
            var reqStr = "https://qun.qq.com/cgi-bin/group_search/group_search"+
                "?keyword={0}&page={1}&wantnum={2}";

            HttpWebRequest request = WebRequest.Create(string.Format(reqStr,System.Web.HttpUtility.UrlEncode(keyword), page,wantnum)) as HttpWebRequest;
            request.CookieContainer = cookies;
            request.Accept = "*/*";
            request.UserAgent = "Mozilla/5.0 (Linux; Android 9; MI 5s Build/PQ2A.190305.002; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/66.0.3359.126 MQQBrowser/6.2 TBS/044704 Mobile Safari/537.36 V1_AND_SQ_7.7.6_899_GM_D QQ/7.7.6.3680 NetType/WIFI WebP/0.3.0 Pixel/1080";

            string respStr = "";
            using(var resp = request.GetResponse())
            {
                using (var respReader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    respStr = respReader.ReadToEnd();
                }
            }
            if(!string.IsNullOrEmpty(respStr))
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(respStr);
            return null;
        }
    }

}
