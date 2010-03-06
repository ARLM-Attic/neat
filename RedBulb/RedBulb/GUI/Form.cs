#region References
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion

namespace RedBulb.GUI
{
    public class Form
    {
        public Dictionary<String, FormObject> objects;
        public string selectedObject;
        public int selectedObjectNo=0;
        public bool hasMouse = false;
        public RedBulbGame game;
        public int mouseSpeed = 7;
        public Form(RedBulbGame g)
        {
            objects = new Dictionary<string, FormObject>();
            game = g;
            hasMouse = game.showMouse;
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var item in objects )
            {
                if (item.Value.visible)
                    item.Value.Update(gameTime);
            }
#if WINDOWS
            if (hasMouse)
            {
                if (game.IsPressed(Keys.Right))
                    Mouse.SetPosition(Mouse.GetState().X + mouseSpeed , Mouse.GetState().Y);

                else if (game.IsPressed(Keys.Left))
                    Mouse.SetPosition(Mouse.GetState().X - mouseSpeed, Mouse.GetState().Y);

                if (game.IsPressed(Keys.Up))
                    Mouse.SetPosition(Mouse.GetState().X, Mouse.GetState().Y - mouseSpeed);

                else if (game.IsPressed(Keys.Down))
                    Mouse.SetPosition(Mouse.GetState().X, Mouse.GetState().Y + mouseSpeed);

                if (game.IsPressed(Keys.Space))
                {
                    foreach (var item in objects)
                    {
                        if (Geometry2D.Vectors2Rectangle(item.Value.position, item.Value.size).Intersects(
                            new Rectangle(Mouse.GetState().X, Mouse.GetState().Y,1, 1)))
                        {
                            item.Value.Pressed();
                        }
                    }
                }

                if (game.IsTapped(Keys.Space))
                {
                    foreach (var item in objects)
                    {
                        if (Geometry2D.Vectors2Rectangle(item.Value.position, item.Value.size).Intersects(
                            new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 2, 2)))
                        {
                            item.Value.Released();
                        }
                    }
                }
            }
#endif
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (var item in objects )
            {
                if (item.Value.visible)
                    item.Value.Draw(gameTime, game.spriteBatch);
            }
        }

        public void NewObject(string name, FormObject item)
        {
            name = name.ToLower();
            objects.Add(name, item);
            objects[name].game = game;
        }

        public FormObject GetObject(string name)
        {
            return objects[name.ToLower()];
        }
    }
}
