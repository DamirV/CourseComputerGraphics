using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.ComponentModel;

namespace _12._02._2018_9._26_ProjectF
{
    abstract class Filters
    {

        protected abstract Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y);

        protected Random rand = new Random();

        public int Clamp(int value, int min, int max)
       {
           if (value < min)
               return min;
           if (value > max)
               return max;
           return value;
       }

     virtual  public Bitmap processImage(Bitmap sourceImage,BackgroundWorker worker)
       {
           Bitmap resultImage=new Bitmap(sourceImage.Width, sourceImage.Height);


           for(int i=0; i < sourceImage.Width;i++)
           {

               worker.ReportProgress((int)((float)i / resultImage.Width * 100));

               if (worker.CancellationPending)
                   return null;


               for(int j=0; j<sourceImage.Height;j++)
               {
                   resultImage.SetPixel(i, j, CalculateNewPixelColor(sourceImage,i,j));
          
               }


           }


           return resultImage;
       }  
    }

    class InvertFilters:Filters
    {
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }
    }

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;

        protected MatrixFilter()
        {

        }

        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }

            }

            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }

    }


    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for(int i = 0; i < sizeX; i++)
            {
                for(int j=0;j<sizeY;j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }

    }

    class GaussianFilter: MatrixFilter
    {

        public GaussianFilter()
        {
            CreateGaussianKernel(3, 2);
        }

        public void CreateGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;

            for(int i = -radius;i<=radius;i++)
            {
                for(int j=-radius;j<=radius;j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }

            for(int i = 0; i< size;i++)
            {
                for(int j=0;j<size;j++)
                {
                    kernel[i, j] /= norm;
                }
            }
        }

    }

    class GrayScaleFilter : Filters
    {

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            int Intensity=(int)(0.36*sourceColor.R+0.53*sourceColor.G+0.11*sourceColor.B);
           


            Color resultColor = Color.FromArgb(Intensity,Intensity,Intensity);

            return resultColor;
        }

    }

    class Sepia: Filters
    {

          protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int Intensity=(int)(0.36*sourceColor.R+0.53*sourceColor.G+0.11*sourceColor.B);

            int k = 20;

            int R = Clamp((int)(Intensity + 2 * k), 0, 255);
            int G = Clamp((int)(Intensity + 0.5 * k), 0, 255);
            int B = Clamp((int)(Intensity - 1 * k), 0, 255);

            Color resultColor = Color.FromArgb(R,G,B);

            return resultColor;
        }
    }

    class Bright:Filters
    {
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int k = 50;
            Color resultColor = Color.FromArgb(Clamp(sourceColor.R + k, 0, 255), Clamp(sourceColor.G + k, 0, 255), Clamp(sourceColor.B + k, 0, 255));


            return resultColor;
        }



    }

    class SobelFilter : MatrixFilter
    {
        public float[,] kernelX = new float[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
        public float[,] kernelY = new float[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

        public SobelFilter()
        {
          
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = 3 / 2;
            int radiusY = 3 / 2;

            float resultXR = 0, resultXG = 0, resultXB = 0,
                resultYR = 0, resultYG = 0, resultYB = 0;

            for (int i = -radiusX; i <= radiusX; i++)
                for (int j = -radiusY; j <= radiusY; j++)
                {

                    int idX = Clamp(x + i, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + j, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultXR += neighborColor.R * kernelX[i + radiusX, j + radiusY];
                    resultXG += neighborColor.G * kernelX[i + radiusX, j + radiusY];
                    resultXB += neighborColor.B * kernelX[i + radiusX, j + radiusY];

                    resultYR += neighborColor.R * kernelY[i + radiusX, j + radiusY];
                    resultYG += neighborColor.G * kernelY[i + radiusX, j + radiusY];
                    resultYB += neighborColor.B * kernelY[i + radiusX, j + radiusY];
                }

            int sum = (int)(Math.Sqrt(resultXR * resultXR + resultYR * resultYR) +
                      Math.Sqrt(resultXG * resultXG + resultYG * resultYG) +
                      Math.Sqrt(resultXB * resultXB + resultYB * resultYB));

            return Color.FromArgb(Clamp(sum, 0, 255), Clamp(sum, 0, 255), Clamp(sum, 0, 255));
        }
    }

    class SharFilter : MatrixFilter
    {
        public float[,] kernelX = new float[3, 3] { { 3, 0, -3 }, { 10, 0, -10 }, { 3, 0, -3 } };
        public float[,] kernelY = new float[3, 3] { { 3, 10, 3 }, { 0, 0, 0 }, { -3, -10, -3 } };

        public SharFilter()
        {

        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = 3 / 2;
            int radiusY = 3 / 2;

            float resultXR = 0, resultXG = 0, resultXB = 0,
                resultYR = 0, resultYG = 0, resultYB = 0;

            for (int i = -radiusX; i <= radiusX; i++)
                for (int j = -radiusY; j <= radiusY; j++)
                {

                    int idX = Clamp(x + i, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + j, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultXR += neighborColor.R * kernelX[i + radiusX, j + radiusY];
                    resultXG += neighborColor.G * kernelX[i + radiusX, j + radiusY];
                    resultXB += neighborColor.B * kernelX[i + radiusX, j + radiusY];

                    resultYR += neighborColor.R * kernelY[i + radiusX, j + radiusY];
                    resultYG += neighborColor.G * kernelY[i + radiusX, j + radiusY];
                    resultYB += neighborColor.B * kernelY[i + radiusX, j + radiusY];
                }

            int sum = (int)(Math.Sqrt(resultXR * resultXR + resultYR * resultYR) +
                      Math.Sqrt(resultXG * resultXG + resultYG * resultYG) +
                      Math.Sqrt(resultXB * resultXB + resultYB * resultYB));

            return Color.FromArgb(Clamp(sum, 0, 255), Clamp(sum, 0, 255), Clamp(sum, 0, 255));
        }
    }

    class PruittFilter : MatrixFilter
    {
        public float[,] kernelX = new float[3, 3] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
        public float[,] kernelY = new float[3, 3] { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } };

        public PruittFilter()
        {

        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = 3 / 2;
            int radiusY = 3 / 2;

            float resultXR = 0, resultXG = 0, resultXB = 0,
                resultYR = 0, resultYG = 0, resultYB = 0;

            for (int i = -radiusX; i <= radiusX; i++)
                for (int j = -radiusY; j <= radiusY; j++)
                {

                    int idX = Clamp(x + i, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + j, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultXR += neighborColor.R * kernelX[i + radiusX, j + radiusY];
                    resultXG += neighborColor.G * kernelX[i + radiusX, j + radiusY];
                    resultXB += neighborColor.B * kernelX[i + radiusX, j + radiusY];

                    resultYR += neighborColor.R * kernelY[i + radiusX, j + radiusY];
                    resultYG += neighborColor.G * kernelY[i + radiusX, j + radiusY];
                    resultYB += neighborColor.B * kernelY[i + radiusX, j + radiusY];
                }

            int sum = (int)(Math.Sqrt(resultXR * resultXR + resultYR * resultYR) +
                      Math.Sqrt(resultXG * resultXG + resultYG * resultYG) +
                      Math.Sqrt(resultXB * resultXB + resultYB * resultYB));

            return Color.FromArgb(Clamp(sum, 0, 255), Clamp(sum, 0, 255), Clamp(sum, 0, 255));
        }
    }



    class Harshness : MatrixFilter
    {
        public Harshness()
        {
         

           kernel= new float[3, 3];

            kernel[0,0] = -1;
            kernel[0, 1] = -1;
            kernel[0, 2] = -1;
            kernel[1, 1] = 9;
            kernel[1, 2] = -1;
            kernel[2, 0] = -1;
            kernel[2, 1] = -1;
            kernel[2, 2] = -1;
            kernel[1, 0] = -1;



        }

    }

    class Stamping:MatrixFilter
    {
       public Stamping()
        {
            kernel = new float[3,3] { { 1, 5, 1 }, { 5, 0, -5 }, { 1, -9, 1 } };
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            int b = 100;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }

            }


            int mid = (int)((resultG + resultR + resultB) / 3);

            return Color.FromArgb(Clamp((int)resultR+b+mid, 0, 255), Clamp((int)resultG+b+mid, 0, 255), Clamp((int)resultB+b+mid, 0, 255));
        }


    }


    class Waves:Filters
    {
        protected override  Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel( Clamp((int)(x+20 * Math.Sin(2*Math.PI*y/60)),0,sourceImage.Width-1), y);


            int R = sourceColor.R;
            int G = sourceColor.G;
            int B = sourceColor.B;

            Color resultColor = Color.FromArgb(R, G, B);

            return resultColor;
        }
    }

    class Glass : Filters
    {


        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            

            Color sourceColor = sourceImage.GetPixel(
                Clamp((int)(x+20*(rand.NextDouble() -0.5)  ), 0, sourceImage.Width - 1), 

                Clamp((int)( y+20*(rand.NextDouble() -0.5)), 0, sourceImage.Height - 1)
                );


            int R = sourceColor.R;
            int G = sourceColor.G;
            int B = sourceColor.B;

            Color resultColor = Color.FromArgb(R, G, B);

            return resultColor;
        }


    }

    class Mediana : MatrixFilter
    {
        public Mediana()
        {
          // kernel = new float[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = 5/2;
            int radiusY = 5/2;

            int[] R = new int[25];
            int[] G = new int[25];
            int[] B = new int[25];

            int flag = 0;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    R[flag] = neighborColor.R;
                    G[flag] = neighborColor.G;
                    B[flag] = neighborColor.B;

                    flag++;
                }

            }

            Array.Sort(R);
            Array.Sort(G);
            Array.Sort(B);

            Color resultColor = Color.FromArgb(R[13], G[13], B[13]);

            return resultColor;
        }
    }


    class GrayWorld : Filters
    {
       int _R, _G, _B, _Arg, f;

       public GrayWorld()
        {
            _R= _G= _B= _Arg=f = 0;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            if (f == 0)
            {
                AllColors(sourceImage);
                f++;
            }

            Color sourceColor = sourceImage.GetPixel(x, y);

            int resR = Clamp(sourceColor.R * _Arg/ _R, 0, 255);
            int resG = Clamp(sourceColor.G * _Arg/ _G, 0, 255);
            int resB = Clamp(sourceColor.B * _Arg/ _B, 0, 255);

            Color resultColor = Color.FromArgb(resR,resG,resB);

            return resultColor;
        }


        public void AllColors(Bitmap sourceImage)
        {
            

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            int flag = 0;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color neighborColor = sourceImage.GetPixel(i, j);

                    _R += neighborColor.R;
                    _G += neighborColor.G;
                    _B += neighborColor.B;

                    flag++;

                }
            }

            _R /= flag;
            _G /= flag;
            _B /= flag;

            _Arg = (_R + _G + _B) / 3;
        }
    }

    class Correction : Filters
    {

        int maxR = 0, minR = 0, maxG = 0, minG = 0, maxB = 0, minB = 0, f = 0;

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            if (f == 0)
            {
                AllColors(sourceImage);
                f++;
            }

            Color sourceColor = sourceImage.GetPixel(x, y);

            int resR = Clamp((sourceColor.R - minR) * 255 / (Clamp(maxR - minR, 1, 255)), 0, 255);
            int resG = Clamp((sourceColor.G - minG) * 255 / (Clamp(maxG - minG, 1, 255)), 0, 255);
            int resB = Clamp((sourceColor.B - minB) * 255 / (Clamp(maxB - minB, 1, 255)), 0, 255);

            Color resultColor = Color.FromArgb(resR, resG, resB);

            return resultColor;
        }

        public void AllColors(Bitmap sourceImage)
        {

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);


            minR = maxR = sourceImage.GetPixel(0, 0).R;
            minG = maxG = sourceImage.GetPixel(0, 0).G;
            minB = maxB = sourceImage.GetPixel(0, 0).B;


            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color neighborColor = sourceImage.GetPixel(i, j);


                    if (neighborColor.R < minR)
                    {
                        minR = neighborColor.R;
                    }

                    if (neighborColor.R > maxR)
                    {
                        maxR = neighborColor.R;
                    }
                    //////////////////////////////////////
                    if (neighborColor.G < minG)
                    {
                        minG = neighborColor.G;
                    }

                    if (neighborColor.G > maxG)
                    {
                        maxG = neighborColor.G;
                    }
                    ///////////////////////////////////////
                    if (neighborColor.B < minB)
                    {
                        minB = neighborColor.B;
                    }

                    if (neighborColor.B > maxB)
                    {
                        maxB = neighborColor.B;
                    }
                }
            }
        }
    }


        abstract class MatMorf : MatrixFilter
        {
            public MatMorf()
            {
              //  kernel = new float[,] { { 0, 1, 0}, { 1, 1, 1 }, { 0, 1, 0 } };
            }

            public MatMorf(float[,] elem)
            {
                kernel = elem;
            }
        }

        class Dilation : MatMorf
        {

        public Dilation(float[,] _kernel)
        {
            kernel = _kernel;
        }

        public Dilation()
        {

        }

            protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int radiusX = kernel.GetLength(0) / 2;
                int radiusY = kernel.GetLength(1) / 2;
                int max = 0;

                Color resultColor = Color.Black;

                for(int i=-radiusY;i<=radiusY;i++)
                {
                    for(int j = -radiusX;j<=radiusX;j++)
                    {
                        int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + i, 0, sourceImage.Height - 1);
                        
                        Color neighborColor = sourceImage.GetPixel(idX, idY);
                        int Intensity = neighborColor.R;

                        if((neighborColor.R!= neighborColor.G)||(neighborColor.R!=neighborColor.B)||(neighborColor.G!= neighborColor.B))
                        {
                            Intensity = Clamp((int)(0.36 * neighborColor.R + 0.53 * neighborColor.G + 0.11 * neighborColor.B),0,255);
                        }
                        if ((kernel[j + radiusX, i + radiusY] > 0) && (Intensity > max))
                        {
                            max = Intensity;
                            resultColor = neighborColor;
                        }
                    }
                }

                return resultColor;
            }
        }

        class Erosion : MatMorf
        {

        public Erosion()
        {

        }

        public Erosion(float[,] _kernel)
        {
            kernel = _kernel;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int radiusX = kernel.GetLength(0) / 2;
                int radiusY = kernel.GetLength(1) / 2;
                    int min = 255;  

                    Color resultColor = Color.White;

             
                for (int i = -radiusY; i <= radiusY; i++)
                    for (int j = -radiusX; j <= radiusX; j++)
                    {
                        int idX = Clamp(x + j, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + i, 0, sourceImage.Height - 1);

                        Color neighborColor = sourceImage.GetPixel(idX, idY);
                        int Intensity = neighborColor.R;

                        if ((neighborColor.R != neighborColor.G) || (neighborColor.R != neighborColor.B) || (neighborColor.G != neighborColor.B))
                        {
                            Intensity = Clamp((int)(0.36 * neighborColor.R + 0.53 * neighborColor.G + 0.11 * neighborColor.B),0,255);
                        }
                        if ((kernel[j + radiusX, i + radiusY] > 0) && (Intensity < min))
                        {
                            min = Intensity;
                            resultColor = neighborColor;
                        }
                    }
                return resultColor;
            }
        }

        class Opening : MatMorf
        {

          public Opening(float[,] _kernel)
         {
            kernel = _kernel;
         }

         public Opening()
         {
           
         }


            public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
            {

            Erosion er = new Erosion(kernel);
            Dilation di = new Dilation(kernel);
            
            return di.processImage(er.processImage(sourceImage,worker),worker);

            }
        }

        class Closing : MatMorf
        {

        public Closing()
        {

        }

        public Closing(float[,] _kernel)
        {
            kernel = _kernel;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
            {
            Dilation di = new Dilation(kernel);
            Erosion er = new Erosion(kernel);

            return er.processImage(di.processImage(sourceImage, worker), worker);
        }
        }

    class Grad : MatMorf
    {
        public Grad(float[,] _kernel)
        {
            kernel = _kernel;
        }

        public Grad()
        {

        }

        public Bitmap ResultGrad(Bitmap sourceImage1, Bitmap sourceImage2, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage1.Width, sourceImage1.Height);
         

            for (int i = 0; i < sourceImage1.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));

                if (worker.CancellationPending)
                    return null;


                for (int j = 0; j < sourceImage1.Height; j++)
                {
                    Color res = new Color();

                    res = Color.FromArgb(Clamp(sourceImage1.GetPixel(i, j).R - sourceImage2.GetPixel(i, j).R,
                        Clamp(sourceImage1.GetPixel(i, j).G - sourceImage2.GetPixel(i, j).G,0,255),
                        Clamp(sourceImage1.GetPixel(i, j).B - sourceImage2.GetPixel(i, j).B,0,255)));


                 /*   if (sourceImage1.GetPixel(i, j).R > 0 && sourceImage2.GetPixel(i, j).R > 0)
                    {
                        res = Color.FromArgb(sourceImage1.GetPixel(i, j).R, sourceImage1.GetPixel(i, j).G, sourceImage1.GetPixel(i, j).B);
                    }
                    else
                    {
                        res = Color.FromArgb(0, 0, 0);
                    }*/

                    resultImage.SetPixel(i, j, res);
                }
            }

            return resultImage;  
        }

        public Bitmap Invert(Bitmap sourceImage,BackgroundWorker worker)
        {
            Bitmap resultImage= new Bitmap(sourceImage);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));

                if (worker.CancellationPending)
                    return null;


                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color resColor = sourceImage.GetPixel(i, j);

                    resColor = Color.FromArgb(255 - resColor.R, 255 - resColor.G, 255 - resColor.B);


                    resultImage.SetPixel(i, j, resColor);

                }
            }
            return resultImage;
        }


        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Dilation di = new Dilation(kernel);
            Erosion er = new Erosion(kernel);

            Bitmap image1= er.processImage(di.processImage(sourceImage, worker), worker);
            Bitmap image2 = di.processImage(er.processImage(sourceImage, worker), worker);


            //image1 = Invert(image1,worker);

            return ResultGrad(image2, image1, worker);
        }
    }

    class Gradient:MatMorf
    {
        public Gradient(float[,] _kernel)
        {
            kernel = _kernel;
        }


        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Filters di = new Dilation(kernel);
            Filters er = new Erosion(kernel);

            Bitmap tempDi = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap tempEr = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap tempRes = new Bitmap(sourceImage.Width, sourceImage.Height);


            tempDi = di.processImage(sourceImage, worker);
            tempEr = er.processImage(sourceImage, worker);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / tempRes.Width * 100));

                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    tempRes.SetPixel(i,j,Color.FromArgb( Clamp(tempDi.GetPixel(i,j).R - tempEr.GetPixel(i,j).R,0,255),
                        Clamp(tempDi.GetPixel(i,j).G-tempEr.GetPixel(i,j).G,0,255),
                        Clamp(tempDi.GetPixel(i,j).B-tempEr.GetPixel(i,j).B,0,255)));
                }

                

            }



            return tempRes;
        }
    }

    class Sepia2 : Filters
    {

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int Intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);

            int k = 20;

            int R = Clamp((int)(Intensity + 2 * k), 0, 255);
            int G = Clamp((int)(Intensity + 0.5 * k), 0, 255);
            int B = Clamp((int)(Intensity - 1 * k), 0, 255);

            Color resultColor = Color.FromArgb(R, G, B);

            return resultColor;
        }

         public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);


            double k =  (double)(sourceImage.Height)/ (double)(sourceImage.Width);

            for (int i = 0; i < sourceImage.Width; i++)
            {

                worker.ReportProgress((int)((float)i / resultImage.Width * 100));

                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    if ( j < -i * k +sourceImage.Height)
                    {
                        resultImage.SetPixel(i, j, CalculateNewPixelColor(sourceImage, i, j));
                    }

                    else
                    {
                         resultImage.SetPixel(i,j,sourceImage.GetPixel(i,j));
                    }


                }


            }


            return resultImage;
        }  
    }

}
