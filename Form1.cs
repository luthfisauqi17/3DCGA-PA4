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
        TPoint[] V = new TPoint[10];
        TPoint[] VW = new TPoint[10];
        TPoint[] VV = new TPoint[10];
        TPoint[] VS = new TPoint[10];

        TLine[] E = new TLine[15];

        double[,] Wt = new double[4, 4];
        double[,] Vt = new double[4, 4];
        double[,] St = new double[4, 4];


        //Functions
        public void setPoint(ref TPoint V, double x, double y, double z)
        {
            V.x = x;
            V.y = y;
            V.z = z;
            V.w = 1;
        }

        public void setLine(ref TLine E, int p1, int p2)
        {
            E.p1 = p1;
            E.p2 = p2;
        }

        public void setRowMatrix(ref double[,] M, int row, double a, double b, double c, double d)
        {
            M[row, 0] = a;
            M[row, 1] = b;
            M[row, 2] = c;
            M[row, 3] = d;
        }

        public TPoint multiplyMatrix(TPoint P, double[,] M)
        {
            TPoint temp;
            double w = P.x * M[0, 3] + P.y * M[1, 3] + P.z * M[2, 3] + P.w * M[3, 3];
            temp.x = (P.x * M[0, 0] + P.y * M[1, 0] + P.z * M[2, 0] + P.w * M[3, 0]) / w;
            temp.y = (P.x * M[0, 1] + P.y * M[1, 1] + P.z * M[2, 1] + P.w * M[3, 1]) / w;
            temp.z = (P.x * M[0, 2] + P.y * M[1, 2] + P.z * M[2, 2] + P.w * M[3, 2]) / w;
            temp.w = 1;
            return temp;
        }

        public void draw()
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);

            Pen redPen = new Pen(Color.Red);
            Pen blackPen = new Pen(Color.Black);
            TPoint p1, p2;
            for (int i = 5; i < 15; i++)
            {
                p1 = VS[E[i].p1];
                p2 = VS[E[i].p2];
                g.DrawLine(blackPen, (float)p1.x, (float)p1.y, (float)p2.x, (float)p2.y);
            }
            for (int i = 0; i < 5; i++)
            {
                p1 = VS[E[i].p1];
                p2 = VS[E[i].p2];
                g.DrawLine(redPen, (float)p1.x, (float)p1.y, (float)p2.x, (float)p2.y);
            }

            pictureBox1.Image = bmp;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setPoint(ref V[0], -1, -1, 1);
            setPoint(ref V[1], 1, -1, 1);
            setPoint(ref V[2], 1, 0, 1);
            setPoint(ref V[3], 0, 1, 1);
            setPoint(ref V[4], -1, 0, 1);
            setPoint(ref V[5], -1, -1, -1);
            setPoint(ref V[6], 1, -1, -1);
            setPoint(ref V[7], 1, 0, -1);
            setPoint(ref V[8], 0, 1, -1);
            setPoint(ref V[9], -1, 0, -1);

            setLine(ref E[0], 0, 1);
            setLine(ref E[1], 1, 2);
            setLine(ref E[2], 2, 3);
            setLine(ref E[3], 3, 4);
            setLine(ref E[4], 4, 0);
            setLine(ref E[5], 5, 6);
            setLine(ref E[6], 6, 7);
            setLine(ref E[7], 7, 8);
            setLine(ref E[8], 8, 9);
            setLine(ref E[9], 9, 5);
            setLine(ref E[10], 0, 5);
            setLine(ref E[11], 1, 6);
            setLine(ref E[12], 2, 7);
            setLine(ref E[13], 3, 8);
            setLine(ref E[14], 4, 9);

            setRowMatrix(ref Wt, 0, 1, 0, 0, 0);
            setRowMatrix(ref Wt, 1, 0, 1, 0, 0);
            setRowMatrix(ref Wt, 2, 0, 0, 1, 0);
            setRowMatrix(ref Wt, 3, 0, 0, 0, 1);

            setRowMatrix(ref Vt, 0, 1, 0, 0, 0);
            setRowMatrix(ref Vt, 1, 0, 1, 0, 0);
            setRowMatrix(ref Vt, 2, 0, 0, 0, 0);
            setRowMatrix(ref Vt, 3, 0, 0, 0, 1);

            setRowMatrix(ref St, 0, 50, 0, 0, 0);
            setRowMatrix(ref St, 1, 0, -50, 0, 0);
            setRowMatrix(ref St, 2, 0, 0, 0, 0);
            setRowMatrix(ref St, 3, 200, 200, 0, 1);

            for (int i = 0; i < 10; i++)
            {
                VW[i] = multiplyMatrix(V[i], Wt);
                VV[i] = multiplyMatrix(VW[i], Vt);
                VS[i] = multiplyMatrix(VV[i], St);
            }

            for(int i=0; i<10; i++)
            {
                debugTextBox.AppendText(i + " => " + "(" + VS[i].x + ", " + VS[i].y + ", " + VS[i].z + ")" + Environment.NewLine);
            }

            draw();
        }
    }
}
