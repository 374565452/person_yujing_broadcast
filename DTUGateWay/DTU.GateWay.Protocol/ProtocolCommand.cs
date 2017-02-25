using Common;
using System;

namespace DTU.GateWay.Protocol
{
    public class ProtocolCommand
    {
        /// <summary>
        /// 生成平台间的通信协议
        /// </summary>
        /// <param name="cmdTag"></param>
        /// <param name="safeCode"></param>
        /// <returns></returns>
        public static byte[] buildPlatFormCommand(Command cmd)
        {
            return buildCommand(cmd, ProtocolConst.CmdPlatFormSafeCode);
        }
        #region 生成平台通信间指令
        private static byte[] buildCommand(Command command, byte safeCode)
        {

            byte[] cmd = new byte[ProtocolConst.CmdMinLength + command.Data.Length];
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
            cmd[cmdIndex] = (byte)(command.CommandType & 0x00FF);
            cmdIndex += 1;

            //生成命字
            cmd[cmdIndex] = (byte)(command.CommandCode & 0x00FF);
            cmdIndex += 1;


            //状态
            cmd[cmdIndex] = command.CommandState;
            cmdIndex += 1;

            //设备ID
            Array.Copy(command.DeviceInfo.SerialNum, 0, cmd, cmdIndex, command.DeviceInfo.SerialNum.Length);
            cmdIndex += command.DeviceInfo.SerialNum.Length;

            //数据区
            Array.Copy(command.Data, 0, cmd, cmdIndex, command.Data.Length);
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

        #region 将接收到的命令转换成原始命令
        /// <summary>
        /// 将接收到的命令转换成原始命令
        /// </summary>
        /// <param name="cmd">需要转换的命令</param>
        /// <param name="safeCode">转义码</param>
        /// <param name="sourceCmd">转换后的命令</param>
        /// <returns>校验CRC</returns>
        public static UInt16 ConvertSafeCodeToCmdSigByte(byte[] cmd, byte safeCode, out byte[] sourceCmd, int cmdSize)
        {
            UInt16 crc = 0;
            byte[] _mycmd = new byte[cmd.Length];
            int length = 1, i = 1;
            byte sigByte = 0;

            sigByte = (byte)(safeCode - 1);

            _mycmd[0] = cmd[0];

            while (i < (cmdSize - 1))
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
    }
}
