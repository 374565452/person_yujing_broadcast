using DTU.GateWay.Protocol;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimClient
{
    public partial class frmParamBase : Form
    {
        public frmParamBase()
        {
            InitializeComponent();
        }

        List<byte> list_IdenCollection;

        private void frmParamBase_Load(object sender, EventArgs e)
        {
            flowLayoutPanel1.MouseWheel += flowLayoutPanel1_MouseWheel;

            Type WorkType = typeof(RTUParam.WorkType);
            this.cboWorkType.Items.Clear();
            foreach (RTUParam.WorkType w in Enum.GetValues(WorkType))
            {
                this.cboWorkType.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(WorkType, w));
            }
            
            Type SimNoType = typeof(RTUParam.SimNoType);
            this.cboSimNoType.Items.Clear();
            foreach (RTUParam.SimNoType w in Enum.GetValues(SimNoType))
            {
                this.cboSimNoType.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(SimNoType, w));
            }

            Type ChannelType = typeof(RTUParam.ChannelType);
            this.cboChannelType1_M.Items.Clear();
            this.cboChannelType1_B.Items.Clear();
            this.cboChannelType2_M.Items.Clear();
            this.cboChannelType2_B.Items.Clear();
            this.cboChannelType3_M.Items.Clear();
            this.cboChannelType3_B.Items.Clear();
            this.cboChannelType4_M.Items.Clear();
            this.cboChannelType4_B.Items.Clear();
            foreach (RTUParam.ChannelType w in Enum.GetValues(ChannelType))
            {
                this.cboChannelType1_M.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(ChannelType, w));
                this.cboChannelType1_B.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(ChannelType, w));
                this.cboChannelType2_M.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(ChannelType, w));
                this.cboChannelType2_B.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(ChannelType, w));
                this.cboChannelType3_M.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(ChannelType, w));
                this.cboChannelType3_B.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(ChannelType, w));
                this.cboChannelType4_M.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(ChannelType, w));
                this.cboChannelType4_B.Items.Add(((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(ChannelType, w));
            }

            this.flowLayoutPanel1.Controls.Clear();
            Type IdenCollection = typeof(RTUParam.IdenCollection);
            foreach (RTUParam.IdenCollection w in Enum.GetValues(IdenCollection))
            {
                CheckBox cb = new CheckBox();
                cb.Text = ((int)w).ToString("X").PadLeft(2, '0') + " - " + EnumUtils.GetDescription(IdenCollection, w);
                cb.Click += cb_Click;
                flowLayoutPanel1.Controls.Add(cb);
            }
        }

        void cb_Click(object sender, EventArgs e)
        {
            if (list_IdenCollection == null)
            {
                list_IdenCollection = new List<byte>();
            }

            CheckBox cb = (CheckBox)sender;
            try
            {
                byte b = Convert.ToByte(cb.Text.Split('-')[0].Trim(), 16);
                if (cb.Checked && !list_IdenCollection.Contains(b))
                    list_IdenCollection.Add(b);

                if (!cb.Checked && list_IdenCollection.Contains(b))
                    list_IdenCollection.Remove(b);
            }
            catch
            { 
            }

            this.lblCount.Text = list_IdenCollection.Count.ToString();
            this.lblCount2.Text = list_IdenCollection.Count.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelIdenCollection_Click(object sender, EventArgs e)
        {
            this.groupBox5.Location = new Point(12, 12);
            this.groupBox5.Visible = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.groupBox5.Visible = false;
        }

        private void flowLayoutPanel1_MouseEnter(object sender, EventArgs e)
        {
            flowLayoutPanel1.Focus();
        }

        void flowLayoutPanel1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                if (flowLayoutPanel1.VerticalScroll.Maximum > flowLayoutPanel1.VerticalScroll.Value + 1)
                    flowLayoutPanel1.VerticalScroll.Value += 1;
                else
                    flowLayoutPanel1.VerticalScroll.Value = flowLayoutPanel1.VerticalScroll.Maximum;
            }
            else
            {
                if (flowLayoutPanel1.VerticalScroll.Value > 1)
                    flowLayoutPanel1.VerticalScroll.Value -= 1;
                else
                {
                    flowLayoutPanel1.VerticalScroll.Value = 0;
                }
            }
        }
    }
}
