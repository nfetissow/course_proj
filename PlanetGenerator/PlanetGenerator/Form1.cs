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
using Painter;
using System.Threading;

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


        private void setSettings()
        {
            this.rbMedium.Checked = true;
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
        interface IFormState
        {
            int progress { get; set; }
        }
        public int progressInBar
        {
            get { return this.pbLoading.Value; }
            set { this.pbLoading.Value = value; }
        }
        private void generatePlanet()
        {
            this.UseWaitCursor = true;

            DoubleRandom rng;
            if(this.tbSeed.Text == "")
            {
                rng = new DoubleRandom();
                int k = rng.Next();
                this.BeginInvoke(new Action(() => { this.tbUsedSeed.Text = k.ToString(); }));
                rng = new DoubleRandom(k);
            }
            else
            {
                int seed;
                bool success = Int32.TryParse(this.tbSeed.Text, out seed);
                if(success)
                {
                    rng = new DoubleRandom(seed);
                    this.BeginInvoke(new Action(() => { this.tbUsedSeed.Text = seed.ToString(); }));

                }
                else
                {
                    this.BeginInvoke(new Action(() => { this.tbSeed.Text = ""; }));
                    
                    rng = new DoubleRandom();
                    int k = rng.Next();
                    rng = new DoubleRandom(k);
                    this.BeginInvoke(new Action(() => { this.tbUsedSeed.Text = k.ToString(); }));

                }
            }
            TriangleMesh mesh = Icosahedron.generateSubdividedIcosahedron(UIData.tesselationLevel);
            this.BeginInvoke(new Action(() => { this.pbLoading.Value = 10; }));
            Distortion.distortAndRelaxMesh(mesh, UIData.distortionLevel, rng);
            this.BeginInvoke(new Action(() => { this.pbLoading.Value = 20; }));
            PolyhedronMesh topology = Polyhedron.getDualPolyhedron(mesh);
            this.BeginInvoke(new Action(() => { this.pbLoading.Value = 30; }));
            planet = PlanetGeneration.Planet.createPlanet(topology, UIData.tectonicPlateCount, UIData.oceanicRate, UIData.heatLevel, UIData.moistureLevel, 1000, rng);
            this.BeginInvoke(new Action(() => { this.pbLoading.Value = 70; }));
            
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

            requests = new ConcurrentQueue<Request>();
            canvas = new Canvas(this.pbScene);


            facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(planet, set));
            this.BeginInvoke(new Action(() => { this.pbLoading.Value = 90; }));
                       
            facade.canTransform = true;
            requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
            this.BeginInvoke(new Action(() => { this.pbLoading.Value = 100; }));


            this.UseWaitCursor = false;
            this.BeginInvoke(new Action(()=> {btGenerate.Enabled = true;}));
            this.BeginInvoke(new Action(() => { this.pbScene.Select(); }));
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
                this.UseWaitCursor = true;
                this.BeginInvoke(new Action(() => { this.pbLoading.Value = 0; }));
                requests = new ConcurrentQueue<Request>();
                canvas = new Canvas(this.pbScene);

                UIData.surfaceDisplaySettings = "biomes";
                facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(planet, Polyhedron.showBiomes));
                facade.canTransform = true;
                requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
                this.BeginInvoke(new Action(() => { this.pbLoading.Value = 100; }));
                this.UseWaitCursor = false;
                this.BeginInvoke(new Action(() => { btGenerate.Enabled = true; }));
                this.tbLegend.Lines = biomesLegend;
            }
        }

        private void rbHeightMap_CheckedChanged(object sender, EventArgs e)
        {
            if(this.rbHeightMap.Checked)
            {
                this.UseWaitCursor = true;
                this.BeginInvoke(new Action(() => { this.pbLoading.Value = 0; }));

                requests = new ConcurrentQueue<Request>();
                canvas = new Canvas(this.pbScene);
                UIData.surfaceDisplaySettings = "height";
                facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(planet, Polyhedron.showElevation));
                facade.canTransform = true;
                requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
                this.BeginInvoke(new Action(() => { this.pbLoading.Value = 100; }));
                this.UseWaitCursor = false;
                this.BeginInvoke(new Action(() => { btGenerate.Enabled = true; }));
                this.tbLegend.Lines = elevationLegend;
            }
        }

        private void rbPlates_CheckedChanged(object sender, EventArgs e)
        {
            if(this.rbPlates.Checked)
            {
                this.UseWaitCursor = true;
                this.BeginInvoke(new Action(() => { this.pbLoading.Value = 0; }));

                requests = new ConcurrentQueue<Request>();
                canvas = new Canvas(this.pbScene);
                               
                UIData.surfaceDisplaySettings = "plates";
                facade = new Facade(canvas, requests, label1, Figure.fromPolyhedronMesh(planet, Polyhedron.showPlates));
                facade.canTransform = true;
                requests.Enqueue(new ScaleRequest(-1, SCALE_SIGN.MINUS, 0));
                this.BeginInvoke(new Action(() => { this.pbLoading.Value = 100; }));
                this.UseWaitCursor = false;
                this.BeginInvoke(new Action(() => { btGenerate.Enabled = true; }));
                this.tbLegend.Lines = platesLegend;
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
            this.btGenerate.Enabled = false;
            new Thread(() => { generatePlanet(); }).Start();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            generatePlanet();
            this.tbLegend.Lines = biomesLegend;
        }

        private void btHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Данная программа позволяет генерировать модели планет, схожих по своим характеристикам с Землей.\n"
                + "Для того, чтобы начать процесс генерации, нажмите кнопку \"Генерировать\".\n"
                + "Вы можете менять уровень детализации планеты, выбирая соответствующие пункты в окне \"Настройки\".\n"
                + "В этом же окне вы можете выбрать способ отображения модели: карту высот, литосферные плиты или природные зоны.\n"
                + "В окне \"Дополнительные настройки\" Вы можете указать параметры генерируемой модели.\n"
                + "Вы можете использовать мышь для вращения модели планеты. Для этого необходимо двигать мышью по окну вывода с зажатой ЛКМ.\n"
                + "Вы можете также использовать клавиши W, A, S, D для вращения планеты.\n"
                + "-----------------------------------------------------------\n"
                + "Автор программы - Фетисов Н. М.", "Справка");
        }
        string[] biomesLegend = {   "Условные обозначения", 
                                    "",
                                    "Желтым цветом выделены зоны с высоким уровнем тепла и низкой влажностью",
                                    "Темно-зеленым цветом выделены зоны с высокой влажностью и теплом", 
                                    "Светло-зеленым цветом выделены зоны с низким уровнем тепла" ,
                                    "Белым цветом выделены зоны с температурой меньше 0",
                                    "Серые цвета обозначают горы и скалы"};
        string[] elevationLegend = {    "Условные обозначения",
                                        "",
                                        "Чем темнее синий, тем глубже океан",
                                        "Чем темнее коричневый, тем выше поверхность"
                                   };
        string[] platesLegend = {
                                        "Условные обозначения",
                                        "",
                                        "Плиты обозначены разными цветами"
                                };

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.facade.ThreadStop();
        }
    }
}
