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

namespace Painter
{
    class Facade
    {
        private List<Figure> figureList = new List<Figure>();
        private List<Camera> cameraList = new List<Camera>();
        private List<Lightning> lightningList = new List<Lightning>();

        private ImageBuffer imageBuffer;
        private Canvas canvas;

        private ConcurrentQueue<Request> requests;
        private Thread facadeThread;

        private int prevCameraNum = 0;

        private volatile bool isRun = true;
        public bool canTransform = false;
        private bool frameCalculating;

        private TimeSpan seconds = DateTime.Now.TimeOfDay;
        private Label logger;

        public Facade (Canvas canvas, ConcurrentQueue<Request> queue, Label logger, Figure figure)
        {
           
            figureList.Add(figure);
            cameraList.Add(new Camera(new Point3D(0, 0, 0), new Point3D(410, 840, 0),
                new Point3D(410, 220, 0)));
         
            imageBuffer = new ImageBuffer(canvas.Width, canvas.Height);
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
                    double frameFPS = tranformAll(request);
                    calculateFrameRate(true);
                }
                else
                {
                    Thread.Sleep(1);
                    if (canTransform == true)
                        calculateFrameRate(false);
                }
            }
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
                        request = new ScaleRequest(request.getCameraNum(), SCALE_SIGN.MINUS, 0);
                    }
                    if (request is DeleteObjectRequest)
                    {
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
                while (requests.TryDequeue(out request));

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


        public void calculateFrameRate(bool inc)
        {
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
