using System;
using System.Text;

namespace Common
{
    /// <summary>
    /// 命令整体结构
    /// </summary>
    [Serializable]
    public class CmdTag
    {
        /// <summary>
        /// 当前指令的socketID
        /// </summary>
        public string SocketID;
        /// <summary>
        /// 协议体
        /// </summary>
        public byte CmdType;
        /// <summary>
        /// 命令字
        /// </summary>
        public byte CmdCode;
        /// <summary>
        /// 执行状态
        /// </summary>
        public byte CmdState;
        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceIdTag DeviceInfo;
        /// <summary>
        /// 参数区
        /// </summary>
        public byte[] Param;
        public CmdTag()
        {
            CmdState = 0x00;
            DeviceInfo = new DeviceIdTag();
            Param = new byte[0];
            SocketID = "";
        }

        /// <summary>
        /// 将字节流转换成命令
        /// </summary>
        /// <param name="cmddata"></param>
        /// <returns></returns>
        public CmdTag Parse(byte[] cmddata)
        {
            CmdTag cmd = new CmdTag();

            //命令类
            cmd.CmdType = cmddata[3];
            // cmd.CmdType = (UInt16)((cmd.CmdType << 8) | cmddata[4]);

            //命令字
            cmd.CmdCode = cmddata[4];
            // cmd.CmdCode = (UInt16)((cmd.CmdCode << 8) | cmddata[6]);

            //指令状态
            cmd.CmdState = cmddata[5];

            //设备ID
            Array.Copy(cmddata, 6, cmd.DeviceInfo.Serial, 0, 7);
            cmd.DeviceInfo.Parse(cmd.DeviceInfo.Serial);

            //参数区
            cmd.Param = new byte[cmddata.Length - CommandCommon.CMD_MIN_LENGTH];

            Array.Copy(cmddata, 13, cmd.Param, 0, cmd.Param.Length);

            return cmd;
        }

    }

    #region 设备id组成
    public class DeviceIdTag
    {
        /// <summary>
        /// 设备序号
        /// </summary>
        public byte[] Serial;
        /// <summary>
        /// 设备序列号字串
        /// </summary>
        public string SerialString;
        /// <summary>
        /// 设备的序号长整型
        /// </summary>
        public long SerialLong;

        public DeviceIdTag()
        {
            Serial = new byte[7];
        }

        /// <summary>
        /// 将字节填充至类
        /// </summary>
        /// <param name="data"></param>
        public void Parse(byte[] data)
        {
            int length = 0;

            length = data.Length;
            if (data.Length > Serial.Length)
            {
                length = Serial.Length;
            }

            Array.Copy(data, Serial, length);
            this.SerialLong = FormatHelper.ByteArrayToBigInt_BigMode(Serial, 0, Serial.Length);
            //this.SerialString = Utils.FormatHelper.BCDToString(Serial);
            this.SerialString = string.Format("{0:X14}", SerialLong);
        }

        /// <summary>
        /// 转换为协议
        /// </summary>
        /// <param name="deviceID"></param>
        public void Parse(long deviceID)
        {
            this.SerialLong = FormatHelper.ToBCD(deviceID);
            FormatHelper.LongToByteArray_BigMode(Serial, this.SerialLong, 0);
            //this.SerialString = Utils.FormatHelper.BCDToString(Serial);
            this.SerialString = string.Format("{0:X14}", SerialLong);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialString"></param>
        /// <returns></returns>
        public byte[] GetDeviceSerialBytes(string serialString)
        {
            byte[] device = new byte[7];
            string t;
            int index = 0;
            byte vv = 0;

            if (serialString.Length < 14) return device;

            for (int i = 0; i < 14; i += 2)
            {
                vv = byte.Parse(serialString.Substring(i, 1));
                vv = (byte)((vv << 4) | (byte.Parse(serialString.Substring(i + 1, 1))));

                device[index] = vv;
                index = index + 1;
            }

            return device;
        }

        public string GetDeviceSerialString(byte[] serial)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;

            if (serial.Length < 7) return "";

            while (i < 7)
            {
                sb.Append(string.Format("{0:X2}", serial[i]));
                i++;
            }

            return sb.ToString();

        }

