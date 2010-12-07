using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.Mathematics;
using System.IO;

namespace Neat.Game.SideScroller
{
    public class TextureWithPosition
    {
        public string Texture;
        public Vector2 Position;

    }
    public class World2d
    {
        NeatGame game;

        List<Polygon> Walls; //optimization, since walls are always static no matter what, why bother calculating their dynamics with the simulator?
        List<Entity> Entities;
        Entity Player;
        List<TextureWithPosition> Backgrounds;
        List<TextureWithPosition> Foregrounds;
        PhysicsSimulator Simulator;

        string LevelTitle;
        Vector2 LevelSize;

        Vector2 ViewOffset;
        Vector2 ViewMoveSpeed;
        Vector2 ViewLookThreshold;
        Vector2 ViewScrollThresholdDistance;

        Entity ObservingEntity;

        bool DrawPolygons;
        void Initialize()
        {
        }

        void Update(GameTime gameTime)
        {
        }

        void LoadLevel(string path)
        {
        }

        void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 offset)
        {
            foreach (var item in Backgrounds)
                spriteBatch.Draw(game.GetTexture(item.Texture), item.Position-ViewOffset, Color.White);

            foreach (var item in Foregrounds)
                spriteBatch.Draw(game.GetTexture(item.Texture), item.Position-ViewOffset, Color.White);

            if (DrawPolygons)
            {
               
            }
        }
    }
}
