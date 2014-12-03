using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlanetGenerator.Painter;
using PlanetGenerator.SphereBuilder;
using STLParserProject;
namespace PlanetGenerator
{
    public partial class Form1 : Form
    {
        //---------------------------------------------------------------------------------------
        const double rotateAngle = 1;
        const double offsetD = 1;
        const double scaleKoef = 0.05;
        Canvas canvas;
        Facade facade;
        ConcurrentQueue<Request> requests;
        int cameraNum = 0;

        //---------------------------------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
            #region old
            //sc = new Scene();
            //TriangleMesh mesh = Icosahedron.generateSubdividedIcosahedron(20);
            //Random rng = new Random();
            //Distortion.distortMesh(mesh, 20, rng);
            //Distortion.relaxMesh(mesh, 0.5);

            //////sc.AddFigure(Polyhedron.getDualPolyhedron(Icosahedron.generateSubdividedIcosahedron(20)));
            //////sc.AddFigure(Icosahedron.generateSubdividedIcosahedron(4));
            //////sc.AddFigure(Polyhedron.getDualPolyhedron(Icosahedron.generateIcosahedron()));
            //sc.AddFigure(Polyhedron.getDualPolyhedron(mesh));
            //////sc.AddFigure(mesh);
            //sc.output = pbScene.CreateGraphics();
            #endregion 

            PolyhedronMesh meshss;// = Polyhedron.getDualPolyhedron(Icosahedron.generateSubdividedIcosahedron(20));
            //meshss = PlanetGeneration.Plates.createPlanet(1, 1, 0.7, new DoubleRandom());


            
            UIData.ThreadCounts = 4;
            requests = new ConcurrentQueue<Request>();
            canvas = new Canvas(this.pbScene);

            //PolyhedronMesh mesh = Polyhedron.getDualPolyhedron(Icosahedron.generateSubdividedIcosahedron(1));

