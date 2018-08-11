using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
   public class Ball
    {
       public string Name { get; set; }

    //physics quantities
    public double Mass { get; set; }//in unit of kg
    public double Radius { get; set; } //in unit of mm

        private double K = 50; // Hooke's constant
        private double C = 0.0; // Damping constant
        private double Cr = 1; // Cr = 1 for elastic collison, Cr=0 for inelastic collison.

    public Vector p = new Vector();//momentum
    public Vector pos = new Vector();//position
    public Color bcolor = new Color();
    public Ellipse BallImage=new Ellipse();

        public Ball()
        {
            Random RND = new Random(this.GetHashCode());
            this.Name = "default";
            this.Mass = 1;
            this.Radius = 8;
            this.p.x = 0;
            this.p.y = 0;
            this.pos.x = 100;
            this.pos.y = 100;
            this.bcolor = Color.FromArgb(255, (byte)RND.Next(255), (byte)RND.Next(255), (byte)RND.Next(255));
            this.BallImage.Height = this.BallImage.Width = 2 * this.Radius;
            this.BallImage.Fill = new SolidColorBrush(this.bcolor);
        }

        public static void Copy(Ball target, Ball copy)
        {
            copy.Name = target.Name;
            copy.Mass = target.Mass;
            copy.Radius = target.Radius;
            copy.p = target.p;
            copy.pos = target.pos;
            copy.bcolor = target.bcolor;
            copy.BallImage = target.BallImage;
         }

        public void Collision(Ball b2, double dt)
        {
            Ball b1 = this;
            Vector dpos = b2.pos - b1.pos;
            Vector dP = b2.p - b1.p;

            double CurrrentDistance = Vector.Getdistance(b1.pos, b2.pos);
            double abbr = 0; //两球形变变量 the amount of abberation of the balls upon collision
            if (CurrrentDistance <= (b1.Radius + b2.Radius)) abbr = (b1.Radius + b2.Radius) - CurrrentDistance;
            Vector Abbr = Vector.GetUnitVector(dpos) * abbr;
            Vector Force1 = new Vector(0, 0);
            Vector Force2 = new Vector(0, 0);
            if (abbr > 0)
            {

                Force1 = -K * Abbr - C * b1.p;
                Force2 = K * Abbr - C * b2.p;

                b1.p += Force1 * dt;
                b2.p += Force2 * dt;

            }
        }

        public void HardBallCollision( Ball b2,double Dt) // to describe the ball collision behavior
        {
            Ball b1 = this;

               Vector dpos = b2.pos - b1.pos;
            Vector dP = b2.p - b1.p;

            double CurrrentDistance = Vector.Getdistance(b1.pos, b2.pos);
            double abbr = 0; //两球形变变量 the amount of abberation of the balls due to the bumping
            if (CurrrentDistance <= (b1.Radius + b2.Radius))
                abbr = (b1.Radius + b2.Radius) - CurrrentDistance;           

            if (abbr > 0)//&& PredictedDistance-CurrrentDistance<0)
            {

                //变换到两球质心坐标系 switch to centre of mass coordinates
                Vector p_axis_1 = Vector.GetUnitVector(dpos) * Vector.GetProjectionOfP1AlongP2(b1.p, dpos);//质心方向 center of mass axis (pointing from center of ball 1 to ball 2)
                Vector p_axis_2 = Vector.GetUnitVector(dpos) * Vector.GetProjectionOfP1AlongP2(b2.p, dpos);
                Vector p_tan_1 = b1.p - p_axis_1;//切线方向，即垂直于质心方向 tangent direction, which is perpendicular to center of mass axis (pointing from center of ball 1 to ball 2)
                Vector p_tan_2 = b2.p - p_axis_2;

                //计算碰撞前速度 calculate the speed before bumping
                double v_axis_1 = Vector.GetProjectionOfP1AlongP2(b1.p, dpos) / b1.Mass;
                double v_axis_2 = Vector.GetProjectionOfP1AlongP2(b2.p, dpos) / b2.Mass;

                //碰撞造成的速度变化 calculate the change of velocity due to the bumping in both dimensions

                double v_axis_1_new = (Cr * b2.Mass * (v_axis_2 - v_axis_1) + b1.Mass * v_axis_1 + b2.Mass * v_axis_2) / (b1.Mass + b2.Mass);
                double v_axis_2_new = (Cr * b1.Mass * (v_axis_1 - v_axis_2) + b1.Mass * v_axis_1 + b2.Mass * v_axis_2) / (b1.Mass + b2.Mass);

                //碰撞后的新动量 calculate the new momentum after bumping
                Vector p_axis_1_new = Vector.GetUnitVector(dpos) * b1.Mass * v_axis_1_new;//
                Vector p_axis_2_new = Vector.GetUnitVector(dpos) * b2.Mass * v_axis_2_new;
                Vector p_tan_1_new = p_tan_1;//切线方向不发生改变 new momentum in tangent direction
                Vector p_tan_2_new = p_tan_2;

                //转换回以前xy坐标系 switch back to original coordinates
                b1.p = (p_axis_1_new + p_tan_1_new);
                b2.p = (p_axis_2_new + p_tan_2_new);

                //撞击结束后使两球脱离接触 force the two balls to depart from each other after the bumping event
                double b1_abbr = b1.Mass / (b1.Mass + b2.Mass) * abbr;
                double b2_abbr = abbr - b1_abbr;
                Vector b1_Recover = -1 * b1_abbr * Vector.GetUnitVector(dpos);//b1 moves from b2 to b1 which is the opposite direction of dpos 
                Vector b2_Recover = b2_abbr * Vector.GetUnitVector(dpos); // b2 mover along dpos which pointing from b1 to b2
                b1.pos += b1_Recover;
                b2.pos += b2_Recover;
            }
        }

    }
}
