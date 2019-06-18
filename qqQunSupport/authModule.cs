using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace qqQunSupport
{
    /// <summary>
    /// 登录模块，依赖Selenium
    /// </summary>
    public class authModule
    {
        /// <summary>
        /// skey，大部分请求需要
        /// </summary>
        public string skey { get; private set; } = "";


        public System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();

        /// <summary>
        /// 通过启动浏览器来开始获取API认证信息
        /// </summary>
        public void startAuth()
        {
            IWebDriver selenium = new ChromeDriver();
            selenium.Navigate().GoToUrl("https://qun.qq.com/member.html");
            while(skey == "")
            {
                foreach (var cookie in selenium.Manage().Cookies.AllCookies)
                {
                    if (cookie.Name.ToLower() == "skey")
                        skey = cookie.Value;
                }
                System.Threading.Thread.Sleep(2000);
            }
            foreach (var cookie in selenium.Manage().Cookies.AllCookies)
            {
                cookieContainer.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }
            selenium.Close();
            return;
        }
    }
}
