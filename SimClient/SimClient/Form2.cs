using DTU.GateWay.Protocol;
using DTU.GateWay.Protocol.WaterMessageClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimClient
{
    public partial class Form2 : Form
    {
        Form1 f1;
        public Form2(Form1 f1)
        {
            InitializeComponent();
            this.f1 = f1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //初始化一个OpenFileDialog类
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "(*.jpg)|*.jpg";

            //判断用户是否正确的选择了文件
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //获取用户选择文件的后缀名
                string extension = Path.GetExtension(fileDialog.FileName);
                //声明允许的后缀名
                string[] str = new string[] { ".jpg" };
                if (!((IList)str).Contains(extension))
                {
                    MessageBox.Show("仅能上传gif,jpge,jpg格式的图片！");
                }
                else
                {
                    //获取用户选择的文件，并判断文件大小不能超过20K，fileInfo.Length是以字节为单位的
                    FileInfo fileInfo = new FileInfo(fileDialog.FileName);
                    if (fileInfo.Length > 20480)
                    {
                        MessageBox.Show("上传的图片不能大于20K");
                    }
                    else
                    {
                        //在这里就可以写获取到正确文件后的代码了
                        this.textBox1.Text = fileDialog.FileName;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] bs = null;
            FileInfo fileInfo = new FileInfo(this.textBox1.Text);
            FileStream fs = null;
            if (fileInfo.Exists)
            {
                try
                {
                    long len = fileInfo.Length;
                    fs = fileInfo.OpenRead();
                    bs = new byte[len];
                    fs.Read(bs, 0, (int)len);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }

            if (bs == null)
            {
                MessageBox.Show("图片选择错误！");
            }

            WaterCmd_36_2 cmd = new WaterCmd_36_2();
            cmd.CenterStation = f1.GetCenterStation();
            cmd.RemoteStation = f1.GetRemoteStation();
            cmd.PW = f1.GetPW();
            cmd.SerialNumber = f1.GetCount();
            cmd.SendTime = DateTime.Now;

            Identifier_F1 iden_F1 = new Identifier_F1();
            iden_F1.RemoteStation = cmd.RemoteStation;
            iden_F1.StationType = (byte)WaterBaseProtocol.StationType.River;
            Identifier_F0 iden_F0 = new Identifier_F0();
            iden_F0.ObsTime = cmd.SendTime;
            Identifier_F3 iden_F3 = new Identifier_F3();
            iden_F3.ImgContent = bs;

            cmd.List_Identifier = new List<Identifier>();
            cmd.List_Identifier.Add(iden_F1);
            cmd.List_Identifier.Add(iden_F0);
            cmd.List_Identifier.Add(iden_F3);

            string msg = cmd.WriteMsg();
            if (msg == "")
            {
                MessageBox.Show("命令数量：" + cmd.MsgList.Length);
                this.textBox2.Text = "";
                wbms = cmd.MsgList;
                Thread t = new Thread(showMsg);
                t.Start();
            }
            else
            {
                MessageBox.Show(msg);
            }
        }

        WaterBaseMessage[] wbms;
        private void showMsg()
        {
            if (wbms != null)
            {
                for (int i = 0; i < wbms.Length; i++)
                {
                    AddText("【" + (i + 1).ToString().PadLeft(wbms.Length.ToString().Length, '0') + "】" + Environment.NewLine);
                    AddText(wbms[i].RawDataStr + Environment.NewLine + Environment.NewLine);
                }
            }
        }

        public delegate void AddTextHandler(string result);
        private void AddText(string result)
        {
            if (textBox2.InvokeRequired == true)
            {
                AddTextHandler set = new AddTextHandler(AddText);//委托的方法参数应和SetCalResult一致  
                textBox2.Invoke(set, new object[] { result }); //此方法第二参数用于传入方法,代替形参result  
            }
            else
            {
                this.textBox2.Text += result;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WaterCmd_36_2 cmd = new WaterCmd_36_2();
            cmd.MsgList = wbms;
            string msg = cmd.ReadMsg();
            if (msg == "")
            {
                if (cmd.iden_F3 != null)
                {
                    byte[] bs = cmd.iden_F3.ImgContent;
                    Image img = byteArrayToImage(bs);
                    img.Save("temp.jpg");
                    MessageBox.Show("图片保存完成！");
                }
                else
                {
                    MessageBox.Show("未获取图片信息！");
                }
            }
            else
            {
                MessageBox.Show("分析失败！" + msg);
            }
        }

        private Image byteArrayToImage(byte[] Bytes)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            return Bitmap.FromStream(ms, true);
        }
    }
}
