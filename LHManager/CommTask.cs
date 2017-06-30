using System;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

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

        private void IntegralTask()
        {
            _lastRecallTime = DateTime.Now;
            for (byte k = 0; k < 23; k++)
            {
                DateTime lastday, thisday;
                DateTime now = DateTime.Now;
                byte[] lastRecord;
                byte[] thisRecord;

                for (int i = 0; i < 3; i++)
                {
                    _unit[k].GetRecord(RS485);
                    Thread.Sleep(300 + 260);
                    byte cmdBack = RS485.Readup(_readBuffer);
                    if (cmdBack == LHSerialPort.cmdGetRecord)
                    {
                        if (_readBuffer[5] == 0 && _readBuffer[6] == 0)
                        {
                            lastday = new DateTime(2000, 1, 1);
                        }
                        else
                        {
                            lastday = new DateTime(2000 + _readBuffer[7], _readBuffer[6], _readBuffer[5]);
                        }
                        if (_readBuffer[8] == 0 && _readBuffer[9] == 0)
                        {
                            thisday = new DateTime(2000, 1, 1);
                        }
                        else
                        {
                            thisday = new DateTime(2000 + _readBuffer[10], _readBuffer[9], _readBuffer[8]);
                        }
                        lastRecord = new byte[] { _readBuffer[11], _readBuffer[12], _readBuffer[13] };
                        thisRecord = new byte[] { _readBuffer[14], _readBuffer[15], _readBuffer[16] };

                        ArrayList dateArrayList = new ArrayList();
                        for (int hourPart = 0; hourPart < 3; hourPart++)
                        {
                            for (int hourBit = 0; hourBit < 8; hourBit++)
                            {
                                int hour = hourPart * 8 + hourBit;
                                if ((DateTime.Compare(lastday.AddHours(hour), _unit[k].lastHour) > 0) && Convert.ToBoolean(lastRecord[hourPart] >> hourBit & 0x01))
                                {
                                    dateArrayList.Add(lastday.AddHours(hour));
                                }
                                if ((DateTime.Compare(thisday.AddHours(hour), _unit[k].lastHour) > 0) && Convert.ToBoolean(thisRecord[hourPart] >> hourBit & 0x01))
                                {
                                    dateArrayList.Add(thisday.AddHours(hour));
                                }
                            }
                        }

                        dateArrayList.Sort();
                        foreach (DateTime timeToRecall in dateArrayList)
                        {
                            _unit[k].lastHour = timeToRecall;
                            IntegralPerHour(k);
                            DBUpdateLastHour(_DBconn, k, _unit[k].lastHour);
                        }
                        break;
                    }
                }
            }
        }
        private void IntegralPerHour(byte unitNo)
        {
            for (byte hourPart = 0; hourPart < 4; hourPart++)
            {
                for (int i = 0; i < 3; i++)
                {
                    _unit[unitNo].Integral(RS485, hourPart);
                    Thread.Sleep(300 + 230);
                    byte cmdBack = RS485.Readup(_readBuffer);
                    if (cmdBack == LHSerialPort.cmdIntegral)
                    {
                        ReplyIntegral();
                        break;
                    }
                }
            }
        }
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
                for (int j = 0; j < 24; j++)
                {
                    if (Convert.ToBoolean(_unit[i].seatChart[j + dataPart * 24]))
                    {
                        double voltage, power, current;
                        voltage = (data[j + 1] * 925 + 291200) / 25600.0;
                        power = 100 * voltage / 16;
                        current = (data[j + 2] * 125 + 48000) / 25600.0 * 1000 / 100;
                        DBWriteResult(_DBconn, _unit[i].testingNum, _unit[i].seatChart[j + dataPart * 24],
                                        Convert.ToByte(1), time, voltage, power, current);
                    }
                }
                return;
            }

            if (_unit[i].productType == "GHB302")     //28V一位
            {
                for (int j = 0; j < 24; j++)
                {
                    if (Convert.ToBoolean(_unit[i].seatChart[j + dataPart * 24]))
                    {
                        double voltage, power, current;
                        voltage = (data[j + 1] * 1525 + 521600) / 25600.0;
                        power = 75 * voltage / 28;
                        current = (data[j + 2] * 125 + 48000) / 25600.0 * 1000 / 62.5;
                        DBWriteResult(_DBconn, _unit[i].testingNum, _unit[i].seatChart[j + dataPart * 24],
                                        Convert.ToByte(1), time, voltage, power, current);
                    }
                }
                return;
            }

            if (_unit[i].productType == "GH137")     //5.5V一位
            {
                for (int j = 0; j < 24; j++)
                {
                    if (Convert.ToBoolean(_unit[i].seatChart[j + dataPart * 24]))
                    {
                        double voltage, power, current;
                        voltage = (data[j + 1] * 275 + 105600) / 25600.0;
                        power = 75 * voltage / 5.5;
                        current = (data[j + 2] * 125 + 48000) / 25600.0 * 1000 / 250 * 3;
                        DBWriteResult(_DBconn, _unit[i].testingNum, _unit[i].seatChart[j + dataPart * 24],
                                        Convert.ToByte(1), time, voltage, power, current);
                    }
                }
                return;
            }

            if (_unit[i].productBits == 0)  //一位通用
            {
                for (int j = 0; j < 24; j++)
                {
                    if (Convert.ToBoolean(_unit[i].seatChart[j + dataPart * 24]))
                    {
                        double voltage = 0.0;
                        double power = 0.0;
                        if (_unit[i].voltageLevel == 0)
                        {     //21V
                            voltage = (data[j + 1] * 1175 + 387200) / 25600.0;
                            power = 75 * voltage / 21;
                        }
                        if (_unit[i].voltageLevel == 1)
                        {     //25V
                            voltage = (data[j + 1] * 1375 + 464000) / 25600.0;
                            power = 75 * voltage / 25;
                        }
                        if (_unit[i].voltageLevel == 2) //28V
                        {
                            voltage = (data[j + 1] * 1525 + 521600) / 25600.0;
                            power = 75 * voltage / 28;
                        }
                        if (_unit[i].voltageLevel == 3) //16V
                        {
                            voltage = (data[j + 1] * 925 + 291200) / 25600.0;
                            power = 75 * voltage / 16;
                        }
                        if (_unit[i].voltageLevel == 4) //5.5V
                        {
                            voltage = (data[j + 1] * 275 + 105600) / 25600.0;
                            power = 75 * voltage / 5.5;
                        }
                        DBWriteResult(_DBconn, _unit[i].testingNum, _unit[i].seatChart[j + dataPart * 24],
                                        Convert.ToByte(1), time, voltage, power, 0);
                    }
                }
            }

            if (_unit[i].productBits == 2)  //四位通用
            {
                for (int j = 0; j < 24; j++)
                {
                    if (Convert.ToBoolean(_unit[i].seatChart[j + dataPart * 24]))
                    {
                        double voltage = 0.0;
                        double power = 0.0;
                        if (_unit[i].voltageLevel == 0)
                        {     //21V
                            voltage = (data[j + 1] * 1175 + 387200) / 25600.0;
                            power = 75 * voltage / 21;
                        }
                        if (_unit[i].voltageLevel == 1)
                        {     //25V
                            voltage = (data[j + 1] * 1375 + 464000) / 25600.0;
                            power = 75 * voltage / 25;
                        }
                        if (_unit[i].voltageLevel == 2) //28V
                        {
                            voltage = (data[j + 1] * 1525 + 521600) / 25600.0;
                            power = 75 * voltage / 28;
                        }
                        if (_unit[i].voltageLevel == 3) //16V
                        {
                            voltage = (data[j + 1] * 925 + 291200) / 25600.0;
                            power = 75 * voltage / 16;
                        }
                        if (_unit[i].voltageLevel == 4) //5.5V
                        {
                            voltage = (data[j + 1] * 1525 + 521600) / 25600.0;
                            power = 75 * voltage / 5.5;
                        }
                        DBWriteResult(_DBconn, _unit[i].testingNum, _unit[i].seatChart[j + dataPart * 24],
                                        Convert.ToByte((j + dataPart * 24) % 4 + 1), time, voltage, power, 0);
                    }
                }
            }
        }

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
                    break;
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

        private void StartupTask(byte unitNo)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!DistributeTask(unitNo))
                {
                    MessageBox.Show("参数下发有误，请检查！");
                    return;
                }
                _unit[unitNo].Startup(RS485);
                Thread.Sleep(300 + 120);
                if (RS485.Readup(_readBuffer) == LHSerialPort.cmdStartup)
                {
                    if (_unit[unitNo].testingStatus == 0x0C)  //340继续
                    {
                        _unit[unitNo].testingStatus = 0x00;
                        DBUpdateResume340Status(_DBconn, _unit[unitNo], unitNo);
                    }
                    else
                    {
                        _unit[unitNo].testingStatus = 0x00;
                        _unit[unitNo].lastHour = DateTime.Now.AddHours(-1);
                        DBUpdateStartTestingStatus(_DBconn, _unit[unitNo], unitNo);
                    }
                    break;
                }
            }
        }
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

        private void Reboot340Task(byte unitNo)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!DistributeTask(unitNo))
                {
                    MessageBox.Show("参数下发有误，请检查！");
                    return;
                }
                _unit[unitNo].Reboot340(RS485);
                Thread.Sleep(300 + 120);
                if (RS485.Readup(_readBuffer) == LHSerialPort.cmdReboot340)
                {
                    _unit[unitNo].testingStatus = 0x00;
                    _unit[unitNo].lastHour = DateTime.Now.AddHours(-1);
                    MessageBox.Show("放弃340小时后试验，并重启新试验，成功！");
                    DBUpdateStartTestingStatus(_DBconn, _unit[unitNo], unitNo);
                    break;
                }
            }
        }

        private void RebootHardTask(byte unitNo)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!DistributeTask(unitNo))
                {
                    MessageBox.Show("参数下发有误，请检查！");
                    return;
                }
                _unit[unitNo].RebootHard(RS485);
                Thread.Sleep(300 + 120);
                if (RS485.Readup(_readBuffer) == LHSerialPort.cmdRebootHard)
                {
                    _unit[unitNo].testingStatus = 0x00;
                    _unit[unitNo].lastHour = DateTime.Now.AddHours(-1);
                    MessageBox.Show("强制终止试验，并重启新试验，成功！");
                    DBUpdateStartTestingStatus(_DBconn, _unit[unitNo], unitNo);
                    break;
                }
            }
        }
    }
}
