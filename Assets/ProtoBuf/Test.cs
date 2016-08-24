using System;
using System.IO;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        NetModel item = new NetModel() { ID = 1, Commit = "Hello", Message = "Unity" };
        
        //序列化对象
        byte[] temp = Serialize(item);
        Debug.Log(temp.Length);
        
        //反序列化为对象
        NetModel result = DeSerialize(temp);
        Debug.Log(result.Message);

    }

    /// <summary>
    /// 将消息序列化为二进制的方法
    /// </summary>
    /// <param name="model">要序列化的对象</param>
    private byte[] Serialize(NetModel model)
    {
        try
        {
            //涉及格式转换，需要用到流，将二进制序列化到流中
            using (MemoryStream ms = new MemoryStream())
            {
                //使用ProtoBuf工具的序列化方法
                ProtoBuf.Serializer.Serialize<NetModel>(ms, model);
                //定义二级制数组，保存序列化后的结果
                byte[] result = new byte[ms.Length];
                //将流的位置设为0，起始点
                ms.Position = 0;
                //将流中的内容读取到二进制数组中
                ms.Read(result, 0, result.Length);
                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("序列化失败: " + ex.ToString());
            return null;
        }
    }

    /// <summary>
    /// 将收到的消息反序列化成对象
    /// </summary>
    /// <returns>The serialize.</returns>
    /// <param name="msg">收到的消息.</param>
    private NetModel DeSerialize(byte[] msg)
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
            Debug.Log("反序列化失败: " + ex.ToString());
            return null;
        }
    }
}