using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Input;

namespace draw_my_thing
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        Stack<Bitmap> states = new Stack<Bitmap>();
        Boolean cursorMoving = false, ctrlPressed = false, zPressed = false, isFilling = false;
        Pen cursorPen;
        int cursorX = -1, cursorY = -1;
        Bitmap drawing;
        int brushWidth;

        public Form1()
        {
            InitializeComponent();
            cursorPen = new Pen(Color.Black, 7);
            brushWidth = 7;
            Brush brush = new SolidBrush(Color.Black);
            cursorPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            cursorPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            drawing = new Bitmap(1443, 682, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            drawing.MakeTransparent();
            graphics = Graphics.FromImage(drawing);
            graphics.Clear(Color.White);
            canvas.Image = drawing;
            canvas.Refresh();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void fillArea(int x, int y, Color color)
        {
            if (x > 1 && y>1 && x<1443 && y<682 && drawing.GetPixel(x, y).Name == color.Name) 
            {
                drawing.SetPixel(x, y, cursorPen.Color);
                if (x >= 0 && x-1 < Width && y >= 0)
                {
                    if (drawing.GetPixel(x + 1, y).Name == color.Name)
                        fillArea(x + 1, y, color);
                    if (drawing.GetPixel(x, y + 1).Name == color.Name)
                        fillArea(x, y + 1, color);
                    if (drawing.GetPixel(x - 1, y).Name == color.Name)
                        fillArea(x - 1, y, color);
                    if (drawing.GetPixel(x, y - 1).Name == color.Name)
                        fillArea(x, y - 1, color);
                }
            }

        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

            }
            else if (!isFilling)
            {
                states.Push(drawing.Clone(new Rectangle(0, 0, 1443, 682), drawing.PixelFormat));
                cursorMoving = true;
                cursorX = e.X;
                cursorY = e.Y;

                if (cursorX != -1 && cursorY != -1 && cursorMoving)
                    graphics.DrawLine(cursorPen, new Point(cursorX - 1, cursorY), e.Location);
                canvas.Refresh();
            }
            else if (isFilling)
            {
                fillArea(e.X, e.Y, drawing.GetPixel(e.X, e.Y));
                canvas.Refresh();
            }
        }


        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            cursorMoving = false;
            cursorX = -1;
            cursorY = -1;
            canvas.Refresh();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (cursorX != -1 && cursorY != -1 && cursorMoving)
            {
                graphics.DrawLine(cursorPen, new Point(cursorX, cursorY), e.Location);
                cursorX = e.X;
                cursorY = e.Y;
                canvas.Refresh();
            }
        }


        private void thickest_Click(object sender, EventArgs e)
        {
            cursorPen.Width = 30;
            brushWidth = 30;
            isFilling = false;
        }

        private void thicker_Click(object sender, EventArgs e)
        {
            cursorPen.Width = 15;
            brushWidth = 15;
            isFilling = false;
        }

        private void normal_Click(object sender, EventArgs e)
        {
            cursorPen.Width = 11;
            brushWidth = 11;
            isFilling = false;
        }

        private void thinner_Click(object sender, EventArgs e)
        {
            cursorPen.Width = 7;
            brushWidth = 7;
            isFilling = false;
        }

        private void thinest_Click(object sender, EventArgs e)
        {
            cursorPen.Width = 3;
            brushWidth = 3;
            isFilling = false;
        }

        private void clear_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            canvas.Refresh();
        }

        private void undo_Click(object sender, EventArgs e)
        {
            canvas.Invalidate();
            if (states.Count > 0) graphics.DrawImage(states.Pop(), 0, 0);
        }

        private void canvas_MouseEnter(object sender, EventArgs e)
        {
            string cursorPath = "\\Cursor\\" + brushWidth + "pixel.ico";
            this.Cursor = new Cursor(Application.StartupPath + cursorPath);
        }

        private void canvas_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void fill_Click(object sender, EventArgs e)
        {
            isFilling = true;
            //this.Cursor = new Cursor(Application.StartupPath + "\\paint bucket.ico");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control) ctrlPressed = true;
            if (e.KeyValue == (char)Keys.Z) zPressed = true;

            if (ctrlPressed && zPressed) undo_Click(sender, e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Z) zPressed = false;
            if ((ModifierKeys & Keys.Control) == Keys.Control) ctrlPressed = false;
        }

        private void black_Click(object sender, EventArgs e)
        {
            PictureBox color = (PictureBox)sender;
            cursorPen.Color = color.BackColor;
        }
    }
}
