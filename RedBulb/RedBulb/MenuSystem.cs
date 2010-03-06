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
using RedBulb;
using RedBulb.MenuSystem;
namespace RedBulb
{
    namespace MenuSystem
    {
        public class MenuSystem
        {
            public Vector2 position;
            
            RedBulbGame game;
            SpriteFont font;
            bool enabled = false;

            const float minItemAlpha = 0.1f;
            const float maxItemAlpha = 0.7f;
            const float itemAlphaRate = 0.002f;
            
            float selectedItemColorRate = -0.001f;
            float selectedItemColor = 1f;
            
            public List<MenuItem> items;
            public int selectedItem=0;

            public bool repeat = true;
            public bool enableShortcuts = false ;
            public Vector2 itemsOffset = new Vector2(0, 20f);
            public Vector2 selectedItemOffset = new Vector2(5f, 0);
            public Color defaultItemColor = Color.CornflowerBlue;
            public Color selectedItemForeground = Color.Yellow;
            public Color disabledItemForeground = Color.Silver;
            float longestItem = 0f;
            
            public MenuSystem(RedBulbGame g, Vector2 b, SpriteFont f)
            {
                font = f;
                game = g;
                
                position = b;
                Initialize();
            }

            void Initialize()
            {
                items = new List<MenuItem>();
                itemSelector = Rectangle.Empty;
            }

            public MenuItem GetLastMenuItem()
            {
                return items[items.Count - 1];
            }
            public void Enable()
            {
                enabled = true;
            }

            public void Update(GameTime gameTime)
            {
                if (enabled)
                {
                    Behave(gameTime);
                    HandleInput(gameTime);
                }
            }

            float bracketDistance = 0;
            const float bracketVelocity = 0.5f;
            const float maxBracketDistance = 15;
            bool bracketDistanceIncreasing = true;

            Rectangle itemSelector;
            void Behave(GameTime gameTime)
            {
                if (bracketDistanceIncreasing)
                {
                    bracketDistance+=bracketVelocity ;
                    if (bracketDistance > maxBracketDistance) bracketDistanceIncreasing = !bracketDistanceIncreasing;
                }
                else
                {
                    bracketDistance-=bracketVelocity ;
                    if (bracketDistance < 0) bracketDistanceIncreasing = !bracketDistanceIncreasing;
                }

                foreach (MenuItem item in items)
                {
                    item.alpha += item.alphaV;
                    if (item.alpha > maxItemAlpha )
                    {
                        item.alphaV *= -1f;
                        item.alpha = maxItemAlpha;
                    }
                    else if (item.alpha < minItemAlpha )
                    {
                        item.alphaV *= -1f;
                        item.alpha = minItemAlpha;
                    }
                }
                Vector3 c = selectedItemForeground.ToVector3();
                if (selectedItemColor > 1f)
                {
                    selectedItemColor = 1f;
                    selectedItemColorRate = -Math.Abs(selectedItemColorRate);
                
                }
                else if (c.Y < 0.5f)
                {
                    selectedItemColor = 0.5f;
                    selectedItemColorRate = Math.Abs(selectedItemColorRate);

                }
                selectedItemColor += selectedItemColorRate;
                c.Y = selectedItemColor;
                selectedItemForeground = new Color(c);
            }

            void HandleInput(GameTime gameTime)
            {
#if ZUNE
                if (IsTapped(Buttons.A)) 
#elif WINDOWS
                if (IsTapped (Keys.Enter))
#endif
                    { game.GetSound("bleep3").Play(1f,0,0); }
#if ZUNE
                if (IsTapped( Buttons.DPadDown)) 
#elif WINDOWS
                if (IsTapped(Keys.Down))
#endif
                    { Bleep(); NextItem(); }
                else
#if ZUNE
                if (IsTapped( Buttons.DPadUp)) 
#elif WINDOWS
                    if (IsTapped(Keys.Up))
#endif
                    { Bleep(); PrevItem(); }
#if WINDOWS
                if (enableShortcuts) HandleShortcuts();
#endif
            }
            void Bleep()
            {
                game.GetSound("bleep10").Play(1f,0,0);
                //game.bleepSound.Play(1f);
            }
            void HandleShortcuts()
            {

            }
            void NextItem()
            {
                selectedItem++;
                if (selectedItem >= items.Count)
                {
                    if (repeat) selectedItem = 0;
                    else selectedItem = items.Count - 1;
                }
                if (!items[selectedItem].enabled)
                {
                    if (selectedItem == items.Count - 1)
                    {
                        if (!repeat) PrevItem();
                        else NextItem();
                    }
                    else
                        NextItem();
                }
            }
            void PrevItem()
            {

                if (selectedItem == 0)
                {
                    if (repeat) selectedItem = items.Count - 1;
                    //else selectedItem = 0;
                }
                else
                    selectedItem--;
                if (!items[selectedItem].enabled)
                {
                    if (selectedItem == 0)
                    {
                        if (!repeat) NextItem();
                        else PrevItem();
                    }
                    else
                        PrevItem();
                }
            }

#if ZUNE
            bool IsTapped(Buttons button)
            {
                return game.IsTapped(button);
            }

