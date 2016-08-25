using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;

/// <summary>
/// 表示一个客户端
/// </summary>
public class NetUserToken
{
    //连接客户端的Socket
    public Socket socket;
    
    //用于存放接收数据
    public byte[] buffer;

    //每次接受和发送数据的大小
    private const int size = 1024;

    //接收数据池
    private List<byte> receiveCache;
    private bool isReceiving;
    
    //发送数据池
    private Queue<byte[]> sendCache;
    private bool isSending;

    //接收到消息之后的回调
    public Action<NetModel> receiveCallBack;

    public NetUserToken()
    {
        buffer = new byte[size];
        receiveCache = new List<byte>();
        sendCache = new Queue<byte[]>();
    }

    /// <summary>
    /// 服务器接受客户端发送的消息
    /// </summary>
    /// <param name="data">Data.</param>
    public void Receive(byte[] data)
    {
        UnityEngine.Debug.Log("接收到数据");
        //将接收到的数据放入数据池中
        receiveCache.AddRange(data);
        //如果没在读数据
        if (!isReceiving)
        {
            isReceiving = true;
            ReadData();
        }
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    private void ReadData()
    {
        byte[] data = NetEncode.Decode(ref receiveCache);
        //说明数据保存成功
        if (data != null)
        {
            NetModel item = DeSerilizer(data);
            UnityEngine.Debug.Log(item.Message);
            if (receiveCallBack != null)
            {
                receiveCallBack(item);
            }
            //尾递归，继续读取数据
            ReadData();
        }
        else
        {
            isReceiving = false;
        }
    }

    /// <summary>
    /// 将收到的消息反序列化成对象
    /// </summary>
    /// <returns>The serialize.</returns>
    /// <param name="msg">收到的消息.</param>
    private NetModel DeSerilizer(byte[] msg)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //将消息写入流中
                ms.Write(msg, 0, msg.Length);
                //将流的位置归0
                ms.Position = 0;
                //使用工具反序列化对象
                NetModel result = ProtoBuf.Serializer.Deserialize<NetModel>(ms);
                return result;
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("反序列化失败: " + ex.ToString());
            return null;
        }
    }

    /// <summary>
    /// 服务器发送消息给客户端
    /// </summary>
    public void Send()
    {
        try
        {
            if (sendCache.Count == 0)
            {
                isSending = false;
                return;
            }
            byte[] data = sendCache.Dequeue();
            int count = data.Length / size;
            int len = size;
            for (int i = 0; i < count + 1; i++)
            {
                if (i == count)
                {
                    len = data.Length - i * size;
                }
                socket.Send(data, i * size, len, SocketFlags.None);
            }
            UnityEngine.Debug.Log("发送成功!");
            Send();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(ex.ToString());
        }
    }

    public void WriteSendDate(byte[] data)
    {
        sendCache.Enqueue(data);
        if (!isSending)
        {
            isSending = true;
            Send();
        }
    }
}