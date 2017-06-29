using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace LHManager
{
    public class prmDistribute
    {
        public byte second;
        public byte minute;
        public byte hour;
        public byte week;
        public byte day;
        public byte month;
        public byte year;
        public byte[] pos = new byte[12];
        public byte type;
        public byte max;
        public byte min;
    }

    public class LHSerialPort
        : SerialPort
    {
        public byte[] outputBuffer = new byte[32];
        public byte outputLength;


        //起始符
        public static byte beginning = 0xA5;
        //结束符
        public static byte terminal = 0x16;

        //命令域
        //轮询请求
        public static byte cmdPolling = 0x03;
        //整点数据请求
        public static byte cmdIntegral = 0x0C;
        //参数下发
        public static byte cmdDistribute = 0x0F;
        //启动
        public static byte cmdStartup = 0x30;
        //340状态直接重启
        public static byte cmdReboot340 = 0xCF;
        //强制重启
        public static byte cmdRebootHard = 0xCC;
        //修改时间
        public static byte cmdTimeModify = 0x3F;
        //已老化时间修改
        public static byte cmdLHTimeModify = 0xC0;
        //招记录信息
        public static byte cmdGetRecord = 0xC3;
        //否定应答
        public static byte cmdNegative = 0x33;
        //器件类型不符合应答
        public static byte cmdNotSame = 0x3C;

        public LHSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
        }

        //计算校验码
        private byte CS(byte[] input, int len)
        {
            byte[] input2 = new byte[len];
            for (int i = 0; i < len; i++)
            {
                input2[i] = input[i];
            }

            int num = 0;
            for (int i = 0; i < input2.Length; i++)
            {
                num += input2[i];
            }
            return Convert.ToByte(num % 256);
        }

        //发送轮询帧
        public void WritePolling(byte address)
        {
            byte[] wbuffer = new byte[7];
            wbuffer[0] = beginning;
            wbuffer[1] = 2;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdPolling;
            wbuffer[5] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[6] = terminal;

            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //发送启动帧
        public void WriteStartup(byte address)
        {
            byte[] wbuffer = new byte[7];
            wbuffer[0] = beginning;
            wbuffer[1] = 2;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdStartup;
            wbuffer[5] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[6] = terminal;

            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //发送招记录信息帧
        public void WriteGetRecord(byte address)
        {
            byte[] wbuffer = new byte[7];
            wbuffer[0] = beginning;
            wbuffer[1] = 2;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdGetRecord;
            wbuffer[5] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[6] = terminal;

            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //发送340状态重启帧
        public void WriteReboot340(byte address)
        {
            byte[] wbuffer = new byte[7];
            wbuffer[0] = beginning;
            wbuffer[1] = 2;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdReboot340;
            wbuffer[5] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[6] = terminal;

            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //发送强制重启帧
        public void WriteRebootHard(byte address)
        {
            byte[] wbuffer = new byte[7];
            wbuffer[0] = beginning;
            wbuffer[1] = 2;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdRebootHard;
            wbuffer[5] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[6] = terminal;

            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //发送参数下发帧
        public void WriteDistribute(byte address, prmDistribute prm)
        {
            byte[] wbuffer = new byte[29];
            wbuffer[0] = beginning;
            wbuffer[1] = 24;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdDistribute;
            wbuffer[5] = prm.second;
            wbuffer[6] = prm.minute;
            wbuffer[7] = prm.hour;
            wbuffer[8] = prm.week;
            wbuffer[9] = prm.day;
            wbuffer[10] = prm.month;
            wbuffer[11] = prm.year;
            for (int i = 0; i < 12; i++)
            {
                wbuffer[12 + i] = prm.pos[i];
            }
            wbuffer[24] = prm.type;
            wbuffer[25] = prm.max;
            wbuffer[26] = prm.min;
            wbuffer[27] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[28] = terminal;
            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //发送整点数据请求帧
        public void WriteIntegral(byte address, byte part, byte time)
        {
            byte[] wbuffer = new byte[8];
            wbuffer[0] = beginning;
            wbuffer[1] = 3;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdIntegral;
            wbuffer[5] = Convert.ToByte((part << 6) + time);
            wbuffer[6] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[7] = terminal;

            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //发送对时帧
        public void WriteTimeModify(byte address)
        {
            byte[] wbuffer = new byte[10];
            System.DateTime now = new System.DateTime();
            now = System.DateTime.Now;
            wbuffer[0] = beginning;
            wbuffer[1] = 5;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdTimeModify;
            wbuffer[5] = Convert.ToByte(now.Second);
            wbuffer[6] = Convert.ToByte(now.Minute);
            wbuffer[7] = Convert.ToByte(now.Hour);
            wbuffer[8] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[9] = terminal;

            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //发送已老化时间修改帧
        public void WriteLHTimeModify(byte address,byte second, byte minute, byte hourl, byte hourh)
        {
            byte[] wbuffer = new byte[11];

            wbuffer[0] = beginning;
            wbuffer[1] = 6;
            wbuffer[2] = beginning;
            wbuffer[3] = address;
            wbuffer[4] = cmdLHTimeModify;
            wbuffer[5] = second;
            wbuffer[6] = minute;
            wbuffer[7] = hourl;
            wbuffer[8] = hourh;
            wbuffer[9] = CS(wbuffer, wbuffer.Length - 2);
            wbuffer[10] = terminal;

            base.Write(wbuffer, 0, wbuffer.Length);

            outputBuffer = wbuffer;
            outputLength = Convert.ToByte(wbuffer.Length);
        }

        //接收信息
        public byte Readup(byte[] rbuffer)
        {
            for(int i = 0; i < rbuffer.Length; i++)
            {
                rbuffer[i] = 0;
            }
            byte len;
            if (BytesToRead > 0)
            {
                //等待起始符
                while (!(base.ReadByte() == 0x63))
                {
                    if (BytesToRead == 0) return 0;
                }

                //获取命令长度，长度=地址域+命令域+数据域
                len = Convert.ToByte(base.ReadByte());
                rbuffer[0] = 0x63;
                rbuffer[1] = len;
                //从下标2开始接收剩余所有信息
                base.Read(rbuffer, 2, len + 3);

                //末位不是结束符错误
                if (rbuffer[len + 4] != 0x16) return 0xFF;
                //校验错误
                if (CS(rbuffer, len + 3) != rbuffer[len + 3]) return 0xFF;

                //信息接收正确，返回命令域
                return rbuffer[4];
            }
            return 0xFF; //未收到信息
        }

        public static byte Num2BCD(byte num)
        {
            return Convert.ToByte((num % 10) + ((num / 10) << 4));
        }
        public static byte BCD2Num(byte bcd)
        {
            return Convert.ToByte((bcd & 0x0F) + ((bcd >> 4) * 10));
        }
    }
}
