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
        PolyhedronMesh planet;

        //---------------------------------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();

            setSettings();
            generatePlanet();
            UIData.ThreadCounts = 4;


        }

        //Mouse
        #region Mouse on scene
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
        #endregion
        
 


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



        //private void viewType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    requests = new ConcurrentQueue<Request>();
        //    canvas = new Canvas(this.pbScene);
        //    //facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(mesh, (double)this.numericUpDown4.Value, (double)this.numericUpDown5.Value, (double)this.numericUpDown6.Value));
        //    Polyhedron.determineColor set = Polyhedron.showPlates;
        //    switch (this.viewType.SelectedIndex)
        //    {
        //        case 0: set = Polyhedron.showElevation; break;
        //        case 1: set = Polyhedron.showPlates; break;
        //        case 2: set = Polyhedron.showBiomes; break;
        //    }
        //    facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(planet, (double)this.numericUpDown4.Value, (double)this.numericUpDown5.Value, (double)this.numericUpDown6.Value, set));
        //    facade.canTransform = true;
        //    requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
        //}

        private void setSettings()
        {
            this.rbHigh.Checked = true;
            this.rbBiomes.Checked = true;
            this.tbHeatLevel.Value = 0;
            this.tbOceanicRate.Value = 70;
            this.tbMoistureLevel.Value = 0;
            this.tbTectonicPlateCount.Value = 50;
            this.tbDetailLevel.Value = 20;
            this.tbDistorionLevel.Value = 90;
            this.tbTectonicPlateCount.Value = 50;
            UIData.tectonicPlateCount = 50;
            UIData.oceanicRate = 0.7;
            this.tbOceanicRate.Value = 70;
        }

        private void generatePlanet()
        {
            this.UseWaitCursor = true;

            DoubleRandom rng;
            if(this.tbSeed.Text == "")
            {
                rng = new DoubleRandom();
                int k = rng.Next();
                this.tbSeed.Text = k.ToString();
                rng = new DoubleRandom(k);
            }
            else
            {
                rng = new DoubleRandom(Int32.Parse(this.tbSeed.Text));
            }
            TriangleMesh mesh = Icosahedron.generateSubdividedIcosahedron(UIData.tesselationLevel);
            this.pbLoading.Value = 10;
            Distortion.distortAndRelaxMesh(mesh, UIData.distortionLevel, rng);
            this.pbLoading.Value = 20;
            PolyhedronMesh topology = Polyhedron.getDualPolyhedron(mesh);
            this.pbLoading.Value = 30;
            planet = PlanetGeneration.Planet.createPlanet(topology, UIData.tectonicPlateCount, UIData.oceanicRate, UIData.heatLevel, UIData.moistureLevel, 1000, rng);
            this.pbLoading.Value = 70;
            requests = new ConcurrentQueue<Request>();
            canvas = new Canvas(this.pbScene);

            Polyhedron.determineColor set = Polyhedron.showBiomes;
            if(UIData.surfaceDisplaySettings == "biomes")
            {
                set = Polyhedron.showBiomes;
            }
            else if(UIData.surfaceDisplaySettings == "height")
            {
                set = Polyhedron.showElevation;
            } 
            else if(UIData.surfaceDisplaySettings == "plates")
            {
                set = Polyhedron.showPlates;
            }

            facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(planet, set));
            this.pbLoading.Value = 90;
            facade.canTransform = true;
            requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
            this.pbLoading.Value = 100;


            this.UseWaitCursor = false;
        }

        #region UI events
        private void tbDetailLevel_ValueChanged(object sender, EventArgs e)
        {
            this.lbDetailLevel.Text = "Детализация (" + this.tbDetailLevel.Value.ToString() + ")";
            UIData.tesselationLevel = this.tbDetailLevel.Value;
        }

        private void tbDistorionLevel_ValueChanged(object sender, EventArgs e)
        {
            this.lbDistortionLevel.Text = "Уровень искажения (" + this.tbDistorionLevel.Value.ToString() + "%)";
            UIData.distortionLevel = ((double)this.tbDistorionLevel.Value) / 100;
        }

        private void tbTectonicPlateCount_ValueChanged(object sender, EventArgs e)
        {
            this.lbTectonicPlateCount.Text = "Количество плит (" + this.tbTectonicPlateCount.Value + ")";
            UIData.tectonicPlateCount = this.tbTectonicPlateCount.Value;
        }

        private void tbOceanicRate_ValueChanged(object sender, EventArgs e)
        {
            this.lbOceanicRate.Text = "Уровень воды (" + this.tbOceanicRate.Value + "%)";
            UIData.oceanicRate = ((double)this.tbOceanicRate.Value) / 100;
        }

        private void tbHeatLevel_ValueChanged(object sender, EventArgs e)
        {
            this.lbHeatLevel.Text = "Уровень тепла (" + this.tbHeatLevel.Value + "%)";
            UIData.heatLevel = ((double)this.tbHeatLevel.Value) / 100;
        }

        private void tbMoistureLevel_ValueChanged(object sender, EventArgs e)
        {
            this.lbMoistureLevel.Text = "Уровень влажности (" + this.tbMoistureLevel.Value + "%)";
            UIData.moistureLevel = ((double)this.tbMoistureLevel.Value) / 100;
        }

        private void rbBiomes_CheckedChanged(object sender, EventArgs e)
        {
            if(this.rbBiomes.Checked)
            {
                UIData.surfaceDisplaySettings = "biomes";
            }
        }

        private void rbHeightMap_CheckedChanged(object sender, EventArgs e)
        {
            if(this.rbHeightMap.Checked)
            {
                UIData.surfaceDisplaySettings = "height";
            }
        }

        private void rbPlates_CheckedChanged(object sender, EventArgs e)
        {
            if(this.rbPlates.Checked)
            {
                UIData.surfaceDisplaySettings = "plates";
            }
        }

        private void rbHigh_CheckedChanged(object sender, EventArgs e)
        {
            if(this.rbHigh.Checked)
            {
                this.tbDetailLevel.Value = 50;
                this.tbDistorionLevel.Value = 100;
            }
        }

        private void rbMedium_CheckedChanged(object sender, EventArgs e)
        {
            if(this.rbMedium.Checked)
            {
                this.tbDetailLevel.Value = 20;
                this.tbDistorionLevel.Value = 70;
            }
        }

        private void rbLow_CheckedChanged(object sender, EventArgs e)
        {
            if(this.rbLow.Checked)
            {
                this.tbDetailLevel.Value = 8;
                this.tbDistorionLevel.Value = 0;
            }
        }
        #endregion

        private void btGenerate_Click(object sender, EventArgs e)
        {
            generatePlanet();
        }
    }
}
