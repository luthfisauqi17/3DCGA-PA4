using System;
using System.CodeDom.Compiler;
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
        #region Structs
        public struct TPoint
        {
            public double x, y, z, w;
        }

        public struct TLine
        {
            public int p1, p2;
        }
        #endregion

        #region Global variables
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
        TPoint VRP, VPN, VUP, COP, N, upUnit, upVec, v, u, DOP, CW = new TPoint();
        double windowUmin, windowVmin, windowUmax, windowVmax, FP, BP, rx, ry, rz, shx, shy, dx, dy, dz, sx, sy, sz;
        double[,] T1 = new double[4, 4];
        double[,] T2 = new double[4, 4];
        double[,] T3 = new double[4, 4];
        double[,] T4 = new double[4, 4];
        double[,] T5 = new double[4, 4];
        double[,] Pr1 = new double[4, 4];
        double[,] Pr2 = new double[4, 4];
        #endregion

        #region Functions
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

        public double[,] matrixMultiplication(double[,] M1, double [,] M2)
        {
            double[,] temp = new double[4,4];
            for(int i=0; i<4; i++)
            {
                for(int j=0; j<4; j++)
                {
                    temp[i, j] = 0;
                    for(int k=0; k<4; k++)
                    {
                        temp[i, j] += M1[i, k] * M2[k, j];
                    }
                }
            }
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
        #endregion

        #region Main code
        public Form1()
        {
            InitializeComponent();
        }

        private void drawBtn_Click(object sender, EventArgs e)
        {
            // Assigning variables value from the textboxes
            setPoint(ref VRP, Convert.ToDouble(VRPxTextBox.Text), Convert.ToDouble(VRPyTextBox.Text), Convert.ToDouble(VRPzTextBox.Text));
            setPoint(ref VPN, Convert.ToDouble(VPNxTextBox.Text), Convert.ToDouble(VPNyTextBox.Text), Convert.ToDouble(VPNzTextBox.Text));
            setPoint(ref VUP, Convert.ToDouble(VUPxTextBox.Text), Convert.ToDouble(VUPyTextBox.Text), Convert.ToDouble(VUPzTextBox.Text));
            setPoint(ref COP, Convert.ToDouble(COPxTextBox.Text), Convert.ToDouble(COPyTextBox.Text), Convert.ToDouble(COPzTextBox.Text));
            windowUmin = Convert.ToDouble(windowUminTextBox.Text);
            windowVmin = Convert.ToDouble(windowVminTextBox.Text);
            windowUmax = Convert.ToDouble(windowUmaxTextBox.Text);
            windowVmax = Convert.ToDouble(windowVmaxTextBox.Text);
            FP = Convert.ToDouble(FPTextBox.Text);
            BP = Convert.ToDouble(BPTextBox.Text);

            // Creating temporary variables
            double temp;
            TPoint tempPoint = new TPoint(); ;

            // Calculating N unit vector
            temp = Math.Sqrt(Math.Pow(VPN.x, 2) + Math.Pow(VPN.y, 2) + Math.Pow(VPN.z, 2));
            setPoint(ref N, VPN.x / temp, VPN.y / temp, VPN.z / temp);
            
            // Calculating up unit vector
            temp = Math.Sqrt(Math.Pow(VUP.x, 2) + Math.Pow(VUP.y, 2) + Math.Pow(VUP.z, 2));
            setPoint(ref upUnit, VUP.x / temp, VUP.y / temp, VUP.z / temp);

            // Calculating up vector
            temp = upUnit.x * N.x + upUnit.y * N.y + upUnit.z * N.z;
            tempPoint.x = temp * N.x;
            tempPoint.y = temp * N.y;
            tempPoint.z = temp * N.z;
            setPoint(ref upVec, upUnit.x - tempPoint.x, upUnit.y - tempPoint.y, upUnit.z - tempPoint.z);

            // Calculating v unit vector
            temp = Math.Sqrt(Math.Pow(upVec.x, 2) + Math.Pow(upVec.y, 2) + Math.Pow(upVec.z, 2));
            setPoint(ref v, upVec.x / temp, upVec.y / temp, upVec.z / temp);

            // Calculating u unit vector
            tempPoint.x = (v.y * N.z) - (N.y * v.z);
            tempPoint.y = (v.z * N.x) - (N.z * v.x);
            tempPoint.z = (v.x * N.y) - (N.x * v.y);
            setPoint(ref u, tempPoint.x, tempPoint.y, tempPoint.z);

            // Calculating CW (Center of Window)
            setPoint(ref CW, (windowUmax + windowUmin) / 2, (windowVmax + windowVmin) / 2, 0);

            // Calculating DOP (Direction of projection)
            setPoint(ref DOP, (CW.x - COP.x), (CW.y - COP.y), (CW.z - COP.z));

            // Creating transformation matrix 1, Translate VRP to the origin (0, 0, 0)WCS
            rx = VRP.x;
            ry = VRP.y;
            rz = VRP.z;
            setRowMatrix(ref T1, 0, 1, 0, 0, 0);
            setRowMatrix(ref T1, 1, 0, 1, 0, 0);
            setRowMatrix(ref T1, 2, 0, 0, 1, 0);
            setRowMatrix(ref T1, 3, -rx, -ry, -rz, 1);

            // Creating transformation matrix 2, rotate VCS such that u, v, N aligned with x, y, z axes
            setRowMatrix(ref T2, 0, u.x, v.x, N.x, 0);
            setRowMatrix(ref T2, 1, u.y, v.y, N.y, 0);
            setRowMatrix(ref T2, 2, u.z, v.z, N.z, 0);
            setRowMatrix(ref T2, 3, 0, 0, 0, 1);

            // Creating transformation matrix 3, shear such that the DOP become parallel to z-axis
            shx = -DOP.x / DOP.z;
            shy = -DOP.y / DOP.z;
            setRowMatrix(ref T3, 0, 1, 0, 0, 0);
            setRowMatrix(ref T3, 1, 0, 1, 0, 0);
            setRowMatrix(ref T3, 2, shx, shy, 1, 0);
            setRowMatrix(ref T3, 3, 0, 0, 0, 1);

            // Creating transformation matrix 4, Translate the FP to the origin
            dx = -CW.x;
            dy = -CW.y;
            dz = -FP;
            setRowMatrix(ref T4, 0, 1, 0, 0, 0);
            setRowMatrix(ref T4, 1, 0, 1, 0, 0);
            setRowMatrix(ref T4, 2, 0, 0, 1, 0);
            setRowMatrix(ref T4, 3, dx, dy, dz, 1);

            // Creating transformation matrix 5, scale so that BP will become -1, and window become -1, -1, 1, 1
            sx = 2 / (windowUmax - windowUmin);
            sy = 2 / (windowVmax - windowVmin);
            sz = 1 / (FP - BP);
            setRowMatrix(ref T5, 0, sx, 0, 0, 0);
            setRowMatrix(ref T5, 1, 0, sy, 0, 0);
            setRowMatrix(ref T5, 2, 0, 0, sz, 0);
            setRowMatrix(ref T5, 3, 0, 0, 0, 1);

            // Creating Pr1 matrix which is the result of all transformation matrix multiplication
            Pr1 = matrixMultiplication(matrixMultiplication(matrixMultiplication(matrixMultiplication(T1, T2), T3), T4), T5);

            // Creating Pr2 which is matrix for view projection
            setRowMatrix(ref Pr2, 0, 1, 0, 0, 0);
            setRowMatrix(ref Pr2, 1, 0, 1, 0, 0);
            setRowMatrix(ref Pr2, 2, 0, 0, 0, 0);
            setRowMatrix(ref Pr2, 3, 0, 0, 0, 1);

            // Assigning all vertices
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

            // Assigning all edges
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

            // Creating World transformation matrix
            setRowMatrix(ref Wt, 0, 1, 0, 0, 0);
            setRowMatrix(ref Wt, 1, 0, 1, 0, 0);
            setRowMatrix(ref Wt, 2, 0, 0, 1, 0);
            setRowMatrix(ref Wt, 3, 0, 0, 0, 1);

            // Creating view transformation matrix
            Vt = matrixMultiplication(Pr1, Pr2);

            // Creating screen transformation matrix
            setRowMatrix(ref St, 0, 50, 0, 0, 0);
            setRowMatrix(ref St, 1, 0, -50, 0, 0);
            setRowMatrix(ref St, 2, 0, 0, 0, 0);
            setRowMatrix(ref St, 3, 200, 200, 0, 1);

            // Multiply all point with Wt, Vt, and St
            for (int i = 0; i < 10; i++)
            {
                VW[i] = multiplyMatrix(V[i], Wt);
                VV[i] = multiplyMatrix(VW[i], Vt);
                VS[i] = multiplyMatrix(VV[i], St);
            }
            
            // Debug
            debugTextBox.Text = "";
            debugTextBox.AppendText("Viewing parameters:" + Environment.NewLine);
            debugTextBox.AppendText("1. VRP = (" + VRP.x.ToString() + ", " + VRP.y.ToString() + ", " + VRP.z.ToString() + ")" + Environment.NewLine);
            debugTextBox.AppendText("2. VPN = (" + VPN.x.ToString() + "   " + VPN.y.ToString() + "   " + VPN.z.ToString() + ")ᵀ" + Environment.NewLine);
            debugTextBox.AppendText("3. VUP = (" + VUP.x.ToString() + "   " + VUP.y.ToString() + "   " + VUP.z.ToString() + ")ᵀ" + Environment.NewLine);
            debugTextBox.AppendText("4. COP = (" + COP.x.ToString() + ", " + COP.y.ToString() + ", " + COP.z.ToString() + ")" + Environment.NewLine);
            debugTextBox.AppendText("5. Window = (" + windowUmin.ToString() + ", " + windowVmin.ToString() + ", " + windowUmax.ToString() + ", " + windowVmax.ToString() + ")" + Environment.NewLine);
            debugTextBox.AppendText("6. Projection type = Parallel" + Environment.NewLine);
            debugTextBox.AppendText("7. Front plane = " + FP.ToString() + ", " + "Back plane = " + BP.ToString() + Environment.NewLine);
            debugTextBox.AppendText(Environment.NewLine);

            debugTextBox.AppendText("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            debugTextBox.AppendText(Environment.NewLine);
            debugTextBox.AppendText("Derived paremeters" + Environment.NewLine);
            debugTextBox.AppendText("1. N = (" + N.x.ToString() + "   " + N.y.ToString() + "   " + N.z.ToString() + ")ᵀ" + Environment.NewLine);
            debugTextBox.AppendText("2. upUnit = (" + upUnit.x.ToString() + "   " + upUnit.y.ToString() + "   " + upUnit.z.ToString() + ")ᵀ" + Environment.NewLine);
            debugTextBox.AppendText("3. upVec = (" + upVec.x.ToString() + "   " + upVec.y.ToString() + "   " + upVec.z.ToString() + ")ᵀ" + Environment.NewLine);
            debugTextBox.AppendText("4. v = (" + v.x.ToString() + "   " + v.y.ToString() + "   " + v.z.ToString() + ")ᵀ" + Environment.NewLine);
            debugTextBox.AppendText("5. u = (" + u.x.ToString() + "   " + u.y.ToString() + "   " + u.z.ToString() + ")ᵀ" + Environment.NewLine);
            debugTextBox.AppendText("6. DOP = (" + DOP.x.ToString() + "   " + DOP.y.ToString() + "   " + DOP.z.ToString() + ")ᵀ" + Environment.NewLine);
            debugTextBox.AppendText("7. CW = (" + CW.x.ToString() + ", " + CW.y.ToString() + ", " + CW.z.ToString() + ")" + Environment.NewLine);
            debugTextBox.AppendText(Environment.NewLine);

            debugTextBox.AppendText("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            debugTextBox.AppendText(Environment.NewLine);

            debugTextBox.AppendText("Points:" + Environment.NewLine);
            for (int i = 0; i < 10; i++)
            {
                debugTextBox.AppendText(i + " => " + "(" + VV[i].x + ", " + VV[i].y + ", " + VV[i].z + ")" + Environment.NewLine);
            }

            // Draw object on the screen
            draw();
        }

        private void defaultSettingsBtn_Click(object sender, EventArgs e)
        {
            VRPxTextBox.Text = "0";
            VRPyTextBox.Text = "0";
            VRPzTextBox.Text = "0";

            VPNxTextBox.Text = "0";
            VPNyTextBox.Text = "0";
            VPNzTextBox.Text = "1";

            VUPxTextBox.Text = "0";
            VUPyTextBox.Text = "1";
            VUPzTextBox.Text = "0";

            COPxTextBox.Text = "0";
            COPyTextBox.Text = "0";
            COPzTextBox.Text = "4";

            windowUminTextBox.Text = "-2";
            windowVminTextBox.Text = "-2";
            windowUmaxTextBox.Text = "2";
            windowVmaxTextBox.Text = "2";

            FPTextBox.Text = "2";
            BPTextBox.Text = "-2";
        }
        #endregion
    }

}
