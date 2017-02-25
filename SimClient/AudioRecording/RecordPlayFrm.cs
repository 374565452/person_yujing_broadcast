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
    public partial class RecordPlayFrm : Form
    {
        private IWavePlayer wavePlayer;
        private WaveStream reader;

        private string filePath;

        public RecordPlayFrm(string m_filePath)
        {
            InitializeComponent();
            this.filePath = m_filePath;
            timer1.Enabled = true;
            timer1.Start();
            try
            {
                loadPlayerFile();

                playFile();
            }
            catch (Exception e)
            {
                cleanUp();
                //this.Dispose();
                //this.Close();
                
            }
        }

        private void playFile()
        {
            if (wavePlayer == null)
            {
                wavePlayer = new WaveOut();

                wavePlayer.PlaybackStopped += wavePlayer_PlaybackStopped;
                wavePlayer.Init(reader);
            }
            wavePlayer.Play();
        }
        private void UpdatePosition()
        {
            labelPosition.Text = string.Format("{0}/{1}", reader.CurrentTime, reader.TotalTime);
            trackBar1.Value = Math.Min((int)((trackBar1.Maximum * reader.Position) / reader.Length), trackBar1.Maximum);
        }
        void wavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            //throw new NotImplementedException();
            reader.Position = 0;
            timer1.Enabled = false;
            timer1.Stop();
            UpdatePosition();
            this.Dispose();
            this.Close();
        }

        private void loadPlayerFile()
        {
            cleanUp();
            reader = new MediaFoundationReader(filePath, 
                new MediaFoundationReader.MediaFoundationReaderSettings() { SingleReaderObject = true });
        }

        private void cleanUp()
        {
            if (wavePlayer != null)
            {
                wavePlayer.Dispose();
                wavePlayer = null;
            }
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (reader != null)
            {
                UpdatePosition();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (reader != null)
            {
                reader.Position = (trackBar1.Value * reader.Length) / trackBar1.Maximum;
            }
        }

        private void RecordPlayFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cleanUp();
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
            }
            cleanUp();
            this.Dispose();
            this.Close();
        }
    }
}
