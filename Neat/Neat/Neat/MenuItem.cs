using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Text;
using Neat.MenuSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Neat
{
    namespace MenuSystem
    {
        public class MenuItem
        {
            MenuSystem system;
            public string Caption = "Menu Item";
            public bool Enabled = true;
            public string Tag;
            public Color Forecolor = Color.CornflowerBlue;
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
                Vector3 c = Forecolor.ToVector3();

                return new Color(new Vector4(c,alpha));
            }

            public MenuItem(MenuSystem System, string Caption, bool Enabled)
            {
                Initialize(System, Caption, Enabled);
            }

            void Initialize(MenuSystem System, string Caption, bool Enabled)
            {
                system = System;
                this.Caption = Caption;
                this.Enabled = Enabled;
            }

            public void Focused()
            {
                if (!Enabled) return;
                if (OnFocus != null) OnFocus();
            }

            public void Selected()
            {
                if (!Enabled) return;
                if (OnSelect != null) OnSelect();
            }

            public event XEventHandler OnFocus;
            public event XEventHandler OnSelect;
        }

    }
}