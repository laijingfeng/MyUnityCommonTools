using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class CreateServer : MonoBehaviour {

    void StartServer () {
        NetServer.Instance.Start();
    }

}

/*
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Temp
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            TcpClient tc = new TcpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 35353);
            tc.Connect(ip);

            if(tc.Connected)
            {
                while(true)
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
*/