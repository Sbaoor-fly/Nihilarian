using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Threading;

namespace Nihilarian
{
    class Program
    {
        public static Dictionary<string,List< LuaFunction>> functions = new Dictionary<string,List< LuaFunction>>();
        public static Dictionary<string, string> hosts = new Dictionary<string, string>();
        public static Dictionary<string, WebSocket> wss = new Dictionary<string, WebSocket>();
        public static string HttpPost(string url, string body)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            byte[] buffer = encoding.GetBytes(body);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        public static void log(string text)
        {
            File.AppendAllText("./logs/"+DateTime.Now.ToString("yyyy-M-d")+".log", $"[{DateTime.Now.ToString()}][INFO] " + text + "\n");
            Console.WriteLine($"[{DateTime.Now.ToString()}][INFO] "+text);
        }
        public static string auth(string url, string key,uint qq)
        {
            string skey = HttpPost(url + "/auth", "{\"authKey\":\"" + key + "\"}");
            Console.WriteLine(skey);
            var temp = JsonConvert.DeserializeObject<auth>(skey);
            if (temp.code != 0)
                Console.WriteLine(temp.session);
            var skye = HttpPost(url + "/verify","{\"sessionKey\":\""+temp.session+"\",\"qq\":"+qq+"}");
            Console.WriteLine(skye);
            var temp2 = JsonConvert.DeserializeObject<auth>(skye);
            if (temp2.code != 0)
                Console.WriteLine(temp2.session);
            return temp.session;
        }
        delegate void LOG(object i);
        delegate void LISTEN(object k,LuaFunction f);
        static string LUAString(object o)
        {
            return o?.ToString();
        }
        static LISTEN cs_listen = (k,f) => 
        {
            #region
            switch (LUAString(k))
            {
                case "onMessage":
                    if (!functions.ContainsKey("onMessage"))
                    {
                        functions["onMessage"] = new List<LuaFunction>();
                        foreach(WebSocket ws in wss.Values)
                        {
                            ws.OnMessage += (sender, e) =>
                            {
                                foreach (LuaFunction fu in functions["onMessage"])
                                {
                                    try
                                    {

                                        fu.Call(e.Data);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            };
                        }
                        
                    }                  
                    functions["onMessage"].Add(f);
                    break;
                case "onOpen":
                    if (!functions.ContainsKey("onOpen")) { 
                        functions["onOpen"] = new List<LuaFunction>();
                        foreach (WebSocket ws in wss.Values)
                        {
                            ws.OnOpen += (sender, e) =>
                            {
                                foreach (LuaFunction fu in functions["onOpen"])
                                {
                                    try
                                    {

                                        fu.Call();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            };
                        }                           
                    }
                    functions["onOpen"].Add(f);
                    break;
                case "onError":
                    if (!functions.ContainsKey("onError")) { 
                        functions["onError"] = new List<LuaFunction>();
                        foreach (WebSocket ws in wss.Values)
                        {
                            ws.OnError += (sender, e) =>
                            {
                                foreach (LuaFunction fu in functions["onError"])
                                {
                                    try
                                    {

                                        fu.Call(e);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            };
                        }                            
                    }
                    functions["onError"].Add(f);
                    break;
                case "onClose":
                    if (!functions.ContainsKey("onClose")) { 
                        functions["onClose"] = new List<LuaFunction>();
                        foreach (WebSocket ws in wss.Values)
                        {
                            ws.OnClose += (sender, e) =>
                            {
                                foreach (LuaFunction fu in functions["onClose"])
                                {
                                    try
                                    {

                                        fu.Call(e);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            };
                        }                            
                    }
                    functions["onClose"].Add(f);
                    break;
                default:
                    log("未定义的key:" + LUAString(k));
                    break;
            }
            #endregion
        };

        static LOG cs_log = (f) =>
        {
            log(LUAString(f));
        };
        public static string getkey(string value)
        {
            foreach (string v in hosts.Keys)
            {
                if (hosts[v.ToString()] == value)
                {
                    return v;
                }
            }
            return null;
        }
        public static void Main()
        {
            Console.Title = "Nihilarian | Bot";
            Console.WriteLine(File.ReadAllText("./data/logo.txt"));
            Console.OutputEncoding = Encoding.UTF8;
            var lua = new Lua();
            lua["Log"] = cs_log;
            lua["Listen"] = cs_listen;
            lua["tool"] = new Tool();
            lua.State.Encoding = Encoding.UTF8;
            dynamic config = lua.DoFile("index.lua");
            string configjson = config[0];
            var cfg = JsonConvert.DeserializeObject<Config>(configjson);
            foreach (WsHostItem i in cfg.WsHost)
            {
                hosts.Add(i.name,i.Host + ":" + i.Port);
            }
            log("Nihilarian框架启动成功");
            log("version = 1.2.0");
            log("MiraiUrl地址 -> " + hosts["Mirai"]);
            log("QQ -> " + cfg.QQ.ToString());
            log("开始连接Mirai-Httpapi");
            string session = auth("http://"+hosts["Mirai"], cfg.authKey,cfg.QQ);
            foreach(string i in hosts.Values)
            {
                int n = 0;
                if(n == 0)
                {
                    var ws = new WebSocket("ws://" + i.ToString() + "/message?sessionKey=" + session);
                    wss.Add(getkey(i.ToString()), ws);
                    ws.Connect();
                    n += 1;
                }
                else
                {
                    var ws = new WebSocket("ws://" + i.ToString());
                    wss.Add(getkey(i.ToString()), ws);
                    ws.Connect();
                }                
                
            }
            DirectoryInfo folder = new DirectoryInfo("./modules");
            foreach (FileInfo file in folder.GetFiles("*.lua"))
            {
                try
                {
                    log("load ./modules/" + file.Name);
                    lua.DoFile(file.FullName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            lua["bot"] = new Bot("http://"+hosts["Mirai"],session);
            log("登入成功，开始接收讯息");
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    foreach (WebSocket ws in wss.Values)
                    {
                        if (!ws.IsAlive)
                        {
                            ws.Connect();
                        }
                    }
                }
            }).Start();         
        }
    }
}
