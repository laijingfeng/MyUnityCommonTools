using ProtoBuf;

//添加特性，表示可以被ProtoBuf工具序列化
[ProtoContract]
public class NetModel
{
    //添加特性，表示该字段可以被序列化，1可以理解为下标
    [ProtoMember(1)]
    public int ID;
    [ProtoMember(2)]
    public string Commit;
    [ProtoMember(3)]
    public string Message;
}