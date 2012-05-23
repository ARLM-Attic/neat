#if KINECT
namespace Neat.Components.KinectForWindows
{
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// A delegate method explaining how to map a SkeletonPoint from one space to another.
    /// </summary>
    /// <param name="point">The SkeletonPoint to map.</param>
    /// <returns>The Vector2 representing the target location.</returns>
    public delegate Vector2 SkeletonPointMap(SkeletonPoint point);

    /// <summary>
    /// This class is responsible for rendering a skeleton stream.
    /// </summary>
    public class SkeletonStreamRenderer : Object2D
    {
        /// <summary>
        /// This is the map method called when mapping from
        /// skeleton space to the target space.
        /// </summary>
        private readonly SkeletonPointMap mapMethod;

        /// <summary>
        /// The last frames skeleton data.
        /// </summary>
        private Skeleton[] skeletonData { get { return Chooser.Skeletons; } }

        /// <summary>
        /// This flag ensures only request a frame once per update call
        /// across the entire application.
        /// </summary>
        private static bool skeletonDrawn = true;

        /// <summary>
        /// The origin (center) location of the joint texture.
        /// </summary>
        private Vector2 jointOrigin;

        /// <summary>
        /// The joint texture.
        /// </summary>
        private string jointTexture;

        /// <summary>
        /// The origin (center) location of the bone texture.
        /// </summary>
        private Vector2 boneOrigin;
        
        /// <summary>
        /// The bone texture.
        /// </summary>
        private string boneTexture;

        /// <summary>
        /// Whether the rendering has been initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Initializes a new instance of the SkeletonStreamRenderer class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        /// <param name="map">The method used to map the SkeletonPoint to the target space.</param>
        public SkeletonStreamRenderer(NeatGame game, SkeletonPointMap map)
            : base(game)
        {
            this.mapMethod = map;
        }

        /// <summary>
        /// This method initializes necessary values.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.Size = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            this.initialized = true;
        }

        /// <summary>
        /// This method retrieves a new skeleton frame if necessary.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If the joint texture isn't loaded, load it now
            if (null == this.jointTexture)
            {
                this.LoadContent();
            }

            // If we don't have data, lets leave
            if (null == skeletonData || null == this.mapMethod)
            {
                return;
            }

            if (false == this.initialized)
            {
                this.Initialize();
            }

            this.SharedSpriteBatch.Begin();

            foreach (var skeleton in skeletonData)
            {
                switch (skeleton.TrackingState)
                {
                    case SkeletonTrackingState.Tracked:
                        // Draw Bones
                        this.DrawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);
                        this.DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine);
                        this.DrawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter);
                        this.DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft);
                        this.DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight);

                        this.DrawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft);
                        this.DrawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft);
                        this.DrawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft);

                        this.DrawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight);
                        this.DrawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight);
                        this.DrawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight);

                        this.DrawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft);
                        this.DrawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft);
                        this.DrawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft);

                        this.DrawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight);
                        this.DrawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight);
                        this.DrawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight);

                        // Now draw the joints
                        foreach (Joint j in skeleton.Joints)
                        {
                            Color jointColor = Color.Green;
                            if (j.TrackingState != JointTrackingState.Tracked)
                            {
                                jointColor = Color.Yellow;
                            }

                            this.SharedSpriteBatch.Draw(
                                (Game as NeatGame).GetTexture(jointTexture),
                                this.mapMethod(j.Position),
                                null,
                                jointColor,
                                0.0f,
                                this.jointOrigin,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
                        }

                        break;
                    case SkeletonTrackingState.PositionOnly:
                        // If we are only tracking position, draw a blue dot
                        this.SharedSpriteBatch.Draw(
                                (Game as NeatGame).GetTexture(this.jointTexture),
                                this.mapMethod(skeleton.Position),
                                null,
                                Color.Blue,
                                0.0f,
                                this.jointOrigin,
                                1.0f,
                                SpriteEffects.None,
                                0.0f);
                        break;
                }
            }

            this.SharedSpriteBatch.End();
            skeletonDrawn = true;

            base.Draw(gameTime);
        }

        /// <summary>
        /// This method loads the textures and sets the origin values.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            var g = Game as NeatGame;
            var jt = g.LoadTexture(@"Sprites\Kinect\Joint");
            g.LoadTexture(@"Sprites\Kinect\Bone");

            this.jointTexture = "joint";
            this.jointOrigin = new Vector2(jt.Width / 2, jt.Height / 2);

            this.boneTexture = "bone";
            this.boneOrigin = new Vector2(0.5f, 0.0f);
        }

        /// <summary>
        /// This method draws a bone.
        /// </summary>
        /// <param name="joints">The joint data.</param>
        /// <param name="startJoint">The starting joint.</param>
        /// <param name="endJoint">The ending joint.</param>
        private void DrawBone(JointCollection joints, JointType startJoint, JointType endJoint)
        {
            var g = Game as NeatGame;
            var boneTex = g.GetTexture(boneTexture);

            Vector2 start = this.mapMethod(joints[startJoint].Position);
            Vector2 end = this.mapMethod(joints[endJoint].Position);
            Vector2 diff = end - start;
            Vector2 scale = new Vector2(1.0f, diff.Length() / boneTex.Height);

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Color color = Color.LightGreen;
            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                joints[endJoint].TrackingState != JointTrackingState.Tracked)
            {
                color = Color.Gray;
            }

            this.SharedSpriteBatch.Draw(boneTex, start, null, color, angle, this.boneOrigin, scale, SpriteEffects.None, 1.0f);
        }
    }
}
#endif