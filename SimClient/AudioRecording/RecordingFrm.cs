using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioRecording
{
    public partial class RecordingFrm : Form
    {
        private DateTime startTime;

        private WaveIn waveInSource = null;

        private WaveFileWriter waveFileWriter = null;

        private string filePath = null;

        public RecordFlagModel RecordFlagModel
        {
            get;
            set;
        }

        public RecordingFrm(string sampleRate,string sampleBits)
        {
            InitializeComponent();
            cleanUp();
            RecordFlagModel = new AudioRecording.RecordFlagModel();
            waveInSource = new WaveIn();
            int rate = 8000;
            int bits = 8;
            try
            {
                rate = int.Parse(sampleRate);
                bits = int.Parse(sampleBits);
            }
            catch (Exception)
            {
                rate = 8000;
                bits = 8;
            }
            waveInSource.WaveFormat = new WaveFormat(rate, bits,1);
            //waveInSource.WaveFormat = new WaveFormat(8000, 1);
            waveInSource.BufferMilliseconds = 1000;

            waveInSource.DataAvailable += waveInSource_DataAvailable;
            waveInSource.RecordingStopped += waveInSource_RecordingStopped;
            string path = Application.StartupPath + "\\record";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //string fileName = Utils.random_str(20)+"-"+DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".wav";
            string fileName =  DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".wav";
            path += "\\" + fileName;
            RecordFlagModel.Flag = false;
            RecordFlagModel.RecprdFileName = fileName;
            RecordFlagModel.RecordFileFullPath = path;
            waveFileWriter = new WaveFileWriter(path, waveInSource.WaveFormat);
            try
            {
                waveInSource.StartRecording();
            }
            catch (Exception)
            {
                waveFileWriter.Dispose();
                waveFileWriter = null;
                waveInSource.Dispose();
                waveInSource = null;
                MessageBox.Show("录音出错，请检查麦克风或话筒等输入设备！");
                this.Dispose();
                this.Close();
            }
           
            startTime = DateTime.Now;
            recordingTimer.Enabled = true;
            recordingTimer.Start();

        }
        void waveInSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            //throw new NotImplementedException();
            //waveInSource.StopRecording();
            //MessageBox.Show("aaaaaaaaaaaaaaaaaaaaaaa");
            if (waveFileWriter != null)
            {
                waveFileWriter.Flush();
                waveFileWriter.Dispose();
                waveFileWriter = null;
                RecordFlagModel.Flag = true;
                //在这里交窗口进行关闭掉
                this.Dispose();
                this.Close();

            }
        }

        void waveInSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            //throw new NotImplementedException();
            if (waveFileWriter != null)
            {
                waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
                waveFileWriter.Flush();
            }
        }
        public void cleanUp()
        {
            if (waveInSource != null)
            {
                waveInSource.Dispose();
                waveInSource = null;
            }
            finalizeWaveFile();
        }
        private void finalizeWaveFile()
        {
            if (waveFileWriter != null)
            {
                waveFileWriter.Dispose();
                waveFileWriter = null;
            }
        }
        private void recordingTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - startTime;
            string str =null;
           
             //ts.Milliseconds
            str = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
            recordingTimeLabel.Text = str;
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            if (waveInSource != null)
            {
                waveInSource.StopRecording();
            }
            recordingTimer.Enabled = false;
            recordingTimer.Stop();
           
        }
    }
}
