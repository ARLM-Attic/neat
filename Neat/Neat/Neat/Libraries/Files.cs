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
 
namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            LoadSong("Sounds\\blank");
            
            LoadVideo("Videos\\errorvideo","error");
            videoPlayers.Add(new VideoPlayer());
            
            LoadSound("Sounds\\mute");
            LoadSound("Sounds\\BLEEP10");
            LoadSound("Sounds\\BLEEP3");
            LoadSound("Sounds\\BLEEP6");

            LoadTexture("Sprites\\Blank");
            LoadTexture("Sprites\\Solid");
            LoadTexture("Sprites\\Error");
            LoadTexture("Sprites\\Transparent");
            LoadTexture("Sprites\\gamerCardHUD");
            LoadTexture("Sprites\\mediaHUD");
            LoadTexture("Sprites\\menuFocus");
            LoadTexture("Sprites\\buttonBG");
            LoadTexture("Sprites\\MessageBox", "MessageBoxWindow");
            LoadTexture("Sprites\\msgbox_text", "MessageBoxWindow_TextBox");
            LoadTexture("Sprites\\msgbox_yesno", "MessageBoxWindow_YesNo");
            LoadTexture("Sprites\\msgbox_button", "MessageBoxWindow_Button");
            LoadTexture("Sprites\\msgbox_buttonFocus", "MessageBoxWindow_ButtonFocused");
            LoadTexture("Sprites\\checked", "checkedbox");
            LoadTexture("Sprites\\Checkbox", "uncheckedbox");
            LoadTexture("Sprites\\mouse", "mousePointer");
            LoadTexture("Sprites\\icon");
            LoadTexture("Sprites\\neatlogo");

            NormalFont = Content.Load<SpriteFont>("Fonts\\normal");
            LoadFont("Normal", NormalFont);
            LoadFont("Fonts\\smallFont");
            LoadFont("Fonts\\menuFont");
            LoadFont("Fonts\\Cambria");
            LoadFont("Fonts\\Calibri");
            LoadFont("Fonts\\messageBoxTitleFont");
            LoadFont("Fonts\\messageBoxTextFont");
            LoadFont("Fonts\\consolefont");
            LoadFont("Fonts\\fxFont");

            LoadEffect("Effects\\ColorFilter");
            LoadEffect("Effects\\ColorAdder");

            foreach (var p in Screens)
                p.Value.LoadContent();
        }

        public virtual void AddScreens()
        {
            Screens.Add("mainmenu", new EasyMenus.MainMenu(this));
            Screens.Add("quitconfirm", new EasyMenus.QuitConfirmationMenu(this));
            Screens.Add("optionsmenu", new EasyMenus.OptionsMenu(this));
            Screens.Add("ingamemenu", new EasyMenus.InGameMenu(this));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    }
}