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
            public static JointType[] BaseJoints = new JointType[] {
                JointType.ShoulderLeft, JointType.ShoulderRight};// JointType.ShoulderCenter;

            public Vector3 BasePos;

            public JointType TrackJoint = JointType.HandLeft;
            public JointType BackupJoint = JointType.WristLeft;
            public static float Z = 0.450f; //mm
            public int SkeletonId = 0;
            public string TextureName = "handleft";
            public string PushedTextureName = "handleft_pushed";
            public bool Pushed = false;
            public bool Hold = false;
            public bool LastHold = false;
            public bool LastPushed = false;
            public Vector2 Position, LastPosition;
            //public int TrackTime = 0;
            public TimeSpan TrackTime = TimeSpan.Zero;
            public Color Tint = Color.Yellow;
            public JointTrackingState TrackingState = JointTrackingState.NotTracked;
        }

        protected new NeatGame Game;
        public KinectEngine Kinect;
        public List<TrackPointData> TrackPoints;
        public Vector2 Sensitivity = new Vector2(1.4f,2.5f);
        //public int Delay = 20;
        public TimeSpan Delay = new TimeSpan(0, 0, 0, 0, 600);
        public float TouchThreshold = 5; //Distance
        public float JumpRange = 10; //Pixels

        public Kintouch(NeatGame game, KinectEngine kinect=null) : base(game)
        {
            Debug.WriteLine("Kintouch object created.", "Kintouch");
            Debug.WriteLineIf(kinect == null, "Kinect is null.", "Kintouch");

            this.Game = game;
            this.Kinect = kinect;

            TrackPoints = new List<TrackPointData>()
            {
                new TrackPointData(),
                new TrackPointData() {
                    TrackJoint = JointType.HandRight,
                    BackupJoint = JointType.WristRight,
                    TextureName = "handright",
                    PushedTextureName = "handright_pushed" },
                new TrackPointData() {
                    SkeletonId = 1, Tint = Color.Pink },
                new TrackPointData() {
                    TrackJoint = JointType.HandRight,
                    BackupJoint = JointType.WristRight,
                    TextureName = "handright",
                    PushedTextureName = "handright_pushed",
                    SkeletonId = 1, Tint = Color.Pink }
            };


        }

        public void Reset()
        {
            Reset(TimeSpan.Zero);
        }

        public void Reset(TimeSpan t)
        {
            for (int i = 0; i < TrackPoints.Count; i++)
            {
                var point = TrackPoints[i];
                point.LastHold = point.Hold = false;
                point.LastPushed = point.Pushed = false;
                point.TrackTime = -t;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Kinect == null || !Kinect.IsSensorReady) return;
            Vector2 Size = new Vector2(Game.GameWidth, Game.GameHeight);
            var touchDiffSq = TouchThreshold * TouchThreshold;
            var jumpDiffSq = JumpRange * JumpRange;

            for (int i = 0; i < TrackPoints.Count; i++)
            {
                var point = TrackPoints[i];
                point.LastPosition = point.Position;
                point.LastPushed = point.Pushed;
                point.LastHold = point.Hold;
                point.TrackingState = JointTrackingState.NotTracked;
                Joint jt;
                int useBackup = 0;
                if (Kinect.TryGetJoint(point.TrackJoint, out jt, point.SkeletonId) && jt.TrackingState == JointTrackingState.Tracked) useBackup = -1;
                else if (Kinect.TryGetJoint(point.BackupJoint, out jt, point.SkeletonId) && jt.TrackingState == JointTrackingState.Tracked) useBackup = 1;
                //if (Game.Frame - Kinect.LastSkeletonFrame > 5)
                if (point.SkeletonId < Kinect.TrackedSkeletonsIndices.Count && useBackup != 0)
                {
                    point.TrackingState = jt.TrackingState;
                    var trackPos = useBackup < 0 ? Kinect.ToVector3(point.TrackJoint, point.SkeletonId) : Kinect.ToVector3(point.BackupJoint, point.SkeletonId);
                    point.BasePos = Kinect.ToVector3(TrackPointData.BaseJoints[0], point.SkeletonId);

                    for (int bjI = 1; bjI < TrackPointData.BaseJoints.Length; bjI++)
                    {
                        point.BasePos += Kinect.ToVector3(TrackPointData.BaseJoints[bjI], point.SkeletonId);
                    }

                    point.BasePos /= new Vector3(TrackPointData.BaseJoints.Length);

                    point.Pushed = (trackPos.Z < point.BasePos.Z - TrackPointData.Z /*point.Z*/);
                    var newPos = Size * (Sensitivity * new Vector2(trackPos.X - point.BasePos.X, trackPos.Y - point.BasePos.Y));
                    newPos.Y = Size.Y - newPos.Y;
                    newPos += new Vector2(Game.GameWidth, -Game.GameHeight) * 0.5f;

                    float alpha = (useBackup > 0) ? 0.99f : 0.95f;

                    point.Position.X = MathHelper.Lerp(newPos.X, point.Position.X, alpha);
                    point.Position.Y = MathHelper.Lerp(newPos.Y, point.Position.Y, alpha);

                    var dV = point.Position - point.LastPosition;
                    if (dV.LengthSquared() > jumpDiffSq)
                    {
                        dV.Normalize();
                        dV *= JumpRange;
                        point.Position = point.LastPosition + dV;
                    }

                    var lengthDiffSq = (point.Position - point.LastPosition).LengthSquared();
                    if (point.Pushed && lengthDiffSq < touchDiffSq) point.Position = point.LastPosition;
                    
                    point.Pushed = point.Pushed && (lengthDiffSq < touchDiffSq);

                    if (point.Pushed) point.TrackTime += gameTime.ElapsedGameTime;
                    else point.TrackTime = TimeSpan.Zero;
                }
                else point.TrackTime = TimeSpan.Zero;

                if (point.TrackTime == TimeSpan.Zero) point.Pushed = false;
                point.Hold = point.TrackTime > Delay;
            }
            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (Kinect == null || !Kinect.IsSensorReady) return;
            var Z = TrackPointData.Z;
            for (int i = 0; i < TrackPoints.Count; i++)
            {
                var point = TrackPoints[i];
                //if (Game.Frame - Kinect.LastSkeletonFrame > 5)
                if (point.SkeletonId < Kinect.TrackedSkeletonsIndices.Count)
                {
                    var tx = Game.GetSlice((point.TrackTime > Delay ? point.PushedTextureName : point.TextureName), gameTime);

                    var trackPos = Kinect.ToVector3(point.TrackJoint, point.SkeletonId);
                    //var basePos = Kinect.ToVector3(point.BaseJoint, point.SkeletonId);

                    var scaleFactor = 2 - (MathHelper.Clamp(point.BasePos.Z - trackPos.Z, 0, Z)) / (Z);
                    var alpha = -scaleFactor + 2;
                    //scaleFactor += 2;

                    var size = tx.Crop.HasValue ? new Vector2(tx.Crop.Value.Width, tx.Crop.Value.Height) :
                        new Vector2(tx.Texture.Width, tx.Texture.Height);

                    var color = point.Hold ? Color.Red : GraphicsHelper.GetColorWithAlpha(point.Tint, alpha);
                    if (point.TrackingState == JointTrackingState.NotTracked) color = gameTime.TotalGameTime.Seconds % 2 == 0 ? Color.Gray : Color.DarkGray;
                    if (point.TrackingState == JointTrackingState.Inferred && (gameTime.TotalGameTime.Milliseconds / 100) % 2 == 0) color = Color.Gray;
                    if (!point.Hold && point.Pushed && (gameTime.TotalGameTime.Milliseconds / 100) % 2 == 0) color = Color.Orange;
                    Game.SpriteBatch.Draw(tx.Texture, point.Position - size / 2.0f, tx.Crop,
                        color, 0, Vector2.Zero, scaleFactor, SpriteEffects.None, 0);
                }
            }
        }
    }
}
#endif
