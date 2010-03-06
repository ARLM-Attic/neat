#region References
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
//using UltimaScroll.IBLib;
#endregion
namespace RedBulb
{
    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
    {
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadSong("Sounds\\blank");

            LoadVideo("Videos\\errorvideo","error");

            videoPlayers.Add(new Microsoft.Xna.Framework.Media.VideoPlayer());

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
            LoadFont("smallFont", "smallFont");
            LoadFont("menuFont", "menuFont");
            LoadFont("Cambria", "Cambria");
            LoadFont("Calibri", "Calibri");
            LoadFont("messageBoxTitleFont", "messageBoxTitleFont");
            LoadFont("messageBoxTextFont", "messageBoxTextFont");
            LoadFont("consolefont", "consolefont");
            LoadFont("fxFont", "fxFont");
            normalFont = Content.Load<SpriteFont>("normal");
            LoadFont("Normal", normalFont);
            InitializeParts0();
        }

        void InitializeParts0()
        {
            parts = new Dictionary<string, GamePart>();

            CreateParts();
            LoadPartsFiles();
            InitializeParts();
            ActivatePart("mainmenu");

        }

        void LoadPartsFiles()
        {
            foreach (var p in parts)
            {
                p.Value.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    }
}