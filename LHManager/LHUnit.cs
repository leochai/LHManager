using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LHManager
{
    public class LHUnit
    {
        public string productType;
        public string qualityLevel;
        public string standardNum;
        public string productNum;
        public string testingNum;
        public DateTime startDate;
        public string testingOperator;
        public byte[] seatChart = new byte[96];
        public bool seatType;   //0-单位；1-四位
        public byte maxVoltage;
        public byte minVoltage;
        public byte productBits; //0-单位；1-双位；2-四位
        public byte address;
        public byte testingStatus; //00-正常；03-请求参数；0C-340暂停；30-停止
        public DateTime lastHour;
        public byte voltageLevel; //0-21；1-25；2-28；3-16；4-5.5；


        public void Polling(LHSerialPort com)
        {
            com.WritePolling(address);
        }

        public void Integral(LHSerialPort com,byte part)
        {
            com.WriteIntegral(address, part, Convert.ToByte(this.lastHour.Hour));
        }

        public void Distribute(LHSerialPort com)
        {
            prmDistribute prm = new prmDistribute();
            prm.type = Convert.ToByte((productBits << 4) + voltageLevel);
            prm.max = maxVoltage;
            prm.min = minVoltage;
            prm.hour = LHSerialPort.Num2BCD(Convert.ToByte(System.DateTime.Now.Hour));
            prm.minute = LHSerialPort.Num2BCD(Convert.ToByte(System.DateTime.Now.Minute));
            prm.second = LHSerialPort.Num2BCD(Convert.ToByte(System.DateTime.Now.Second));
            prm.week = LHSerialPort.Num2BCD(Convert.ToByte(System.DateTime.Now.DayOfWeek));
            if (prm.week == 0) prm.week = 7;
            prm.day = LHSerialPort.Num2BCD(Convert.ToByte(System.DateTime.Now.Day));
            prm.month = LHSerialPort.Num2BCD(Convert.ToByte(System.DateTime.Now.Month));
            prm.year = LHSerialPort.Num2BCD(Convert.ToByte(System.DateTime.Now.Year - 2000));
            for(int i = 0; i < 12; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (seatChart[i * 8 + 12] != 0)
                    {
                        prm.pos[i] += Convert.ToByte(0x01 << j);
                    }
                }
            }

            com.WriteDistribute(address, prm);
        }

        public void Startup(LHSerialPort com)
        {
            com.WriteStartup(address);
        }

        public void Reboot340(LHSerialPort com)
        {
            com.WriteReboot340(address);
        }

        public void RebootHard(LHSerialPort com)
        {
            com.WriteRebootHard(address);
        }

        public static void TimeModify(LHSerialPort com)
        {
            com.WriteTimeModify(0x3F);
        }

        public void LHTimeModify(LHSerialPort com,byte second, byte minute, int hour)
        {
            byte hourh = Convert.ToByte(hour / 256);
            byte hourl = Convert.ToByte(hour % 256);
            byte secondBCD = LHSerialPort.Num2BCD(second);
            byte minuteBCD = LHSerialPort.Num2BCD(minute);
            com.WriteLHTimeModify(address, secondBCD, minuteBCD, hourl, hourh);
        }

        public void GetRecord(LHSerialPort com)
        {
            com.WriteGetRecord(address);
        }
    }
}
