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
#if WINDOWS
 
 
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.EasyMenus;
using Neat.Graphics;
using Neat.Mathematics;
namespace Neat.GUI
{
    public class Control
    {
        public NeatGame Game;
        public Form Parent;

        public bool Enabled = true;
        public bool Selectable = true;
        public bool Visible = true;
        protected Vector2 _position = Vector2.Zero;
        public Vector2 Position { get { return _position; } set { Move(value); } }
        protected Vector2 _size = new Vector2(200,60);
        public Vector2 Size { get { return _size; } set { Resize(value); } }

        public string Caption = "";
        public string BackgroundImage = "blank";
        public string PushSound = "bleep10";
        public string Font = "FormFont";

        public Color TintColor = Color.White;
        public Color ForeColor = Color.White ;
        public Color MouseHoverColor = Color.YellowGreen  ;
        public Color MouseHoldColor = Color.Yellow  ;
        public Color DisabledColor = Color.Silver;

        public bool IsMouseHold = false;
        public bool IsMouseHovered = false;

        public Control()
        {
        }

        public event XEventHandler OnPress;
        public event XEventHandler OnHold;
        public event XEventHandler OnHover;
        public event XEventHandler OnRelease;

        public string OnPressRun, OnReleaseRun;

        public virtual void Pressed()
        {
            if (!Enabled) return;
            if (OnPressRun != null) Game.Console.Run(OnPressRun);
            if (OnPress != null) OnPress();
        }
        public virtual void Released() { 
            if (!Enabled) return; Game.GetSound(PushSound).Play(1f, 0f, 0f); if (OnRelease != null)  OnRelease();
            if (OnReleaseRun != null) Game.Console.Run(OnReleaseRun);
        }

        public virtual void Holded() { if (!Enabled) return; if (OnHold != null)  OnHold(); }
        public virtual void Hovered() { if (!Enabled) return; if (OnHover != null)  OnHover(); }
        public virtual void Initialize() { }

        public virtual void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
            Rectangle bounds = new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Size.X), (int)(Size.Y));
            spriteBatch.Draw(Game.GetTexture(BackgroundImage), bounds, TintColor); // Draw Background
            
            Vector2 textsize = Game.GetFont(Font).MeasureString(Caption);
            GraphicsHelper.DrawShadowedString(spriteBatch,Game.GetFont(Font), Caption, Position + new Vector2(Size.X / 2 - textsize.X / 2, Size.Y / 2 - textsize.Y / 2),
                ( Enabled ?( IsMouseHold ? MouseHoldColor :
                ( IsMouseHovered ? MouseHoverColor :
                ForeColor)):DisabledColor),
                Color.Black ); 
        }

        public virtual void Resize(Vector2 newSize)
        {
            _size = newSize;
        }

        public virtual void Move(Vector2 newPos)
        {
            _position = newPos;
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void Focused()
        {
            AttachToConsole();
        }

        public virtual void HandleInput(GameTime gameTime)
        {
            if (Enabled)
            {
                IsMouseHovered = GeometryHelper.IsVectorInRectangle(Position, Size, Parent.MousePosition) && (Parent.Size == Vector2.Zero ||
                    GeometryHelper.IsVectorInRectangle(Vector2.Zero, Parent.Size, Parent.MousePosition));
#if WINDOWS
                IsMouseHold = (IsMouseHovered) && (Mouse.GetState().LeftButton == ButtonState.Pressed);
#elif ZUNE
            isMouseHold = (isMouseHovered) && (game.IsPressed(Buttons.A));
#endif

                if (IsMouseHovered) Hovered();
                if (lastMouseHold && !IsMouseHold && IsMouseHovered) Released();

                if (IsMouseHold)
                {
                    if (!Parent.ClickHandled)
                    {
                        Parent.ClickHandled = true;
                        if (!lastMouseHold) Pressed();
                        Holded();
                    }
                }

                lastMouseHold = IsMouseHold;
                lastMouseHovered = IsMouseHovered;
                lastMousePosition = Parent.MousePosition;
            }
        }
        //Privates
        bool lastMouseHovered;
        bool lastMouseHold;
        Vector2 lastMousePosition;

        protected Color GetColorWithAlpha(Color col, float alpha)
        {
            //Vector3 c = col.ToVector3();
            //return new Color(new Vector4(c, alpha));
            return GraphicsHelper.GetColorWithAlpha(col, alpha);
        }

        public CheckBox ToCheckBox()
        {
            return ((CheckBox)this);
        }
        public Box ToBox()
        {
            return ((Box)this);
        }
        public Label ToLabel()
        {
            return ((Label)this);
        }
        public Button ToButton()
        {
            return ((Button)this);
        }
        public Image ToImage()
        {
            return ((Image)this);
        }

        public virtual void AttachToConsole()
        {
            Game.Console.AddCommand("fc_enabled", fc_enabled);
            Game.Console.AddCommand("fc_selectable", fc_selectable);
            Game.Console.AddCommand("fc_visible", fc_visible);
            Game.Console.AddCommand("fc_position", fc_position);
            Game.Console.AddCommand("fc_size", fc_size);
            Game.Console.AddCommand("fc_caption", fc_caption);
            Game.Console.AddCommand("fc_bgimg", fc_bgimg);
            Game.Console.AddCommand("fc_pushsound", fc_pushsound);
            Game.Console.AddCommand("fc_font", fc_font);
            Game.Console.AddCommand("fc_tint", fc_tint);
            Game.Console.AddCommand("fc_forecolor", fc_forecolor);
            Game.Console.AddCommand("fc_hovercolor", fc_hovercolor);
            Game.Console.AddCommand("fc_holdcolor", fc_holdcolor);
            Game.Console.AddCommand("fc_disabledcolor", fc_disabledcolor);
            Game.Console.AddCommand("fc_onpress", fc_onpress);
            Game.Console.AddCommand("fc_onrelease", fc_onrelease);
            Game.Console.AddCommand("fc_selparent", fc_attachparent);
        }

        void fc_onpress(IList<string> args)
        {
            OnPressRun = Game.Console.Args2Str(args, 1);
        }

        void fc_onrelease(IList<string> args)
        {
            OnReleaseRun = Game.Console.Args2Str(args, 1);
        }

        void fc_enabled(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            Enabled = bool.Parse(args[1]);
        }

        void fc_selectable(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            Selectable = bool.Parse(args[1]);
        }

        void fc_visible(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            Visible = bool.Parse(args[1]);
        }

        void fc_position(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [x,y]");
                return;
            }
            Position = GeometryHelper.String2Vector(args[1]);
        }

        void fc_size(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [x,y]");
                return;
            }
            Size = GeometryHelper.String2Vector(args[1]);
        }

        void fc_caption(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            Caption = Game.Console.Args2Str(args,1);
        }

        void fc_bgimg(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            BackgroundImage = args[1];
        }

        void fc_pushsound(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            PushSound = args[1];
        }

        void fc_font(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            Font = args[1];
        }

        void fc_tint(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            TintColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }

        void fc_forecolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            ForeColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }

        void fc_hovercolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            MouseHoverColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }

        void fc_holdcolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            MouseHoldColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }

        void fc_disabledcolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            DisabledColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }

        void fc_attachparent(IList<string> args)
        {
            if (Parent == null) Game.Console.WriteLine("Orphan Control. Cannot attach parent to the console.");
            else Parent.AttachToConsole();
        }
    }
}