            bool IsPressed(Buttons button)
            {
                return game.IsPressed( button);
            }
#elif WINDOWS
            bool IsTapped(Keys key)
            {
                return game.IsTapped(key);
            }

            bool IsPressed(Keys key)
            {
                return game.IsPressed(key);
            }
#endif
            public void Draw(GameTime gameTime)
            {
                if (enabled) Render(gameTime);   
            }

            Color GetShadowColorFromAlpha(float alpha)
            {
                return game.GetColorWithAlpha(Color.Black, alpha);
                //Vector3 c = Color.Black.ToVector3();
                //return new Color(new Vector4(c, alpha));
            }
            Color GetColorWithAlpha(Color col,float alpha)
            {
                return game.GetColorWithAlpha(col, alpha);
                //Vector3 c = col.ToVector3();
                //return new Color(new Vector4(c, alpha));
            }
            public void Render(GameTime gameTime)
            {
                for (int i = 0; i < items.Count ; i++)
                {
                    Color drawColor = items[i].GetForeColor();
                    bool isSelected = (selectedItem == i);
                    //if (selectedItem == i) drawColor = selectedItemForeground;
                    if (!items[i].enabled) drawColor = GetColorWithAlpha( disabledItemForeground, items[i].alpha);
                    
                    game.DrawShadowedString(
                        font,
                        (items[i].caption),
                        (position-centerOffset)+i*itemsOffset+(isSelected?selectedItemOffset:Vector2.Zero),
                        (isSelected?selectedItemForeground :drawColor),
                        (isSelected ? Color.Black : GetShadowColorFromAlpha(items[i].alpha ))
                        );
                    if (isSelected)
                    {
                        game.spriteBatch.DrawString(
                            font,
                            "| ",
                            ((position - centerOffset) + i * itemsOffset + (isSelected ? selectedItemOffset : Vector2.Zero)) -
                            new Vector2(font.MeasureString("| ").X + bracketDistance, 0),
                            Color.Brown);
                        game.spriteBatch.DrawString(
                            font,
                            " |",
                            ((position - centerOffset) + i * itemsOffset + (isSelected ? selectedItemOffset : Vector2.Zero)) +
                            (new Vector2(longestItem + bracketDistance, 0)),
                            Color.Brown);
                    }
                }

                game.spriteBatch.Draw(
                    game.getTexture("menuFocus"),
                    itemSelector,
                    Color.White);
            }

            public void AddItem(string Caption)
            {
                AddItem(Caption, true);
            }

            Vector2 centerOffset=Vector2.Zero;
            public void AddItem(string Caption, bool Enabled)
            {
                items.Add(new MenuItem(this, Caption, Enabled));
                RecalculateSizes();
                GetLastMenuItem().forecolor = defaultItemColor;
                GetLastMenuItem().alpha = (float)items.Count * 0.1f;
                GetLastMenuItem().alphaV = itemAlphaRate;
            }
            public void RecalculateSizes()
            {
                foreach (MenuItem item in items)
                {
                    if (font.MeasureString(item.caption).X > longestItem) longestItem = font.MeasureString(item.caption).X;
                }
                centerOffset = new Vector2(longestItem / 2, ((items.Count - 1) * itemsOffset.Y + font.MeasureString("|").Y) / 2);
            }
            
        }

    }
}
