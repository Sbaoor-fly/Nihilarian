using NLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nihilarian
{

    public class Tool
    {
        private static Dictionary<int, Thread> thr = new Dictionary<int, Thread>();
        #region TOOLAPI
        public void WriteAllText(string path, string contenst)
        {
            File.WriteAllText(path, contenst);
        }
        public void AppendAllText(string path, string contenst)
        {
            File.AppendAllText(path, contenst);
        }
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
        public string WorkingPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
        public string HttpGet(string Url)
        {
            var web = new WebClient();
            byte[] outputb = web.DownloadData(Url);
            return Encoding.UTF8.GetString(outputb);
        }
        public void CreateDir(string path)
        {
            Directory.CreateDirectory(path);
        }
        public bool IfFile(string path)
        {
            return File.Exists(path);
        }
        public bool IfDir(string path)
        {
            return Directory.Exists(path);
        }
        public void ThrowException(string msg)
        {
            throw new ArgumentOutOfRangeException(msg);
        }
        public Task TaskRun(LuaFunction func)
        {
            return Task.Run(() => func.Call());
        }
        public int Schedule(LuaFunction func, int delay, int cycle)
        {
            var t = new Thread(() =>
            {
                for (int i = 0; i < cycle; i++)
                {
                    func.Call();
                    Thread.Sleep(delay);
                }
            });
            t.Start();
            int id = t.ManagedThreadId;
            thr.Add(id, t);
            new Thread(() =>
            {
                t.Join();
                if (thr.ContainsKey(id))
                    thr.Remove(id);
            }).Start();
            return id;
        }
        public int Schedule(LuaFunction func, int delay)
        {
            return Schedule(func, delay, 1);
        }
        public bool Cancel(int id)
        {
            if (!thr.ContainsKey(id))
                return false;
            thr[id].Abort();
            thr.Remove(id);
            return true;
        }
        public string[] getMemberInfo(dynamic obj)
        {
            List<string> list = new List<string>();
            foreach (var i in obj.GetType().GetMembers())
                list.Add(i.Name);
            return list.ToArray();
        }
        public void sleep(int time)
        {
            Thread.Sleep(1000 * time);
        }
        public int ToInt(object number)
        {
            try
            {
                return Convert.ToInt32(number);
            }
            catch
            {
                return 0;
            }

        }
        public bool IsNullOrEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion
    }
}
