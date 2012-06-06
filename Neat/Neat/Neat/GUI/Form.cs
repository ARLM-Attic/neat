using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.EasyMenus;
using Neat.Mathematics;
using Neat.Components;

namespace Neat.GUI
{
    public class Form
    {
#if KINECT
        public KinectEngine Kinect { get { return game.Kinect; } }
        public Kintouch Touch { get { return game.Touch; } }
        public bool TrackKinect = true;
#endif
        public Dictionary<String, Control> Controls;
        public string SelectedControl;
        public int SelectedControlNo=0;
        public bool HasMouse = false;
        public Vector2 MouseOffset = Vector2.Zero;
        public bool ClickHandled = false;
        public bool MainForm = true;
        public Vector2 Size = Vector2.Zero;
        public bool Visible = true;
        public Vector2 MousePosition
        {
            get
            {
                return game.MousePosition + MouseOffset;
            }
        }
        protected NeatGame game;
        public int MouseSpeed = 7;
        LinkedList<Control> controlChain;
        public Form(NeatGame g)
        {
            Controls = new Dictionary<string, Control>();
            controlChain = new LinkedList<Control>();
            game = g;
            HasMouse = game.ShowMouse;
        }

        public void BringControlToFront(Control c)
        {
            LinkedListNode<Control> n = controlChain.First;
            while (n != null)
            {
                if (n.Value == c) break;
                n = n.Next;
            }
            if (n != null) controlChain.Remove(n);
            controlChain.AddFirst(c);
            c.Focused();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (MainForm)
                ClickHandled = false;
            var n = controlChain.First;
            while (n != null)
            {
                n.Value.Update(gameTime);
                if (!ClickHandled)
                {
                    if (n.Value.Visible)
                    {
                        n.Value.HandleInput(gameTime);

                        if (GeometryHelper.Vectors2Rectangle(n.Value.Position, n.Value.Size).Intersects(
                                new Rectangle((int)(MousePosition.X), (int)(MousePosition.Y), 1, 1)))
                        {
                            n.Value.IsMouseHovered = true;
                            n.Value.Hovered(MousePosition);

                            if (game.IsPressed(Keys.Space, Buttons.A) || game.IsMousePressed())
                            {
                                n.Value.Held(MousePosition);
                            }
                            else n.Value.IsMouseHold = false;

                            if (game.IsReleased(Keys.Space, Buttons.A) || game.IsMouseClicked())
                            {
                                n.Value.Released(MousePosition);
                            }
                            if (game.IsTapped(Keys.Space, Buttons.A) || game.IsMouseClicked(false))
                            {
                                n.Value.Pressed(MousePosition);
                            }
                            ClickHandled = true;
                        }
                        else n.Value.IsMouseHovered = false;
#if KINECT
                        if (Touch.Enabled)
                        {
                            foreach (var item in Touch.TrackPoints)
                            {
                                var pos = item.Position + MouseOffset;

                                if (GeometryHelper.Vectors2Rectangle(n.Value.Position, n.Value.Size).Intersects(
                                    new Rectangle((int)(pos.X), (int)(pos.Y), 32, 32)))
                                {
                                    n.Value.IsMouseHovered = true;
                                    n.Value.Hovered(pos);

                                    if (item.Pushed)
                                    {
                                        n.Value.IsMouseHold = true;
                                        n.Value.Held(pos);
                                    }

                                    if (item.LastHold && !item.Hold)
                                    {
                                        n.Value.Released(pos);
                                        //Touch.Reset();
                                    }
                                    else if (!item.LastHold && item.Hold)
                                    {
                                        n.Value.Pressed(pos);
                                        //Touch.Reset();
                                    }
                                    ClickHandled = true;
                                }
                            }
                        }
#endif
                    }
                }
                n = n.Next;
            }

#if WINDOWS
            if (HasMouse)
            {
                if (game.IsPressed(Keys.Right) || game.IsPressed(Buttons.LeftThumbstickRight))
                    Mouse.SetPosition(Mouse.GetState().X + MouseSpeed , Mouse.GetState().Y);

                else if (game.IsPressed(Keys.Left) || game.IsPressed(Buttons.LeftThumbstickLeft))
                    Mouse.SetPosition(Mouse.GetState().X - MouseSpeed, Mouse.GetState().Y);

                if (game.IsPressed(Keys.Up) || game.IsPressed(Buttons.LeftThumbstickUp))
                    Mouse.SetPosition(Mouse.GetState().X, Mouse.GetState().Y - MouseSpeed);

                else if (game.IsPressed(Keys.Down) || game.IsPressed(Buttons.LeftThumbstickDown))
                    Mouse.SetPosition(Mouse.GetState().X, Mouse.GetState().Y + MouseSpeed);
                
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
            if (!Visible) return;
            if (game.AutoDraw)
                game.SpriteBatch.End();
            game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null);
            /*
            foreach (var item in Controls)
            {
                item.Value.Draw(gameTime, game.SpriteBatch);
            }
            return;*/
            var n = controlChain.Last;
            while (n != null)
            {
                if (n.Value.Visible)
                    n.Value.Draw(gameTime, game.SpriteBatch);
                n = n.Previous;
            }
        }

        public Control NewControl(string name, Control item)
        {
            name = name.ToLower();
            Controls.Add(name, item);
            Controls[name].Game = game;
            Controls[name].Parent = this;
            Controls[name].Initialize();
            BringControlToFront(item);
            return item;
        }

        public Control GetControl(string name)
        {
            return Controls[name.ToLower()];
        }

        #region Console
        public void AttachToConsole()
        {
            if (game.Console == null) return;

            game.Console.AddCommand("f_hasmouse", f_hasmouse);
            game.Console.AddCommand("f_mouseoffset", f_mouseoffset);
            game.Console.AddCommand("f_mousespeed", f_mousespeed);
            game.Console.AddCommand("f_newcontrol", f_newcontrol);
            game.Console.AddCommand("f_delcontrol", f_delcontrol);
            game.Console.AddCommand("f_selcontrol", f_selcontrol);
            game.Console.AddCommand("f_listcontrols", f_listobjects);
            game.Console.AddCommand("f_reset", f_reset);
        }

        void f_hasmouse(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            HasMouse = bool.Parse(args[1]);
        }

        void f_mousespeed(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [int]");
                return;
            }
            MouseSpeed = int.Parse(args[1]);
        }

        void f_mouseoffset(IList<string> args)
        {
            if (args.Count < 2)
                game.Console.WriteLine(GeometryHelper.Vector2String(MouseOffset));
            else MouseOffset = GeometryHelper.String2Vector(args[1]);
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
                case "container":
                    NewControl(args[1], new Container());
                    break;
                case "window":
                    NewControl(args[1], new Window());
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

        void f_reset(IList<string> args)
        {
            Controls = new Dictionary<string, Control>();
            controlChain = new LinkedList<Control>();
        }
        #endregion
    }
}
