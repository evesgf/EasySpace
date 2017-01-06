using EasySpaceServer.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasySpaceServer.Core
{
    public class DataMgr
    {
        MySqlConnection sqlConn;

        public static DataMgr instance;
        private DataMgr()
        {
            instance = this;
            Connect();
        }

        public static DataMgr Create()
        {
            if (instance == null)
            {
                instance = new DataMgr();
            }
            return instance;
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        private void Connect()
        {
            //数据库
            string connStr = "Database=easyspace;Data Source=127.0.0.1;User Id=root;Password=123456;port=3306";

            sqlConn = new MySqlConnection(connStr);

            try
            {
                sqlConn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]Connect "+e.Message);
                return;
            }
        }

        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private bool CanRegister(string userName)
        {
            if (!isSafeStr(userName)) return false;

            //查询用户名是否存在
            string cmdStr = string.Format("select * from user where user_name='{0}';", userName);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);

            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return !hasRows;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]CanRegister fail " + e.Message);
                return false;
            }
        }

        public bool Register(string userName, string pwd)
        {
            if (!isSafeStr(userName) || !isSafeStr(pwd))
            {
                Console.WriteLine("[DataMgr]Register:非法字符串");
                return false;
            }
            //能否注册
            if (!CanRegister(userName))
            {
                Console.WriteLine("[DataMgr]Register: !CanRegister");
                return false;
            }
            //写入数据库user表
            string cmdStr = string.Format("insert into user set user_name='{0}',user_pwd='{1}';", userName, pwd);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]Register "+e.Message);
                return false;
            }
        }

        /// <summary>
        /// 字符串检测
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool isSafeStr(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        public bool CreatePlayer(string userName)
        {
            if (!isSafeStr(userName)) return false;

            //序列化
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            PlayerData playerdata = new PlayerData();

            try
            {
                formatter.Serialize(stream, playerdata);
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]CreatePlayer 序列化 " + e.Message);
                return false;
            }

            byte[] byterArr = stream.ToArray();
            //写入数据库
            string cmdStr = string.Format("insert into player set user_name='{0}',data=@data;",userName);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            cmd.Parameters.Add("@data", MySqlDbType.Blob);
            cmd.Parameters[0].Value = byterArr;

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]CreatePlayer 写入 " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 检测用户名和密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool CheckPWD(string userName, string pwd)
        {
            if (!isSafeStr(userName) || !isSafeStr(pwd)) return false;

            //查询
            string cmdStr = string.Format("select * from user where user_name='{0}' and user_pwd='{1}';", userName, pwd);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);

            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return hasRows;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]CheckPWD " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取玩家数据
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public PlayerData GetPlayerData(string userName)
        {
            PlayerData playData = null;

            if (!isSafeStr(userName)) return playData;

            string cmdStr = string.Format("select * from player where user_Name='{0}';", userName);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            byte[] buffer = new byte[1];

            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    return playData;
                }

                dataReader.Read();
                long len = dataReader.GetBytes(2, 0, null, 0, 0);
                //1是data
                buffer = new byte[len];
                dataReader.GetBytes(2, 0, buffer, 0, (int)len);
                dataReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]GetPlayerData 查询 " + e.Message);
                return playData;
            }

            //反序列化
            MemoryStream stream = new MemoryStream(buffer);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                playData = formatter.Deserialize(stream) as PlayerData;
                return playData;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]GetPlayerData 反序列化 " + e.Message);
                return playData;
            }

            return playData;
        }

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool SavePlayer(Player player)
        {
            string userName = player.userName;
            PlayerData playerData = player.data;

            //序列化
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, playerData);
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]GetPlayerData 序列化 " + e.Message);
                return false;
            }
            byte[] byteArr = stream.ToArray();
            //写入数据库
            string formatStr = "Update player set data=@data where user_name='{0}';";
            string cmdStr = string.Format(formatStr, player.userName);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            cmd.Parameters.Add("@data", MySqlDbType.Blob);
            cmd.Parameters[0].Value = byteArr;
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]GetPlayerData 写入数据库 " + e.Message);
                return false;
            }
        }
    }
}
