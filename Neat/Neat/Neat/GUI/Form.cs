using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
#if WINDOWS
 
 
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.EasyMenus;
using Neat.Mathematics;

namespace Neat.GUI
{
    public class Form
    {
        public Dictionary<String, FormObject> Objects;
        public string SelectedObject;
        public int SelectedObjectNo=0;
        public bool HasMouse = false;
        public NeatGame game;
        public int MouseSpeed = 7;
        public Form(NeatGame g)
        {
            Objects = new Dictionary<string, FormObject>();
            game = g;
            HasMouse = game.ShowMouse;
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var item in Objects )
            {
                if (item.Value.Visible)
                    item.Value.Update(gameTime);
            }

#if WINDOWS
            if (HasMouse)
            {
                if (game.IsPressed(Keys.Right))
                    Mouse.SetPosition(Mouse.GetState().X + MouseSpeed , Mouse.GetState().Y);

                else if (game.IsPressed(Keys.Left))
                    Mouse.SetPosition(Mouse.GetState().X - MouseSpeed, Mouse.GetState().Y);

                if (game.IsPressed(Keys.Up))
                    Mouse.SetPosition(Mouse.GetState().X, Mouse.GetState().Y - MouseSpeed);

                else if (game.IsPressed(Keys.Down))
                    Mouse.SetPosition(Mouse.GetState().X, Mouse.GetState().Y + MouseSpeed);

                if (game.IsPressed(Keys.Space))
                {
                    foreach (var item in Objects)
                    {
                        if (GeometryHelper.Vectors2Rectangle(item.Value.Position, item.Value.Size).Intersects(
                            new Rectangle(Mouse.GetState().X, Mouse.GetState().Y,1, 1)))
                        {
                            item.Value.Pressed();
                        }
                    }
                }

                if (game.IsTapped(Keys.Space))
                {
                    foreach (var item in Objects)
                    {
                        if (GeometryHelper.Vectors2Rectangle(item.Value.Position, item.Value.Size).Intersects(
                            new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 2, 2)))
                        {
                            item.Value.Released();
                        }
                    }
                }
            }
#elif WINDOWS_PHONE
            if (game.IsTouched())
            {
                Vector2 p = game.GetHardestTouchPosition();
                foreach (var item in objects)
                {
                    if (game.IsTouched(
                        Geometry2D.Vectors2Rectangle(item.Value.position, item.Value.size)))
                        item.Value.Pressed();
                }
            }
#endif
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (var item in Objects)
            {
                if (item.Value.Visible)
                    item.Value.Draw(gameTime, game.SpriteBatch);
            }
        }

        public void NewObject(string name, FormObject item)
        {
            name = name.ToLower();
            Objects.Add(name, item);
            Objects[name].game = game;
        }

        public FormObject GetObject(string name)
        {
            return Objects[name.ToLower()];
        }

        public void AttachToConsole()
        {
            if (game.Console == null) return;
            
            game.Console.AddCommand("f_newobject", f_newobject);
            game.Console.AddCommand("f_delobject", f_delobject);
            game.Console.AddCommand("f_selobject", f_selobject);
            game.Console.AddCommand("f_listobjects", f_listobjects);
        }

        void f_newobject(IList<string> args)
        {
            if (args.Count != 3)
            {
                game.Console.WriteLine("syntax: f_newobject [objectname] [objecttype]");
                return;
            }
            switch (args[2].ToLower())
            {
                case "box":
                    NewObject(args[1], new Box());
                    break;
                case "button":
                    NewObject(args[1], new Button());
                    break;
                case "checkbox":
                    NewObject(args[1], new CheckBox());
                    break;
                case "fancylabel":
                    NewObject(args[1], new FancyLabel());
                    break;
                case "formobject":
                    NewObject(args[1], new FormObject());
                    break;
                case "image":
                    NewObject(args[1], new Image());
                    break;
                case "label":
                    NewObject(args[1], new Label());
                    break;
                case "typewriter":
                    NewObject(args[1], new TypeWriter());
                    break;
                default:
                    game.Console.WriteLine("Invalid Object type.");
                    return;
            }
        }

        void f_delobject(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: f_delobject [objectname]");
                return;
            }
            if (Objects.ContainsKey(args[1].ToLower())) Objects.Remove(args[1].ToLower());
        }

        void f_selobject(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: f_selobject [objectname]");
                return;
            }
            if (Objects.ContainsKey(args[1].ToLower())) GetObject(args[1]).AttachToConsole();
        }

        void f_listobjects(IList<string> args)
        {
            foreach (var item in Objects.Keys)
            {
                game.Console.WriteLine(item);
            }
        }
    }
}
