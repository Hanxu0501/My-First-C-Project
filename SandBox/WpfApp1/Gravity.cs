using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Gravity
    {
        public enum GravityMode { Linear, Concentric, None }
        public GravityMode gmod = GravityMode.Linear;
        public Vector Centre = new Vector(0, 0);
        public Vector G = new Vector(0, 100);
        public double softcore = 1;

        private double Gcon = 10;//引力常数 gravity constant

        public Vector GetForce(Ball b, GravityMode _gmod)
        {
            Vector Force = new Vector(0, 0);
            switch (_gmod)
            {
                case GravityMode.Linear:
                    Force = b.Mass * this.G;
                    break;

                case GravityMode.Concentric:
                    Vector R =  Centre- b.pos; //vector from centre of potential to object
                    double force = 1000*Vector.Abs(G) * b.Mass / ((Vector.Abs(R)+softcore) * (Vector.Abs(R)+softcore));//Here G = M (产生中心力场物体的质量)×Gconstant(万有引力常数)
                    Force = force * Vector.GetUnitVector(R);
                    break;

                default: break;

            }
            return Force;
        }

        public Vector GetMutualGravity(Ball b, List<Ball> Blist) // gravity produced by ball
        { Vector MutualForce = new Vector(0, 0);
            Vector dpos = new Vector(0, 0);
            double R = 0;
            double softcorefactor;
            foreach (Ball b1 in Blist)
            {
               softcorefactor = b.Radius + b1.Radius;
               dpos = b1.pos - b.pos;
                R = Vector.Abs(dpos);
                if (R > softcorefactor)
                {
                    //R += softcorefactor;
                    MutualForce += Gcon * b.Mass * b1.Mass / (R * R) * Vector.GetUnitVector(dpos);
                }
               
                }
            return MutualForce; 

        }
    }

}
