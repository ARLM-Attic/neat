using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Neat.Mathematics
{
    public static class GeometryHelper
    {
        
#if WINDOWS
        public static Point GetMousePosition()
        {
            return new Point(Mouse.GetState().X, Mouse.GetState().Y);
        }
        public static Rectangle GetMouseRectangle()
        {
            return new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
        }
#endif

        public static Vector2 GetIntersectionPoint(Vector2 line1_a, Vector2 line1_b, Vector2 line2_a, Vector2 line2_b)
        {
            LineSegment line1 = new LineSegment();
            line1.StartPos = line1_a;
            line1.EndPos = line1_b;

            LineSegment line2 = new LineSegment();
            line2.StartPos = line2_a;
            line2.EndPos = line2_b;

            return GetIntersectionPoint(line1, line2);
        }
        public static bool IsNaN(Vector2 v)
        {
            return float.IsNaN(v.X);
        }

        public static Vector2 GetIntersectionPoint(LineSegment firstLine, LineSegment secondLine)
        {
            return GetIntersectionPoint(firstLine, secondLine, true, true);
        }

        public static Vector2 GetIntersectionPoint(LineSegment firstLine, LineSegment secondLine, bool firstSegment, bool secondSegment)
        {
            double Ua, Ub;

            // Equations to determine whether lines intersect
            Ua = ((secondLine.EndPos.X - secondLine.StartPos.X) * (firstLine.StartPos.Y - secondLine.StartPos.Y) - (secondLine.EndPos.Y - secondLine.StartPos.Y) * (firstLine.StartPos.X - secondLine.StartPos.X)) /
                    ((secondLine.EndPos.Y - secondLine.StartPos.Y) * (firstLine.EndPos.X - firstLine.StartPos.X) - (secondLine.EndPos.X - secondLine.StartPos.X) * (firstLine.EndPos.Y - firstLine.StartPos.Y));

            Ub = ((firstLine.EndPos.X - firstLine.StartPos.X) * (firstLine.StartPos.Y - secondLine.StartPos.Y) - (firstLine.EndPos.Y - firstLine.StartPos.Y) * (firstLine.StartPos.X - secondLine.StartPos.X)) /
                    ((secondLine.EndPos.Y - secondLine.StartPos.Y) * (firstLine.EndPos.X - firstLine.StartPos.X) - (secondLine.EndPos.X - secondLine.StartPos.X) * (firstLine.EndPos.Y - firstLine.StartPos.Y));

            if (((Ua >= 0.0f && Ua <= 1.0f)||(!firstSegment)) && 
                ((Ub >= 0.0f && Ub <= 1.0f)||(!secondSegment)))
            {
                double x = firstLine.StartPos.X + Ua * (firstLine.EndPos.X - firstLine.StartPos.X);
                double y = firstLine.StartPos.Y + Ua * (firstLine.EndPos.Y - firstLine.StartPos.Y);

                return new Vector2((float)x, (float)y);

            }
            else
            {
                return new Vector2(float.NaN);
            }
        }
 
        public static Vector2 MoveInCircle(GameTime gameTime, float speed)
        {
            double time = gameTime.TotalGameTime.TotalSeconds * speed;

            float x = (float)Math.Cos(time);
            float y = (float)Math.Sin(time);
           
            return new Vector2(x, y);
        }

        public static Rectangle MoveRectangle(Rectangle r, Point offset)
        {
            return new Rectangle(r.X + offset.X, r.Y + offset.Y,
                r.Width, r.Height);
        }

        public static Rectangle MoveRectangle(Rectangle r, Vector2 offset)
        {
            return MoveRectangle(r, new Point((int)offset.X, (int)offset.Y));
        }

        public static bool IsVectorInCircle(Vector2 position, float radius, Vector2 vector)
        {
            return (position - vector).Length() < radius;
        }

        public static bool IsVectorInRectangle(Vector2 position, Vector2 size, Vector2 vector)
        {
            return 
                (vector.X > position.X) &&
                (vector.X < position.X + size.X) &&
                (vector.Y > position.Y) &&
                (vector.Y < position.Y + size.Y);
        }

        public static bool IsVectorInRectangle(Rectangle r, Vector2 vector)
        {
            return r.Contains(Vector2Point(vector));
        }

        public static bool IsVectorInside(Vector3 point, List<Vector3> polygon)
        {
            int i, j = 0;
            bool c = false;
            for (i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if ((((polygon[i].Y <= point.Y) && (point.Y < polygon[j].Y)) ||
                    ((polygon[j].Y <= point.Y) && (point.Y < polygon[i].Y))) &&
                    (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) /
                    (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    c = !c;
            }
            return c;
        }

        public static  Point Vector2Point(Vector2 vector)
        {
            return new Point((int)(vector.X), (int)(vector.Y));
        }

        public static Vector2 Point2Vector(Point point)
        {
            return new Vector2((float)(point.X), (float)(point.Y));
        }

        public static Rectangle Vectors2Rectangle(Vector2 position,Vector2 size)
        {
            return new Rectangle((int)(position.X), (int)(position.Y), (int)(size.X), (int)(size.Y));
        }

        public static Rectangle Points2Rectangle(Point position, Point size)
        {
            return new Rectangle((position.X), (position.Y), (size.X), (size.Y));
        }

        public static string Coords2String(Vector2 p)
        {
            return "(" + p.X.ToString() + "," + p.Y.ToString() + ")";
        }
        public static string Coords2String(Point  p)
        {
            return "(" + p.X.ToString() + "," + p.Y.ToString() + ")";
        }

        public static Vector2 String2Vector(String s)
        {
            Vector2 result = Vector2.Zero;
            string number = "";
            char d = '0';
            int i = 0;
            for (; d != ',' && i<s.Length; i++)
            {
                d = s[i];
                if (d != ',') number += d;
            }
            result.X = float.Parse(number.Trim());

            number = "";
            for (; i < s.Length; i++) number += s[i];
            result.Y = float.Parse(number);
            return result;
        }
        public static string Vector2String(Vector2 v)
        {
            return v.X.ToString() + "," + v.Y.ToString();
        }
        public static string Point2String(Point p)
        {
            return Vector2String(Point2Vector(p));
        }
        public static Point String2Point(string s)
        {
            return Vector2Point(String2Vector(s));
        }

        public static float Min(params float[] nums)
        {
            float r = nums[0];
            foreach (var item in nums)
            {
                if (r > item) r = item;               
            }
            return r;
        }
        public static float Max(params float[] nums)
        {
            float r = 0;
            foreach (var item in nums)
            {
                if (r < item) r = item;
            }
            return r;
        }

        public static Vector2 Min(params Vector2[] vecs)
        {
            Vector2 v = vecs[0];
            foreach (var item in vecs)
            {
                if (v.X > item.X) v.X = item.X;
                if (v.Y > item.Y) v.Y = item.Y;
            }
            return v;
        }
        public static Vector2 Max(params Vector2[] vecs)
        {
            Vector2 v = vecs[0];
            foreach (var item in vecs)
            {
                if (v.X < item.X) v.X = item.X;
                if (v.Y < item.Y) v.Y = item.Y;
            }
            return v;
        }

        public static Vector2 Polar2Cart(float r, float t)
        {
            return new Vector2(
                r * (float)Math.Cos((double)t),
                r * (float)Math.Sin((double)t));
        }
    }
}
