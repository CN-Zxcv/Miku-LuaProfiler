#if UNITY_EDITOR || USE_LUA_PROFILER
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Rendering;
using UnityEngine;

namespace MikuLuaProfiler
{
    // Token: 0x02000002 RID: 2
    public sealed class MBinaryWriter : BinaryWriter
    {
        private MemoryStream memoryStream;
        private Stream realOutStream;
        // Hx@流式写入第三方不好处理读取, 阻塞等待读取不方便实现, 效率也不高
        // 改为采用包长+包体方式
        // BeginWrite/EndWrite拦截写入流
        public void BeginWrite()
        {
            memoryStream = new MemoryStream();
            OutStream = memoryStream;
        }
        public unsafe void EndWrite()
        {
            fixed (byte* ptr = _buffer)
            {
                *(uint*)ptr = (uint)memoryStream.Length;
            }
            Debug.Log("EndWrite: " + memoryStream.Length);

            realOutStream.Write(this._buffer, 0, 4);
            realOutStream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            memoryStream.Dispose();
            memoryStream = null;
            OutStream = realOutStream;
        }

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public MBinaryWriter(Stream output) : base(output)
        {
            this._buffer = new byte[8];
            realOutStream = OutStream;
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
        public unsafe override void Write(float value)
        {
            fixed (byte* ptr = _buffer)
            {
                *(float*)ptr = value;
            }
            this.OutStream.Write(this._buffer, 0, 4);
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000020AC File Offset: 0x000002AC
        public unsafe override void Write(short value)
        {
            fixed (byte* ptr = _buffer)
            {
                *(short*)ptr = value;
            }
            this.OutStream.Write(this._buffer, 0, 2);
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020F0 File Offset: 0x000002F0
        public unsafe override void Write(ushort value)
        {
            fixed (byte* ptr = _buffer)
            {
                *(ushort*)ptr = value;
            }
            this.OutStream.Write(this._buffer, 0, 2);
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002134 File Offset: 0x00000334
        public unsafe override void Write(int value)
        {
            fixed (byte* ptr = _buffer)
            {
                *(int*)ptr = value;
            }
            this.OutStream.Write(this._buffer, 0, 4);
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002178 File Offset: 0x00000378
        public unsafe override void Write(uint value)
        {
            fixed (byte* ptr = _buffer)
            {
                *(uint*)ptr = value;
            }
            this.OutStream.Write(this._buffer, 0, 4);
        }

        // Token: 0x04000001 RID: 1
        private byte[] _buffer;
        
        #region string
        private int m_key = 0;
        private int GetUniqueKey()
        {
            return m_key++;
        }
        private Dictionary<string, KeyValuePair<int, byte[]>> m_strDict = new Dictionary<string, KeyValuePair<int, byte[]>>(8192);
        private bool GetBytes(string s, out byte[] result, out int index)
        {
            bool ret = false;
            KeyValuePair<int, byte[]> keyValuePair;
            if (!m_strDict.TryGetValue(s, out keyValuePair))
            {
                result = Encoding.UTF8.GetBytes(s);
                index = GetUniqueKey();
                keyValuePair = new KeyValuePair<int, byte[]>(index, result);
                m_strDict.Add(s, keyValuePair);
                ret = false;
            }
            else
            {
                ret = true;
                index = keyValuePair.Key;
                result = keyValuePair.Value;
            }

            return ret;
        }
        
        public override void Write(string v)
        {
            byte[] datas;
            int index = 0;
            bool isRef = GetBytes(v, out datas, out index);
            Write(isRef);
            Write(index);
            if (!isRef)
            {
                Write(datas.Length);
                Write(datas);
            }
        }
        
        #endregion
    }
}
#endif