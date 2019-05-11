// ===============================
// AUTHOR           : Nick Sienkiewicz 
// CREATE DATE      : 5/11/2019
// PURPOSE          : Create targets and handle their parameters
// ===============================
// Change History   : 
// 5/11/2019 - Documentation and publish code
//==================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

// Extend main namespace
namespace Microsoft.Samples.Kinect.DepthBasics
{
    class Target
    {
        private int screenWidth; 
        private int screenHeight;
        private int diameter;
        public int radius; 

        public Point position;
        private Point velocity;
        private int speed;
        private Random RNG;

        // Constructor for parameters
        public Target(int rad, int speed, Random random, int width, int height)
        {
            this.diameter = rad;
            this.radius = rad / 2;
            this.speed = speed;
            this.RNG = random;
            this.position.X = RNG.Next(width);
            this.position.Y = RNG.Next(height);
            this.screenWidth = width;
            this.screenHeight = height;

            this.calcRandVel();
        }

        // Assign random velocity depending on speed parameter
        private void calcRandVel()
        {
            int sign;
            sign = RNG.Next(2);
            if (sign == 0) sign = -1;
            this.velocity.X = RNG.Next(-this.speed, this.speed);
            this.velocity.Y = (this.speed - Math.Abs(this.velocity.X)) * sign;
        }

        // If target was hit
        public void hitTarget()
        {
            this.position.X = RNG.Next(this.screenWidth);
            this.position.Y = RNG.Next(this.screenHeight);
            this.calcRandVel();
        }

        // Move target by velocity 
        public void moveTarget()
        {
            this.position.X += this.velocity.X / 2;
            this.position.Y += this.velocity.Y / 2;

            if (this.position.X > this.screenWidth - this.radius) this.position.X = 0 + this.radius;
            if (this.position.X < 0 + this.radius) this.position.X = this.screenWidth - this.radius;
            if (this.position.Y > this.screenHeight - this.radius) this.position.Y = 0 + this.radius;
            if (this.position.Y < 0 + this.radius) this.position.Y = this.screenHeight - this.radius;
        }

        // Draw target to Canvas
        public void drawTarget(Canvas canvas)
        {
            Ellipse target = new Ellipse();
            ImageBrush targetImage = new ImageBrush();
            targetImage.ImageSource = new BitmapImage(new Uri("../../Images/fly_1.png", UriKind.Relative));
            target.Fill = targetImage;

            //target.Fill = Brushes.Red;
            //target.StrokeThickness = 1;
            //target.Stroke = Brushes.White;

            target.Width = this.diameter;
            target.Height = this.diameter;

            Canvas.SetLeft(target, this.position.X - this.radius);
            Canvas.SetTop(target, this.position.Y - this.radius);            

            canvas.Children.Add(target);
        }

    }
}
