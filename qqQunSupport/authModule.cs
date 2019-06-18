using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace qqQunSupport
{
    public class authModule
    {
        public string uin { get; private set; } = "";
        public string skey { get; private set; } = "";

        public void startAuth()
        {
            IWebDriver selenium = new ChromeDriver();
            selenium.Navigate().GoToUrl("http://open.qq.com/reg");
            while(string.IsNullOrEmpty(uin) || string.IsNullOrEmpty(skey))
            {
                foreach (var cookie in selenium.Manage().Cookies.AllCookies)
                {
                    if (cookie.Name == "uin")
                        uin = cookie.Value;
                    else if (cookie.Name == "skey")
                        skey = cookie.Value;
                }
                System.Threading.Thread.Sleep(2000);
            }
            selenium.Close();
            return;
        }
    }
}
