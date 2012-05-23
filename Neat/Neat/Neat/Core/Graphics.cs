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
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.Graphics;
using Neat.MenuSystem;
using Neat.Mathematics;
using System.Diagnostics;
 

namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        public enum StretchModes { None, Stretch, Fit, Center };

        public Color GameBackgroundColor = Color.Black;
        public Color OutputBackgroundColor = Color.Black;

        public bool AutoDraw = true;
        public bool AutoClear = true;
        public bool ShowMouse = false;
        public bool ShowConsoleOnBottom = false;
        public bool Disable2DRendering = false;
        StretchModes _stretchMode = StretchModes.None;
        public RenderTarget2D DefaultTarget { get { 
            return _renderTarget; } set { _renderTarget = value; } }
        public Point OutputResolution;
        
        bool _landscape = false;

        RenderTarget2D _renderTarget;
        Vector2 _vecOrigin, _vecDest, _vecSize;

        protected Stack<RenderTarget2D> targetStack = new Stack<RenderTarget2D>();

        public void PushTarget(RenderTarget2D target, bool switchTarget = true)
        {
            targetStack.Push(target);
            if (switchTarget) GraphicsDevice.SetRenderTarget(target);
        }

        public RenderTarget2D PopTarget(bool switchTarget = true)
        {
            RenderTarget2D target = DefaultTarget;
            if (targetStack.Count > 0) target = targetStack.Pop();
            if (switchTarget)
            {
                GraphicsDevice.SetRenderTarget(CurrentTarget);
            }
            return target;
        }

        public RenderTarget2D CurrentTarget
        {
            get
            {
                if (targetStack.Count > 0) return targetStack.Peek();
                return DefaultTarget;
            }
        }

        public StretchModes StretchMode 
        {
            get { return _stretchMode; }
            set
            {
                _stretchMode = value;
                ResetRenderTarget();
            }
        }

        void ResetRenderTarget()
        {
            if (GraphicsDevice == null)
            {
                Debug.WriteLine("GraphicsDevice is null. Cannot ResetRenderTarget();");
                return;
            }

            _renderTarget = new RenderTarget2D(GraphicsDevice, GameWidth, GameHeight, 
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 1, RenderTargetUsage.PreserveContents);
            if (StretchMode == StretchModes.Center)
            {
                _vecDest.X = (OutputResolution.X - GameWidth) / 2f;
                _vecDest.Y = (OutputResolution.Y - GameHeight) / 2f;
                _vecSize.X = GameWidth;
                _vecSize.Y = GameHeight;
            }
            else if (StretchMode == StretchModes.Fit)
            {
                var ar = (float)GameWidth/(float)GameHeight;
                var aro = (float)OutputResolution.X/(float)OutputResolution.Y;
                if (ar > aro)
                {
                    _vecSize.X = OutputResolution.X;
                    _vecSize.Y = OutputResolution.X / ar;
                    _vecDest.X = 0;
                    _vecDest.Y = (OutputResolution.Y - _vecSize.Y) / 2f;
                }
                else
                {
                    _vecSize.Y = OutputResolution.Y;
                    _vecSize.X = OutputResolution.Y * ar;
                    _vecDest.Y = 0;
                    _vecDest.X = (OutputResolution.X - _vecSize.X) / 2f;
                }
            }
            else if (StretchMode == StretchModes.Stretch)
            {
                _vecDest.X = 0;
                _vecDest.Y = 0;
                _vecSize.X = OutputResolution.X;
                _vecSize.Y = OutputResolution.Y;
            }
            else
            {
                _vecDest.X = 0;
                _vecDest.Y = 0;
                _vecSize.X = GameWidth;
                _vecSize.Y = GameHeight;
            }
        }

        public bool Landscape
        {
            get
            {
                return _landscape;
            }
            set
            {
                if (value)
                {
                    _renderTarget = new RenderTarget2D(GraphicsDevice, GameWidth, GameHeight);
                    _vecOrigin = new Vector2(GameWidth/2f, GameHeight/2f);
                    _vecDest = new Vector2(_vecOrigin.Y, _vecOrigin.X);
                }
                _landscape = value;
            }
        }
        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Clear(Color c)
        {
            Graphics.GraphicsDevice.Clear(c);
        }

        protected override void Draw(GameTime gameTime)
        {
            gamestime = gameTime;


            if (!Disable2DRendering)
            {
                //if (Landscape || StretchMode != StretchModes.None)
                    GraphicsDevice.SetRenderTarget(_renderTarget);

                if (AutoClear)
                    GraphicsDevice.Clear(GameBackgroundColor);

                    if (AutoDraw)
                        SpriteBatch.Begin();
            }

#if LIVE
            if (SignedInGamer.SignedInGamers.Count > 0 || !NeedSignIn || !ForceSignIn)
#endif

#if KINECT
            if (Kinect.Draw)
            {
                Kinect.Render(gameTime);
            }
#endif
            Render(gameTime);
            if (!Disable2DRendering)
            {
                if (ShowMouse
#if LIVE
                && !Guide.IsVisible
#endif
)
                {
#if WINDOWS
                    DrawMouse(MousePosition);
#endif
                }

                if (AutoDraw)
                    SpriteBatch.End();
#if WINDOWS
                if (HasConsole/* && Console.IsActive*/)
                {
                    SpriteBatch.Begin();
                    Console.Draw(ShowConsoleOnBottom);
                    SpriteBatch.End();
                }


                SpriteBatch.Begin();
                TextEffects.Draw(gameTime);
                ElegantTextEngine.Draw(gameTime);
                SpriteBatch.End();
#endif

                if (Landscape)
                {
                    GraphicsDevice.SetRenderTarget(null);

                    SpriteBatch.Begin();
                    SpriteBatch.Draw(
                        _renderTarget,
                        _vecDest,
                        null,
                        Color.White,
                        MathHelper.PiOver2,
                        _vecOrigin,
                        1f,
                        SpriteEffects.None,
                        0);

                    SpriteBatch.End();
                }
                else if (_renderTarget != null)// if (StretchMode != StretchModes.None)
                {
                    GraphicsDevice.SetRenderTarget(null);
                    if (AutoClear)
                        Clear(OutputBackgroundColor);
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(
                        _renderTarget,
                        GeometryHelper.Vectors2Rectangle(_vecDest, _vecSize), Color.White);
                    SpriteBatch.End();
                }
            }

            base.Draw(gameTime);
        }
        public Color MouseTint = Color.White;
        protected virtual void DrawMouse(Vector2 pos)
        {
            var tx = GetSlice("mousePointer");
            SpriteBatch.Draw(tx.Texture, pos, tx.Crop, MouseTint);
        }

        public Transition Transition = null;
        protected virtual void Render(GameTime gameTime)
        {
            if (Screens.ContainsKey(ActiveScreen))
            {
                if (Transition == null || Transition.Finished)
                    RenderScreen(ActiveScreen, gameTime);
                else
                    Transition.Draw(gameTime);
            }
        }

        
        public void RenderScreen(string name, GameTime gameTime)
        {
            Screens[name].BeforeRender(gameTime);
            Screens[name].Render(gameTime);
            Screens[name].AfterRender(gameTime);
        }
        
        public void Write(string text, Vector2 position)
        {
            GraphicsHelper.DrawShadowedString(SpriteBatch, NormalFont, text, position, Color.White);
        }

        public void DrawCircle(Vector2 center, float radius, Color color, Vector2 offset, float thickness=1)
        {
            var fx = GetEffect("circle");
            if (AutoDraw) SpriteBatch.End();

            fx.CurrentTechnique = fx.Techniques["DrawCircle"];
            fx.Parameters["thickness"].SetValue(thickness / radius);
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null,
                null, null, fx);
            SpriteBatch.Draw(GetTexture("solid"), GeometryHelper.Vectors2Rectangle(
                offset + center - new Vector2(radius), new Vector2(radius * 2)), color);
            if (AutoDraw) RestartBatch();
            else SpriteBatch.End();
        }

        public void DrawCircle(Vector2 center, float radius, Color color, float thickness=1)
        {
            DrawCircle(center, radius, color, Vector2.Zero, thickness);
        }

        public void FillCircle(Vector2 center, float radius, Color color, Vector2 offset)
        {
            var fx = GetEffect("circle");
            if (AutoDraw) SpriteBatch.End();

            fx.CurrentTechnique = fx.Techniques["FillCircle"];
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null,
                null, null, fx);
            SpriteBatch.Draw(GetTexture("solid"), GeometryHelper.Vectors2Rectangle(
                offset + center - new Vector2(radius), new Vector2(radius * 2)), color);
            if (AutoDraw) RestartBatch();
            else SpriteBatch.End();
        }

        public void FillCircle(Vector2 center, float radius, Color color)
        {
            FillCircle(center, radius, color, Vector2.Zero);
        }
    }
}