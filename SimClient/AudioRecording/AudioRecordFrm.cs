using NAudio.CoreAudioApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioRecording
{
    public partial class AudioRecordFrm : Form
    {
        private Dictionary<String, String> pList = new Dictionary<String, String>();
        private string path = Application.StartupPath + "\\record";

        private BackgroundWorker m_backgroudWorker;// 申明后台对象

        private UploadWaitFrm waitFrm;

        private string configFilePath = Application.StartupPath + "\\config\\setup.xml";

        private string uploadServerIp;
        private string uploadServerPort;

        private string sampleRate;
        private string sampleBits;

        private XmlHelper xmlHelper;

        public AudioRecordFrm()
        {
            CheckForIllegalCrossThreadCalls = true;
            InitializeComponent();

            m_backgroudWorker = new BackgroundWorker();
            m_backgroudWorker.WorkerSupportsCancellation = true; // 设置可以取消
            m_backgroudWorker.WorkerReportsProgress = true;
            m_backgroudWorker.DoWork +=m_backgroudWorker_DoWork;
           // m_BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateProgress);
            m_backgroudWorker.RunWorkerCompleted += m_backgroudWorker_RunWorkerCompleted;

            loadRecorderFile();

            xmlHelper = new XmlHelper(configFilePath);
            loadConfiguration();
        }

        private void loadConfiguration()
        {
            xmlHelper.loadXml();
            uploadServerIp = xmlHelper.getValue("uploadServerIp");
            uploadServerPort = xmlHelper.getValue("uploadServerPort");
            sampleRate = xmlHelper.getValue("sampleRate");
            sampleBits = xmlHelper.getValue("sampleBits");
        }

        void m_backgroudWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Cancelled)
           //{
              //  MessageBox.Show("Canceled");
               // Debug.Write("canceled......");
           // }//throw new NotImplementedException();
            if (waitFrm != null)
            {
                waitFrm.Dispose();
                waitFrm.Close();
                waitFrm = null;
            }
            

        }
        //上传文件时，选中的文件名称
        private string fileName;
        private void m_backgroudWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //throw new NotImplementedException();

            /*if (waitFrm != null)
            {
                waitFrm.Dispose();
                waitFrm = null;
            }
            waitFrm = new UploadWaitFrm();
            waitFrm.ShowDialog();*/
            //http://localhost:39991/UploadRecordFile.ashx
            string url = string.Format("http://{0}:{1}/UploadRecordFile.ashx", uploadServerIp, uploadServerPort);
            Debug.Print(url);
            //string url = "http://localhost:58622/UploadFile.ashx";

            //string path = "D:\\vs_git\\123.mp3";
            //string fileName = recordListBox.SelectedItem.ToString();
            string path = pList[fileName];
            try
            {
                //开始执行后台任务
                //m_backgroudWorker.RunWorkerAsync();
                string msg = Utils.uploadFile(fileName, path, url);
                UploadFileResult result = JavaScriptConvert.DeserializeObject<UploadFileResult>(msg);
                m_backgroudWorker.CancelAsync();
                if (result.Result == true)
                {
                    //m_backgroudWorker.CancelAsync();
                    MessageBox.Show("上传成功！！！");

                }
                else
                {
                    MessageBox.Show("上传失败！错误信息为：" + result.Msg);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("上传失败，发生错误：" + ex.Message);
                //m_backgroudWorker.CancelAsync();
            }
        }

        public void loadRecorderFile()
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            updateListBox(path);  
        }
        public bool checkMicrophone()
        {
            MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
            List<MMDevice> devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
            if (devices == null || devices.Count <= 0)
            {
                return false;
            }
            return true;
        }
        private void startRecordBtn_Click(object sender, EventArgs e)
        {
            if (!checkMicrophone())
            {
                MessageBox.Show("未检测到录音设备，请插入麦克风或话筒等输入设备！");
                return;
            }
            RecordingFrm recordingFrm = new RecordingFrm(sampleRate,sampleBits);
            recordingFrm.FormClosing += recordingFrm_FormClosing;
            recordingFrm.ShowDialog();
        }

        void recordingFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
            RecordFlagModel model = (sender as RecordingFrm).RecordFlagModel;
            if (model.Flag == true)
            {
                pList.Add(model.RecprdFileName, model.RecordFileFullPath);
                this.recordListBox.Items.Add(model.RecprdFileName);
            }
        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            if (recordListBox.SelectedIndex != -1)
            {
                string keyName = recordListBox.SelectedItem.ToString();
                string fullPath = pList[keyName];
                if (fullPath != null)
                {
                    File.Delete(fullPath);
                }
                //MessageBox.Show(listBox1.SelectedItem.ToString());
                updateListBox(path);
            }
            //string name=this.listBox1.SelectedItem;
           //MessageBox.Show(listBox1.SelectedIndex+"");
            //Form1 form = new Form1();
            //form.ShowDialog();
        }
        
        private void uploadBtn_Click(object sender, EventArgs e)
        {
            if (recordListBox.SelectedIndex == -1)
            {
                MessageBox.Show("请选中需要上传的录音文件！");
                return;
            }
            m_backgroudWorker.RunWorkerAsync();
            fileName = recordListBox.SelectedItem.ToString();
            waitFrm = new UploadWaitFrm();
            waitFrm.ShowDialog();
            /*string url = "http://localhost:58622/uploadFile.ashx";

            //string path = "D:\\vs_git\\123.mp3";
            string fileName = recordListBox.SelectedItem.ToString();
            string path = pList[fileName];
            try
            {
                //开始执行后台任务
                m_backgroudWorker.RunWorkerAsync();
                string msg = Utils.uploadFile(fileName,path, url);
                UploadFileResult result=JavaScriptConvert.DeserializeObject<UploadFileResult>(msg);
                m_backgroudWorker.CancelAsync();
                if (result.Result == true)
                {
                     //m_backgroudWorker.CancelAsync();
                    MessageBox.Show("上传成功！！！");
                   
                }
                else
                {
                    MessageBox.Show("上传失败！错误信息为："+result.Msg);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("上传失败，发生错误："+ex.Message);
                m_backgroudWorker.CancelAsync();
            }
            */
        }

        

        public void updateListBox(string path)
        {
            this.recordListBox.Items.Clear();
            pList.Clear();
            DirectoryInfo recordDir = new DirectoryInfo(path);
            foreach (FileInfo infos in recordDir.GetFiles())
            {
                //Debug.Print(infos.FullName);
                pList.Add(infos.Name, infos.FullName);
                this.recordListBox.Items.Add(infos.Name);
                
            }
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            if (recordListBox.SelectedIndex == -1)
            {
                MessageBox.Show("请选中需要上传的录音文件！");
                return;
            }
            //m_backgroudWorker.RunWorkerAsync();
            string playFilePath = pList[recordListBox.SelectedItem.ToString()];
            RecordPlayFrm ppf = new RecordPlayFrm(playFilePath);
            ppf.ShowDialog();
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigFrm cf = new ConfigFrm(configFilePath, uploadServerIp, uploadServerPort,sampleRate,sampleBits);
            if (cf.ShowDialog() == DialogResult.OK)
            {
               // MessageBox.Show("aaaaaaaaaaaaaaaaaa");
                loadConfiguration();
            }
        }
    }
}
