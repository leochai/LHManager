using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

namespace LHManager
{

    public partial class Form1
    {
        public ArrayList textBoxArrayList = new ArrayList();

        public void ViewInitiate()
        {
            for (int i = 0; i < 12; i++)
            {
                RichTextBox textBox = new RichTextBox();
                textBox.BorderStyle = BorderStyle.None;
                textBox.Multiline = false;
                textBox.BackColor = System.Drawing.Color.Gold;
                textBox.Width = 250;
                textBox.Height = 25;
                textBox.Left = 30;
                textBox.Top = 30 + i * 33;
                textBox.Parent = groupBox1;
                textBox.ReadOnly = true;
                textBox.SelectionFont = new System.Drawing.Font("yahei", 12);
                textBox.AppendText("  单元-" + (i + 1) + ": 正在试验中");
                textBox.Click += RichTextBox_Click;
                textBoxArrayList.Add(textBox);
            }

            for (int i = 12; i < 24; i++)
            {
                RichTextBox textBox = new RichTextBox();
                textBox.BorderStyle = BorderStyle.None;
                textBox.Multiline = false;
                textBox.BackColor = System.Drawing.Color.Gold;
                textBox.Width = 250;
                textBox.Height = 25;
                textBox.Left = 310;
                textBox.Top = 30 + (i - 12) * 33;
                textBox.Parent = groupBox1;
                textBox.ReadOnly = true;
                textBox.SelectionFont = new System.Drawing.Font("yahei", 12);
                textBox.AppendText("  单元-" + (i + 1) + ": 等待开始新试验");
                textBox.Click += RichTextBox_Click;
                textBoxArrayList.Add(textBox);
            }

        }

        private void RichTextBox_Click(object sender, EventArgs e)
        {
            string s = ((RichTextBox)sender).Text;
            string[] sArray = s.Split(new char[] { '-', ':' });
            int unitNo = Convert.ToByte(sArray[1]) - 1;
            string unitInfomation = "";
            unitInfomation += "单元编号:" + _unit[unitNo].address + Environment.NewLine;
            unitInfomation += "试验编号:" + _unit[unitNo].testingNum;
            //unitInfomation +=
            MessageBox.Show(unitInfomation,"老化单元详细信息");
        }
    }
}
