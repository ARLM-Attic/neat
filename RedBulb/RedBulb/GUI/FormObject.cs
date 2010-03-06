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
   // public delegate void XEventHandler();
    public class FormObject
    {
        public RedBulbGame game;

        public bool enabled = true;
        public bool selectable = true;
        public bool visible = true;
        public Vector2 position = Vector2.Zero ;
        public Vector2 size = new Vector2(200,60);

        public string caption = "";
        public string backgroundImage = "blank";
        public string pushSound = "bleep10";
        public string font = "MenuFont";

        public Color tintColor = Color.White;
        public Color foreColor = Color.White ;
        public Color mouseHoverColor = Color.YellowGreen  ;
        public Color mouseHoldColor = Color.Yellow  ;
        public Color disabledColor = Color.Silver;

        public bool isMouseHold = false;
        public bool isMouseHovered = false;

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
            if (!enabled) return;
            
            if (OnPress != null) OnPress();
        }
        public virtual void Released() { if (!enabled) return; game.GetSound(pushSound).Play(1f,0f,0f); if (OnRelease != null)  OnRelease(); }
        public virtual void Holded() { if (!enabled) return; if (OnHold != null)  OnHold(); }
        public virtual void Hovered() { if (!enabled) return; if (OnHover != null)  OnHover(); }
        public virtual void Initialize() { }

        public virtual void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
            Rectangle bounds = new Rectangle((int)(position.X), (int)(position.Y), (int)(size.X), (int)(size.Y));
            spriteBatch.Draw(game.getTexture(backgroundImage), bounds, tintColor); // Draw Background
            
            Vector2 textsize = game.GetFont(font).MeasureString(caption);
            game.DrawShadowedString (game.GetFont(font), caption, position + new Vector2(size.X / 2 - textsize.X / 2, size.Y / 2 - textsize.Y / 2),
                ( enabled ?( isMouseHold ? mouseHoldColor :
                ( isMouseHovered ? mouseHoverColor :
                foreColor)):disabledColor),
                Color.Black ); 
        }

        public virtual void Update(GameTime gameTime)
        {
            if (enabled)
            {
                isMouseHovered = Geometry2D.IsVectorInRectangle(position, size, game.mousePosition);
#if WINDOWS
                isMouseHold = (isMouseHovered) && (Mouse.GetState().LeftButton == ButtonState.Pressed);
#elif ZUNE
            isMouseHold = (isMouseHovered) && (game.IsPressed(Buttons.A));
#endif

                if (isMouseHovered) Hovered();
                if (lastMouseHold && !isMouseHold && isMouseHovered) Released();
                if (isMouseHold && !lastMouseHold) Pressed();
                if (isMouseHold) Holded();

                lastMouseHold = isMouseHold;
                lastMouseHovered = isMouseHovered;
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
            return game.GetColorWithAlpha(col, alpha);
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
    }
}
