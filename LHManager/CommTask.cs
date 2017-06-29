using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LHManager
{
    public class CommFlag
    {
        public bool polling;
        public bool startup;
        public bool integral;
        public bool timeModify;
        public bool reboot340;
        public bool rebootHard;
        public byte unitNoOfPolling;
        public byte unitNoOfStartup;

        public CommFlag()
        {
            polling = false;
            startup = false;
            integral = false;
            timeModify = false;
            reboot340 = false;
            rebootHard = false;
            unitNoOfPolling = 0;
            unitNoOfStartup = 0;
        }
    }


    public partial class Form1
    {
        CommFlag _commFlag = new CommFlag();

        private void CommTask()
        {
            while (true)
            {
                if (_commFlag.integral)
                {
                    IntegralTask();
                    _commFlag.integral = false;
                }
                if (_commFlag.startup)
                {
                    StartupTask(_commFlag.unitNoOfStartup);
                    _commFlag.startup = false;
                }
                if (_commFlag.polling)
                {
                    PollingTask(_commFlag.unitNoOfPolling);
                    _commFlag.polling = false;
                }
                if (_commFlag.timeModify)
                {
                    TimeModifyTask();
                    _commFlag.timeModify = false;
                }
                if (_commFlag.reboot340)
                {
                    Reboot340Task(_commFlag.unitNoOfStartup);
                    _commFlag.reboot340 = false;
                }
                if (_commFlag.rebootHard)
                {
                    RebootHardTask(_commFlag.unitNoOfStartup);
                    _commFlag.rebootHard = false;
                }
            }
        }

        private void IntegralTask() { }
        private void PollingTask(byte unitNo)
        {
            for (int i = 0; i < 3; i++)
            {
                _unit[unitNo].Polling(RS485);
                Thread.Sleep(300 + 210);
                byte cmdBack = RS485.Readup(_readBuffer);
                if (cmdBack == LHSerialPort.cmdPolling)
                {
                    ReplyPolling();
                    return;
                }
            }
        }
        private void ReplyPolling()
        {
            byte dataLength = _readBuffer[1];
            byte address = _readBuffer[3];
            byte[] data = new byte[dataLength - 2];
            for (int i = 0; i < dataLength - 2; i++)
            {
                data[i] = _readBuffer[i + 5];
            }

            byte status = data[0];
            //正常状态
            if (status == 0x00) { return; }
            //请求参数
            if (status == 0x03)
            {
                for (byte i = 0; i < 24; i++)
                {
                    if (_unit[i].address == address)
                    {
                        DistributeTask(i);
                        break;
                    }
                }
            }
            //340暂停
            if (status == 0x0C)
            {
                for (byte i = 0; i < 24; i++)
                {
                    if (_unit[i].address == address)
                    {
                        _unit[i].testingStatus = 0x0C;
                        DBUpdateStatus(_DBconn, i, _unit[i].testingStatus);
                        break;
                    }
                }
            }
            //1000停止
            if (status == 0x30)
            {
                for (byte i = 0; i < 24; i++)
                {
                    if (_unit[i].address == address)
                    {
                        _unit[i].testingStatus = 0x30;
                        DBUpdateStatus(_DBconn, i, _unit[i].testingStatus);
                        break;
                    }
                }
            }
        }
        private void StartupTask(byte unitNo) { }
        private bool DistributeTask(byte unitNo)
        {
            for (int i = 0; i < 3; i++)
            {
                _unit[unitNo].Distribute(RS485);
                Thread.Sleep(300 + 260);
                byte cmdBack = RS485.Readup(_readBuffer);
                if (cmdBack == LHSerialPort.cmdDistribute)
                {
                    return true;
                }
            }
            return false;
        }
        private void TimeModifyTask()
        {
            LHUnit.TimeModify(RS485);
        }
        private void Reboot340Task(byte unitNo) { }
        private void RebootHardTask(byte unitNo) { }

        private void ReplyIntegral()
        {
            byte dataLength = _readBuffer[1];
            byte address = _readBuffer[3];
            byte[] data = new byte[dataLength - 2];
            byte i;
            for (i = 0; i < dataLength - 2; i++)
            {
                data[i] = _readBuffer[i + 5];
            }

            for (i = 0; i < 24; i++)
            {
                if (_unit[i].address == address) break;
            }

            byte dataPart = Convert.ToByte(data[0] >> 6);
            byte dataHour = Convert.ToByte(data[0] & 0x1F);
            DateTime time = _unit[i].lastHour;

            if (_unit[i].productType == "GO11")     //16V一位
            {
                for (int j=0; j<24; j += 2)
                {
                    if (Convert.ToBoolean(_unit[i].seatChart[j + dataPart * 24]))
                    {
                        double voltage, power, current;
                        voltage = (data[j + 1] * 925 + 291200) / 25600.0;
                        power = 100 * voltage / 16;
                        current = (data[j + 2] * 125 + 48000) / 25600.0 * 1000 / 100;
                        DBWriteResult(_DBconn, _unit[i].testingNum, _unit[1].seatChart[j + dataPart * 24],
                                        Convert.ToByte(1), time, voltage, power, current);
                    }
                }
                return;
            }

            if (_unit[i].productType == "GHB302")     //28V一位
            {
                for (int j = 0; j < 24; j += 2)
                {
                    if (Convert.ToBoolean(_unit[i].seatChart[j + dataPart * 24]))
                    {
                        double voltage, power, current;
                        voltage = (data[j + 1] * 1525 + 521600) / 25600.0;
                        power = 75 * voltage / 28;
                        current = (data[j + 2] * 125 + 48000) / 25600.0 * 1000 / 62.5;
                        DBWriteResult(_DBconn, _unit[i].testingNum, _unit[1].seatChart[j + dataPart * 24],
                                        Convert.ToByte(1), time, voltage, power, current);
                    }
                }
                return;
            }

            if (_unit[i].productType == "GH137")     //5.5V一位
            {
                for (int j = 0; j < 24; j += 2)
                {
                    if (Convert.ToBoolean(_unit[i].seatChart[j + dataPart * 24]))
                    {
                        double voltage, power, current;
                        voltage = (data[j + 1] * 275 + 105600) / 25600.0;
                        power = 75 * voltage / 5.5;
                        current = (data[j + 2] * 125 + 48000) / 25600.0 * 1000 / 250 * 3;
                        DBWriteResult(_DBconn, _unit[i].testingNum, _unit[1].seatChart[j + dataPart * 24],
                                        Convert.ToByte(1), time, voltage, power, current);
                    }
                }
                return;
            }

            if (_unit[i].productBits == 0)  //一位通用
            {

            }
        }
    }
}
