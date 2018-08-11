using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Vector
    {
        //only for 2D
        public double x { get; set; }
        public double y { get; set; }

        public Vector() : this(0, 0) { }
        public Vector(double _x, double _y)
            {
            this.x = _x;
            this.y = _y;
        }
        public Vector(double _R, double _Ang, string arg="polar")
        {
            this.x = GetX(_R, _Ang);
            this.y = GetY(_R, _Ang);
        }
        public static double GetX(double R, double Ang)
        {
            double X = R * Math.Cos(Ang*Math.PI /180);
            return X;
        }

        public static double GetY(double R, double Ang)
        {
            double X = R * Math.Sin(Ang*Math.PI/180);
            return X;
        }

        //重载矢量的“+”； “-”； “*”(一个标量)； “/”（一个标量）的操作符

        public static bool operator ==(Vector p1, Vector p2)
        {
            if (p1.x == p2.x && p1.y == p2.y)
                return true;
            else
                return false;
        }

        public static bool operator !=(Vector p1, Vector p2)
        {
            return !(p1==p2);
        }

       

        public static Vector operator +(Vector p1, Vector p2)

        {
            Vector psum=new Vector();
            psum.x = p1.x + p2.x;
            psum.y = p1.y + p2.y;
            return psum;
        }

        public static Vector operator -(Vector p1, Vector p2)

        {
            Vector psub = new Vector();
            psub.x = p1.x - p2.x;
            psub.y = p1.y - p2.y;
            return psub;
        }

        public static Vector operator *(Vector p, double l)
        {
            Vector pnew = new Vector();
            pnew.x = p.x * l;
            pnew.y = p.y *l;
            return pnew;
        }

        public static Vector operator *(double l, Vector p)
        {
            Vector pnew = new Vector();
            pnew.x = p.x * l;
            pnew.y = p.y * l;
            return pnew;
        }

        public static Vector operator /(Vector p, double l)
        {
            Vector pnew = new Vector();
            if (l != 0)
            {
                pnew.x = p.x / l;
                pnew.y = p.y / l;
            }
            else
            {
                pnew.x = double.PositiveInfinity;
                pnew.y = double.PositiveInfinity;
            }
            return pnew;
        }

        //得到矢量P1和P2的内积 inner product of p1 and p2
        public static double GetInnerProduct(Vector p1, Vector p2)
        {
            double innerproduct = p1.x*p2.x+p1.y*p2.y;
            return innerproduct;
        }

        //得到矢量的模 norm of vector p
        public static double Abs(Vector p)
        {
            return Math.Sqrt(GetInnerProduct(p, p));
        }

        //得到矢量的角度,在直角坐标系中 angle of vector p, in unit of degrees
        public static double Angle(Vector P)
        {
            return Math.Atan2(P.x, P.y)*180/Math.PI; //in unit of degree
        }

        //得到矢量P1和P2之间的夹角 get angle between two vectors p1 and p2, in unit of radian
        public static double GetAngleofTwoVectors(Vector p1, Vector p2)
        {
            Vector unitP1 = GetUnitVector(p1);//calculate unit vector of p1
            Vector unitP2 = GetUnitVector(p2);//unit vector of p2
            double Cosangle = GetInnerProduct(unitP1, unitP2); //unitp1.* unitp2 = cos(Angle)
            double Angle = Math.Acos(Cosangle);
            return (Angle);
        }

        //得到与矢量P同方向的的单位矢量 get a unit vector with angle same as P 
        public static Vector GetUnitVector(Vector P)
        {
            return P / Abs(P);
        }

        //得到P1在P2方向上的投影 get projection of P1 along P2
        public static double GetProjectionOfP1AlongP2(Vector P1, Vector P2)
        {
            return (GetInnerProduct(P1, GetUnitVector(P2)));
        }

        //得到矢量空间2点的距离 get distance between p1 and p2
        public static double Getdistance(Vector p1, Vector p2)
        {
            double distance = 0;
            distance = Abs(p1 - p2);
            return (distance);
        }

    }
}
