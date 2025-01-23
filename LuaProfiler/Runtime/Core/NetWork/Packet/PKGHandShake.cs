#if UNITY_EDITOR || USE_LUA_PROFILER

namespace MikuLuaProfiler 
{
    [PacketMsg(MsgHead.Handshake)]
    public class PKGHandshake : PacketBase<PKGHandshake>
    {
        public override MsgHead MsgHead
        {
            get { return MsgHead.Handshake; }
        }
        public string lua;

        public override void Read(MBinaryReader br)
        {
            lua = br.ReadString();
        }

        public override void Write(MBinaryWriter bw)
        {
            bw.Write(lua);
        }
    }
}

#endif