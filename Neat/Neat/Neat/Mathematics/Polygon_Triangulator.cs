using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Polygon_Triangulator.cs
//Written by Saeed Afshari (www.saeedoo.com)

//Based on "CTriangulator" class which can be found here (as of November 2010):
//      http://members.gamedev.net/ysaneya/Code/CTriangulator.cpp
//      http://members.gamedev.net/ysaneya/Code/CTriangulator.h

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Neat.Mathematics
{
    public class Triangle
    {
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;

        public Triangle()
        {
        }

        public Triangle(Triangle t)
        {
            A = t.A;
            B = t.B;
            C = t.C;
        }

        public Triangle(Polygon p)
        {
            if (p.Vertices.Count > 1)
            {
                A = p.Vertices[0];
                if (p.Vertices.Count > 2)
                {
                    B = p.Vertices[1];
                    if (p.Vertices.Count > 3)
                    {
                        C = p.Vertices[2];
                    }
                }
            }
        }

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Triangle(float xa, float ya, float xb, float yb, float xc, float yc)
        {
            A = new Vector2(xa, ya);
            B = new Vector2(xb, yb);
            C = new Vector2(xc, yc);
        }

        public Triangle Offset(Vector2 amount)
        {
            A += amount;
            B += amount;
            C += amount;
            return this;
        }

        public Triangle Scale(Vector2 amount)
        {
            A *= amount;
            B *= amount;
            C *= amount;
            return this;
        }

        public bool IsInside(Vector2 point)
        {
            Vector2 a = new Vector2(C.X - B.X, C.Y - B.Y);
            Vector2 b = new Vector2(A.X - C.X, A.Y - C.Y);
            Vector2 c = new Vector2(B.X - A.X, B.Y - A.Y);
            Vector2 ap = new Vector2(point.X - A.X, point.Y - A.Y);
            Vector2 bp = new Vector2(point.X - B.X, point.Y - B.Y);
            Vector2 cp = new Vector2(point.X - C.X, point.Y - C.Y);

            var aCbp = a.X * bp.Y - a.Y * bp.X;
            var cCap = c.X * ap.Y - c.Y * ap.X;
            var bCcp = b.X * cp.Y - b.Y * cp.X;

            return ((aCbp >= 0f) && (bCcp >= 0f) && (cCap >= 0f));
        }

        public Polygon ToPolygon()
        {
            Polygon p = new Polygon();
            p.Vertices.Add(A);
            p.Vertices.Add(B);
            p.Vertices.Add(C);
            return p;
        }

        public void Draw(SpriteBatch spriteBatch, Graphics.LineBrush lineBrush, Color color)
        {
            Draw(spriteBatch, lineBrush, Vector2.Zero, color);
        }

        public void Draw(SpriteBatch spriteBatch, Graphics.LineBrush lineBrush, Vector2 offset, Color color)
        {
            lineBrush.Draw(spriteBatch, A + offset, B + offset, color);
            lineBrush.Draw(spriteBatch, B + offset, C + offset, color);
            lineBrush.Draw(spriteBatch, C + offset, A + offset, color);
        }
    }

    public partial class Polygon
    {
        public List<Triangle> Triangles;

        public void Triangulate()
        {
            Triangles = new List<Triangle>();

            if (n < 3) return;
            else if (n == 3)
            {
                Triangles.Add(new Triangle(this));
                return;
            }

            var V = new Polygon( GetVerticesClockwise() ).Vertices;

            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2; )
            {
                if (0 >= (count--))
                    return;

                int u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                int w = v + 1;
                if (nv <= w)
                    w = 0;

                if (TestTriangle(u, v, w, V))
                {
                    Triangles.Add(new Triangle(V[u], V[v], V[w]));
                    m++;
                    V.RemoveAt(v);
                    nv--;
                    count = 2 * nv;
                }
            }
        }

        bool TestTriangle(int u, int v, int w, List<Vector2> V)
        {
            Triangle t = new Triangle(V[u], V[v], V[w]);
            if (((t.B.X - t.A.X) * (t.C.Y - t.A.Y)) - ((t.B.Y - t.A.Y) * (t.C.X - t.A.X)) < _epsilon)
                return false;

            for (int p = 0; p < V.Count; p++)
            {
                if ((V[p] != t.A) && (V[p] != t.B) && (V[p] != t.C))
                {
                    if (t.IsInside(V[p]))
                        return false;
                }
            }

            return true;
        }

        public bool IsInside(Vector2 p)
        {
            Triangle t = new Triangle();
            return IsInside(p, out t);
        }

        public bool IsInside(Vector2 p, out Triangle triangle)
        {
            triangle = null;
            foreach (var item in Triangles)
            {
                if (item.IsInside(p))
                {
                    triangle = item;
                    return true;
                }
            }
            return false;
        }
    }
}