        public string GetDeviceSerialString(byte[] serial, int startIndex)
        {
            byte[] param = new byte[7];

            Array.Copy(serial, startIndex, param, 0, 7);

            return GetDeviceSerialString(param);
        }
    }
    #endregion

    #region 命令执行状态
    /// <summary>
    /// 命令执行状态
    /// </summary>
    public class CommandState
    {
        /// <summary>
        /// 命令执行成功
        /// </summary>
        public const byte CMD_STATE_SUCCESS = 0x00;
        /// <summary>
        /// 命令执行失败
        /// </summary>
        public const byte CMD_STATE_FAIL = 0x01;
        /// <summary>
        /// 主动发送的指令
        /// </summary>
        public const byte CMD_STATE_SEND = 0x02;
        /// <summary>
        /// 命令不支持
        /// </summary>
        public const byte CMD_STATE_NOT_SUPPORT = 0x03;
    }
    #endregion

    #region 构造和解析协议的类
    /// <summary>
    /// 用于构造和解析消息的类
    /// </summary>
    public class Command
    {
        #region private

        /// <summary>
        /// 将接收到的命令转换成原始命令
        /// </summary>
        /// <param name="cmd">需要转换的命令</param>
        /// <param name="safeCode">转义码</param>
        /// <param name="sourceCmd">转换后的命令</param>
        /// <returns>校验CRC</returns>
        private static UInt16 ConvertSafeCodeToCmdSigByte(byte[] cmd, byte safeCode, out byte[] sourceCmd)
        {
            UInt16 crc = 0;
            byte[] _mycmd = new byte[cmd.Length];
            int length = 1, i = 1;
            byte sigByte = 0;

            sigByte = (byte)(safeCode - 1);

            _mycmd[0] = cmd[0];

            while (i < (cmd.Length - 1))
            {
                _mycmd[length] = cmd[i];

                if (cmd[i] == sigByte)
                {
                    if (cmd[i + 1] == 0x01)
                    {
                        // length -= 1;
                        _mycmd[length] = safeCode;
                        length += 1;
                        i = i + 2;
                        continue;
                    }
                    if (cmd[i + 1] == 0x02)
                    {
                        //length -= 1;
                        // _mycmd[length] = (byte)(safeCode + 0x01);
                        length += 1;
                        i = i + 2;
                        continue;
                    }
                }

                i++;
                length += 1;
            }

            length += 1;
            sourceCmd = new byte[length];
            Array.Copy(_mycmd, 0, sourceCmd, 0, length);
            sourceCmd[length - 1] = cmd[i];

            crc = FormatHelper.CRC16(sourceCmd, 0, sourceCmd.Length - 3);

            return crc;
        }

        #endregion

        #region 生成命令

        /// <summary>
        /// 生成平台间通信协议
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdCode"></param>
        /// <param name="cmdState"></param>
        /// <returns></returns>
        public static byte[] BuildPlatformCmd(byte cmdType, byte cmdCode, byte cmdState)
        {
            return BuildCmd(cmdType, cmdCode, cmdState, CommandCommon.CMD_PLATFORM_SAFE_CODE);
        }

        /// <summary>
        /// 生成平台间的通信协议
        /// </summary>
        /// <param name="cmdTag"></param>
        /// <param name="safeCode"></param>
        /// <returns></returns>
        public static byte[] BuildPlatformCmd(CmdTag cmdTag)
        {
            return BuildCmd(cmdTag, CommandCommon.CMD_PLATFORM_SAFE_CODE);
        }

        /// <summary>
        /// 生成设备间的通信协议
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdCode"></param>
        /// <param name="cmdState"></param>
        /// <returns></returns>
        public static byte[] BuildDeviceCmd(byte cmdType, byte cmdCode, byte cmdState)
        {
            return BuildCmd(cmdType, cmdCode, cmdState, CommandCommon.CMD_DEVICE_SAFE_CODE);
        }

