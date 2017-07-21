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
using System.Collections;
using System.Diagnostics;

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
            //try
            //{
            //    RS485.Open();
            //}
            //catch 
            //{
            //    MessageBox.Show("请检查串口连接后再尝试打开程序！");
            //    this.Close();
            //}


            _DBconn.Open();
            for(int i = 0; i < 24; i++)
            {
                _unit[i] = new LHUnit();
            }
            DBInitial(_DBconn, _unit);

            ViewInitiate();
            
        }
        private void Form1_FormClosed(object sender, EventArgs e)
        {
            _DBconn.Close();
        }

        private void btnStartup_Click(object sender, EventArgs e)
        {
            frmStartup myfrmStartup = new frmStartup();
            myfrmStartup.Show();
        }
    }
}
