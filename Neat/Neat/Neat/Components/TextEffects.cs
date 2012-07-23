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
using Neat.Mathematics;


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

        public void Echo(string message)
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

    public class ElegantTextEngine : Microsoft.Xna.Framework.GameComponent
    {
        public class ElegantMessage
        {
            public string Text;
            public static int TTL = 100; //Frames
            public int Life;
            public bool Dead = false;
            public ElegantMessage(string text, int life=-1)
            {
                Text = text;
                if (life < 0)
                    Life = TTL;
                else Life = life;
            }
        }

        public enum ElegantMessageAlignModes
        {
            Left, Right
        }

        public enum ElegantMessageSortModes
        {
            NewOnTop, OldOnTop
        }

        public new NeatGame Game;
        protected LinkedList<ElegantMessage> Messages = new LinkedList<ElegantMessage>();
        public int MaxLines = 3;
        public string FontName = "elegantfont";
        public Color BackColor = Color.White;
        public Color ForeColor = Color.Black;
        public ElegantMessageAlignModes Align = ElegantMessageAlignModes.Left;
        public ElegantMessageSortModes Sort = ElegantMessageSortModes.NewOnTop;
        public Vector2 Position;
        public bool Mute = false;
        RenderTarget2D target;
        public ElegantTextEngine(NeatGame game)
            : base(game)
        {
            this.Game = game;
        }

        public override void Initialize()
        {
            base.Initialize();
            AttachToConsole();
        }

        public void Echo(string text)
        {
            if (Messages.Count >= MaxLines + 1 && Messages.Count > 0)
            {
                if (Sort == ElegantMessageSortModes.NewOnTop)
                    Messages.RemoveLast();
                else
                    Messages.RemoveFirst();
            }

            var msg = new ElegantMessage(text,
                Messages.Count == 0 ? -1 :
                (Messages.First.Value.Life * 3 / 4) + ElegantMessage.TTL);

            if (Sort == ElegantMessageSortModes.NewOnTop)
                Messages.AddFirst(msg);
            else
                Messages.AddLast(msg);
        }

        public virtual void LoadContent()
        {
        }

        public virtual void GraphicsReinitialized()
        {
            if (target != null)
            {
                Vector2 oldSize = new Vector2(target.Width, target.Height);
                Vector2 newSize = Game.GameSize;
                //Position = Game.GameSize - new Vector2(200);
                Position *= newSize / oldSize;
            }
            else
                Position = Game.GameSize - new Vector2(200);

            target = new RenderTarget2D(Game.GraphicsDevice, Game.GameWidth, Game.GameHeight, false,
                SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        }

        public override void Update(GameTime gameTime)
        {
            var n = Messages.First;
            while (n != null)
            {
                var next = n.Next;
                n.Value.Life--;
                if (n.Value.Life <= 0)
                    Messages.Remove(n);
                n = next;
            }
        }

        public void PlayDeathSound()
        {
            if (Mute) return;
            Game.PlaySound("elegantdying");
        }

        public virtual void Draw(GameTime gameTime)
        {
            var font = Game.GetFont(FontName);
            var solid = Game.GetTexture("solid");
            var n = Messages.First;
            float y = Position.Y;
            const float vspacing = 4.0f;
            const float hspacing = 10.0f;
            Effect crop = Game.GetEffect("crop");
            while (n != null)
            {
                Vector2 size = font.MeasureString(n.Value.Text);
                float rectWidth = (size.X + 2 * hspacing);
                float coef = (float)n.Value.Life * 10 / (float)ElegantMessage.TTL;
                rectWidth = Math.Min(rectWidth, rectWidth * coef);
                if (coef < 1.0f && !n.Value.Dead)
                {
                    n.Value.Dead = true;
                    PlayDeathSound();
                }
                Rectangle bounds = new Rectangle(
                        (int)((Align == ElegantMessageAlignModes.Left ? 0 : -size.X) - hspacing + Position.X),
                        (int)(y),
                        (int)(rectWidth),
                        (int)(size.Y + 2.0f * vspacing));

                Game.SpriteBatch.Draw(solid,
                    bounds, BackColor);
                y += vspacing;

                Game.SpriteBatch.End();
                Game.PushTarget(target);
                Game.GraphicsDevice.Clear(Color.Transparent);
                Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Game.SpriteBatch.DrawString(font, n.Value.Text,
                    new Vector2((Align == ElegantMessageAlignModes.Left ? 0 : -size.X) + Position.X,
                        y),
                    ForeColor);
                Game.SpriteBatch.End();
                Game.PopTarget();
                Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    null, DepthStencilState.None, RasterizerState.CullNone, crop);
                crop.Parameters["top"].SetValue((float)bounds.Top/(float)Game.GameHeight);
                crop.Parameters["down"].SetValue((float)bounds.Bottom / (float)Game.GameHeight);
                crop.Parameters["left"].SetValue((float)bounds.Left / (float)Game.GameWidth);
                crop.Parameters["right"].SetValue((float)bounds.Right / (float)Game.GameWidth);
                Game.SpriteBatch.Draw(target, Vector2.Zero, ForeColor);
                /*Game.SpriteBatch.DrawString(font, n.Value.Text,
                    new Vector2((Align == ElegantMessageAlignModes.Left ? 0 : -size.X) + Position.X, 
                        y),
                    ForeColor);*/
                Game.RestartBatch();
                y += vspacing + size.Y;
                n = n.Next;
            }
        }

        #region Console
        public void AttachToConsole()
        {
            if (Game.Console == null) return;
            Game.Console.AddCommand("et_reset", et_reset);
            Game.Console.AddCommand("et_echo", et_echo);
            Game.Console.AddCommand("et_ttl", et_ttl);
            Game.Console.AddCommand("et_newfirst", et_newfirst);
            Game.Console.AddCommand("et_oldfirst", et_oldfirst);
            Game.Console.AddCommand("et_rtl", et_rtl);
            Game.Console.AddCommand("et_ltr", et_ltr);
            Game.Console.AddCommand("et_forecolor", et_forecolor);
            Game.Console.AddCommand("et_backcolor", et_backcolor);
            Game.Console.AddCommand("et_position", et_position);
            Game.Console.AddCommand("et_lines", et_lines);
            Game.Console.AddCommand("et_mute", et_mute);
        }

        void et_reset(IList<string> args)
        {
            Messages.Clear();
        }

        void et_echo(IList<string> args)
        {
            Echo(Game.Console.Args2Str(args, 1));
        }

        void et_ttl(IList<string> args)
        {
            if (args.Count > 1)
                ElegantMessage.TTL = int.Parse(args[1]);
            else
                Game.Console.WriteLine(ElegantMessage.TTL.ToString());
        }

        void et_newfirst(IList<string> args)
        {
            Sort = ElegantMessageSortModes.NewOnTop;
        }

        void et_oldfirst(IList<string> args)
        {
            Sort = ElegantMessageSortModes.OldOnTop;
        }

        void et_rtl(IList<string> args)
        {
            Align = ElegantMessageAlignModes.Right;
        }

        void et_ltr(IList<string> args)
        {
            Align = ElegantMessageAlignModes.Left;
        }

        void et_forecolor(IList<string> args)
        {
            ForeColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }

        void et_backcolor(IList<string> args)
        {
            BackColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }

        void et_position(IList<string> args)
        {
            if (args.Count > 1)
                Position = GeometryHelper.String2Vector(args[1]);
            else
                Game.Console.WriteLine(GeometryHelper.Vector2String(Position));
        }

        void et_lines(IList<string> args)
        {
            if (args.Count > 1)
                MaxLines = int.Parse(args[1]);
            else
                Game.Console.WriteLine(MaxLines.ToString());
        }

        void et_mute(IList<string> args)
        {
            if (args.Count > 1)
                Mute = bool.Parse(args[1]);
            else
                Game.Console.WriteLine(Mute.ToString());
        }
        #endregion
    }
}
