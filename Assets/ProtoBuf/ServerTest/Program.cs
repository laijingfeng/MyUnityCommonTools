using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient tc = new TcpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 35353);
            tc.Connect(ip);

            if (tc.Connected)
            {
                while (true)
                {
                    string msg = Console.ReadLine();
                    byte[] result = Encoding.UTF8.GetBytes(msg);
                    tc.GetStream().Write(result, 0, result.Length);
                }
            }
            Console.ReadLine();
        }
    }
}