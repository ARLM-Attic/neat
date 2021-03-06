﻿using System;
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
using System.Diagnostics;
using Neat.Graphics;
 
namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        public enum ContentDuplicateBehaviors
        {
            Replace,
            Ignore
        }

        public ContentDuplicateBehaviors ContentDuplicateBehavior = 
            ContentDuplicateBehaviors.Replace;

        public LineBrush BasicLine;

        protected override void LoadContent()
        {
            Debug.WriteLine("Begin NeatGame.LoadContent");
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            LoadSong("Sounds\\blank");
            
            //LoadVideo("Videos\\errorvideo","error");
            videoPlayers.Add(new VideoPlayer());
            
            LoadSound("Sounds\\mute");
            LoadSound("Sounds\\BLEEP10");
            LoadSound("Sounds\\BLEEP3");
            LoadSound("Sounds\\BLEEP6");
            LoadSound(@"Sounds\elegantdying");

            LoadSound(@"Sounds\typewriter-key-1", "console_keystroke");
            LoadSound(@"Sounds\typewriter-backspace-1", "console_backspace");
            LoadSound(@"Sounds\typewriter-paper-roll-up-1", "console_echo");
            LoadSound(@"Sounds\typewriter-return-1", "console_return");
            LoadSound(@"Sounds\typewriter-space-bar-1", "console_space");
            
            LoadTexture("Sprites\\Blank");
            LoadTexture("Sprites\\Solid");
            LoadTexture("Sprites\\Error");
            LoadTexture("Sprites\\Transparent");
            LoadTexture("Sprites\\gamerCardHUD");
            LoadTexture("Sprites\\mediaHUD");
            LoadTexture("Sprites\\menuFocus");
            LoadTexture("Sprites\\buttonBG1", "buttonBG");
            LoadTexture("Sprites\\MessageBox", "MessageBoxWindow");
            LoadTexture("Sprites\\msgbox_text", "MessageBoxWindow_TextBox");
            LoadTexture("Sprites\\msgbox_yesno", "MessageBoxWindow_YesNo");
            LoadTexture("Sprites\\msgbox_button", "MessageBoxWindow_Button");
            LoadTexture("Sprites\\msgbox_buttonFocus", "MessageBoxWindow_ButtonFocused");
            LoadTexture("Sprites\\checked", "checkedbox");
            LoadTexture("Sprites\\Checkbox", "uncheckedbox");
            LoadTexture("Sprites\\icon");
            LoadTexture("Sprites\\neatlogo");

            LoadTexture("Sprites\\pointers");
            CreateSprite("mousepointer", "pointers", new Rectangle(1, 33, 17, 32));
            CreateSprite("handleft", "pointers", new Rectangle(33, 1, 32, 32));
            CreateSprite("handright", "pointers", new Rectangle(1, 1, 32, 32));
            CreateSprite("handleft_pushed", "pointers", new Rectangle(33, 1, 32, 32));
            CreateSprite("handright_pushed", "pointers", new Rectangle(1, 1, 32, 32));

            LoadTexture("Sprites\\WindowSheet");
            CreateSprite("window_tl", "windowsheet", new Rectangle(0, 0, 11, 35));
            CreateSprite("window_tm", "windowsheet", new Rectangle(19, 0, 25, 38));
            CreateSprite("window_tr", "windowsheet", new Rectangle(55, 0, 7, 35));
            CreateSprite("window_v", "windowsheet", new Rectangle(0, 40, 4, 10));
            CreateSprite("window_bl", "windowsheet", new Rectangle(0, 61, 5, 5));
            CreateSprite("window_br", "windowsheet", new Rectangle(57, 61, 5, 5));
            CreateSprite("window_h", "windowsheet", new Rectangle(19, 62, 25, 4));

            CreateSprite("trackbar_lu", "windowsheet", new Rectangle(68, 2, 17, 20));
            CreateSprite("trackbar_lc", "windowsheet", new Rectangle(68, 22, 17, 15));
            CreateSprite("trackbar_lb", "windowsheet", new Rectangle(68, 37, 17, 22));
            CreateSprite("trackbar_ct", "windowsheet", new Rectangle(85, 2, 14, 20));
            CreateSprite("trackbar_cc", "windowsheet", new Rectangle(85, 22, 14, 15));
            CreateSprite("trackbar_cb", "windowsheet", new Rectangle(85, 37, 14, 22));
            CreateSprite("trackbar_ru", "windowsheet", new Rectangle(215, 2, 17, 20));
            CreateSprite("trackbar_rc", "windowsheet", new Rectangle(215, 22, 17, 15));
            CreateSprite("trackbar_rb", "windowsheet", new Rectangle(215, 37, 17, 22));
            CreateSprite("trackbar_gu", "windowsheet", new Rectangle(232, 2, 21, 20));
            CreateSprite("trackbar_gc", "windowsheet", new Rectangle(232, 22, 21, 15));
            CreateSprite("trackbar_gb", "windowsheet", new Rectangle(232, 37, 21, 22));

            LoadTexture("Sprites\\kinect\\tiltbuttons");
            CreateSprite("tilt_up", "tiltbuttons", new Rectangle(0, 0, 128, 128));
            CreateSprite("tilt_down", "tiltbuttons", new Rectangle(0, 128, 128, 128));
            CreateSprite("kinect_seated", "tiltbuttons", new Rectangle(128, 128, 128, 128));
            CreateSprite("kinect_standing", "tiltbuttons", new Rectangle(128, 0, 128, 128));

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
            LoadFont(@"Fonts\FormFont");
            LoadFont(@"Fonts\ElegantFont");

            LoadEffect("Effects\\ColorFilter");
            LoadEffect(@"Effects\Ripple");
            LoadEffect(@"Effects\Crop");
            LoadEffect(@"Effects\TextureWrapper");
            LoadEffect(@"Effects\Circle");

            BasicLine = new LineBrush(GraphicsDevice, 1);

            ElegantTextEngine.LoadContent();

            Debug.WriteLine("Begin Screens LoadContent");
            foreach (var p in Screens)
            {
                Debug.WriteLine("LoadContent: " + p.Key);
                p.Value.LoadContent();
            }
            Debug.WriteLine("End NeatGame.LoadContent");
        }

        public virtual void AddScreens()
        {
            Screens.Add("mainmenu", new EasyMenus.MainMenu(this));
            Screens.Add("quitconfirm", new EasyMenus.QuitConfirmationMenu(this));
            Screens.Add("optionsmenu", new EasyMenus.OptionsMenu(this));
            Screens.Add("ingamemenu", new EasyMenus.InGameMenu(this));

#if KINECT
            Screens["kinect"] = new CalibrateScreen(this);
#endif
        }

        protected string getNameFromPath(string path)
        {
            var fileName = path.Split('\\').Last();
            int len = fileName.LastIndexOf('.');
            if (len < 0) len = fileName.Length;
            fileName = fileName.Substring(0, len);
            return fileName.ToLower();
        }

        protected override void UnloadContent()
        {
            try
            {
#if KINECT
                Kinect.Uninitialize();
#endif
                //Unload any non ContentManager content here
                base.UnloadContent();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in NeatGame.UnloadContent", "Content");
                Debug.WriteLine(e, "Content");
            }
        }
    }
}