        /// <summary>
        /// 生成设备间的通信协议
        /// </summary>
        /// <param name="cmdTag"></param>
        /// <returns></returns>
        public static byte[] BuildDeviceCmd(CmdTag cmdTag)
        {
            return BuildCmd(cmdTag, CommandCommon.CMD_DEVICE_SAFE_CODE);
        }

        private static byte[] BuildCmd(byte cmdType, byte cmdCode, byte cmdState, byte safeCode)
        {
            CmdTag tag = new CmdTag();

            tag.CmdType = cmdType;
            tag.CmdCode = cmdCode;
            tag.CmdState = cmdState;

            tag.Param = new byte[0];
            tag.DeviceInfo = new DeviceIdTag();

            return BuildCmd(tag, safeCode);
        }

        /// <summary>
        /// 生成命令
        /// </summary>
        /// <param name="cmdTag">指令结构体</param>
        /// <param name="safeCode">安全码，即命令头尾</param>
        /// <returns>相关命令</returns>
        private static byte[] BuildCmd(CmdTag cmdTag, byte safeCode)
        {
            byte[] cmd = new byte[CommandCommon.CMD_MIN_LENGTH + cmdTag.Param.Length];
            int cmdIndex = 0;
            int i = 0;
            byte sigByte = 0x00;

            sigByte = (byte)(safeCode - 0x01);

            //开始组合
            //1.起始标识位
            cmd[0] = safeCode;
            cmdIndex += 1;

            //2.长度
            cmd[cmdIndex] = (byte)(cmd.Length & 0x00FF);
            cmdIndex += 1;
            cmd[cmdIndex] = (byte)((cmd.Length & 0xFF00) >> 8);
            cmdIndex += 1;

            //生成命令类
            cmd[cmdIndex] = (byte)(cmdTag.CmdType & 0x00FF);
            cmdIndex += 1;

            //生成命字
            cmd[cmdIndex] = (byte)(cmdTag.CmdCode & 0x00FF);
            cmdIndex += 1;

            //状态
            cmd[cmdIndex] = cmdTag.CmdState;
            cmdIndex += 1;

            //设备ID
            Array.Copy(cmdTag.DeviceInfo.Serial, 0, cmd, cmdIndex, cmdTag.DeviceInfo.Serial.Length);
            cmdIndex += cmdTag.DeviceInfo.Serial.Length;

            //数据区
            Array.Copy(cmdTag.Param, 0, cmd, cmdIndex, cmdTag.Param.Length);
            //cmdIndex += cmdTag.Param.Length;

            //计算校验码
            UInt16 crc = FormatHelper.CRC16(cmd, 0, cmd.Length - 3);
            //添加校验码
            cmd[cmd.Length - 3] = (byte)((crc & 0xFF00) >> 8);
            // cmdIndex += 1;
            cmd[cmd.Length - 2] = (byte)(crc & 0x00FF);
            // cmdIndex += 1;
            cmd[cmd.Length - 1] = safeCode;
            // cmdIndex += 1;

            //处理转义字
            byte[] temp = new byte[cmd.Length * 2];
            i = 1;
            cmdIndex = 1;
            temp[0] = cmd[0];
            while (i < (cmd.Length - 1))
            {
                temp[cmdIndex] = cmd[i];

                if (temp[cmdIndex] == sigByte)
                {
                    // temp[cmdIndex] = safeCode;
                    cmdIndex += 1;
                    temp[cmdIndex] = 0x02;
                }

                if (temp[cmdIndex] == safeCode)
                {
                    temp[cmdIndex] = sigByte;
                    cmdIndex += 1;
                    temp[cmdIndex] = 0x01;
                }

                cmdIndex += 1;
                i++;
            }

            //添加标识尾
            temp[cmdIndex] = safeCode;
            cmdIndex += 1;

            byte[] ret = new byte[cmdIndex];
            Array.Copy(temp, ret, cmdIndex);

            return ret;
        }

        #endregion

        #region 解析网络命令

