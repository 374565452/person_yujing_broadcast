using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioRecording
{
    public partial class Form1 : Form
    {
        private WaveIn waveInSource = null;

        public WaveFileWriter waveFileWriter = null;

        public Form1()
        {
            InitializeComponent();
           
        }

        public bool checkMicrophone()
        {
            MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
            List<MMDevice> devices=deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
            if (devices == null || devices.Count <= 0)
            {
                return false;
            }
            return true;
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            cleanUp();
            if (!checkMicrophone())
            {
                MessageBox.Show("未检测到录音设备，请插入麦克风或话筒等输入设备！");
                return;
            }
            waveInSource = new WaveIn();
            //waveInSource.WaveFormat = new WaveFormat(44100, 1);
            waveInSource.WaveFormat = new WaveFormat(8000, 1);
            waveInSource.DataAvailable += waveInSource_DataAvailable;
            waveInSource.RecordingStopped += waveInSource_RecordingStopped;

            waveFileWriter = new WaveFileWriter(@"D:\Test0001.wav", waveInSource.WaveFormat);
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
            }
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

        private void stopBtn_Click(object sender, EventArgs e)
        {
            if (waveInSource != null)
            {
                waveInSource.StopRecording();
            }
        }
    }
}
