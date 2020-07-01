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

namespace RayTracing
{
    public partial class Form1 : Form
    {
        RayTracing scene;
        public Form1()
        {
            InitializeComponent();
            scene = new RayTracing();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            scene.Resize(glControl1.Width, glControl1.Height);
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            scene.Update();
            glControl1.SwapBuffers();
            //scene.closeProgram();
        }

    }
}
