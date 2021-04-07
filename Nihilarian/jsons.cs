using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nihilarian
{
    public class WsHostItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Host { get; set; }
        public string name { get; set; }
    }

    public class Config
    {
        /// <summary>
        /// 
        /// </summary>
        public uint QQ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string authKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<WsHostItem> WsHost { get; set; }
    }


    public class auth
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string session { get; set; }
    }


    public class MessageChain
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string text { get; set; }
    }

    public class Msg
    {
        /// <summary>
        /// 激活的sessionkey
        /// </summary>
        public string sessionKey { get; set; }
        /// <summary>
        /// 目标好友QQ号
        /// </summary>
        public uint target { get; set; }
        /// <summary>
        /// 引用一条消息的messageId进行回复
        /// </summary>
        public List<MessageChain> messageChain { get; set; }
    }

    public class recall
    {
        /// <summary>
        /// 已经激活的Session
        /// </summary>
        public string sessionKey { get; set; }
        /// <summary>
        /// 需要撤回的消息的messageId
        /// </summary>
        public int target { get; set; }
    }
    public class mute
    {
        /// <summary>
        /// 你的session key
        /// </summary>
        public string sessionKey { get; set; }
        /// <summary>
        /// 指定群的群号
        /// </summary>
        public uint target { get; set; }
        /// <summary>
        /// 指定群员QQ号
        /// </summary>
        public uint memberId { get; set; }
        /// <summary>
        /// 禁言时长，单位为秒，最多30天，默认为0
        /// </summary>
        public int time { get; set; }
    }
}
