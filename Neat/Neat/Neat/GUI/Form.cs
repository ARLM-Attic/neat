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
        public Dictionary<String, Control> Controls;
        public string SelectedControl;
        public int SelectedControlNo=0;
        public bool HasMouse = false;
        protected NeatGame game;
        public int MouseSpeed = 7;
        public Form(NeatGame g)
        {
            Controls = new Dictionary<string, Control>();
            game = g;
            HasMouse = game.ShowMouse;
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var item in Controls )
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
                    foreach (var item in Controls)
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
                    foreach (var item in Controls)
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
            foreach (var item in Controls)
            {
                if (item.Value.Visible)
                    item.Value.Draw(gameTime, game.SpriteBatch);
            }
        }

        public void NewControl(string name, Control item)
        {
            name = name.ToLower();
            Controls.Add(name, item);
            Controls[name].game = game;
        }

        public Control GetControl(string name)
        {
            return Controls[name.ToLower()];
        }

        public void AttachToConsole()
        {
            if (game.Console == null) return;
            
            game.Console.AddCommand("f_newcontrol", f_newcontrol);
            game.Console.AddCommand("f_delcontrol", f_delcontrol);
            game.Console.AddCommand("f_selcontrol", f_selcontrol);
            game.Console.AddCommand("f_listcontrols", f_listobjects);
        }

        void f_newcontrol(IList<string> args)
        {
            if (args.Count != 3)
            {
                game.Console.WriteLine("syntax: f_newcontrol [controlname] [controltype]");
                return;
            }
            switch (args[2].ToLower())
            {
                case "box":
                    NewControl(args[1], new Box());
                    break;
                case "button":
                    NewControl(args[1], new Button());
                    break;
                case "checkbox":
                    NewControl(args[1], new CheckBox());
                    break;
                case "fancylabel":
                    NewControl(args[1], new FancyLabel());
                    break;
                case "formobject":
                    NewControl(args[1], new Control());
                    break;
                case "image":
                    NewControl(args[1], new Image());
                    break;
                case "label":
                    NewControl(args[1], new Label());
                    break;
                case "typewriter":
                    NewControl(args[1], new TypeWriter());
                    break;
                default:
                    game.Console.WriteLine("Invalid Control type.");
                    return;
            }
        }

        void f_delcontrol(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: f_delcontrol [objectname]");
                return;
            }
            if (Controls.ContainsKey(args[1].ToLower())) Controls.Remove(args[1].ToLower());
        }

        void f_selcontrol(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: f_selcontrol [objectname]");
                return;
            }
            if (Controls.ContainsKey(args[1].ToLower())) GetControl(args[1]).AttachToConsole();
        }

        void f_listobjects(IList<string> args)
        {
            foreach (var item in Controls.Keys)
            {
                game.Console.WriteLine(item);
            }
        }
    }
}
