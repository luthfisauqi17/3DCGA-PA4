using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DCGA_PA4
{
    public partial class Form1 : Form
    {
        //Define struct
        public struct TPoint
        {
            public double x, y, z, w;
        }

        public struct TLine
        {
            public int p1, p2;
        }


        //Global variables
        Bitmap bmp;
        Graphics g;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Blue);

            g.DrawLine(pen, new Point(0, pictureBox1.Height / 2), new Point(pictureBox1.Width, pictureBox1.Height / 2));
            g.DrawLine(pen, new Point(pictureBox1.Width / 2, 0), new Point(pictureBox1.Width / 2, pictureBox1.Height));

            pictureBox1.Image = bmp;
        }
    }
}
