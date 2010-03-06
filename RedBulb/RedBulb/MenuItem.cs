using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Text;
using RedBulb.MenuSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace RedBulb
{
    namespace MenuSystem
    {
        public class MenuItem
        {
            MenuSystem system;
            public string caption = "Menu Item";
            public bool enabled = true;
            public string tag;
            public Color forecolor = Color.CornflowerBlue;
            public float alpha = 0.5f;
            public float alphaV = 0.001f;
            //public Keys shortcut = Keys.OemTilde ;
            public MenuItem(MenuSystem System)
            {
                Initialize(System, "Menu Item", true);
            }
            public MenuItem(MenuSystem System, string Caption)
            {
                Initialize(System, Caption, true);
            }

            public Color GetForeColor()
            {
                Vector3 c = forecolor.ToVector3();

                return new Color(new Vector4(c,alpha));
            }

            public MenuItem(MenuSystem System, string Caption, bool Enabled)
            {
                Initialize(System, Caption, Enabled);
            }

            void Initialize(MenuSystem System, string Caption, bool Enabled)
            {
                system = System;
                caption = Caption;
                enabled = Enabled;
            }

            public void OnFocus()
            {
               
            }

            public void OnSelect()
            {
               
            }

            public delegate void OnFocus2();
            public delegate void OnSelect2();
        }

    }
}