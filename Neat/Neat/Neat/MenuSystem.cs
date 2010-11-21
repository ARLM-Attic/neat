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
using Neat.Graphics;
 

namespace Neat
{
    namespace MenuSystem
    {
        public class MenuSystem
        {
            public Vector2 Position;
            
            NeatGame game;
            SpriteFont font;
            bool enabled = false;

            const float minItemAlpha = 0.1f;
            const float maxItemAlpha = 0.7f;
            const float itemAlphaRate = 0.002f;
            
            float selectedItemColorRate = -0.001f;
            float selectedItemColor = 1f;
            
            public List<MenuItem> Items;

            int _selectedItem = 0;
            public int SelectedItem
            {
                get { return _selectedItem; }
                set
                {
                    _selectedItem = value;
                    try
                    {
                        if (Items.Count > value) Items[value].Focused();
                    }
                    catch { }
                }
            }
            public bool Repeat = true;
            public bool EnableShortcuts = false ;
            public Vector2 ItemsOffset = new Vector2(0, 65f);
            public Vector2 SelectedItemOffset = new Vector2(5f, 0);
            public Color DefaultItemColor = Color.CornflowerBlue;
            public Color SelectedItemForeground = Color.Yellow;
            public Color DisabledItemForeground = Color.Silver;
            float longestItem = 0f;
            
            public MenuSystem(NeatGame g, Vector2 b, SpriteFont f)
            {
                font = f;
                game = g;
                
                Position = b;
                Initialize();
            }

            void Initialize()
            {
                Items = new List<MenuItem>();
                itemSelector = Rectangle.Empty;
            }

            public MenuItem GetLastMenuItem()
            {
                return Items[Items.Count - 1];
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

                foreach (MenuItem item in Items)
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
                Vector3 c = SelectedItemForeground.ToVector3();
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
                SelectedItemForeground = new Color(c);
            }

            void HandleInput(GameTime gameTime)
            {

#if ZUNE
                if (IsTapped(Buttons.A)) 
#elif WINDOWS || WINDOWS_PHONE
                if (game.IsTapped (Keys.Enter))
#endif
                    { game.GetSound("bleep3").Play(1f,0,0); 
                    Items[ SelectedItem].Selected();}
#if ZUNE
                 (IsTapped( Buttons.DPadDown)) 
#elif WINDOWS || WINDOWS_PHONE
                if (game.IsTapped(Keys.Down))
#endif
                    { Bleep(); NextItem(); }
                else
#if ZUNE
                if (IsTapped( Buttons.DPadUp)) 
#elif WINDOWS || WINDOWS_PHONE
                    if (game.IsTapped(Keys.Up))
#endif
                    { Bleep(); PrevItem(); }
#if WINDOWS || WINDOWS_PHONE
                if (EnableShortcuts) HandleShortcuts();
#endif
#if WINDOWS_PHONE
                if (game.IsTouched())
                {
                    for (int i=0; i<items.Count; i++)
                        if (items[i].enabled)
                        {
                            bool isSelected = (selectedItem == i);
                            Rectangle r = Geometry2D.Vectors2Rectangle(
                                (position - centerOffset) + i * itemsOffset + (isSelected ? selectedItemOffset : Vector2.Zero),
                                font.MeasureString(items[i].caption));
                            if (game.IsTouched(r))
                            {
                                if (!isSelected) 
                                    selectedItem = i;
                                else items[i].Selected();
                                break;
                            }
                        }
                }

#endif
            }
            void Bleep()
            {
                game.GetSound("bleep10").Play(1f,0,0);
            }
            void HandleShortcuts()
            {

            }
            void NextItem()
            {
                SelectedItem++;
                if (SelectedItem >= Items.Count)
                {
                    if (Repeat) SelectedItem = 0;
                    else SelectedItem = Items.Count - 1;
                }
                if (!Items[SelectedItem].Enabled)
                {
                    if (SelectedItem == Items.Count - 1)
                    {
                        if (!Repeat) PrevItem();
                        else NextItem();
                    }
                    else
                        NextItem();
                }
            }
            void PrevItem()
            {

                if (SelectedItem == 0)
                {
                    if (Repeat) SelectedItem = Items.Count - 1;
                    //else selectedItem = 0;
                }
                else
                    SelectedItem--;
                if (!Items[SelectedItem].Enabled)
                {
                    if (SelectedItem == 0)
                    {
                        if (!Repeat) NextItem();
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

            public void Render(GameTime gameTime)
            {
                for (int i = 0; i < Items.Count ; i++)
                {
                    Color drawColor = Items[i].GetForeColor();
                    bool isSelected = (SelectedItem == i);
                    //if (selectedItem == i) drawColor = selectedItemForeground;
                    if (!Items[i].Enabled) drawColor = GraphicsHelper.GetColorWithAlpha( DisabledItemForeground, Items[i].alpha);

                    GraphicsHelper.DrawShadowedString(game.SpriteBatch,
                        font,
                        (Items[i].Caption),
                        (Position-centerOffset)+i*ItemsOffset+(isSelected?SelectedItemOffset:Vector2.Zero),
                        (isSelected?SelectedItemForeground :drawColor),
                        (isSelected ? Color.Black :GraphicsHelper.GetShadowColorFromAlpha(Items[i].alpha ))
                        );
                    if (isSelected)
                    {
                        game.SpriteBatch.DrawString(
                            font,
                            "| ",
                            ((Position - centerOffset) + i * ItemsOffset + (isSelected ? SelectedItemOffset : Vector2.Zero)) -
                            new Vector2(font.MeasureString("| ").X + bracketDistance, 0),
                            Color.Brown);
                        game.SpriteBatch.DrawString(
                            font,
                            " |",
                            ((Position - centerOffset) + i * ItemsOffset + (isSelected ? SelectedItemOffset : Vector2.Zero)) +
                            (new Vector2(longestItem + bracketDistance, 0)),
                            Color.Brown);
                    }
                }

                game.SpriteBatch.Draw(
                    game.GetTexture("menuFocus"),
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
                Items.Add(new MenuItem(this, Caption, Enabled));
                RecalculateSizes();
                GetLastMenuItem().Forecolor = DefaultItemColor;
                GetLastMenuItem().alpha = (float)Items.Count * 0.1f;
                GetLastMenuItem().alphaV = itemAlphaRate;
            }
            public void RecalculateSizes()
            {
                if (font == null) return;
                foreach (MenuItem item in Items)
                {
                    if (item.Caption.Trim().Length > 0)
                    {
                        var s = font.MeasureString(item.Caption).X;
                        if (s > longestItem) longestItem = s;
                    }
                }
                centerOffset = new Vector2(longestItem / 2, ((Items.Count - 1) * ItemsOffset.Y + font.MeasureString("|").Y) / 2);
            }
            
        }

    }
}
