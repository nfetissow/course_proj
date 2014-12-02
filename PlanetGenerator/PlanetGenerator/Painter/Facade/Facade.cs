using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlanetGenerator.SphereBuilder;

namespace STLParserProject
{
    // капец а не класс
    class Facade
    {
//private ChessBoard chessBoard;
        private List<Figure> figureList = new List<Figure>();
        private List<Camera> cameraList = new List<Camera>();
        private List<Lightning> lightningList = new List<Lightning>();

        private ImageBuffer imageBuffer;
        private List<Shadow> shadowList;
        private Canvas canvas;

        private ConcurrentQueue<Request> requests;
        private Thread facadeThread;

        private int prevCameraNum = 0;

        //private FiguresCopiesMaker copyMaker;
        private volatile bool isRun = true;
        public bool canTransform = false;
        private bool frameCalculating;
        //private int yGroups = 16;

        private TimeSpan seconds = DateTime.Now.TimeOfDay;
        private Label logger;

        public Facade (Canvas canvas, ConcurrentQueue<Request> queue, Label logger, Figure figure)
        {
            //ChessBoardBuilder boardBuilder = new ChessBoardBuilder();
            //figureList.Add(boardBuilder.getChessBoard());
            //chessBoard = figureList[0] as ChessBoard;
            //chessBoard.setFigureList(figureList);
            //chessBoard.setRequests(queue);

            PolyhedronMesh q = PlanetGenerator.PlanetGeneration.Plates.createPlanet(4, 3, 0.7, new PlanetGenerator.DoubleRandom());


            //figureList.Add(Figure.fromPolyhedronMesh
            //    (PlanetGenerator.PlanetGeneration.Plates.createPlanet(20, 5, 0.7, new PlanetGenerator.DoubleRandom()), 0, 0 , 0));//Polyhedron.getDualPolyhedron(Icosahedron.generateSubdividedIcosahedron(10))));
            ////figureList.Add(Figure.fromPolyhedronMesh(Polyhedron.getDualPolyhedron(Icosahedron.generateSubdividedIcosahedron(4))));
            figureList.Add(figure);
            cameraList.Add(new Camera(new Point3D(0, 0, 0), new Point3D(410, 840, 0),
                new Point3D(200, 400, 0)));//boardBuilder.getCamera());
            //cameraList.Add(new Camera(new Point3D(320, 320, 300), new Point3D(320, 320, 0),
            //    new Point3D(330, 320, 300)));
//             lightningList.Add(new GlobalLightning(new Point3D(-160, -160, 200), 1));
            //lightningList.Add(new LocalLightning(new Point3D(320, 320, 300),  new Point3D(320, 320, 0), new Point3D(330, 320, 300), 0.9));
            //cameraList.Add(lightningList.Last());
            //lightningList.Add(new LocalLightning(new Point3D(160, 160, 300), new Point3D(320, 320, 0), new Point3D(170, 150, 300), 0.3));
            //cameraList.Add(lightningList.Last());

            imageBuffer = new ImageBuffer(canvas.Width, canvas.Height);
//             shadowList = new List<Shadow>();
//             for (int i = 0; i < lightningList.Count; i++)
//                 shadowList.Add(new Shadow(lightningList[i], figureList, canvas.Width, canvas.Height));

            this.requests = queue;
            this.canvas = canvas;
            this.logger = logger;

            facadeThread = new Thread(this.FacadeThreadRun);
            facadeThread.Start();

            ScaleRequest request = new ScaleRequest(0, SCALE_SIGN.PLUS, 0);
            requests.Enqueue(request);
        }

        private void FacadeThreadRun()
        {
            Request request;
            while (isRun)
            {
                if (requests.TryDequeue(out request) == true && canTransform == true)
                {
                    double frameFPS = 1000 / tranformAll(request);
                    calculateFrameRate(true);
                }
                else
                {
                    Thread.Sleep(1);
                    if (canTransform == true)
                        calculateFrameRate(false);
                }
            }

            //fpsWriter.Close();
        }

        public void ThreadStop()
        {
            isRun = false;
        }

        public double tranformAll(Request request)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                int cameraNum = request.getCameraNum();
                if (cameraNum == -1)
                    cameraNum = prevCameraNum;
                Camera camera = cameraList[cameraNum];

                Matrix3D cameraTrasformMatrix;
                int succesfullDataCounter = 0;
                do
                {
                    if (request.getCameraNum() != -1 && request.getCameraNum() != cameraNum)
                        break;

                    if (request is AllocateObjectRequest)
                    {
                        //chessBoard.allocateCell(request as AllocateObjectRequest, cameraList[cameraNum]);
                        request = new ScaleRequest(request.getCameraNum(), SCALE_SIGN.MINUS, 0);
                    }
                    if (request is DeleteObjectRequest)
                    {
                        //if (chessBoard.removeSelectedFigure() == true)
                           // request = new ScaleRequest(request.getCameraNum(), SCALE_SIGN.MINUS, 0);
                    }

                    cameraTrasformMatrix = camera.getCameraTransformationMatrix(request);

                    if (camera.canTransform(cameraTrasformMatrix) != true)
                    {
                        if (--succesfullDataCounter == 0)
                            return 0;
                    }
                    else
                    {
                        camera.transformAroundCenter(cameraTrasformMatrix);
                        succesfullDataCounter++;
                    }
                }
                while (/*succesfullDataCounter<=5 &&*/ requests.TryDequeue(out request));

                prevCameraNum = cameraNum;

                while (canTransform == false)
                    Thread.Sleep(2);

                frameCalculating = true;
                imageBuffer.update(figureList, lightningList, camera);
                canvas.setBitmap(imageBuffer.getBitmap());
                frameCalculating = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            timer.Stop();
            return timer.ElapsedMilliseconds;
            
        }

        static int posY = 0, posX = 0;
        //public void addFigure(string fileName)
        //{
        //    canTransform = false;
        //    try
        //    {
        //        while (frameCalculating == true)
        //            Thread.Sleep(2);
        //        for (int i = 0; i < 5; i++)
        //            chessBoard.addFigure(fileName, cameraList);
        //        ScaleRequest request = new ScaleRequest(-1, SCALE_SIGN.PLUS, 0);
        //        requests.Enqueue(request);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    canTransform = true;

        //}

        private static int lastTick;
        private static int lastFrameRate;
        private static int frameRate;
        //StreamWriter fpsWriter = new StreamWriter("FPSData.txt");


        public void calculateFrameRate(bool inc)
        {
            //if (System.Environment.TickCount - lastTick >= 1000 && canTransform == true)
            //{
            //    lastFrameRate = frameRate;
            //    if (UIData.FPSWriteInFile == true)
            //        fpsWriter.WriteLine("{0, -10}{1, -10}", UIData.ThreadCounts, lastFrameRate);
            //    logger.Invoke(new Action(delegate() { logger.Text = "FPS: " + lastFrameRate; }));
            //    frameRate = 0;
            //    lastTick = System.Environment.TickCount;

            //}
            //if (inc)
            //    frameRate++;
        }

        public void lightningAmpChangedHandler(int lightningSourceNum, double newValue)
        {
            lightningList[lightningSourceNum].Amp = newValue;
        }

        public double lightningSelectedChangedHandler(int lightningSourceNum)
        {
            return lightningList[lightningSourceNum].Amp;
        }

    }
}
