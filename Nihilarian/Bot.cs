
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nihilarian
{
    class Bot
    {
        private string url;
        private string key;
        private static string HttpPost(string url, string body)
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
        public Bot(string url, string key)
        {
            this.url = url;
            this.key = key;
        }
        public string sendFriendMessage(uint qq, string text)
        {
            var m = new MessageChain()
            {
                type = "Plain",
                text = text
            };
            var msg = new List<MessageChain>();
            msg.Add(m);
            var messgae = new Msg()
            {
                sessionKey = key,
                target = qq,
                messageChain = msg
            };
            string newmsg = JsonConvert.SerializeObject(messgae);
            return HttpPost(url + "/sendFriendMessage", newmsg);
        }
        public string sendGroupMessage(uint groupqq, string text)
        {
            var m = new MessageChain()
            {
                type = "Plain",
                text = text
            };
            var msg = new List<MessageChain>();
            msg.Add(m);
            var messgae = new Msg()
            {
                sessionKey = key,
                target = groupqq,
                messageChain = msg
            };
            string newmsg = JsonConvert.SerializeObject(messgae);
            return HttpPost(url + "/sendGroupMessage", newmsg);
        }
        public string sendTempMessage( uint qq, string text)
        {
            var m = new MessageChain()
            {
                type = "Plain",
                text = text
            };
            var msg = new List<MessageChain>();
            msg.Add(m);
            var messgae = new Msg()
            {
                sessionKey = key,
                target = qq,
                messageChain = msg
            };
            string newmsg = JsonConvert.SerializeObject(messgae);
            return HttpPost(url+ "/sendTempMessage", newmsg);
        }

        public string recall(int msgid)
        {
            var a = new recall()
            {
                sessionKey = key,
                target = msgid
            };
            string newmsg = JsonConvert.SerializeObject(a);
            return HttpPost(url + "/recall", newmsg);
        }

        public string mute(uint qq, uint groupqq, int time)
        {
            var a = new mute()
            {
                memberId = qq,
                sessionKey = key,
                target = groupqq,
                time = time
            };
            string msg = JsonConvert.SerializeObject(a);
            return HttpPost(url + "/mute", msg);
        }
        public bool wssend(string key,string msg)
        {
            foreach(string k in Program.wss.Keys)
            {
                if(k.ToString() == key)
                {
                    Program.wss[key].Send(msg);
                    return true;
                }
                
            }
            return false;
        }
    }
}