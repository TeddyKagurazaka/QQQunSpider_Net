using System;

namespace qqQunSearch_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
           

            if (System.IO.File.Exists("Input.txt"))
            {
                MainV2();
                Environment.Exit(0);
            }

            Console.WriteLine("请登录");
            var authModule = new qqQunSupport.authModule();
            authModule.startAuth();
            Console.WriteLine("登录成功");

            var qunModule = new qqQunSupport.qqQunModule(authModule);
            int Page = 0;
            int Limit = 20;

            var result3 = qunModule.RequestUserFriend();

            var result2 = qunModule.RequestQun("DNF", Page.ToString(), Limit.ToString());
            if (result2[0].ContainsKey("error"))
            {
                Console.WriteLine("=======触发了反爬虫，请稍后再试========");
                return;
            }
            else
            {
                foreach (var resultDict in result2)
                {
                    Console.WriteLine("====================");
                    Console.WriteLine("群ID:" + resultDict["code"]);
                    Console.WriteLine("群创建人QQ:" + resultDict["owner_uin"]);
                    Console.WriteLine("群名:" + resultDict["name"]);
                    Console.WriteLine("群简介:" + resultDict["memo"]);
                    Console.WriteLine("====================");
                }
            }

            Console.WriteLine("完成");
            Console.ReadLine();
        }


        static void MainV2()
        {
            Console.WriteLine("请登录");
            var authModule = new qqQunSupport.authModule();
            authModule.startAuth();
            Console.WriteLine("登录成功");

            var qunModule = new qqQunSupport.qqQunModule(authModule);
            int Page = 0;
            int Limit = 50;

            using (var reader = new System.IO.StreamReader("Input.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var query = reader.ReadLine();
                    Console.Title = query;
                    Page = 0;
                    Limit = 50;
                    using (var writer = new System.IO.StreamWriter(query + ".txt"))
                    {
                        while (true)
                        {
                            Console.Title = query + " page:" + Page;
                            var result2 = qunModule.RequestQun(query, Page.ToString(), Limit.ToString());
                            if(result2 == null)
                            {
                                Console.WriteLine("======获取请求失败======");
                                return;
                            }
                            if (result2[0].ContainsKey("error"))
                            {
                                if(result2[0]["code"].ToString() == "0")
                                {
                                    writer.WriteLine("无结果");
                                    writer.Flush();
                                    System.Threading.Thread.Sleep(10000);
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("=======触发了反爬虫，请稍后再试========");
                                    return;
                                }

                                
                            }
                            foreach(var res in result2)
                            {
                                var plain = Newtonsoft.Json.JsonConvert.SerializeObject(res);
                                Console.WriteLine(plain);
                                writer.WriteLine(plain);
                                writer.Flush();
                            }

                            System.Threading.Thread.Sleep(10000);
                            if (result2.Count >= Limit)
                                //数量=限制量 -> 还有结果
                                Page++;
                            else
                            {
                                //数量！=限制量 -> 无结果
                                break;
                            }
                        }
                        writer.Close();
                    }
                    
                }

            }
        }
    }
}
