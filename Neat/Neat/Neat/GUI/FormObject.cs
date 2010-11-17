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
using Neat.Graphics;
using Neat.Mathematics;
namespace Neat.GUI
{
   // public delegate void XEventHandler();
    public class FormObject
    {
        public NeatGame game;

        public bool Enabled = true;
        public bool Selectable = true;
        public bool Visible = true;
        public Vector2 Position = Vector2.Zero ;
        public Vector2 Size = new Vector2(200,60);

        public string Caption = "";
        public string BackgroundImage = "blank";
        public string PushSound = "bleep10";
        public string Font = "MenuFont";

        public Color TintColor = Color.White;
        public Color ForeColor = Color.White ;
        public Color MouseHoverColor = Color.YellowGreen  ;
        public Color MouseHoldColor = Color.Yellow  ;
        public Color DisabledColor = Color.Silver;

        public bool IsMouseHold = false;
        public bool IsMouseHovered = false;

        public FormObject()
        {
            Initialize();
        }

        public event XEventHandler OnPress;
        public event XEventHandler OnHold;
        public event XEventHandler OnHover;
        public event XEventHandler OnRelease;

        public virtual void Pressed()
        {
            if (!Enabled) return;
            
            if (OnPress != null) OnPress();
        }
        public virtual void Released() { if (!Enabled) return; game.GetSound(PushSound).Play(1f,0f,0f); if (OnRelease != null)  OnRelease(); }
        public virtual void Holded() { if (!Enabled) return; if (OnHold != null)  OnHold(); }
        public virtual void Hovered() { if (!Enabled) return; if (OnHover != null)  OnHover(); }
        public virtual void Initialize() { }

        public virtual void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
            Rectangle bounds = new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Size.X), (int)(Size.Y));
            spriteBatch.Draw(game.getTexture(BackgroundImage), bounds, TintColor); // Draw Background
            
            Vector2 textsize = game.GetFont(Font).MeasureString(Caption);
            GraphicsHelper.DrawShadowedString(spriteBatch,game.GetFont(Font), Caption, Position + new Vector2(Size.X / 2 - textsize.X / 2, Size.Y / 2 - textsize.Y / 2),
                ( Enabled ?( IsMouseHold ? MouseHoldColor :
                ( IsMouseHovered ? MouseHoverColor :
                ForeColor)):DisabledColor),
                Color.Black ); 
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                IsMouseHovered = GeometryHelper.IsVectorInRectangle(Position, Size, game.mousePosition);
#if WINDOWS
                IsMouseHold = (IsMouseHovered) && (Mouse.GetState().LeftButton == ButtonState.Pressed);
#elif ZUNE
            isMouseHold = (isMouseHovered) && (game.IsPressed(Buttons.A));
#endif

                if (IsMouseHovered) Hovered();
                if (lastMouseHold && !IsMouseHold && IsMouseHovered) Released();
                if (IsMouseHold && !lastMouseHold) Pressed();
                if (IsMouseHold) Holded();

                lastMouseHold = IsMouseHold;
                lastMouseHovered = IsMouseHovered;
                lastMousePosition = game.mousePosition;
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
            game.Console.AddCommand("fo_enabled", fo_enabled);
            game.Console.AddCommand("fo_selectable", fo_selectable);
            game.Console.AddCommand("fo_visible", fo_visible);
            game.Console.AddCommand("fo_position", fo_position);
            game.Console.AddCommand("fo_size", fo_size);
            game.Console.AddCommand("fo_caption", fo_caption);
            game.Console.AddCommand("fo_bgimg", fo_bgimg);
            game.Console.AddCommand("fo_pushsound", fo_pushsound);
            game.Console.AddCommand("fo_font", fo_font);
            game.Console.AddCommand("fo_tint", fo_tint);
            game.Console.AddCommand("fo_forecolor", fo_forecolor);
            game.Console.AddCommand("fo_hovercolor", fo_hovercolor);
            game.Console.AddCommand("fo_holdcolor", fo_holdcolor);
            game.Console.AddCommand("fo_disabledcolor", fo_disabledcolor);
        }

        void fo_enabled(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            Enabled = bool.Parse(args[1]);
        }

        void fo_selectable(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            Selectable = bool.Parse(args[1]);
        }

        void fo_visible(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            Visible = bool.Parse(args[1]);
        }

        void fo_position(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [x,y]");
                return;
            }
            Position = GeometryHelper.String2Vector(args[1]);
        }

        void fo_size(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [x,y]");
                return;
            }
            Size = GeometryHelper.String2Vector(args[1]);
        }

        void fo_caption(IList<string> args)
        {
            if (args.Count < 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            Caption = game.Console.Args2Str(args,1);
        }

        void fo_bgimg(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            BackgroundImage = args[1];
        }

        void fo_pushsound(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            PushSound = args[1];
        }

        void fo_font(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            Font = args[1];
        }

        void fo_tint(IList<string> args)
        {
            if (args.Count < 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            TintColor = game.Console.ParseColor(game.Console.Args2Str(args, 1));
        }

        void fo_forecolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            ForeColor = game.Console.ParseColor(game.Console.Args2Str(args, 1));
        }

        void fo_hovercolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            MouseHoverColor = game.Console.ParseColor(game.Console.Args2Str(args, 1));
        }

        void fo_holdcolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            MouseHoldColor = game.Console.ParseColor(game.Console.Args2Str(args, 1));
        }

        void fo_disabledcolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            DisabledColor = game.Console.ParseColor(game.Console.Args2Str(args, 1));
        }
    }
}
