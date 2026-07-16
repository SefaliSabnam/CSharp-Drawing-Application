using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawingApp
{

    public partial class frmDrawing : Form
    {
        private Point startPoint;
        private Point endPoint;
        private bool isDrawing = false;

        private Color currentColor = Color.Red;
        private DashStyle currentDashStyle = DashStyle.Solid;

        private enum ShapeType
        {
            Line,
            Rectangle,
            Circle
        }

        private ShapeType currentShape = ShapeType.Line;
        private enum BrushType
        {
            Solid,
            Hatch,
            LinearGradient,
            Texture,
            PathGradient
        }

        private BrushType currentBrush = BrushType.Solid;
        private class DrawingShape
        {
            public ShapeType Shape;
            public Point Start;
            public Point End;
            public Color Color;
            public DashStyle DashStyle;
            public BrushType Brush;
        }

        private List<DrawingShape> shapes = new List<DrawingShape>();
        public frmDrawing()
        {
            InitializeComponent();

            this.MouseDown += frmDrawing_MouseDown;
            this.MouseMove += frmDrawing_MouseMove;
            this.MouseUp += frmDrawing_MouseUp;
            this.Paint += frmDrawing_Paint;

            this.DoubleBuffered = true;
        }
        

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void frmDrawing_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                endPoint = e.Location;
                isDrawing = true;
            }
        }

        private void frmDrawing_MouseMove(object sender, MouseEventArgs e)
        {

            if (isDrawing)
            {
                endPoint = e.Location;
                Invalidate();
            }
        }

        private void frmDrawing_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                endPoint = e.Location;
                isDrawing = false;

                shapes.Add(new DrawingShape
                {
                    Shape = currentShape,
                    Start = startPoint,
                    End = endPoint,
                    Color = currentColor,
                    DashStyle = currentDashStyle,
                    Brush = currentBrush
                });

                Invalidate();
            }
        }



        private void frmDrawing_Paint(object sender, PaintEventArgs e)
        {
            foreach (DrawingShape shape in shapes)
            {
                using (Pen pen = new Pen(shape.Color, 2))
                {
                    pen.DashStyle = shape.DashStyle;

                    Rectangle rect = new Rectangle(
                        Math.Min(shape.Start.X, shape.End.X),
                        Math.Min(shape.Start.Y, shape.End.Y),
                        Math.Abs(shape.End.X - shape.Start.X),
                        Math.Abs(shape.End.Y - shape.Start.Y));

                    Brush brush = null;

                    switch (shape.Brush)
                    {
                        case BrushType.Solid:
                            brush = new SolidBrush(shape.Color);
                            break;

                        case BrushType.Hatch:
                            brush = new HatchBrush(HatchStyle.Cross, shape.Color, Color.White);
                            break;

                        case BrushType.LinearGradient:
                            brush = new LinearGradientBrush(rect, shape.Color, Color.White, 45);
                            break;

                        case BrushType.Texture:
                            Bitmap bmp = new Bitmap(10, 10);
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.Clear(Color.White);
                                g.FillEllipse(new SolidBrush(shape.Color), 2, 2, 6, 6);
                            }
                            brush = new TextureBrush(bmp);
                            break;

                        case BrushType.PathGradient:
                            GraphicsPath path = new GraphicsPath();
                            path.AddEllipse(rect);

                            PathGradientBrush pgb = new PathGradientBrush(path);
                            pgb.CenterColor = shape.Color;
                            pgb.SurroundColors = new Color[] { Color.White };

                            brush = pgb;
                            break;
                    }

                    switch (shape.Shape)
                    {
                        case ShapeType.Line:
                            e.Graphics.DrawLine(pen, shape.Start, shape.End);
                            break;

                        case ShapeType.Rectangle:
                            if (brush != null)
                                e.Graphics.FillRectangle(brush, rect);

                            e.Graphics.DrawRectangle(pen, rect);
                            break;

                        case ShapeType.Circle:
                            if (brush != null)
                                e.Graphics.FillEllipse(brush, rect);

                            e.Graphics.DrawEllipse(pen, rect);
                            break;
                    }

                    if (brush != null)
                        brush.Dispose();
                }
            }

            // Draw the shape currently being dragged
            if (isDrawing)
            {
                using (Pen pen = new Pen(currentColor, 2))
                {
                    pen.DashStyle = currentDashStyle;

                    Rectangle rect = new Rectangle(
                        Math.Min(startPoint.X, endPoint.X),
                        Math.Min(startPoint.Y, endPoint.Y),
                        Math.Abs(endPoint.X - startPoint.X),
                        Math.Abs(endPoint.Y - startPoint.Y));

                    switch (currentShape)
                    {
                        case ShapeType.Line:
                            e.Graphics.DrawLine(pen, startPoint, endPoint);
                            break;

                        case ShapeType.Rectangle:
                            e.Graphics.DrawRectangle(pen, rect);
                            break;

                        case ShapeType.Circle:
                            e.Graphics.DrawEllipse(pen, rect);
                            break;
                    }
                }
            }
        }


        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentShape = ShapeType.Line;
            lblStatus.Text = "Status : Line";


        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentShape = ShapeType.Rectangle;
            lblStatus.Text = "Status : Rectangle";
        
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentShape = ShapeType.Circle;
            lblStatus.Text = "Status : Circle";
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentColor = Color.Red;
            
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentColor = Color.Green;
           
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentColor = Color.Blue;
            
        }

        private void solidBrushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBrush = BrushType.Solid;
        }

        private void hatchBrushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBrush = BrushType.Hatch;
        }

        private void linearGradientBrushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBrush = BrushType.LinearGradient;
        }

        private void textureBrushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBrush = BrushType.Texture;
        }

        private void pathGradientBrushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentBrush = BrushType.PathGradient;
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnRed_Click(object sender, EventArgs e)
        {
            redToolStripMenuItem_Click(sender, e);
        }

        private void btnGreen_Click(object sender, EventArgs e)
        {
            greenToolStripMenuItem_Click(sender, e);
        }

        private void btnBlue_Click(object sender, EventArgs e)
        {
            blueToolStripMenuItem_Click(sender, e);
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            lineToolStripMenuItem_Click(sender, e);
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            rectangleToolStripMenuItem_Click(sender, e);
        }

        private void btnCircle_Click(object sender, EventArgs e)
        {
            circleToolStripMenuItem_Click(sender, e);
        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentDashStyle = DashStyle.Solid;
        }

        private void dashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentDashStyle = DashStyle.Dash;
        }

        private void dotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentDashStyle = DashStyle.Dot;
        }

        private void dashDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentDashStyle = DashStyle.DashDot;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shapes.Clear();

            Invalidate();

            lblStatus.Text = "Status : Canvas Cleared";
        
    }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
            saveFileDialog1.Filter = "PNG Image|*.png";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

                this.DrawToBitmap(bmp, this.ClientRectangle);

                bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);

                MessageBox.Show("Drawing saved successfully!", "Success");

                lblStatus.Text = "Status : Image Saved";
            }
        }
    }
    }
    