            PolyhedronMesh q = PlanetGenerator.PlanetGeneration.Plates.createPlanet(8, 5, 0.5, new PlanetGenerator.DoubleRandom());
            facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(q, (double)this.numericUpDown4.Value, (double)this.numericUpDown5.Value, (double)this.numericUpDown6.Value, (int)this.nudColor1.Value, (int)this.nudColor2.Value));
            facade.canTransform = true;
            requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
            int b = 0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //sc.draw();
        }

        //Mouse

        bool isDown = false;
        Point3D prev;
        Point3D selectedPoint;

        private void pbScene_MouseDown(object sender, MouseEventArgs e)
        {
            isDown = true;
            prev = new Point3D(e.X, e.Y, 0);
            this.pbScene.Select();
        }

        private void pbScene_MouseUp(object sender, MouseEventArgs e)
        {
            if (prev == null)
                return;
            selectedPoint = new Point3D(e.X, e.Y, 0);
            if (prev.Equals(selectedPoint))
            {
                selectedPoint.Y = pbScene.Height - selectedPoint.Y - 1;
                requests.Enqueue(new AllocateObjectRequest(cameraNum, selectedPoint));
            }
            isDown = false;
        }

        private void pbScene_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = "Cursor: x=" + e.X + "\n            y=" + (pbScene.Height - e.Y - 1);
            if (isDown)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point3D next = new Point3D(e.X, e.Y, 0);
                    RotateRequest requestRotateRight = new RotateRequest(cameraNum, (next.X - prev.X), ROTATE_AXIS.RIGHT);
                    RotateRequest requestRotateDown = new RotateRequest(cameraNum, (next.Y - prev.Y), ROTATE_AXIS.DOWN);
                    //Matrix3D bufMatrix = data.getMatrix();

                    //bufMatrix *= data2.getMatrix();

                    //data.setMatrix(bufMatrix);

                    requests.Enqueue(requestRotateRight);
                    requests.Enqueue(requestRotateDown);
                    //prev = null;
                    prev = next;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TriangleMesh traMesh = Icosahedron.generateSubdividedIcosahedron((int)numericUpDown1.Value);
            Distortion.distortMesh(traMesh, (int)numericUpDown2.Value, new Random());
            if(checkBox1.Checked)
            {
                Distortion.relaxMesh(traMesh,(double) numericUpDown3.Value);
            }
            

            //PolyhedronMesh mesh = Polyhedron.getDualPolyhedron(traMesh);
            PolyhedronMesh q = PlanetGenerator.PlanetGeneration.Plates.createPlanet(8, 5, 0.5, new PlanetGenerator.DoubleRandom());
            
            requests = new ConcurrentQueue<Request>();
            canvas = new Canvas(this.pbScene);
            //facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(mesh, (double)this.numericUpDown4.Value, (double)this.numericUpDown5.Value, (double)this.numericUpDown6.Value));
            facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(q, (double)this.numericUpDown4.Value, (double)this.numericUpDown5.Value, (double)this.numericUpDown6.Value, (int)this.nudColor1.Value, (int)this.nudColor2.Value));
            facade.canTransform = true;
            requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Request request = null;
            switch (e.KeyCode)
            {
                case (Keys.A):
                    request = new RotateRequest(cameraNum, rotateAngle, ROTATE_AXIS.LEFT);
                    break;
                case (Keys.W):
                    request = new RotateRequest(cameraNum, rotateAngle, ROTATE_AXIS.UP);
                    break;
                case (Keys.D):
                    request = new RotateRequest(cameraNum, rotateAngle, ROTATE_AXIS.RIGHT);
                    break;
                case (Keys.S):
                    request = new RotateRequest(cameraNum, rotateAngle, ROTATE_AXIS.DOWN);
                    break;
                case (Keys.Up):
                    request = new OffsetRequest(cameraNum, OFFSET_DIRECTION.UP, 1);
                    break;
                case (Keys.Down):
                    request = new OffsetRequest(cameraNum, OFFSET_DIRECTION.DOWN, 1);
                    break;
                case (Keys.Left):
                    request = new OffsetRequest(cameraNum, OFFSET_DIRECTION.LEFT, 1);
                    break;
                case (Keys.Right):
                    request = new OffsetRequest(cameraNum, OFFSET_DIRECTION.RIGHT, 1);
                    break;
                case (Keys.Subtract):
                    request = new ScaleRequest(cameraNum, SCALE_SIGN.MINUS, scaleKoef);
                    break;
                case (Keys.Add):
                    request = new ScaleRequest(cameraNum, SCALE_SIGN.PLUS, scaleKoef);
                    break;
                case (Keys.Delete):
                    request = new DeleteObjectRequest(cameraNum);
                    break;
            }

            if (request != null)
                requests.Enqueue(request);
        }

        private void pbScene_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Request request = null;
            switch (e.KeyCode)
            {
                case (Keys.A):
                    request = new RotateRequest(cameraNum, rotateAngle, ROTATE_AXIS.LEFT);
                    break;
                case (Keys.W):
                    request = new RotateRequest(cameraNum, rotateAngle, ROTATE_AXIS.UP);
                    break;
                case (Keys.D):
                    request = new RotateRequest(cameraNum, rotateAngle, ROTATE_AXIS.RIGHT);
                    break;
                case (Keys.S):
                    request = new RotateRequest(cameraNum, rotateAngle, ROTATE_AXIS.DOWN);
                    break;
                case (Keys.Up):
                    request = new OffsetRequest(cameraNum, OFFSET_DIRECTION.UP, 1);
                    break;
                case (Keys.Down):
                    request = new OffsetRequest(cameraNum, OFFSET_DIRECTION.DOWN, 1);
                    break;
                case (Keys.Left):
                    request = new OffsetRequest(cameraNum, OFFSET_DIRECTION.LEFT, 1);
                    break;
                case (Keys.Right):
                    request = new OffsetRequest(cameraNum, OFFSET_DIRECTION.RIGHT, 1);
                    break;
                case (Keys.Y):
                    request = new ScaleRequest(cameraNum, SCALE_SIGN.MINUS, scaleKoef);
                    break;
                case (Keys.T):
                    request = new ScaleRequest(cameraNum, SCALE_SIGN.PLUS, scaleKoef);
                    break;
                case (Keys.Delete):
                    request = new DeleteObjectRequest(cameraNum);
                    break;
            }

            if (request != null)
                requests.Enqueue(request);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TriangleMesh traMesh = Icosahedron.generateSubdividedIcosahedron((int)numericUpDown1.Value);
            Distortion.distortMesh(traMesh, (int)numericUpDown2.Value, new Random());
            if (checkBox1.Checked)
            {
                Distortion.relaxMesh(traMesh, (double)numericUpDown3.Value);
            }


            //PolyhedronMesh mesh = Polyhedron.getDualPolyhedron(traMesh);
            requests = new ConcurrentQueue<Request>();
            canvas = new Canvas(this.pbScene);
            facade = new Facade(canvas, requests, label1, Figure.fromTriangleMesh(traMesh));
            facade.canTransform = true;
            requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
        }
    }
}
