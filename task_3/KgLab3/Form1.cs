using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KgLab3
{
    public partial class Form1 : Form
    {
        Graphics gr;
        Shaders scene;
        public Form1()
        {
            InitializeComponent();
        //    gr = new Graphics();
            scene = new Shaders();
        }

        private void glControl1_Load_1(object sender, EventArgs e)
        {
            scene.Resize(glControl1.Width, glControl1.Height);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void Application_Idle(object sender, PaintEventArgs e)
        {

          //  glControl1_Paint(sender, e);

        }

        private void glControl1_Paint_1(object sender, PaintEventArgs e)
        {
            // gr.Update();
            scene.Update();
            glControl1.SwapBuffers();
            //gr.closeProgram();
            scene.closeProgram();
        }
    }
}
