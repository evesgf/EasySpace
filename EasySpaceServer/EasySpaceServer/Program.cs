using EasySpaceServer.Core;
using EasySpaceServer.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasySpaceServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //Serv serv = new Serv();
            //serv.Start("127.0.0.1", 1234);

            //while (true)
            //{
            //    string str = Console.ReadLine();
            //    switch (str)
            //    {
            //        case "quit":
            //            return;
            //    }
            //}

            DataMgr dataMgr = DataMgr.Create();

            //注册
            bool ret = dataMgr.Register("admin", "123456");
            Console.WriteLine("注册状态：" + ret);

            //创建玩家
            ret = dataMgr.CreatePlayer("admin");
            Console.WriteLine("创建玩家：" + ret);

            //获取玩家数据
            PlayerData pd = dataMgr.GetPlayerData("admin");
            Console.WriteLine("获取玩家数据：" + pd.score);

            pd.score += 100;

            Player p = new Player()
            {
                userName = "admin",
                data = pd
            };
            dataMgr.SavePlayer(p);

            //重新读取
            pd = dataMgr.GetPlayerData("admin");
            if(pd!=null)
                Console.WriteLine("重新读取：" + pd.score);
            else
                Console.WriteLine("重新读取：false");

            Console.ReadKey(true);
        }
    }
}
