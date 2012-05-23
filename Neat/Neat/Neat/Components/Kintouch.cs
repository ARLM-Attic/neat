#if KINECT
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using Neat.Components;
using Neat;
using Neat.Mathematics;
using System.Diagnostics;
using Neat.Graphics;

namespace Neat.Components
{
    public class Kintouch : Microsoft.Xna.Framework.GameComponent
    {
        public class TrackPointData
        {
            public JointType TrackJoint = JointType.HandLeft;
            public JointType BaseJoint = JointType.Head;
            public float Z = 0.450f; //mm
            public int SkeletonId = 0;
            public string TextureName = "handleft";
            public string PushedTextureName = "handleft_pushed";
            public bool Pushed = false;
            public Vector2 Position, LastPosition;
            public int TrackTime = 0;
            public Color Tint = Color.Blue;
        }

        protected new NeatGame Game;
        public KinectEngine Kinect;
        public List<TrackPointData> TrackPoints;
        public Vector2 Sensitivity = new Vector2(1.3f);
        public int Delay = 20;

        public Kintouch(NeatGame game, KinectEngine kinect=null) : base(game)
        {
            this.Game = game;
            this.Kinect = kinect;

            TrackPoints = new List<TrackPointData>()
            {
                new TrackPointData(),
                new TrackPointData() {
                    TrackJoint = JointType.HandRight,
                    TextureName = "handright",
                    PushedTextureName = "handright_pushed" },
                new TrackPointData() {
                    SkeletonId = 1, Tint = Color.Red },
                new TrackPointData() {
                    TrackJoint = JointType.HandRight,
                    TextureName = "handright",
                    PushedTextureName = "handright_pushed",
                    SkeletonId = 1, Tint = Color.Red }
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (Kinect == null) return;
            Vector2 Size = new Vector2(Game.GameWidth, Game.GameHeight);
            for (int i = 0; i < TrackPoints.Count; i++)
            {
                var point = TrackPoints[i];
                point.LastPosition = point.Position;
                //if (Game.Frame - Kinect.LastSkeletonFrame > 5)
                if (point.SkeletonId < Kinect.TrackedSkeletonsIndices.Count)
                {
                    var trackPos = Kinect.ToVector3(point.TrackJoint, point.SkeletonId);
                    var basePos = Kinect.ToVector3(point.BaseJoint, point.SkeletonId);
                    point.Pushed = trackPos.Z < basePos.Z - point.Z;
                    var newPos = Size * (Sensitivity * new Vector2(trackPos.X, trackPos.Y));
                    newPos.Y = Size.Y - newPos.Y;

                    point.Position.X = MathHelper.Lerp(newPos.X, point.Position.X, 0.9f);
                    point.Position.Y = MathHelper.Lerp(newPos.Y, point.Position.Y, 0.9f);
                    
                    if (point.Pushed) point.TrackTime++;
                    else point.TrackTime = 0;
                }
                else point.TrackTime = 0;
            }
            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (Kinect == null) return;
            for (int i = 0; i < TrackPoints.Count; i++)
            {
                var point = TrackPoints[i];
                //if (Game.Frame - Kinect.LastSkeletonFrame > 5)
                if (point.SkeletonId < Kinect.TrackedSkeletonsIndices.Count)
                {
                    var tx = Game.GetSlice((point.TrackTime > Delay ? point.PushedTextureName : point.TextureName), gameTime);

                    var trackPos = Kinect.ToVector3(point.TrackJoint, point.SkeletonId);
                    var basePos = Kinect.ToVector3(point.BaseJoint, point.SkeletonId);

                    Game.Window.Title = GeometryHelper.Vector2String(trackPos);

                    var scaleFactor = 2 - (MathHelper.Clamp(basePos.Z - trackPos.Z, 0, point.Z)) / (point.Z);
                    var alpha = -scaleFactor + 2;
                    //scaleFactor += 2;
                    var size = tx.Crop.HasValue ? new Vector2(tx.Crop.Value.Width, tx.Crop.Value.Height) :
                        new Vector2(tx.Texture.Width, tx.Texture.Height);
                    Game.SpriteBatch.Draw(tx.Texture, point.Position - size / 2.0f, tx.Crop,
                        point.TrackTime > Delay ? Color.Black : GraphicsHelper.GetColorWithAlpha(point.Tint, alpha), 0, Vector2.Zero, scaleFactor, SpriteEffects.None, 0);
                }
            }
        }
    }
}
#endif
