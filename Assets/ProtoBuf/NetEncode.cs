using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 编码和解码
/// </summary>
public class NetEncode
{
    /// <summary>
    /// 将数据编码 长度+内容
    /// </summary>
    /// <param name="data">内容</param>
    public static byte[] Encode(byte[] data)
    {
        //整形占四个字节，所以声明一个+4的数组
        byte[] result = new byte[data.Length + 4];
        //使用流将编码写二进制
        MemoryStream ms = new MemoryStream();
        BinaryWriter br = new BinaryWriter(ms);
        br.Write(data.Length);
        br.Write(data);
        //将流中的内容复制到数组中
        System.Buffer.BlockCopy(ms.ToArray(), 0, result, 0, (int)ms.Length);
        br.Close();
        ms.Close();
        return result;
    }

    /// <summary>
    /// 将数据解码
    /// </summary>
    /// <param name="cache">消息队列</param>
    public static byte[] Decode(ref List<byte> cache)
    {
        //首先要获取长度，整形4个字节，如果字节数不足4个字节
        if (cache.Count < 4)
        {
            return null;
        }
        //读取数据
        MemoryStream ms = new MemoryStream(cache.ToArray());
        BinaryReader br = new BinaryReader(ms);
        int len = br.ReadInt32();
        //根据长度，判断内容是否传递完毕
        if (len > ms.Length - ms.Position)
        {
            return null;
        }
        //获取数据
        byte[] result = br.ReadBytes(len);
        //清空消息池
        cache.Clear();
        //讲剩余没处理的消息存入消息池
        cache.AddRange(br.ReadBytes((int)ms.Length - (int)ms.Position));

        return result;
    }
}