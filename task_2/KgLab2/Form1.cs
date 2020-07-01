using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KgLab2
{
    public partial class Form1 : Form
    {
        bool loaded = false;
        bool needReload = true;
        Bin rd = new Bin();
        View vw = new View();
        int currentLayer;
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);

        public Form1()
        {
            InitializeComponent();
            View.Min = trackBar2.Value;
            View.Max = trackBar3.Value + trackBar2.Value;
            textBox3.Text = Convert.ToString(trackBar1.Minimum);
            textBox4.Text = Convert.ToString(trackBar1.Value);
            textBox5.Text = Convert.ToString(trackBar1.Maximum);
        }

        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }

            FrameCount++;
        }


        private void glControl1_Load(object sender, EventArgs e)
        {

        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                rd.readBin(str);

                vw.SetupView(glControl1.Width, glControl1.Height);
                vw.SetupLighting();

                trackBar1.Maximum = Bin.Z - 1;
                trackBar1.Minimum = 0;
                trackBar1.Value = 0;
                loaded = true;
                glControl1.Invalidate();

            }
            textBox3.Text = Convert.ToString(trackBar1.Minimum);
            textBox5.Text = Convert.ToString(trackBar1.Maximum);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (radioButton1.Checked)
                {
                    if (needReload)
                    {
                        vw.generateTextureImage(currentLayer);
                        vw.Load2DTexture();
                        needReload = false;
                    }
                    vw.DrawTexture();
                    glControl1.SwapBuffers();
                }
                if (radioButton2.Checked)
                {
                    vw.DrawQuads(currentLayer);
                    glControl1.SwapBuffers();
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
            textBox3.Text = Convert.ToString(trackBar1.Minimum);
            textBox4.Text = Convert.ToString(trackBar1.Value);
            textBox5.Text = Convert.ToString(trackBar1.Maximum);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            View.Min = trackBar2.Value;
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            View.Max = trackBar3.Value + trackBar2.Value;
            needReload = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                trackBar1.Value = Convert.ToInt32(textBox4.Text);
                currentLayer = trackBar1.Value;
                needReload = true;
                textBox3.Text = Convert.ToString(trackBar1.Minimum);
                textBox4.Text = Convert.ToString(trackBar1.Value);
                textBox5.Text = Convert.ToString(trackBar1.Maximum);
            }
            catch
            {

            }
        }
    }
}
