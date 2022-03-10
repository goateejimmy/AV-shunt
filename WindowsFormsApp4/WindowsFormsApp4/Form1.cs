using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.Util;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        int i;
        int fourcc;
        int width;
        int height;
        int fps;
        bool flag;
        List<Mat> buffer = new List<Mat>();
        VideoCapture capture;
        VideoWriter writer;
        Image<Bgr, byte> frame;
        Image<Bgr, byte> copy;
        Thread forcap;
        public Form1()
        {
            i = 0;
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            flag = false;
            capture = new VideoCapture(0);
            capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 30);
            capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Buffersuze,3);
            fourcc = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FourCC));
            width = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth));
            height = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight));
            fps = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
           

           
            frame = new Image<Bgr, byte>(capture.Width, capture.Height);
           
        }

        //private void Capture_ImageGrabbed(object sender, EventArgs e)
        //{
        //    if (capture.Grab())
        //    { capture.Retrieve(frame, 0); }
           
        //    pictureBox1.Image = frame.Bitmap;
        //    writer.Write(frame.Mat);
        //}
        private void cap()
        {
            while (true)
            {
                if (capture.Grab())
                {
                    Console.WriteLine(i);


                    //capture.Retrieve(frame, 0);

                 
                   

                    //pictureBox1.Image = frame.Bitmap;

                    //if (flag)
                    //{
                    //    buffer.Add(frame.Mat);
                    //}
                    i = i + 1;
                }
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            writer = new VideoWriter(@"D:\jim\thesis\testvideo\testvid.mp4", fourcc, fps, new Size(width, height), true);
            forcap = new Thread(cap);
            flag = true;
            forcap.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            flag = false;
           
            MessageBox.Show("done");
            
            forcap.Abort();
            var buf_array = buffer.ToArray();
            textBox1.Text =buf_array.Length.ToString();
            foreach (Mat i in buf_array)
            {
                writer.Write(i);
            }
            
            writer.Dispose();
        }
    }
}
