using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO.Ports;
using System.Threading;

namespace LHManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RS485.WriteBufferSize = 2048;
            RS485.ReadTimeout = 200;
            RS485.ReceivedBytesThreshold = 3;
            RS485.RtsEnable = true;
            try
            {
                RS485.Open();
            }
            catch 
            {
                MessageBox.Show("请检查串口连接后再尝试打开程序！");
                this.Close();
            }


            _DBconn.Open();
            for(int i = 0; i < 24; i++)
            {
                _unit[i] = new LHUnit();
            }
            DBInitial(_DBconn, _unit);
        }
        private void Form1_FormClosed(object sender, EventArgs e)
        {

        }

        private void ReplyPolling()
        {
            byte dataLength = _readBuffer[1];
            byte address = _readBuffer[3];
            byte[] data = new byte[dataLength - 2];
            for(int i=0; i<dataLength-2; i++)
            {
                data[i] = _readBuffer[i + 5];
            }

            byte status = data[0];
            if(status == 0x00)
            {

            }

            if(status == 0x03)
            {

            }

            if (status == 0x0C)
            {
                for(byte i = 0; i < 24; i++)
                {
                    if(address == _unit[i].address)
                    {
                        _unit[i].testingStatus = 0x0C;
                        DBUpdateStatus(_DBconn, i, _unit[i].testingStatus);
                        break;
                    }
                }
            }
        }
    }
}
