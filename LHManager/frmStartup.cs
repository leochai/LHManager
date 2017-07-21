using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LHManager
{
    public partial class frmStartup : Form
    {
        public frmStartup()
        {
            InitializeComponent();
        }

        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void txtVoltBias_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void frmStartup_Load(object sender, EventArgs e)
        {
            lblUnitInfo.Text = "";

            label2.Location = new Point(label2.Location.X, label2.Location.Y + 12);
            txtOperator.Location = new Point(txtOperator.Location.X, txtOperator.Location.Y + 12);

            label3.Location = new Point(label3.Location.X, label3.Location.Y + 24);
            cmbType.Location = new Point(cmbType.Location.X, cmbType.Location.Y + 24);
            lblProductBits.Location = new Point(lblProductBits.Location.X, lblProductBits.Location.Y + 24);
            lblProductBits.Text = "";

            label4.Location = new Point(label4.Location.X, label4.Location.Y + 36);
            txtProductNum.Location = new Point(txtProductNum.Location.X, txtProductNum.Location.Y + 36);

            label5.Location = new Point(label5.Location.X, label5.Location.Y + 48);
            txtQualityLevel.Location = new Point(txtQualityLevel.Location.X, txtQualityLevel.Location.Y + 48);

            label6.Location = new Point(label6.Location.X, label6.Location.Y + 60);
            txtStandardNum.Location = new Point(txtStandardNum.Location.X, txtStandardNum.Location.Y + 60);

            label7.Location = new Point(label7.Location.X, label7.Location.Y + 72);
            txtTestingNum.Location = new Point(txtTestingNum.Location.X, txtTestingNum.Location.Y + 72);

            txtVoltBias.Text = "10";
        }

        private void cmbUnitNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUnitNo.SelectedIndex == -1)
            {
                lblUnitInfo.Text = "";
                return;
            }
            byte unitNo = Convert.ToByte(cmbUnitNo.SelectedIndex);
            lblUnitInfo.Text = "";
            if (Form1._unit[unitNo].seatType)
            {
                lblUnitInfo.Text += "一位插座  ";
            }
            else
            {
                lblUnitInfo.Text += "四位插座  ";
            }

            if (Form1._unit[unitNo].voltageLevel == 0)
            {
                lblUnitInfo.Text += "电压等级-21V";
            }
            else if (Form1._unit[unitNo].voltageLevel == 1)
            {
                lblUnitInfo.Text += "电压等级-25V";
            }
            else if (Form1._unit[unitNo].voltageLevel == 2)
            {
                lblUnitInfo.Text += "电压等级-28V";
            }
            else if (Form1._unit[unitNo].voltageLevel == 3)
            {
                lblUnitInfo.Text += "电压等级-16V";
            }
            else if (Form1._unit[unitNo].voltageLevel == 4)
            {
                lblUnitInfo.Text += "电压等级-5.5V";
            }

            if (Form1._unit[unitNo].testingStatus == 0x00 || Form1._unit[unitNo].testingStatus == 0x0C)
            {
                DialogResult dr = MessageBox.Show((unitNo + 1).ToString() + "号单元正在试验中，确定终断？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    cmbUnitNo.SelectedIndex = -1;
                }
            }

        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblProductBits.Text = "";
            if (cmbType.SelectedIndex <= 3)
            {
                lblProductBits.Text += "一位器件";
            }
            else
            {
                lblProductBits.Text += "四位器件";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbUnitNo.SelectedIndex == -1)
            {
                MessageBox.Show("请选择单元编号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtOperator.Text == "")
            {
                MessageBox.Show("请输入操作员姓名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (cmbType.SelectedIndex == -1)
            {
                MessageBox.Show("请选择待试验产品型号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtProductNum.Text == "")
            {
                MessageBox.Show("请输入生产批号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtQualityLevel.Text == "")
            {
                MessageBox.Show("请输入质量等级", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtStandardNum.Text == "")
            {
                MessageBox.Show("请输入标准号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtTestingNum.Text == "")
            {
                MessageBox.Show("请输入试验编号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtNumber.Text == "")
            {
                MessageBox.Show("请输入器件数量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtVoltBias.Text == "")
            {
                MessageBox.Show("请输入电压允许偏移范围", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtNumber.Text == "0")
            {
                MessageBox.Show("待试验器件数为0，不用试验", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if ((lblUnitInfo.Text[0] == '四' && Convert.ToByte(txtNumber.Text) > 24) ||
                (lblUnitInfo.Text[0] == '一' && Convert.ToByte(txtNumber.Text) > 48))
            {
                MessageBox.Show("待试验器件数太大，请检查", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lblUnitInfo.Text[0] != lblProductBits.Text[0])
            {
                MessageBox.Show("请确认插座位数和器件位数是否匹配", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte unitNo = Convert.ToByte(cmbUnitNo.SelectedIndex);
            if (Form1._unit[unitNo].testingStatus == 0x00 || Form1._unit[unitNo].testingStatus == 0x0C)
            {
                DialogResult dr = MessageBox.Show((unitNo + 1).ToString() + "号单元正在试验中，再次确定是否终断？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    cmbUnitNo.SelectedIndex = -1;
                    return;
                }
            }

            LHUnit tempUnit = Form1._unit[unitNo];
            tempUnit.testingOperator = txtOperator.Text;
            tempUnit.testingNum = txtTestingNum.Text;
            tempUnit.productType = cmbType.SelectedText;
            tempUnit.qualityLevel = txtQualityLevel.Text;
            tempUnit.productNum = txtProductNum.Text;
            tempUnit.startDate = DateTime.Now.Date;
            if (lblProductBits.Text[0] == '一')
            {
                tempUnit.productBits = 0;
            }
            else
            {
                tempUnit.productBits = 2;
            }

            //对位表怎么搞
            MessageBox.Show("begin");
        }

        


    }
}
