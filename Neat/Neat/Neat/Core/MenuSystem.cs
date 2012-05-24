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
using Neat.Mathematics;
 

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
                        if (Items.Count > value)
                        {
                            Items[value].Focused();
                            if (Items[value].Enabled && !string.IsNullOrEmpty(Items[value].OnFocusScript))
                                game.Console.Run(Items[value].OnFocusScript);
                        }
                    }
                    catch { }
                }
            }
            public bool Repeat = true;
            public bool EnableShortcuts = false ;
            public bool EnableSoundEffects = true;
            public Vector2 ItemsOffset = new Vector2(0, 65f);
            public Vector2 SelectedItemOffset = new Vector2(5f, 0);
            public Color DefaultItemColor = Color.CornflowerBlue;
            public Color SelectedItemForeground = Color.Yellow;
            public Color DisabledItemForeground = Color.Silver;
            public Color ShadowColor = Color.Black;
            public bool DrawShadow = true;
            float longestItem = 0f;
            public bool FixedAlpha = false;
            public static string SelectSound = "bleep3";
            public static string BleepSound = "bleep10";

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
                    //HandleInput(gameTime);
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

                if (!FixedAlpha)
                {
                    foreach (MenuItem item in Items)
                    {
                        item.Alpha += item.AlphaV;
                        if (item.Alpha > maxItemAlpha)
                        {
                            item.AlphaV *= -1f;
                            item.Alpha = maxItemAlpha;
                        }
                        else if (item.Alpha < minItemAlpha)
                        {
                            item.AlphaV *= -1f;
                            item.Alpha = minItemAlpha;
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
            }

            void select(int i)
            {
                game.PlaySound(SelectSound);
                Items[i].Selected();
                if (Items[i].Enabled && !string.IsNullOrEmpty(Items[i].OnSelectScript))
                    game.Console.Run(Items[i].OnSelectScript);
            }

            public void HandleInput(GameTime gameTime)
            {
                if (game.IsTapped(Keys.Enter) || game.IsTapped(Buttons.A) || game.IsTapped(Buttons.Start))
                {
                    select(SelectedItem);
                }
                if (game.IsTapped(Keys.Down) || game.IsTapped(Buttons.DPadDown) || game.IsTapped(Buttons.LeftThumbstickDown))
                    { Bleep(); NextItem(); }
                else
                    if (game.IsTapped(Keys.Up) || game.IsTapped(Buttons.DPadUp) || game.IsTapped(Buttons.LeftThumbstickUp))
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
#if WINDOWS
                if (game.IsMouseClicked())
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (!Items[i].Enabled) continue;
                        bool isSelected = (SelectedItem == i);
                        var ipos = (Position - centerOffset) + i * ItemsOffset + (isSelected ? SelectedItemOffset : Vector2.Zero) - new Vector2(10,0);
                        var size = font.MeasureString(Items[i].Caption) + new Vector2(20, 0);

                        if (GeometryHelper.Vectors2Rectangle(ipos, size).Contains(
                            GeometryHelper.Vector2Point(game.MousePosition)))
                        {
                            select(i);
                            break;
                        }


                    }
                }
#endif

#if KINECT
                if (game.Touch.Enabled)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        bool breakAll = false;
                        if (!Items[i].Enabled) continue;
                        bool isSelected = (SelectedItem == i);
                        var ipos = (Position - centerOffset) + i * ItemsOffset + (isSelected ? SelectedItemOffset : Vector2.Zero) - new Vector2(10, 0);
                        var size = font.MeasureString(Items[i].Caption) + new Vector2(20, 0);

                        foreach (var item in game.Touch.TrackPoints)
                        {
                            if (GeometryHelper.Vectors2Rectangle(ipos, size).Intersects(
                                new Rectangle((int)(item.Position.X), (int)(item.Position.Y), 32, 32)) &&
                                item.Hold && !item.LastHold)
                            {
                                select(i);
                                game.Touch.Reset(new TimeSpan(0,0,0,1));
                                //item.Hold = item.LastHold;
                                breakAll = true;
                                break;
                            }
                        }

                        if (breakAll) break;
                    }
                }
#endif
            }
            void Bleep()
            {
                if (EnableSoundEffects)
                    game.PlaySound(BleepSound);
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
                    if (!Items[i].Enabled) drawColor = GraphicsHelper.GetColorWithAlpha( DisabledItemForeground, Items[i].Alpha);

                    GraphicsHelper.DrawShadowedString(game.SpriteBatch,
                        font,
                        (Items[i].Caption),
                        (Position-centerOffset)+i*ItemsOffset+(isSelected?SelectedItemOffset:Vector2.Zero),
                        (isSelected?SelectedItemForeground :drawColor),
                        (DrawShadow ? (isSelected ? Color.Black : GraphicsHelper.GetColorWithAlpha(ShadowColor, Items[i].Alpha)) : Color.Transparent)
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

            Vector2 centerOffset=Vector2.Zero;
            public MenuItem AddItem(string Caption, bool Enabled = true, string OnFocusScript = null, string OnSelectScript = null)
            {
                Items.Add(new MenuItem(this, Caption, Enabled));
                RecalculateSizes();
                var lastItem = GetLastMenuItem();
                lastItem.Forecolor = DefaultItemColor;
                lastItem.Alpha = (float)Items.Count * 0.1f;
                lastItem.AlphaV = itemAlphaRate;
                lastItem.OnFocusScript = OnFocusScript;
                lastItem.OnSelectScript = OnSelectScript;
                return lastItem;
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
