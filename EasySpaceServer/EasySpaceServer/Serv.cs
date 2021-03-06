﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace EasySpaceServer
{
    class Serv
    {
        //监听嵌套字
        public Socket listenfd;
        //客户端连接
        public Conn[] conns;
        //最大连接数
        public int maxConn = 50;

        /// <summary>
        /// 获取连接池索引，返回负数表示获取失败
        /// </summary>
        /// <returns></returns>
        public int NewIndex()
        {
            if (conns == null) return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if(conns[i].isUse==false)
                {
                    return i;
                }
            }
            return -1;
        }


        MySqlConnection sqlConn;
        string connStr = "Database=msgboard;Data Source=127.0.0.1;";

        /// <summary>
        /// 开启服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Start(string host, int port)
        {
            //数据库
            string connStr = "Database=msgboard;Data Source=127.0.0.1;";
            connStr += "User Id=root;Password=123456;port=3306";
            sqlConn = new MySqlConnection(connStr);
            try
            {
                sqlConn.Open();
                Console.WriteLine("[数据库]连接成功");
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库]连接失败" + e.Message);
                return;
            }

            //连接池
            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(maxConn);
            //Accept
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("[服务器]启动成功");
        }

        /// <summary>
        /// Accept回调
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    socket.Close();
                    Console.WriteLine("[警告]连接已满");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAdress();
                    Console.WriteLine("客户端连接["+adr+"] conn池ID："+index);
                    conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
                }
                listenfd.BeginAccept(AcceptCb, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("[错误]AcceptCb失败："+e.Message);
            }
        }

        /// <summary>
        /// BeginReceive的回调函数
        /// 接收并处理消息，同时转发给所有人
        /// 如果收到关闭连接的信号（count==0）则断开连接
        /// 继续调用BeginReceive接收下一个数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn =(Conn)ar.AsyncState;
            try
            {
                int count = conn.socket.EndReceive(ar);
                //关闭信号
                if (count <= 0)
                {
                    Console.WriteLine("收到[" + conn.GetAdress() + "] 断开连接");
                    conn.Close();
                    return;
                }
                //数据处理
                string str = System.Text.Encoding.UTF8.GetString(conn.readBuff, 0, count);
                Console.WriteLine("收到[" + conn.GetAdress() + "] 数据："+str);

                HandleMsg(conn, str);

                str = conn.GetAdress() + "：" + str;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
                //广播
                for (int i = 0; i < conns.Length; i++)
                {
                    if (conns[i] == null) continue;
                    if (!conns[i].isUse) continue;

                    Console.WriteLine("将消息传播给 " + conn.GetAdress());
                    conns[i].socket.Send(bytes);
                }
                //继续接收
                conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
            }
            catch (Exception e)
            {
                Console.WriteLine("收到[" + conn.GetAdress() + "] 断开连接");
                conn.Close();
            }
        }

        private void HandleMsg(Conn conn, string str)
        {
            ////获取数据
            //if (str == "_GET")
            //{
            //    string cmdStr = "SELECT * FROM msg ORDER BY id DESC LIMIT 10";
            //    MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            //    try
            //    {
            //        MySqlDataReader dataReader = cmd.ExecuteReader();
            //        str = "";
            //        while (dataReader.Read())
            //        {
            //            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            //            conn.socket.Send(bytes);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("[数据库]查询失败 " + e.Message);
            //        throw;
            //    }
            //}
            //else
            //{
            //    //插入数据
            //    string cmdStrFormat = "insert into msg set name='{0}',msg='{1}';";
            //    string cmdStr = string.Format(cmdStrFormat, conn.GetAdress(), str);
            //    MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            //    try
            //    {
            //        cmd.ExecuteNonQuery();
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("[数据库]插入失败 " + e.Message);
            //    }
            //}

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            //广播消息
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null) continue;
                if (!conns[i].isUse) continue;
                Console.WriteLine("将消息转播给 " + conns[i].GetAdress());
                conns[i].socket.Send(bytes);
            }
        }
    }
}
