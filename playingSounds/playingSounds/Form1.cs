using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace playingSounds {
    public partial class Form1 : Form {
        XAudio2 xaudio2 = new XAudio2();
        MasteringVoice masteringVoice;

        public Form1() {
            InitializeComponent();
            masteringVoice = new MasteringVoice(xaudio2);
        }

        private void Form1_Load(object sender,EventArgs e) { }

        private void bttn_load_Click(object sender,EventArgs e) {
            pnl_buttons.Controls.Clear();

            DirectoryInfo d = new DirectoryInfo(@"Sounds");
            FileInfo[] files = d.GetFiles("*.wav");

            int bttnY = 10;
            foreach(FileInfo file in files) {
                Button bttn = new Button();
                bttn.Location = new System.Drawing.Point(10,bttnY);
                bttn.Size = new System.Drawing.Size(200,30);
                bttnY += 40;
                bttn.Text = file.Name;
                bttn.Name = @"Sounds\"+file.Name;
                bttn.Click += Bttn_Click;
                pnl_buttons.Controls.Add(bttn);
            }

        }

        private void Bttn_Click(object sender,EventArgs e) {
            string sound = ((Button)sender).Name;
            PLaySoundFile(xaudio2,sound);
        }

        /// <summary>
        /// Play a sound file. Supported format are Wav(pcm+adpcm) and XWMA
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="fileName">Name of the file.</param>
        void PLaySoundFile(XAudio2 device,string fileName) {
            Thread thread = new Thread(Play);
            thread.Start();

            void Play() {
                SoundStream stream = new SoundStream(File.OpenRead(fileName));
                WaveFormat waveFormat = stream.Format;
                AudioBuffer buffer = new AudioBuffer { Stream = stream.ToDataStream(), AudioBytes = (int)stream.Length, Flags = BufferFlags.EndOfStream };
                stream.Close();

                SourceVoice sourceVoice = new SourceVoice(device, waveFormat, true);
                //Adds a sample callback to check that they are working on source voices
                sourceVoice.SubmitSourceBuffer(buffer,stream.DecodedPacketsInfo);
                sourceVoice.Start();

                while(sourceVoice.State.BuffersQueued > 0) {
                    Thread.Sleep(0);
                }

                sourceVoice.DestroyVoice();
                sourceVoice.Dispose();
                buffer.Stream.Dispose();
            }

        }
    }
}