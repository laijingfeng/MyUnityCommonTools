using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;

public class NetServer
{
    //单例脚本
    public static readonly NetServer Instance = new NetServer();
    //定义tcp服务器
    private Socket server;
    private int maxClient = 10;
    //定义端口
    private int port = 35353;
    //用户池
    private Stack<NetUserToken> pools;
    private NetServer()
    {
        //初始化socket
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Any, port));
    }

    //开启服务器
    public void Start()
    {
        server.Listen(maxClient);
        UnityEngine.Debug.Log("Server OK!");
        //实例化客户端的用户池
        pools = new Stack<NetUserToken>(maxClient);
        for (int i = 0; i < maxClient; i++)
        {
            NetUserToken usertoken = new NetUserToken();
            pools.Push(usertoken);
        }
        //可以异步接受客户端, BeginAccept函数的第一个参数是回调函数，当有客户端连接的时候自动调用
        server.BeginAccept(AsyncAccept, null);
    }

    //回调函数， 有客户端连接的时候会自动调用此方法
    private void AsyncAccept(IAsyncResult result)
    {
        try
        {
            //结束监听，同时获取到客户端
            Socket client = server.EndAccept(result);
            UnityEngine.Debug.Log("有客户端连接");
            //来了一个客户端
            NetUserToken userToken = pools.Pop();
            userToken.socket = client;
            //客户端连接之后，可以接受客户端消息
            BeginReceive(userToken);

            //尾递归，再次监听是否还有其他客户端连入
            server.BeginAccept(AsyncAccept, null);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(ex.ToString());
        }
    }

    //异步监听消息
    private void BeginReceive(NetUserToken userToken)
    {
        try
        {
            //异步方法
            userToken.socket.BeginReceive(userToken.buffer, 0, userToken.buffer.Length, SocketFlags.None,
                                          EndReceive, userToken);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(ex.ToString());
        }
    }

    //监听到消息之后调用的函数
    private void EndReceive(IAsyncResult result)
    {
        try
        {
            //取出客户端
            NetUserToken userToken = result.AsyncState as NetUserToken;
            //获取消息的长度
            int len = userToken.socket.EndReceive(result);
            if (len > 0)
            {
                byte[] data = new byte[len];
                Buffer.BlockCopy(userToken.buffer, 0, data, 0, len);
                //用户接受消息
                userToken.Receive(data);
                //尾递归，再次监听客户端消息
                BeginReceive(userToken);
            }

        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(ex.ToString());
        }
    }
}