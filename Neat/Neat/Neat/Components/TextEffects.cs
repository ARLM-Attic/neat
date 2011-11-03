using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Neat.Graphics;


namespace Neat.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TextEffects : Microsoft.Xna.Framework.GameComponent
    {
        public TextEffects(NeatGame game)
            : base(game)
        {
            this.Game = game;
        }

        public new NeatGame Game;
        #region Special Effects
        List<string> messages = new List<string>();
        string fx_text = "Get Ready!";
        bool fx_on, fx_end;
        float fx_scale, fx_scale_end, fx_scale_speed,
          fx_alpha, fx_alpha_speed, fx_alpha_end,
          fx_alpha2, fx_alpha2_speed, fx_alpha2_end;
        public Vector2 Offset, Velocity;
        public Color ForeColor = Color.White;

        public void ShoutText(string message)
        {
            messages.Add(message);
        }

        public void UpdateMessages()
        {
            if (messages.Count > 0)
            {
                if (!fx_on && !fx_end)
                {
                    fx_text = messages[0];
                    fx_Start();
                    messages.RemoveAt(0);
                }
            }
        }

        void fx_Start()
        {
            fx_on = true;
            fx_scale = 1.5f;
            fx_scale_end = 0.6f;
            fx_scale_speed = -0.025f;
            fx_alpha = 0f;
            fx_alpha_speed = 0.02f;
            fx_alpha_end = 1f;
            fx_end = false;
            fx_alpha2 = 1f;
            fx_alpha2_speed = -0.125f;
            fx_alpha2_end = 0f;
            Offset = new Vector2(0, 0 + Game.GameHeight / 2 - 100);
            Velocity = new Vector2(0, 0.6f);
        }

        void fx_Update()
        {
            if (fx_on || fx_end)
            {
                Vector2 size = Game.GetFont("fxFont").MeasureString(fx_text) * fx_scale;
                Vector2 position = new Vector2(
                    Game.GameWidth / 2 - size.X / 2 + 0,
                    Offset.Y);
                Offset += Velocity;

                if (fx_on)
                {
                    Game.SpriteBatch.DrawString(
                        Game.GetFont("fxFont"),
                        fx_text,
                        position,
                        GraphicsHelper.GetColorWithAlpha(ForeColor, fx_alpha), 0,
                        Vector2.Zero, // size / 2,
                        fx_scale,
                        SpriteEffects.None, 0);
                    if (fx_scale > fx_scale_end) fx_scale += fx_scale_speed;
                    else { fx_on = false; fx_end = true; }
                    if (fx_alpha < fx_alpha_end) fx_alpha += fx_alpha_speed;
                }
                else
                {
                    Game.SpriteBatch.DrawString(
                        Game.GetFont("fxFont"),
                        fx_text,
                        position,
                        GraphicsHelper.GetColorWithAlpha(ForeColor, fx_alpha2), 0,
                        Vector2.Zero,
                        fx_scale,
                        SpriteEffects.None, 0);
                    if (fx_alpha2 > fx_alpha2_end) fx_alpha2 += fx_alpha2_speed;
                    else fx_end = false;
                }
            }
        }
        #endregion
        
        public override void Update(GameTime gameTime)
        {
            UpdateMessages();
            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            fx_Update();
        }
    }
}
