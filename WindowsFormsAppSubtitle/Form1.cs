using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace WindowsFormsAppSubtitle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            Control.CheckForIllegalCrossThreadCalls = false;
            
        }
        MyStopwatch stopWatch = new MyStopwatch(new TimeSpan());
        public List<StrModelTR> trsrtModel = new List<StrModelTR>();
        public List<StrModelUA> uasrtModel = new List<StrModelUA>();   
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Choose File";
            label2.Text = "Choose File";
            label3.Text = "00:00:00";
            this.Text = "V1.2";
            this.BackColor = Color.Gray;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool status = true;
            if (trsrtModel.Count != 0 && uasrtModel.Count != 0)
            {
                trackBar1.Maximum = (int)TimeSpan.FromMilliseconds(trsrtModel.Max(x => x.EndTime)).TotalSeconds;
            }
            else if (trsrtModel.Count != 0)
            {

                trackBar1.Maximum = (int)TimeSpan.FromMilliseconds(trsrtModel.Max(x => x.EndTime)).TotalSeconds;
                label2.Visible = false;
            }
            else if (uasrtModel.Count != 0)
            {
                trackBar1.Maximum = (int)TimeSpan.FromMilliseconds(uasrtModel.Max(x => x.EndTime)).TotalSeconds;
                label1.Visible = false;
            }
            else
            {
                MessageBox.Show("Choose File Before Starting!");
                status = false;
                this.Text = "V1.2 | Choose File Before Starting!";
            }
            if (status)
                if (backgroundWorker1.IsBusy != true)
                {
                    stopWatch.Start();
                    backgroundWorker1.RunWorkerAsync();
                }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                backgroundWorker1.CancelAsync();
                stopWatch.Stop();
            }
            
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    try
                    {
                        if (checkBox1.Checked)
                        {
                            label1.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(StrHelperTR.GetTitle(Math.Round(stopWatch.ElapsedSeconds, 7).ToString())));

                            label2.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(StrHelperUA.GetTitle(Math.Round(stopWatch.ElapsedSeconds, 7).ToString())));
                        }
                        else
                        {
                            label1.Text = StrHelperTR.GetTitle(Math.Round(stopWatch.ElapsedSeconds, 7).ToString());
                            label2.Text = StrHelperUA.GetTitle(Math.Round(stopWatch.ElapsedSeconds, 7).ToString());
                        }
                        label3.Text = stopWatch.Elapsed.ToString();
                        trackBar1.Value = (int)stopWatch.ElapsedSeconds;
                        this.Text = stopWatch.Elapsed.ToString();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                backgroundWorker1.CancelAsync();
                stopWatch.Stop();
            }
            stopWatch = new MyStopwatch(new TimeSpan());
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TimeSpan time = TimeSpan.FromSeconds(trackBar1.Value);
            stopWatch = new MyStopwatch(time);
            stopWatch.Start();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //MessageBox.Show($"KeyPress keychar: {e.KeyChar}" + "\r\n");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {
                this.BackColor = Color.LimeGreen;
                this.TransparencyKey = Color.LimeGreen;
                this.FormBorderStyle = FormBorderStyle.None;
                panel1.Visible = true;
            }
            else if (e.KeyCode == Keys.S)
            {
                this.BackColor = Color.LimeGreen;
                this.TransparencyKey = Color.LimeGreen;
                this.FormBorderStyle = FormBorderStyle.None;
                panel1.Visible = false;
            }
            else
            {
                panel1.Visible = true;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.BackColor = Color.Gray;

            }
        }

        private void label1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"C:\Users\ahmet\Desktop\Movie";
                openFileDialog.Filter = "Srt Files (*.srt)|*.srt|All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    trsrtModel = StrHelperTR.ParseSrt(filePath);
                    //Read the contents of the file into a stream
                    //var fileStream = openFileDialog.OpenFile();
                }
            }
        }

        private void label2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"C:\Users\ahmet\Desktop\Movie";
                openFileDialog.Filter = "Srt Files (*.srt)|*.srt|All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    uasrtModel = StrHelperUA.ParseSrt(filePath);
                    //Read the contents of the file into a stream
                    //var fileStream = openFileDialog.OpenFile();
                }
            }
        }
    }

    public class MyStopwatch : System.Diagnostics.Stopwatch
    {
        public TimeSpan StartOffset { get; private set; }

        public MyStopwatch(TimeSpan startOffset)
        {
            StartOffset = startOffset;
        }

        public new long ElapsedMilliseconds
        {
            get
            {
                return base.ElapsedMilliseconds + (long)StartOffset.TotalMilliseconds;
            }
        }

        public new double ElapsedSeconds
        {
            get
            {
                return base.Elapsed.TotalSeconds + StartOffset.TotalSeconds;
            }
        }

        public new TimeSpan Elapsed
        {
            get
            {
                return base.Elapsed + StartOffset;
            }
        }
        public new long ElapsedTicks
        {
            get
            {
                return base.ElapsedTicks + StartOffset.Ticks;
            }
        }
    }
}
