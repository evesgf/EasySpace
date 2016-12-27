using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySpaceServer
{
    class SqlHelper
    {
        MySqlConnection sqlConn;
        string connStr = "Database=msgboard;Data Source=127.0.0.1;";

        public void ConnSQL()
        {
            connStr += "User Id=root;Password=123456;port=3306";
            sqlConn = new MySqlConnection(connStr);

            try
            {
                sqlConn.Open();
            }
            catch (Exception e)
            {
                Console.Write("[数据库]连接失败" + e.Message); return;
            }
        }
    }
}
