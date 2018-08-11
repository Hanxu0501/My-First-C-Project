using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class Softwall
    {
        public double Thickness = 20;
        public struct Boundary
        {
            public double Left;
            public double Top;
            public double Right;
            public double Bottom;

            public Boundary(double l, double t, double r, double b)
            {
                this.Left = l;
                this.Top = t;
                this.Right = r;
                this.Bottom = b;
            }
        }
        public Boundary boundary = new Boundary(0,0,500,500);
        private double K = 50;//恢复系数 F = -K * dx
        private double C = 0.1;//阻尼系数 F = -C * v

        private Vector GetWallBumpingForce(Ball ball)
        {
            Vector Force = new Vector(0, 0);
            double dx, dy;
            if ((ball.pos.x -ball.Radius) - boundary.Left < 0)
            {
                dx = (ball.pos.x-ball.Radius)-boundary.Left;
                Force += new Vector(-K * dx-C*ball.p.x, 0);
            }

            if ((ball.pos.x+ball.Radius) - boundary.Right > 0)
            {
                dx = (ball.pos.x+ball.Radius) - boundary.Right;
                Force += new Vector(-K * dx - C * ball.p.x, 0);

            }

            if ((ball.pos.y-ball.Radius) - boundary.Top < 0)
            {
                dy = (ball.pos.y-ball.Radius)-boundary.Top;
                Force += new Vector(0, -K * dy - C * ball.p.y);

            }

            if ((ball.pos.y+ball.Radius) - boundary.Bottom > 0)
            {
                dy = (ball.pos.y+ball.Radius) - boundary.Bottom;
                Force += new Vector(0, -K * dy - C * ball.p.y);
            }

            return Force;
        }
        public void SoftWallBumping(double Dt, Ball b)
        {


            Vector BumpForce = new Vector();
            BumpForce = GetWallBumpingForce(b);
            b.p += BumpForce * Dt;

        }
    }

    

   public class Sandbox
    {
        public double Width { get; set; }//height of the 2D sandbox
        public double Height { get; set; }//Width of the 2D 
        public double Rightoffest = 4;
        public double Bottomoffset = 25;
        public double dt { get; set; }//time increament of the sandbox
        public bool IsErrorControlled = true;
        private double Error = 0.25;

        public Gravity gravity = new Gravity();
        public Softwall myWall = new Softwall();

        //define the balls in the sandbox
        public List<Ball> BallLsit = new List<Ball>();

        //for screen displayer
        public SandboxScreenConvertor myConvertor = new SandboxScreenConvertor();
     
        private void EffectOfGravity(double Dt, Ball b,List<Ball> Blist)
        {
            
            Vector Force = new Vector(0,0);
            Vector MForce = new Vector(0, 0);
            
            Force = gravity.GetForce(b,gravity.gmod);
            MForce = gravity.GetMutualGravity(b, Blist);
            b.p += (Force+MForce) * Dt;
           
        }

        /////////////// Error Controlled  ////////////////////////////////////////////////////////
        private void MotionWithFixedDt(double Dt)
        {
            foreach (Ball b in BallLsit)
            {
                EffectOfGravity(Dt, b,BallLsit);
            }

            foreach (Ball b1 in BallLsit)
            {
                foreach (Ball b2 in BallLsit)
                {
                    if (b1 != b2)
                        b1.Collision(b2, Dt);

                }

            }

            foreach (Ball b in BallLsit)
                //WallBumping(b);
                myWall.SoftWallBumping(Dt, b);

            //position change in dt,dynamic precision needed here:
            foreach (Ball b in BallLsit)
            {
                Vector dpos = (b.p) / b.Mass * Dt;
                b.pos += dpos;
            }

        }
        private void MotionInDtWithControlledError(double Dt, double Err)
        {

            int div = 2;
            int Generation = 1;
            double Max_E = 0;

            Ball Temp = new Ball();


            List<Ball> OrgBall = new List<Ball>();
            List<Vector> Pos_g_old = new List<Vector>();
            List<Vector> Pos_g_new = new List<Vector>();

            foreach (Ball ball in BallLsit)
            {
                Ball b = new Ball();
                Ball.Copy(ball, b);
                OrgBall.Add(b);
            }

            MotionWithFixedDt(Dt);

            foreach (Ball ball in BallLsit)
            {
                Vector p = new Vector();
                p.x = ball.pos.x;
                p.y = ball.pos.y;

                Pos_g_old.Add(p);
            }

            do
            {
                Max_E = 0;
                BallLsit.Clear();
                foreach (Ball ball in OrgBall)
                {
                    Ball b = new Ball();
                    Ball.Copy(ball, b);
                    BallLsit.Add(b);
                }


                int NumberofSteps = (int)Math.Pow(div, Generation);
                double timestep = Dt / NumberofSteps;
                for (int i = 0; i < NumberofSteps; i++)
                {
                    MotionWithFixedDt(timestep);
                }

                Pos_g_new.Clear();
                foreach (Ball ball in BallLsit)
                {
                    Vector p = new Vector();
                    p.x = ball.pos.x;
                    p.y = ball.pos.y;

                    Pos_g_new.Add(p);
                }


                for (int i = 0; i < Pos_g_new.Count(); i++)
                {

                    double _E = Vector.Abs(Pos_g_old[i] - Pos_g_new[i]);
                    if (_E > Max_E) Max_E = _E;
                }


                for (int i = 0; i < Pos_g_new.Count(); i++)
                {
                    Pos_g_old[i] = Pos_g_new[i];

                }

                Generation++;

            }
            while (Max_E > Err && Generation <= 10);
           // if (Generation > 10) MessageBox.Show(Generation.ToString());



        }
        public void MotionInDt_New()
        {
            double ErrorScaled = Error * myConvertor.SpatialZoomRatio;
            Visualize();
            MotionInDtWithControlledError(dt, ErrorScaled);

            //Visualize();
        }
        ///////////////////////////////////////////////////////////////////////////////////

        //without Error Controlled version
        public void MotionInDt()
        {
            
            foreach (Ball b in BallLsit)
            {
                EffectOfGravity(dt,b,BallLsit);
            }

            foreach (Ball b1 in BallLsit)
            {
                foreach (Ball b2 in BallLsit)
                {
                    if (b1 != b2)
                        b1.HardBallCollision(b2,dt);
                       // b1.Collision(b2, dt);

                }
                         
            }

            foreach (Ball b in BallLsit)
                //WallBumping(b);
                myWall.SoftWallBumping(dt,b);

            
            foreach (Ball b in BallLsit)
            {
                Vector dpos = (b.p) / b.Mass * dt;
                b.pos += dpos;
            }
            
            Visualize();
        }
      
        //now only balls are shown, 
        //potential is not shown yet
        public void Visualize()
        {
            myConvertor.PutAllofBallOnScreen(BallLsit);
        }

        //Saving and Loading
        public void SaveBallListToFile(List<Ball> bList, string path)
        {
            using (StreamWriter SW = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                if(SW!=null)
                {
                    SW.WriteLine("Number of Ball {0}", bList.Count);
                    foreach (Ball b in bList)
                    {
                        SW.WriteLine(b.Name);
                        SW.WriteLine("Mass = {0}", b.Mass);
                        SW.WriteLine("Radius = {0}", b.Radius);
                        SW.WriteLine("Color(ARGB) A = {0}", b.bcolor.A);
                        SW.WriteLine("Color(ARGB) R = {0}", b.bcolor.R);
                        SW.WriteLine("Color(ARGB) G = {0}", b.bcolor.G);
                        SW.WriteLine("Color(ARGB) B = {0}", b.bcolor.B);
                        SW.WriteLine("Position X = {0}", b.pos.x);
                        SW.WriteLine("Position Y = {0}", b.pos.y);
                        SW.WriteLine("Momentum X = {0}", b.p.x);
                        SW.WriteLine("Momentum Y = {0}", b.p.y);
                    }
                }
                
            } 
        }

        public void LoadBallListFromFile(List<Ball> bList, string path)
        {
            using (StreamReader SR = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                int num = 0;
                //string temp = SR.ReadLine();
                Match match = Regex.Match(SR.ReadLine(), @"\d+");
                if (!int.TryParse(match.Value, out num)) { MessageBox.Show("File loading error!"); return; }

                
                for (int i = 0; i < num; i++)
                {
                    Ball b = new Ball();    
                    b.Name = SR.ReadLine();
                    double mass;
                    double r;
                    byte A;
                    byte R;
                    byte G;
                    byte B;
                    double posx;
                    double posy;
                    double px;
                    double py;

                    match = Regex.Match(SR.ReadLine(), @"\d+|(\d+)(\.\d+)");
                    if (!double.TryParse(match.Value, out mass)){ MessageBox.Show("File loading error: MASS" + match.Value);return;}

                    match = Regex.Match(SR.ReadLine(), @"\d+|(\d+)(\.\d+)");
                    if (!double.TryParse(match.Value, out r)) { MessageBox.Show("File loading error:RADIUS" + match.Value); return; }

                    match = Regex.Match(SR.ReadLine(), @"\d+");
                    if (!byte.TryParse(match.Value, out A)) { MessageBox.Show("File loading error: COLOR A" + match.Value); return; }

                    match = Regex.Match(SR.ReadLine(), @"\d+");
                    if (!byte.TryParse(match.Value, out R)) { MessageBox.Show("File loading error:" + match.Value); return; }

                    match = Regex.Match(SR.ReadLine(), @"\d+");
                    if (!byte.TryParse(match.Value, out G)) { MessageBox.Show("File loading error:" + match.Value); return; }

                    match = Regex.Match(SR.ReadLine(), @"\d+");
                    if (!byte.TryParse(match.Value, out B)) { MessageBox.Show("File loading error:" + match.Value); return; }

                    match = Regex.Match(SR.ReadLine(), @"-?\d+|(\d+)(\.\d+)");
                    if (!double.TryParse(match.Value, out posx)) { MessageBox.Show("File loading error:" + match.Value); return; }
                    match = Regex.Match(SR.ReadLine(), @"-?\d+|(\d+)(\.\d+)");
                    if (!double.TryParse(match.Value, out posy)) { MessageBox.Show("File loading error:" + match.Value); return; }
                    match = Regex.Match(SR.ReadLine(), @"-?\d+|(\d+)(\.\d+)");
                    if (!double.TryParse(match.Value, out px)) { MessageBox.Show("File loading error:" + match.Value); return; }
                    match = Regex.Match(SR.ReadLine(), @"-?\d+|(\d+)(\.\d+)");
                    if (!double.TryParse(match.Value, out py)) { MessageBox.Show("File loading error:" + match.Value); return; }
                    b.Mass = mass;
                    b.Radius = r;
                    b.bcolor = Color.FromArgb(A, R, G, B);
                    b.pos = new Vector(posx, posy);
                    b.p = new Vector(px, py);
                    b.BallImage.Fill = new SolidColorBrush(b.bcolor);
                    bList.Add(b);
                }
            }
        }

    }

    public class SandboxScreenConvertor
    {
        public double SpatialZoomRatio;

        public void SetSandBox(Sandbox sb, Window w)
        {
            sb.Height = (w.Height-sb.Bottomoffset) * SpatialZoomRatio;
            sb.Width = (w.Width-sb.Rightoffest) * SpatialZoomRatio;
            sb.gravity.Centre = new Vector(sb.Width / 2, sb.Height / 2);
            sb.myWall.boundary
            = new Softwall.Boundary(sb.myWall.Thickness,
                                    sb.myWall.Thickness,
                                    sb.Width - sb.myWall.Thickness,
                                    sb.Height - sb.myWall.Thickness);

        }


        public void SetMarginOfBallOnScreen(Ball b)
        {
            Thickness T = new Thickness(0,0,0,0);
            
            double LeftMargin = (b.pos.x - b.Radius) / SpatialZoomRatio;
            double TopMargin = (b.pos.y - b.Radius) / SpatialZoomRatio;
            T.Left = LeftMargin;
            T.Top = TopMargin;
            b.BallImage.Margin = T;
        }

        public void SetSizeofBall(Ball b)
        {
            b.BallImage.Height = b.BallImage.Width = 2 * b.Radius/SpatialZoomRatio;
            //b.BallImage.Fill = new SolidColorBrush(b.bcolor);
        }

        public void PutABallOnScreen(Ball b)
        {
            SetSizeofBall(b);
            SetMarginOfBallOnScreen(b);
        }

        public void PutAllofBallOnScreen(List<Ball> bl)
        {
            foreach (Ball b in bl)
            {
                PutABallOnScreen(b);
            }
        }

        public Vector GetSandboxVectorforAPointOnScreen(Point p)
        {
            Vector vec = new Vector();
            vec.x = p.X * SpatialZoomRatio;
            vec.y = p.Y * SpatialZoomRatio;

            return vec;
        }

        public bool IsAPointOnBall(Ball b, Point p)
        {
            Vector pVec = GetSandboxVectorforAPointOnScreen(p);
            if (Vector.Getdistance(b.pos, pVec) < b.Radius)
                return true;
            else
                return false;
        }

        public bool FindBallContainAPoint(List<Ball> bl, Point p, out Ball bOut)
        {
            bOut = null;
            foreach (Ball b in bl)
            {
                if (IsAPointOnBall(b, p))
                {
                    bOut = b; 
                    return true;
                }
            }
            return false;
        }

        /////
    }
}
