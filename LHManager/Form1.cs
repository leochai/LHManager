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

namespace LHManager
{
    public partial class Form1 : Form
    {
        public OleDbConnection _DBconn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0;Data Source=" + Application.StartupPath + "/db/老化台.accdb");
        LHUnit[] unit = new LHUnit[24];
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 24; i++)
            {
                unit[i] = new LHUnit();
            }
            DBManager.Initial(_DBconn, unit);
            label2.Text = unit[2].seatType.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _DBconn.Open();
        }
        private void Form1_FormClosed(object sender, EventArgs e)
        {
            _DBconn.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            DBManager.WriteResult(_DBconn, "试验编号111", 1, 23, System.DateTime.Now, 24.95, 75.01, 3.01);
            DBManager.UpdateStartTestingStatus(_DBconn, unit[2], 2);
        }
    }
}
