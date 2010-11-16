using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Neat.Mathematics
{
    public class LineSegment
    {
        public Vector2 StartPos;
        public Vector2 EndPos;

        public LineSegment()
        {
        }

        public LineSegment(Vector2 v1, Vector2 v2)
        {
            StartPos = v1;
            EndPos = v2;
        }

        public LineSegment(float ax, float ay, float bx, float by)
        {
            StartPos = new Vector2(ax, ay);
            EndPos = new Vector2(bx, by);
        }
    }
}
