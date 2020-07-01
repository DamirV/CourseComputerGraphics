using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace _12._02._2018_9._26_ProjectF
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Stack<Bitmap> stack = new Stack<Bitmap>();
        int size;

        public Form1()
        {
            InitializeComponent();
            size = 3;

            
            dataGridView1.ColumnCount = size;
            dataGridView1.RowCount = size;

            dataGridView1.Rows[0].Cells[0].Value = 0;
            dataGridView1.Rows[0].Cells[1].Value = 1;
            dataGridView1.Rows[0].Cells[2].Value = 0;

            dataGridView1.Rows[1].Cells[0].Value = 1;
            dataGridView1.Rows[1].Cells[1].Value = 1;
            dataGridView1.Rows[1].Cells[2].Value = 1;

            dataGridView1.Rows[2].Cells[0].Value = 0;
            dataGridView1.Rows[2].Cells[1].Value = 1;
            dataGridView1.Rows[2].Cells[2].Value = 0;

        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = " Image files|*.png; *.jpg; *.bmp| All Files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);

                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }



        }

        private void inversiyaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);

            Filters filter = new InvertFilters();
            backgroundWorker1.RunWorkerAsync(filter);


        }



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);

            if (backgroundWorker1.CancellationPending != true)
            {
                image = newImage;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }






        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;

        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрГаусаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void чернобелыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void степияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Sepia();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void яркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Bright();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сохранитьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPeg Image|.jpg|Bitmap Image|.bmp|Gif Image|*.gif";
            dialog.Title = "Save an Image File";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {

               System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();

                switch (dialog.FilterIndex)
                {
                    case 1:
                        this.image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        this.image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        this.image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }

                fs.Close();
            }


        }




        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Harshness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрСобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Stamping();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            image = stack.Pop();
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        private void операторЩараToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new SharFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void операторПрюиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new PruittFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void волныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Waves();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void стеклоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Glass();
            backgroundWorker1.RunWorkerAsync(filter);

        }

        

        private void медианныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Mediana();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void назадToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stack.Count == 0)
            {
                return;
            }
            else
            {
                image = stack.Pop();
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new GrayWorld();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void коррекцияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Correction();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[,] kernel = new float[size,size];

            for(int i=0;i< size; i++)
                for(int j=0; j< size; j++)
                {
                    kernel[j, i] = (float)(Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value));
                }

            stack.Push(image);
            Filters filter = new Dilation(kernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[,] kernel = new float[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    kernel[j, i] = (float)(Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value));
                }

            stack.Push(image);
            Filters filter = new Erosion(kernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void открытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[,] kernel = new float[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    kernel[j, i] = (float)(Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value));
                }

            stack.Push(image);
            Filters filter = new Opening(kernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[,] kernel = new float[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    kernel[j, i] = (float)(Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value));
                }

            stack.Push(image);
            Filters filter = new Closing(kernel);
            backgroundWorker1.RunWorkerAsync(filter);
             
        }

        private void градToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[,] kernel = new float[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    kernel[j, i] = (float)(Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value));
                }

            stack.Push(image);
            Filters filter = new Gradient(kernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            size+=2;
            dataGridView1.ColumnCount = size;
            dataGridView1.RowCount = size;

            if (size > 1)
            {
                button2.Enabled = true;
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            size-=2;
            dataGridView1.ColumnCount = size;
            dataGridView1.RowCount = size;

            if(size==1)
            {
                button2.Enabled = false;
            }
        }

        private void сепия2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stack.Push(image);
            Filters filter = new Sepia2();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }


}