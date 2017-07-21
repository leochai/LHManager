using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LHManager
{

    public partial class Form1
    {
        public ArrayList textBoxArrayList = new ArrayList();

        public void UpdateUnitLabel()
        {
            foreach(RichTextBox textBox in textBoxArrayList)
            {
                string[] sArray = (textBox.Text).Split(new char[] { '-', ':' });
                byte index = Convert.ToByte(sArray[1]);
            }
        }

        public void ViewInitiate()
        {
            for (int i = 0; i < 24; i++)
            {
                RichTextBox textBox = new RichTextBox();
                textBox.BorderStyle = BorderStyle.None;
                textBox.Multiline = false;
                textBox.BackColor = System.Drawing.Color.Azure;
                textBox.Width = 250;
                textBox.Height = 25;
                textBox.Left = 30 + 280 * (i / 12);
                textBox.Top = 30 + (i % 12) * 33;
                textBox.Parent = groupBox1;
                textBox.ReadOnly = true;
                textBox.SelectionFont = new System.Drawing.Font("yahei", 12);
                textBox.AppendText("  单元-" + (i + 1) + ": 正在试验中");
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

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(groupBox1.BackColor);

            Rectangle Rtg_LT = new Rectangle();
            Rectangle Rtg_RT = new Rectangle();
            Rectangle Rtg_LB = new Rectangle();
            Rectangle Rtg_RB = new Rectangle();
            Rtg_LT.X = 0; Rtg_LT.Y = 7; Rtg_LT.Width = 10; Rtg_LT.Height = 10;
            Rtg_RT.X = e.ClipRectangle.Width - 11; Rtg_RT.Y = 7; Rtg_RT.Width = 10; Rtg_RT.Height = 10;
            Rtg_LB.X = 0; Rtg_LB.Y = e.ClipRectangle.Height - 11; Rtg_LB.Width = 10; Rtg_LB.Height = 10;
            Rtg_RB.X = e.ClipRectangle.Width - 11; Rtg_RB.Y = e.ClipRectangle.Height - 11; Rtg_RB.Width = 10; Rtg_RB.Height = 10;

            Color color = Color.FromArgb(51, 94, 168);
            Pen Pen_AL = new Pen(color, 0);
            Pen_AL.Color = color;
            Brush brush = new HatchBrush(HatchStyle.Divot, color);

            e.Graphics.DrawString(groupBox1.Text, groupBox1.Font, brush, 6, 0);
            e.Graphics.DrawArc(Pen_AL, Rtg_LT, 180, 90);
            e.Graphics.DrawArc(Pen_AL, Rtg_RT, 270, 90);
            e.Graphics.DrawArc(Pen_AL, Rtg_LB, 90, 90);
            e.Graphics.DrawArc(Pen_AL, Rtg_RB, 0, 90);
            e.Graphics.DrawLine(Pen_AL, 5, 7, 6, 7);
            e.Graphics.DrawLine(Pen_AL, e.Graphics.MeasureString(groupBox1.Text, groupBox1.Font).Width + 3, 7, e.ClipRectangle.Width - 7, 7);
            e.Graphics.DrawLine(Pen_AL, 0, 13, 0, e.ClipRectangle.Height - 7);
            e.Graphics.DrawLine(Pen_AL, 6, e.ClipRectangle.Height - 1, e.ClipRectangle.Width - 7, e.ClipRectangle.Height - 1);
            e.Graphics.DrawLine(Pen_AL, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 7, e.ClipRectangle.Width - 1, 13);
        }
    }
}
