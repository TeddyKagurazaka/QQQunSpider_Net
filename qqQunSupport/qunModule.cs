using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace qqQunSupport
{

    public class qqQunModule
    {
        readonly CookieContainer cookies = new CookieContainer();
        readonly string BKN = "";

        public class ApiType
        {
            private ApiType(string value) { Value = value; }
            public string Value { get; set; }

            /// <summary>
            /// 搜索群API入口
            /// </summary>
            public static ApiType RequestQunInfo { get { return new ApiType("https://qun.qq.com/cgi-bin/group_search/group_search?keyword={0}&page={1}&wantnum={2}"); } }
            /// <summary>
            /// 获取群成员API入口
            /// </summary>
            public static ApiType RequestMember { get { return new ApiType("https://qun.qq.com/cgi-bin/qun_mgr/search_group_members"); } }
            /// <summary>
            /// 获取本账号好友列表
            /// </summary>
            public static ApiType RequestUserFriend { get { return new ApiType("https://qun.qq.com/cgi-bin/qun_mgr/get_friend_list"); } }
            /// <summary>
            /// 获取本账号加群列表
            /// </summary>
            public static ApiType RequestUserGroup { get { return new ApiType ("https://qun.qq.com/cgi-bin/qun_mgr/get_group_list"); } }
            /// <summary>
            /// 获取本群统计
            /// </summary>
            public static ApiType RequestQunStatistics { get { return new ApiType("http://web.qun.qq.com/cgi-bin/misc/statistic_group_member"); } }
        }
        /// <summary>
        /// 用户所在的群的类型
        /// </summary>
        public enum UserGroupType
        {
            /// <summary>
            /// 用户已加入的群
            /// </summary>
            Join,
            /// <summary>
            /// 用户管理的群
            /// </summary>
            Manage
        }
        /// <summary>
        /// 初始化群搜索模块
        /// </summary>
        /// <param name="authInfo">登录模组信息，用于获取sKey和uin</param>
        public qqQunModule(authModule authInfo)
        {
            cookies = authInfo.cookieContainer;
            BKN = CalcBKN(authInfo.skey).ToString();
        }
        /// <summary>
        /// 以关键字查找群信息
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="page">页数</param>
        /// <param name="wantnum">获取数量(建议50以内)</param>
        /// <returns>若返回List的第0项包含Error或List为空则请求失败（不为null时第1项包含原始错误数据），否则为获取到的群列表</returns>
        public List<Dictionary<string, object>> RequestQun(string keyword,string page = "0",string wantnum = "20")
        {
            try
            {
                var result = makeRequest(string.Format(ApiType.RequestQunInfo.Value, System.Web.HttpUtility.UrlEncode(keyword), page, wantnum));
                return PostResponse(result, "group_list");
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取群员信息
        /// （要求用户需要在群里）
        /// </summary>
        /// <param name="qunID">群号</param>
        /// <param name="page">起始id数</param>
        /// <param name="wantnum">结束id数</param>
        /// <returns>若返回项为null则请求失败，返回List第0项包含Error则回复失败（第1项包含原始数据），否则返回群员列表</returns>
        public List<Dictionary<string,object>> RequestQunUser(string qunID,string page = "0",string wantnum = "20")
        {
            try
            {
                var body = string.Format("gc={0}&st={1}&end={2}&sort={3}&bkn={4}", qunID, page, wantnum, "0", BKN);
                var result = makeRequest(ApiType.RequestMember.Value, body);
                return PostResponse(result,"mems");
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取用户好友信息
        /// </summary>
        /// <returns>用户好友列表，包含分组名、qq号、名称</returns>
        public Dictionary<string,object> RequestUserFriend()
        {
            try
            {
                var body = string.Format("bkn={0}", BKN);
                var result = makeRequest(ApiType.RequestUserFriend.Value, body);
                if (result == null) return null;
                else if (!result.ContainsKey("result"))
                {
                    //请求失败或无有效结果
                    if (result["ec"].ToString() != "0")
                    {
                        return new Dictionary<string, object>()
                        {
                            {"error","反爬虫" },
                            {"code","-1" },
                            {"data",result }
                        };
                    }
                    else
                    {
                        return new Dictionary<string, object>()
                        {
                            {"error","无结果" },
                            {"code","-1" },
                            {"data",result }
                        };
                    }
                }
                else
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(result["result"].ToString());
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取用户加群情况
        /// </summary>
        /// <param name="joinType">加群类型</param>
        /// <returns>该类型下所加群的列表</returns>
        public List<Dictionary<string,object>> RequestUserJoinedGroup(UserGroupType joinType)
        {
            try
            {
                var body = string.Format("bkn={0}", BKN);
                var result = makeRequest(ApiType.RequestUserGroup.Value, body);
                switch (joinType)
                {
                    case UserGroupType.Join:
                        return PostResponse(result, "join");
                    case UserGroupType.Manage:
                        return PostResponse(result, "manage");
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取群概括(男女、地区、活跃群成员头像)
        /// </summary>
        /// <param name="qunID"></param>
        /// <returns></returns>
        public Dictionary<string,object> RequestQunStatistics(string qunID)
        {
            try
            {
                var body = string.Format("bkn={0}&gc={1}&callback=init", BKN,qunID);
                var result = makeRequest(ApiType.RequestQunStatistics.Value, body);
                if (result == null) return null;

                if (result["ec"].ToString() != "0")
                {
                    return new Dictionary<string, object>()
                        {
                            {"error","无结果或反爬虫" },
                            {"code","-1" },
                            {"data",result }
                        };
                }

                var returnDict = new Dictionary<string, object>();
                foreach (string keys in new string[] { "gender", "age", "provice" })
                {
                    var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(result[keys].ToString());
                    returnDict[keys] = info;
                }
                var tops = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result["tops"].ToString());
                returnDict["tops"] = tops;

                return returnDict;
            }
            catch
            {
                return null;
            }
        }

        #region 内部操作
        /// <summary>
        /// 统一后期处理
        /// </summary>
        /// <param name="result">请求结果</param>
        /// <param name="key">要获取的列表键名</param>
        /// <returns>列表数组，或解析失败时回复错误数组</returns>
        List<Dictionary<string,object>> PostResponse(Dictionary<string,object> result,string key)
        {
            if (result == null) return null;
            else if (!result.ContainsKey(key))
            {
                //请求失败或无有效结果
                if (result["ec"].ToString() != "0")
                {
                    return new List<Dictionary<string, object>>
                        { new Dictionary<string, object> { { "error","反爬虫"},{"code","-1" } } ,result};
                }
                else
                {
                    return new List<Dictionary<string, object>>
                        { new Dictionary<string, object> { { "error","无结果或不在群里"},{"code","0" } } ,result};
                }
            }
            else
                return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(result[key].ToString());
        }

        /// <summary>
        /// 将群信息的标签字段转为List，此字段内一般包含成员数量
        /// </summary>
        /// <param name="labeljson">Lebel项的JSON字符串</param>
        /// <returns>Label项的List数组</returns>
        public List<Dictionary<string, string>> LabelToList(string labeljson)
        {
            return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(labeljson);
        }

        /// <summary>
        /// 制造请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns>服务器回复</returns>
        Dictionary<string, object> makeRequest(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
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
        /// <summary>
        /// 制造POST请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="body">请求内容</param>
        /// <returns>服务器回复</returns>
        Dictionary<string,object> makeRequest(string url,string body)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = cookies;
            request.Accept = "*/*";
            request.UserAgent = "Mozilla/5.0 (Linux; Android 9; MI 5s Build/PQ2A.190305.002; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/66.0.3359.126 MQQBrowser/6.2 TBS/044704 Mobile Safari/537.36 V1_AND_SQ_7.7.6_899_GM_D QQ/7.7.6.3680 NetType/WIFI WebP/0.3.0 Pixel/1080";
            request.Method = "POST";
            request.ContentType = "x-www-form-urlencoded";
            using (var writer = new System.IO.StreamWriter(request.GetRequestStream()))
            {
                writer.WriteLine(body);
                writer.Flush();
                writer.Close();
            }

            string respStr = "";
            using (var resp = request.GetResponse())
            {
                using (var respReader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    respStr = respReader.ReadToEnd();
                }
            }
            if (!string.IsNullOrEmpty(respStr))
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(respStr);
            return null;

        }

        /// <summary>
        /// 计算BKN，部分请求需要
        /// </summary>
        /// <param name="sKey">sKey</param>
        /// <returns>计算后的BKN值</returns>
        int CalcBKN(string sKey)
        {
            int hash = 5381;
            for (int i = 0, len = sKey.Length; i < len; ++i)
            {
                hash += (hash << 5) + sKey[i];
            }

            return hash & 0x7fffffff;
        }
        #endregion
    }

}