        /// <summary>
        /// 解析平台间的通信协议
        /// </summary>
        /// <param name="cmdData"></param>
        /// <param name="cmdList"></param>
        /// <param name="socketID">数据源SocketID</param>
        public static void ParsePlatFormCmd(ref CmdBuffer cmdData, ref MemQueue<CmdTag> cmdList, string socketID)
        {
            ParseCmd(ref cmdData, CommandCommon.CMD_PLATFORM_SAFE_CODE, ref cmdList, socketID);
        }

        /// <summary>
        /// 解析设备间通信协议
        /// </summary>
        /// <param name="cmdData"></param>
        /// <param name="cmdList"></param>
        public static void ParseDeviceCmd(ref CmdBuffer cmdData, ref MemQueue<CmdTag> cmdList, string socketID)
        {
            ParseCmd(ref cmdData, CommandCommon.CMD_DEVICE_SAFE_CODE, ref cmdList, socketID);
        }

        /// <summary>
        /// 解析命令，返回处理后的命令列表
        /// </summary>
        /// <param name="cmdData">被处理命令的buffer</param>
        /// <param name="safeCode">需要转义的转义码</param>
        /// <param name="cmdList">处理后的命令列表</param>
        private static void ParseCmd(ref CmdBuffer cmdData, byte safeCode, ref MemQueue<CmdTag> cmdList, string socketID)
        {

            CmdBuffer temp = new CmdBuffer();
            byte sigByte = 0x00;
            UInt16 cmd_length = 0;

            sigByte = (byte)(safeCode - 0x01);

            //解析命令,如果接收的数据长度小于最短命令长度,即命令接收不完整
            if (cmdData.Length < CommandCommon.CMD_MIN_LENGTH)
            {
                return;
            }

            int sigByteStartIndex = 0;
            lock (cmdData.Buffer)
            {
                //  Debug.Print("Parse:{0}", FormatHelper.ByteArrayToHexString(cmdData.Buffer, cmdData.Length));
                //查找协议头
                sigByteStartIndex = Array.IndexOf(cmdData.Buffer, safeCode, 0, cmdData.Length);
            }

            //没有找到协议头
            if (sigByteStartIndex < 0)
            {
                cmdData.Length = 0;
                return;
            }

            //数据长度不够,即最后一个字节为协议标识
            if ((sigByteStartIndex + 1) == cmdData.Length)
            {
                cmdData.Buffer[0] = safeCode;
                cmdData.Length = 1;
                return;
            }

            //第一字节非协议头
            if (sigByteStartIndex > 0)
            {
                cmdData.Length = cmdData.Length - sigByteStartIndex;

                if ((cmdData.Length <= 0) || (cmdData.Length >= CmdBuffer.BUFFER_MAX_LENGTH))
                {
                    cmdData.Length = 0;
                    return;
                }

                Array.Copy(cmdData.Buffer, sigByteStartIndex, cmdData.Buffer, 0, cmdData.Length);
            }

            //查找协议尾
            int sigByteEndIndex = 0;

            lock (cmdData.Buffer)
            {
                sigByteEndIndex = Array.IndexOf(cmdData.Buffer, safeCode, 1, cmdData.Length - 1);
            }

            //没有找到协议尾
            if (sigByteEndIndex < 0)
            {
                return;
            }

            //包头包尾粘连
            if (sigByteEndIndex == 1)
            {
                cmdData.Length -= 1;

                if (cmdData.Length <= 0)
                {
                    cmdData.Length = 0;
                    return;
                }
                if (cmdData.Length == 1)
                {
                    cmdData.Buffer[0] = safeCode;
                    cmdData.Length = 1;
                    return;
                }
                Array.Copy(cmdData.Buffer, 1, cmdData.Buffer, 0, cmdData.Length);
                sigByteEndIndex = Array.IndexOf(cmdData.Buffer, safeCode, 1, cmdData.Length - 1);
            }

            //长度小于最短命令长度
            if ((sigByteEndIndex + 1) < CommandCommon.CMD_MIN_LENGTH)
            {
                cmdData.Length = cmdData.Length - sigByteEndIndex - 1;

                if (cmdData.Length > 0)
                {
                    Array.Copy(cmdData.Buffer, sigByteEndIndex + 1, cmdData.Buffer, 0, cmdData.Length);
                }
                else
                {
                    cmdData.Length = 0;
                }
                return;
            }

            //当前命令超长
            if ((sigByteEndIndex + 1) >= CommandCommon.CMD_MAX_LENGTH)
            {
                cmdData.Length = cmdData.Length - sigByteEndIndex - 1;

                if (cmdData.Length >= CmdBuffer.BUFFER_MAX_LENGTH)
                {
                    cmdData.Length = 0;
                    return;
                }

                if (cmdData.Length > 0)
                {
                    Array.Copy(cmdData.Buffer, sigByteEndIndex + 1, cmdData.Buffer, 0, cmdData.Length);

                    //递归调用
                    if (cmdData.Length >= CommandCommon.CMD_MIN_LENGTH)
                    {
                        ParseCmd(ref cmdData, safeCode, ref cmdList, socketID);
                    }
                    return;
                }
                if (cmdData.Length < 0)
                {
                    cmdData.Length = 0;
                }
                return;
            }

            //拷贝命令数据
            byte[] cmd_bytearray = new byte[sigByteEndIndex + 1];
            Array.Copy(cmdData.Buffer, 0, cmd_bytearray, 0, cmd_bytearray.Length);

            cmdData.Length = cmdData.Length - cmd_bytearray.Length;

            if (cmdData.Length >= CmdBuffer.BUFFER_MAX_LENGTH)
            {
                cmdData.Length = 0;
                return;
            }

            //安全容错
            if (cmdData.Length > 0)
            {
                Array.Copy(cmdData.Buffer, sigByteEndIndex + 1, cmdData.Buffer, 0, cmdData.Length);
            }

            if (cmdData.Length < 0)
            {
                cmdData.Length = 0;
            }

            //执行解析
            //1:处理转义字符
            byte[] parsed_bytearray;
            //调用时,除去头尾
            UInt16 crc1 = ConvertSafeCodeToCmdSigByte(cmd_bytearray, safeCode, out parsed_bytearray);

            UInt16 crc2 = 0x00;

            // Debug.Print("Cmd:{0}", Utils.FormatHelper.ByteArrayToHexString( parsed_bytearray ));
            crc2 = parsed_bytearray[parsed_bytearray.Length - 3];
            crc2 = (UInt16)((crc2 << 8) | parsed_bytearray[parsed_bytearray.Length - 2]);
            //2:校验crc
            if (crc1 != crc2)
            {
                //Debug.Print("CRC非法,计算值:0x{0:X4} 指令值:{1:X4}", crc1, crc2);
                //递归调用
                if (cmdData.Length >= CommandCommon.CMD_MIN_LENGTH)
                {
                    ParseCmd(ref cmdData, safeCode, ref cmdList, socketID);
                }

                return;
            }

            //长度
            cmd_length = parsed_bytearray[2];
            cmd_length = (UInt16)((cmd_length << 8) | parsed_bytearray[1]);

            //指令长度非法
            if ((cmd_length < CommandCommon.CMD_MIN_LENGTH) || (cmd_length > CommandCommon.CMD_MAX_LENGTH))
            {
                //Debug.Print("指令长度与协议长度不匹配,指令长度:{0} 数据:{1}", cmd_length, parsed_bytearray.Length);
                //递归调用
                if (cmdData.Length >= CommandCommon.CMD_MIN_LENGTH)
                {
                    ParseCmd(ref cmdData, safeCode, ref cmdList, socketID);
                }
                return;
            }

            //3:开始解析成命令
            CmdTag cmd = new CmdTag();

            cmd = cmd.Parse(parsed_bytearray);
            cmd.SocketID = socketID;

            cmdList.Add(cmd);           //加入至命令组

            ////递归调用
            //if (cmdData.Length >= Common.CMD_MIN_LENGTH)
            //{
            //    ParseCmd(ref cmdData, safeCode, ref cmdList,socketID);
            //}
        }
        #endregion
    }
    #endregion
}
