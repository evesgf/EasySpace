using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SendBox
{
    class Program
    {
        static void Main(string[] arg)
        {
            string str = "POS 127.0.0.1:4455 30 40 65";
            string[] args = str.Split(' ');
            string protoName = args[0];
            string id = args[1];
            float x = float.Parse(args[2]);
            float y = float.Parse(args[3]);
            float z = float.Parse(args[4]);
            Console.WriteLine(x + "-" + y + "-" + z);

            Timer timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(Tick);
            timer.Start();

            Console.ReadKey(true);
        }

        private static void Tick(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("每秒执行一次");
        }
    }
}
