// ===============================
// AUTHOR           : Nick Sienkiewicz 
// CREATE DATE      : 5/11/2019
// PURPOSE          : Create a kinect based real-time target game 
// ===============================
// Change History   : 
// 5/11/2019 - Documentation and publish code
//==================================

namespace Microsoft.Samples.Kinect.DepthBasics
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Diagnostics;

    // Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        // Sensor Variables
        private KinectSensor sensor; // Active Kinect sensor        
        private WriteableBitmap colorBitmap; // Bitmap that will hold color information
        private DepthImagePixel[] depthPixels; // Intermediate storage for the depth data received from the camera
        private byte[] colorPixels; // Intermediate storage for the depth data converted to color

        // Calibration Variables
        private DepthImagePixel[] depthPixelsCalibration; // Calibration storage for the depth data received from the camera
        private bool calibrationNeeded = false; // Calibration flag for the depth data received from the camera
        private bool calibrated = false;
        private int minPixelDepth = 0;
        private int maxPixelDepth = 0;
        private int avgPixelDepth = 0;
        private int wallDistThresh = 0;

        // Blob Variables
        private struct Blob
        {
            public Point averagePoint;
            public int totalPoints;
        }
        private int blobDistThresh = 50000;
        private List<Blob> blobs = new List<Blob>();

        // Game Variables
        private int numTargets = 3;
        private List<Target> targets = new List<Target>();
        private int score = 0;
        private int highScore = 0;
        private DispatcherTimer gameThreadTimer = new DispatcherTimer();
        private Stopwatch gameLoopTimer = new Stopwatch();

        // Initializes a new instance of the MainWindow class.
        public MainWindow()
        {
            InitializeComponent();
            // Set to fullscreen
            this.Topmost = true;
            WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
        }

        // Execute startup tasks
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the depth stream to receive depth frames
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                
                // Allocate space to put the depth pixels we'll receive
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];

                // Allocate space to put the depth pixels we'll receive
                this.depthPixelsCalibration = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];

                // Allocate space to put the color pixels we'll create
                this.colorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.Image.Source = this.colorBitmap;
                this.Image.Visibility = Visibility.Collapsed;      

                // Set calibration      
                this.sensor.DepthFrameReady += this.CalibrationDepthFrameReady;

                // Add an event handler to be called whenever there is new depth frame data
                this.sensor.DepthFrameReady += this.SensorDepthFrameReady;

                // Declare game thread
                gameThreadTimer.Tick += new EventHandler(GameLoop);

                // Create Menu 
                Menu();

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

        }

        // Execute shutdown tasks
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        // Handles the user clicking on the calibration button
        private void ButtonCalibrationClick(object sender, EventArgs e)
        {
            this.calibrationNeeded = true;
        }

        // Calibrate sensor with a single frame of the sensor 
        private void CalibrationDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {           
                if (depthFrame != null && this.calibrationNeeded)
                {
                    Console.WriteLine("Calibrating...");

                    // Toggle Calibration image visibility 
                    if (this.Image.Visibility == Visibility.Collapsed)
                    {
                        this.Image.Visibility = Visibility.Visible;           
                    }
                    else
                    {
                        this.Image.Visibility = Visibility.Collapsed;
                    }

                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixelsCalibration);
                    this.calibrationNeeded = false;
                    this.calibrated = true;

                    for (int i = 0; i < this.depthPixelsCalibration.Length; ++i)
                    {
                        // Get the depth for this pixel
                        short depth = depthPixels[i].Depth;

                        if (this.minPixelDepth == 0)
                        {
                            this.minPixelDepth = depth;
                        }
                        else if (depth < this.minPixelDepth && depth != 0)
                        {
                            this.minPixelDepth = depth;
                        }

                        if (depth > maxPixelDepth)
                        {
                            this.maxPixelDepth = depth;
                        }

                        this.avgPixelDepth += depth;
                    }

                    // Calculate calibrated wall threshold 
                    this.avgPixelDepth = this.avgPixelDepth / this.depthPixelsCalibration.Length;
                    this.wallDistThresh = (int)((this.maxPixelDepth - this.avgPixelDepth) * .80);       

                    Console.WriteLine("Pixel: " + minPixelDepth.ToString() + " : " + maxPixelDepth.ToString() + " : " + avgPixelDepth.ToString() + " : " + this.wallDistThresh.ToString());
                }
            }
        }

        // Event handler for Kinect sensor's DepthFrameReady event
        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null && this.calibrated)
                {
                    // Console.WriteLine("Getting Points");
                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    // Convert the depth to RGB
                    int colorPixelIndex = 0;
                    int indexX = 0;
                    int indexY = 0;
                    int pointIndex = 0;

                    List<Point> pointsList = new List<Point>();
                    Point tempPoint = new Point();

                    for (int i = 0; i < this.depthPixels.Length; ++i)
                    {

                        // Get the depth for this pixel
                        short newDepth = depthPixels[i].Depth;
                        short calDepth = depthPixelsCalibration[i].Depth;

                        byte wall = (byte)(newDepth <= this.maxPixelDepth && newDepth >= this.minPixelDepth ? newDepth : 0);
                        byte hit = (byte)(newDepth <= calDepth - this.wallDistThresh * 0.50 && newDepth >= calDepth - this.wallDistThresh * 1.0 ? newDepth : 0);

                        // Write out blue byte - wall
                        this.colorPixels[colorPixelIndex++] = wall;

                        // Write out green byte - hit
                        this.colorPixels[colorPixelIndex++] = hit;
                 
                        if (hit > 5)
                        {
                            // linear interpolation for image dimensions to screen dimensions 
                            tempPoint.X = linear(indexX, this.sensor.DepthStream.FrameWidth, 0, 0, Game.ActualWidth);
                            tempPoint.Y = linear(indexY, 0, this.sensor.DepthStream.FrameHeight, 0, Game.ActualHeight);

                            // sample pixel data for points to add
                            if (pointIndex <= i - 2000)
                            {
                                pointsList.Add(tempPoint);
                                pointIndex = i;
                            }                                                   
                        }

                        // Write out red byte                        
                        this.colorPixels[colorPixelIndex++] = 0;

                        // We're outputting BGR, the last byte in the 32 bits is unused so skip it
                        // If we were outputting BGRA, we would write alpha here.
                        ++colorPixelIndex;

                        indexX++;
                        if (indexX == this.sensor.DepthStream.FrameWidth)
                        {
                            indexX = 0;
                            indexY++;
                        }
                    }                   

                    this.blobs.Clear();
                    GetBlobs(pointsList);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        // linear interpolation
        static public double linear(double n, double start1, double stop1, double start2, double stop2)
        {
            return ((n - start1) / (stop1 - start1)) * (stop2 - start2) + start2;
        }

        private void GetBlobs(List<Point> pointsList)
        {
            // Console.WriteLine("Getting Blobs!");
            // Console.WriteLine("Points: " + pointsList.Count);

            double distance;
            Blob tempBlob = new Blob();
            for (int i = 0; i < pointsList.Count; i++)
            {
                if (this.blobs.Count == 0) // make first blob
                {                    
                    tempBlob.averagePoint = pointsList[i];
                    tempBlob.totalPoints = 1;
                    this.blobs.Add(tempBlob);
                    //Console.WriteLine(this.blobs.Count);
                    continue;
                }                
            
                for (int j = 0; j < this.blobs.Count; j++)
                {
                    distance = (this.blobs[j].averagePoint - pointsList[i]).LengthSquared;
                    //Console.WriteLine(distance + " < " + this.blobDistThresh);
                    if (distance <= this.blobDistThresh) // add to existing blob if near old blob/blobs
                    { 
                        tempBlob = CalcNewAvg(this.blobs[j], pointsList[i]);
                        this.blobs[j] = tempBlob;
                        break;
                    }
                    else if (j == this.blobs.Count - 1) // add new blob if not near old blob/blobs
                    {                    
                        tempBlob.averagePoint = pointsList[i];
                        tempBlob.totalPoints = 1;
                        this.blobs.Add(tempBlob);
                        //Console.WriteLine("Added Blob: " + this.blobs.Count);
                    }
                }
            }         
        }

        // Calculate new average point coordinates when adding points to blob 
        private Blob CalcNewAvg(Blob blob, Point point)
        {
            double distance = (blob.averagePoint - point).LengthSquared;
            //Console.WriteLine("X: " + blob.averagePoint.X + ", Y: " + blob.averagePoint.Y + ", Dist: " + distance + "Total: " + blob.totalPoints);
            //Console.WriteLine("X: " + point.X + ", Y: " + point.Y);

            blob.averagePoint.X = ((blob.averagePoint.X * blob.totalPoints) + point.X) / (blob.totalPoints + 1);
            blob.averagePoint.Y = ((blob.averagePoint.Y * blob.totalPoints) + point.Y) / (blob.totalPoints + 1);
            blob.totalPoints++;

            distance = (blob.averagePoint - point).LengthSquared;
            //Console.WriteLine("X: " + blob.averagePoint.X + ", Y: " + blob.averagePoint.Y + ", Dist: " + distance + ", Total: " + blob.totalPoints);
            return blob;
        }

        // Create menu for game on screen
        private void Menu()
        {
            Console.WriteLine("Menu");
            Game.Children.Clear();

            Button easyButton = new Button();
            easyButton.Click += new RoutedEventHandler(StartGame);
            easyButton.Content = "Easy";
            easyButton.FontSize = 25;
            easyButton.Width = Game.ActualWidth / 2;
            easyButton.Height = Game.ActualHeight / 5;
            Canvas.SetLeft(easyButton, Game.ActualWidth / 4);
            Canvas.SetTop(easyButton, (Game.ActualHeight / 5) * 1);
            Game.Children.Add(easyButton);

            Button medButton = new Button();
            medButton.Click += new RoutedEventHandler(StartGame);
            medButton.Content = "Medium";
            medButton.FontSize = 25;
            medButton.Width = Game.ActualWidth / 2;
            medButton.Height = Game.ActualHeight / 5;
            Canvas.SetLeft(medButton, Game.ActualWidth / 4);
            Canvas.SetTop(medButton, (Game.ActualHeight / 5) * 2);
            Game.Children.Add(medButton);

            Button hardButton = new Button();
            hardButton.Click += new RoutedEventHandler(StartGame);
            hardButton.Content = "Hard";
            hardButton.FontSize = 25;
            hardButton.Width = Game.ActualWidth / 2;
            hardButton.Height = Game.ActualHeight / 5;
            Canvas.SetLeft(hardButton, Game.ActualWidth / 4);
            Canvas.SetTop(hardButton, (Game.ActualHeight / 5) * 3);
            Game.Children.Add(hardButton);

            Button calButton = new Button();
            calButton.Click += new RoutedEventHandler(ButtonCalibrationClick);
            calButton.Content = "Calibrate";
            calButton.FontSize = 15;
            calButton.Width = Game.ActualWidth / 8;
            calButton.Height = Game.ActualHeight / 8;
            Canvas.SetRight(calButton, 0);
            Canvas.SetTop(calButton, (Game.ActualHeight / 8) * 7);
            Game.Children.Add(calButton);

            TextBlock currentScoreText = new TextBlock();
            currentScoreText.Text = this.score.ToString();
            currentScoreText.TextAlignment = TextAlignment.Center;
            currentScoreText.FontSize = 50;
            currentScoreText.Width = Game.ActualWidth / 8;
            currentScoreText.Height = Game.ActualHeight / 8;
            Canvas.SetLeft(currentScoreText, 0);
            Canvas.SetTop(currentScoreText, 0);
            Game.Children.Add(currentScoreText);

            TextBlock highScoreText = new TextBlock();
            highScoreText.Text = this.highScore.ToString();
            highScoreText.TextAlignment = TextAlignment.Center;
            highScoreText.FontSize = 50;
            highScoreText.Width = Game.ActualWidth / 8;
            highScoreText.Height = Game.ActualHeight / 8;
            Canvas.SetRight(highScoreText, 0);
            Canvas.SetTop(highScoreText, 0);
            Game.Children.Add(highScoreText);
        }

        // Start game loop based on the difficulty selected on menu
        private void StartGame(object sender, EventArgs e)
        {
            Console.WriteLine("Start");
            Random RNG = new Random();
            this.targets.Clear();
            this.score = 0;
            this.numTargets = 4;
            switch ((sender as Button).Content)
            {
                case ("Easy"):
                    Console.WriteLine("Easy");
                    for (int i = 0; i < this.numTargets; i++)
                    {
                        var tempTarget = new Target(200, 3, RNG, (int)Game.ActualWidth, (int)Game.ActualHeight);
                        this.targets.Add(tempTarget);
                    }
                    break;
                case ("Medium"):
                    Console.WriteLine("Medium");
                    for (int i = 0; i < this.numTargets; i++)
                    {
                        var tempTarget = new Target(185, 4, RNG, (int)Game.ActualWidth, (int)Game.ActualHeight);
                        this.targets.Add(tempTarget);
                    }
                    break;
                case ("Hard"):
                    Console.WriteLine("Hard");
                    for (int i = 0; i < this.numTargets; i++)
                    {
                        var tempTarget = new Target(170, 5, RNG, (int)Game.ActualWidth, (int)Game.ActualHeight);
                        this.targets.Add(tempTarget);
                    }
                    break;
                default:                
                    Menu();
                    break;
            }

            this.gameLoopTimer.Start(); // start game thread 
            this.gameThreadTimer.Start(); // start game timer
        }

        // control and draw the game and look at blobs 
        private void GameLoop(object sender, EventArgs e)
        {
            Game.Children.Clear();

            for (int i = 0; i < this.targets.Count; i++) // take care of targets
            { 
                this.targets[i].moveTarget();
                this.targets[i].drawTarget(Game);
            }

            for (int i = 0; i < this.blobs.Count; i++) // draw blobs to screen
            {
                Ellipse hit = new Ellipse();

                hit.Fill = Brushes.Green;
                hit.StrokeThickness = 1;
                hit.Stroke = Brushes.Black;

                hit.Width = 20;
                hit.Height = 20;

                Canvas.SetLeft(hit, this.blobs[i].averagePoint.X - 10);
                Canvas.SetTop(hit, this.blobs[i].averagePoint.Y - 10);

                Game.Children.Add(hit);

                for (int j = 0; j < this.targets.Count; j++) // look for hits 
                {
                    if (Math.Abs(this.blobs[i].averagePoint.X - this.targets[j].position.X) < this.targets[j].radius &&
                        Math.Abs(this.blobs[i].averagePoint.Y - this.targets[j].position.Y) < this.targets[j].radius)
                    {
                        this.targets[j].hitTarget();
                        this.score++;
                    }
                }
            }

            // draw scores and timers
            TextBlock currentScore = new TextBlock();
            currentScore.Text = this.score.ToString();
            currentScore.TextAlignment = TextAlignment.Center;
            currentScore.FontSize = 50;
            currentScore.Width = Game.ActualWidth / 8;
            currentScore.Height = Game.ActualHeight / 8;
            Canvas.SetLeft(currentScore, 0);
            Canvas.SetTop(currentScore, 0);
            Game.Children.Add(currentScore);

            TextBlock timeLeft = new TextBlock();
            timeLeft.Text = this.gameLoopTimer.Elapsed.Seconds.ToString();
            timeLeft.TextAlignment = TextAlignment.Center;
            timeLeft.FontSize = 50;
            timeLeft.Width = Game.ActualWidth / 8;
            timeLeft.Height = Game.ActualHeight / 8;
            Canvas.SetRight(timeLeft, 0);
            Canvas.SetTop(timeLeft, 0);
            Game.Children.Add(timeLeft);

            if (this.gameLoopTimer.Elapsed > TimeSpan.FromSeconds(30)) // end the game 
            {
                Console.WriteLine("End Game");
                if (this.score > this.highScore) this.highScore = this.score;
                this.gameLoopTimer.Stop();
                this.gameLoopTimer.Reset();
                this.gameThreadTimer.Stop();
                Menu();
            }
        }       
       
    }
}