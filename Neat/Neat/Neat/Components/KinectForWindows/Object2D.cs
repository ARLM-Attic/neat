#if KINECT
namespace Neat.Components.KinectForWindows
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A very basic game component to track common values.
    /// </summary>
    public class Object2D : DrawableGameComponent
    {
        /// <summary>
        /// Initializes a new instance of the Object2D class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        public Object2D(NeatGame game)
            : base(game)
        {
        }

        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the size of the object.
        /// </summary>
        public Vector2 Size { get; set; }

        public KinectEngine Chooser
        {
            get
            {
                return (Game as NeatGame).Kinect;
            }
        }

        public SpriteBatch SharedSpriteBatch
        {
            get
            {
                return (Game as NeatGame).SpriteBatch;
            }
        }
    }
}
#endif