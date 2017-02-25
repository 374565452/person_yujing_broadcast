using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTU.GateWay.Protocol.WaterMessageClass
{
    public class Identifier_00: Identifier
    {
        public Identifier_00(byte Key)
        {
            this.Key = Key;
        }

        /// <summary>
        /// 标识符引导符
        /// </summary>
        public byte Key
        {
            get;
            set;
        }

        /// <summary>
        /// 标识符数据定义
        /// </summary>
        public byte Length
        {
            get;
            set;
        }

        public override byte GetKey()
        {
            return Key;
        }

        public override byte GetLength()
        {
            return Length;
        }

        /// <summary>
        /// 标识符数据
        /// </summary>
        public string Val
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("【" + Key + "】【" + Length + "】：");
            builder.Append(Val + "，");
            return builder.ToString();
        }

        public override string GetHexStr()
        {
            return "";
        }

        public override string GetHexStrHead()
        {
            return "";
        }

        public override void SetVal(int ByteCount, int Digits, string hexStr)
        {
            this.Length = Convert.ToByte(WaterBaseProtocol.GetLengthHexStr(ByteCount, Digits), 16);
            this.Val = hexStr;
        }
    }
}
