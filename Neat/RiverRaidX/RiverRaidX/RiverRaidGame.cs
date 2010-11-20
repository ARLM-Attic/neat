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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.EasyMenus;
using Neat.GUI;
using Neat.Mathematics;
using Neat.Graphics;
using System.IO;

namespace RiverRaidX
{
    public class RiverRaidGame : Screen
    {
        public RiverRaidGame(NeatGame Game)
            : base(Game)
        {
        }

        #region FIELDS
        enum Items
        {
            Fuel,
            Ship,
            Heli,
            House,
            Tree,
            Bridge,
            Jet
        }

        class Entity
        {
            public string textureName;
            public Vector2 position;
            public Vector2 size;
            public bool noClip;
            public float speed;
            public bool flip;
            public int wakeUpDistance;
            public Items itype;
            public int scorePoints=0;
            public Polygon mesh;
        }

        LineBrush lb;

        float stillPlayerWidth;
        float movingPlayerWidth;

        int _score;
        int score { get { return _score; } set { if (_score != value) { _score = value; scoreValue.Caption = _score.ToString(); scoreValue.Reset(); } } }
        float _fuel;
        float fuel { get { return _fuel; } set { _fuel = value > maxFuel ? maxFuel : value; UpdateGauge(); } }
        int _lives;
        int lives { get { return _lives; } set { if (_lives != value) { _lives = value; livesValue.Caption = _lives.ToString(); livesValue.Reset(); } } }
        float rocketY;

        float yOffset;
        float ySpeed;
        float rocketSpeed;
       
        bool rocketLaunched = false;
        float xForce;
        float dampCoEf = 0.97f;
        float pxVel = 0f;
        float speedBoostRate = 1.5f;
        float maxFuel = 1000;
        int initLives = 3;
        float fuelUseRate;

        Vector2 originalGameSize = new Vector2(1280, 720);
        Vector2 scale;
        Vector2 rocketSize;
        Vector2 pPos, pSize;
        Vector2 ar;
        Polygon playerMesh;

        List<Vector2> outerWalls;
        List<Vector2> innerWalls;
        List<Entity> entities;
        List<Polygon> walls;
        List<float> checkpoints;
        List<string> levelPaths;
        List<Color> grassColors;

        int lastLoadedLevel;
        int currentLevel, startingLevel;
        int stage;

        Label scoreText, livesText;
        FancyLabel scoreValue, livesValue;
        Image fuelBg;
        Box fuelArrow;
        
        Color grassColor;

        int blinkFrame = 0;
        int blinkTime = 30;
        int blinkPeriod = 2;

        int deathTime = 40;
        int deathFrame = 0;
        bool toggleBlink = false;

        Vector4 siren = Color.Black.ToVector4();
        #endregion
        #region Setup
        public override void Initialize()
        {
            base.Initialize();

            lb = new LineBrush(game.GraphicsDevice, 1);

            Reset();
        }

        public override void LoadContent()
        {
            game.LoadTexture(@"Sprites\RiverRaid\bridge");
            game.LoadTexture(@"Sprites\RiverRaid\fuel");
            game.LoadTexture("heli", 1, @"Sprites\RiverRaid\heli", @"Sprites\RiverRaid\heli1");
            game.LoadTexture(@"Sprites\RiverRaid\house");
            game.LoadTexture(@"Sprites\RiverRaid\jet");
            game.LoadTexture(@"Sprites\RiverRaid\raider");
            game.LoadTexture(@"Sprites\RiverRaid\raider_mv");
            game.LoadTexture(@"Sprites\RiverRaid\road");
            game.LoadTexture(@"Sprites\RiverRaid\Ship");
            game.LoadTexture(@"Sprites\RiverRaid\tree");
            game.LoadTexture(@"Sprites\RiverRaid\gauge");

            game.LoadFont(@"Fonts\Vani35");
            base.LoadContent();
        }

        void InitForm()
        {
            Form = new Neat.GUI.Form(game);
            Form.MouseSpeed = 0;

            Box b = new Box();
            Form.NewControl("dashboard", b);
            b.TintColor = Color.LightGray;
            b.Position = new Vector2(0, game.GameHeight - 120);
            b.Size = new Vector2(game.GameWidth, 120);

            fuelBg = new Image();
            Form.NewControl("fuelBg", fuelBg);
            fuelBg.BackgroundImage = "gauge";
            fuelBg.Position = new Vector2(600, game.GameHeight - 100);
            fuelBg.CenterX();
            fuelBg.AutoSize = true;
            var ft = game.GetTexture(fuelBg.BackgroundImage);

            fuelArrow = new Box();
            Form.NewControl("fuelArrow", fuelArrow);
            fuelArrow.Size = new Vector2(8, 30);
            fuelArrow.TintColor = Color.Brown;
            UpdateGauge();

            scoreText = new Label();
            Form.NewControl("scoreText", scoreText);
            scoreText.ForeColor = Color.Black;
            scoreText.Font = "Vani35";
            scoreText.Position = new Vector2(fuelBg.Position.X - 250, game.GameHeight - 120);// new Vector2(20, game.GameHeight - 120);
            scoreText.Caption = "Score";
            scoreText.DrawShadow = false;

            livesText = new Label();
            Form.NewControl("livesText", livesText);
            livesText.ForeColor = Color.Black;
            livesText.Font = "Vani35";
            livesText.Position = new Vector2(fuelBg.Position.X + 400, game.GameHeight - 120);//new Vector2(320, game.GameHeight - 120);
            livesText.Caption = "Lives";
            livesText.DrawShadow = false;

            scoreValue = new FancyLabel();
            Form.NewControl("scoreValue", scoreValue);
            scoreValue.ForeColor = new Color(47, 172, 102);
            scoreValue.Font = "Vani35";
            scoreValue.Position = new Vector2(fuelBg.Position.X - 200, game.GameHeight - 80);// new Vector2(40, game.GameHeight - 80);
            scoreValue.Caption = "0";
            scoreValue.Speed *= 2;
            scoreValue.DrawShadow = false;

            livesValue = new FancyLabel();
            Form.NewControl("livesValue", livesValue);
            livesValue.ForeColor = scoreValue.ForeColor;
            livesValue.Font = scoreValue.Font;
            livesValue.Position = new Vector2(fuelBg.Position.X+450, game.GameHeight - 80);//new Vector2(340, game.GameHeight - 80);
            livesValue.Caption = lives.ToString();
            livesValue.DrawShadow = false;
            
        }

        public override void Activate()
        {
            game.BackGroundColor = Color.DarkBlue;
            game.Console.BackColor = GraphicsHelper.GetColorWithAlpha(Color.Tomato, 0.5f);
            base.Activate();
        }

        public void Reset()
        {
            InitForm();

            yOffset = 0;
            score = 0;
            fuel = maxFuel; 
            lives = initLives;
            pxVel = 0f;
            stage = -1;
            ar = new Vector2((float)game.GameWidth / originalGameSize.X, 
                (float)game.GameHeight / originalGameSize.Y);
            scale = new Vector2(0.4f)* ar;

            rocketSize = new Vector2(5, 20) * ar;
            fuelUseRate = 1 * ar.Y;
            xForce = 150f * ar.X ;
            ySpeed = 7f * ar.Y;
            rocketSpeed = 40f * ar.Y;

            var playerTexture = game.GetTexture("raider");
            var heliTexture = game.GetTexture("heli");
            var fuelTexture = game.GetTexture("fuel");
            var shipTexture = game.GetTexture("ship");
            var jetTexture = game.GetTexture("jet");
            var bridgeTexture = game.GetTexture("bridge");

            stillPlayerWidth = playerTexture.Width * scale.X;
            movingPlayerWidth = game.GetTexture("raider_mv").Width * scale.X;
            playerMesh = Polygon.BuildRectangle(
                (game.GameWidth - playerTexture.Width * scale.X) / 2f,
                (game.GameHeight - 100),
                playerTexture.Width * scale.X,
                playerTexture.Height * scale.Y);

            grassColors = new List<Color>();
            grassColors.Add(Color.ForestGreen);
            grassColors.Add(Color.DarkGreen);
            grassColor = grassColors[0];

            outerWalls = new List<Vector2>();
            innerWalls = new List<Vector2>();
            entities = new List<Entity>();
            checkpoints = new List<float>();

            siren.X = 0.0f;
            siren.W = 0.0f;
            
            levelPaths = new List<string>();
            levelPaths.Add("lvl1.rrx");
            levelPaths.Add("lvl2.rrx");
            levelPaths.Add("lvl3.rrx");
            levelPaths.Add("lvl4.rrx");
            levelPaths.Add("lvl5.rrx");
            levelPaths.Add("lvl6.rrx");

            LoadLevel(levelPaths[ lastLoadedLevel = 0]);
            currentLevel = startingLevel = lastLoadedLevel;

            CreateRects();
            yOffset -= game.GameHeight;
        }
        
        void Die()
        {
            stage = 1;
            deathFrame = deathTime;
        }

        void Reborn()
        {
            lives--;
            stage = -1;
            yOffset = 0;
            fuel = maxFuel;
            pxVel = 0f;
            var playerTexture = game.GetTexture("raider");
            playerMesh = Polygon.BuildRectangle(
                (game.GameWidth - playerTexture.Width * scale.X) / 2f,
                (game.GameHeight - 100),
                playerTexture.Width * scale.X,
                playerTexture.Height * scale.Y);
            outerWalls = new List<Vector2>();
            innerWalls = new List<Vector2>();
            entities = new List<Entity>();
            checkpoints = new List<float>();
            startingLevel = currentLevel;
            if (currentLevel > levelPaths.Count) currentLevel %= levelPaths.Count;
            LoadLevel(levelPaths[currentLevel]);
            lastLoadedLevel = currentLevel;
            CreateRects();
            yOffset -= game.GameHeight;
            rocketLaunched = false;
        }
  
        void LoadLevel(string filename)
        {
            if (!File.Exists(filename))
            {
                game.Console.WriteLine(filename + " does not exists.");
                return;
            }

            bool firstLevel = outerWalls.Count == 1;
            Vector2 offset = new Vector2(0, game.GameHeight);
            if (outerWalls.Count > 1)
                offset.Y = outerWalls[outerWalls.Count - 1].Y;
            else
            {
                outerWalls.Add(new Vector2(
                    (game.GameWidth - ( game.GetTexture("bridge").Width * scale.X))/2f, ySpeed * speedBoostRate * 10000));
            }
            
            StreamReader sr = new StreamReader(filename);
            if (sr.ReadLine() != "~inner")
            {
                //TODO: File is damaged
                sr.Close();
                return;
            }
            string s;
            while ((s = sr.ReadLine().Trim()) != "~outer")
            {
                var v = GeometryHelper.String2Vector(s)*ar + offset;
                v.X = (int)v.X;
                v.Y = (int)v.Y;
                innerWalls.Add(v);
            }
            while ((s = sr.ReadLine().Trim()) != "~entities")
            {
                var v = GeometryHelper.String2Vector(s)*ar+offset;
                v.X = (int)v.X;
                v.Y = (int)v.Y;
                outerWalls.Add(v);
            }
            while (!sr.EndOfStream)
            {
                Entity e = new Entity();
                e.textureName = "";
                e.position =ar* GeometryHelper.String2Vector(sr.ReadLine())+offset;
                e.flip = bool.Parse(sr.ReadLine());
                Items itype = (Items)(sr.ReadLine()[0]);
                e.itype = itype;
                e.scorePoints = 10;
                e.wakeUpDistance = (yOffset > -2000) ? game.RandomGenerator.Next(10, 200) :
                            -game.RandomGenerator.Next(10, 1000);
                switch (itype)
                {
                    case Items.Fuel:
                        e.textureName = "fuel";
                        e.speed = 0;
                        e.flip = false;
                        e.wakeUpDistance = 0;
                        e.scorePoints = 500;
                        break;
                    case Items.Heli:
                        e.textureName = "heli";
                        e.speed = 5;
                        e.noClip = false;
                        e.scorePoints = 80;
                        break;
                    case Items.House:
                        e.textureName = "house";
                        e.speed = 0;
                        e.noClip = true;
                        break;
                    case Items.Jet:
                        e.textureName = "jet";
                        e.speed = 10;
                        e.noClip = true;
                        e.scorePoints = 120;
                        e.wakeUpDistance = game.GameHeight;
                        break;
                    case Items.Ship:
                        e.textureName = "ship";
                        e.speed = 3;
                        e.noClip = false;
                        e.scorePoints = 100;
                        break;
                    case Items.Tree:
                        e.textureName = "tree";
                        e.speed = 0;
                        e.noClip = true;
                        break;
                }
                var t = game.GetTexture(e.textureName);
                e.size = new Vector2(t.Width * scale.X, t.Height * scale.Y);
                if (e.textureName != "") entities.Add(e);
            }
            //if (!firstLevel)
            {
                var lastwall = outerWalls[outerWalls.Count - 1];
                var b = new Entity();
                b.textureName = "bridge";
                b.itype = Items.Bridge;
                b.speed = 0;
                b.noClip = true;
                b.position = new Vector2(lastwall.X, lastwall.Y - game.GetTexture(b.textureName).Height * scale.Y);
                b.size = new Vector2(game.GetTexture(b.textureName).Width, game.GetTexture(b.textureName).Height) * scale;
                entities.Add(b);
                checkpoints.Add(lastwall.Y - game.GetTexture(b.textureName).Height * scale.Y);
                outerWalls.Add(new Vector2(
                   (game.GameWidth - (game.GetTexture("bridge").Width * scale.X)) / 2f,checkpoints[checkpoints.Count-1]));
            }
            sr.Close();

            CreateRects();
        }
        
        void CreateRects()
        {
            var half = game.GameWidth / 2;
            walls = new List<Polygon>();
            for (int i = 1; i < outerWalls.Count; i++)
            {
                walls.Add(Polygon.BuildRectangle(
                    new Rectangle(0, (int)(outerWalls[i].Y),
                        (int)(outerWalls[i - 1].X), (int)(outerWalls[i - 1].Y - outerWalls[i].Y))));
                walls.Add(Polygon.BuildRectangle(
                   new Rectangle((int)(game.GameWidth - outerWalls[i - 1].X), (int)(outerWalls[i].Y),
                       (int)(outerWalls[i - 1].X), (int)(outerWalls[i - 1].Y - outerWalls[i].Y))));
            }

            for (int i = 1; i < innerWalls.Count; i++)
            {
                if (innerWalls[i - 1].X == 0) continue;
                walls.Add(Polygon.BuildRectangle(
                    new Rectangle((int)(half - innerWalls[i - 1].X), (int)(innerWalls[i].Y),
                        (int)(2 * innerWalls[i - 1].X), (int)(innerWalls[i - 1].Y - innerWalls[i].Y))));
            }
        }
        #endregion
        #region Update

        Vector2 mouse;
        bool touch = false;

        const float thres = 6;
        void UpdateGauge()
        {
            float fr = (float)fuel / (float)maxFuel;
            if (fr < 0) fr = 0;
            float pos = fuelBg.Position.X + 3;
            float width = game.GetTexture(fuelBg.BackgroundImage).Width - 6 - fuelArrow.Size.X;
            fuelArrow.Position.X = (pos + fr * width);
            fuelArrow.Position.Y = fuelBg.Position.Y + 3;
        }

        public override void Behave(GameTime gameTime)
        {
            if (outerWalls.Count == 0 || (outerWalls[outerWalls.Count - 1].Y + yOffset > 0 && levelPaths.Count > lastLoadedLevel))
            {
                game.Console.WriteLine("Level " + lastLoadedLevel.ToString() + " loaded.");
                if (lastLoadedLevel + 1 >= levelPaths.Count) lastLoadedLevel = -1;
                LoadLevel(levelPaths[++lastLoadedLevel]);
            }

            mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            int center = game.GameWidth / 2;

            playerMesh.GetPositionAndSize(out pPos, out pSize);
            touch = false;

            if (stage == 1)
            {
                deathFrame--;
                if (deathFrame < 0)
                {
                    Reborn();
                }
            }
            else if (stage == -1)
            {
                yOffset += ySpeed * speedBoostRate;
                var yS = game.GameHeight - 100 - Form.GetControl("dashboard").Size.Y;
                var yO = -pPos.Y + yS;
                if (yOffset >= yO)
                {
                    stage++;
                }

                for (int i = 0; i < entities.Count; i++)
                {
                    entities[i].mesh = Polygon.BuildRectangle(entities[i].position.X,
                            entities[i].position.Y, entities[i].size.X, entities[i].size.Y);
                }
            }
            else
            {
                //if (!paused)
                {
                    var yStart = game.GameHeight - 100 - Form.GetControl("dashboard").Size.Y;
                    yOffset = -pPos.Y + yStart;

                    fuel -= fuelUseRate;
                    if (fuel <= 0) Die();

                    if (rocketLaunched)
                    {
                        rocketY -= rocketSpeed;
                        if (rocketY + yOffset < 0) rocketLaunched = false;
                    }

                    Polygon rocket = Polygon.BuildRectangle(
                        (pSize.X - rocketSize.X) / 2 + pPos.X,
                        rocketY,
                        rocketSize.X, 2 * rocketSize.Y + rocketSpeed);

                    float d;
                    for (int i = 0; i < entities.Count; i++)
                    {
                        d = entities[i].position.Y + entities[i].size.Y + yOffset;
                        if (d > 0)
                        {
                            if (d > game.GameHeight)
                            {
                                //game.console.WriteLine("Removed " + entities[i].itype.ToString());
                                entities.RemoveAt(i);
                                continue;
                            }

                            if (pPos.Y < entities[i].wakeUpDistance + entities[i].position.Y && currentLevel >= 1) 
                                entities[i].position.X += entities[i].speed * (entities[i].flip ? -1 : 1);

                            if (entities[i].position.X < 0) entities[i].position.X = game.GameWidth - 1;
                            else if (entities[i].position.X > game.GameWidth) entities[i].position.X = 1;

                            entities[i].mesh = Polygon.BuildRectangle(entities[i].position.X,
                                entities[i].position.Y, entities[i].size.X, entities[i].size.Y);
                            if (entities[i].itype == Items.Fuel && Polygon.Collide(entities[i].mesh, playerMesh))
                                fuel = fuel >= maxFuel ? maxFuel : fuel + 8 * fuelUseRate;
                            else if (Polygon.Collide(entities[i].mesh, playerMesh)) { Die(); break; }

                            if (rocketLaunched && Polygon.Collide(entities[i].mesh, rocket))
                            {
                                //TODO: EXPLOSION CODE
                                score += entities[i].scorePoints;
                                if (entities[i].itype == Items.Bridge)
                                {
                                    currentLevel++;
                                    blinkFrame = blinkTime;
                                }

                                entities.RemoveAt(i);
                                rocketLaunched = false;
                            }
                        }
                    }

                    foreach (var item in walls)
                    {
                        if (rocketLaunched)
                        {
                            if (Polygon.Collide(rocket, item)) rocketLaunched = false;
                        }
                        if (Polygon.Collide(item, playerMesh)) { Die(); touch = true; break; }

                        for (int i = 0; i < entities.Count; i++)
                        {
                            var e = entities[i];
                            Vector2 MTD;
                            if (!e.noClip)
                            {
                                if (Polygon.Collide(e.mesh, item, out MTD))
                                {
                                    e.position += MTD;
                                    e.flip = !e.flip;
                                }
                            }
                        }
                    }
                }
            }

            base.Behave(gameTime);
        }
        #endregion
        #region Input
        public override void HandleInput(GameTime gameTime)
        {
            if (stage == 0)
            {
                var t = (float)gameTime.ElapsedGameTime.Milliseconds * 0.002f;
                if (game.IsPressed(Keys.Left)) { if (pxVel > 0) pxVel = 0; pxVel -= xForce * t; }
                else if (game.IsPressed(Keys.Right)) { if (pxVel < 0) pxVel = 0; pxVel += xForce * t; }
                else pxVel = 0;
                playerMesh.Offset(new Vector2(pxVel * t, -ySpeed * (game.IsPressed(Keys.Up) ? speedBoostRate : (game.IsPressed(Keys.Down) ? 1 / speedBoostRate : 1))));
                pxVel *= dampCoEf;

                if (game.IsPressed(Keys.Space) && !rocketLaunched)
                {
                    rocketLaunched = true;
                    rocketY = pPos.Y - rocketSize.Y - 1;
                }
            }

            if (game.IsTapped(Keys.R)) Reset();
            if (game.IsTapped(Keys.D)) Die();
            base.HandleInput(gameTime);
        }
        #endregion

        #region Draw
        void ProcessShaders()
        {
            if (blinkFrame > 0)
            {
                blinkFrame--;
                if (blinkFrame % blinkPeriod == 0)
                {
                    game.GetEffect("ColorFilter").Parameters["mulColor"].SetValue(GraphicsHelper.GetRandomColor().ToVector4());
                    toggleBlink = !toggleBlink;
                }
                if (toggleBlink)
                {
                    game.SpriteBatch.End();
                    game.SpriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, game.GetEffect("ColorFilter"));
                }
            }
            else if (stage == 1)
            {
                game.SpriteBatch.End();
                game.GetEffect("ColorFilter").Parameters["mulColor"].SetValue(Color.OrangeRed.ToVector4());
                game.SpriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, game.GetEffect("ColorFilter"));
            }
            else if (fuel < maxFuel * 0.25f)
            {
                siren.X += 0.1f;
                if (siren.X > 1) siren.X = 0.0f;
                game.GetEffect("ColorAdder").Parameters["ovlColor"].SetValue(siren);
                game.UseEffect("ColorAdder");
            }
        }
        public override void Render(GameTime gameTime)
        {
            ProcessShaders();

            var half = game.GameWidth / 2;

            ////////////////////
            // Draw Exterior
            ////////////////////
            float currentY = 0;
            if (checkpoints.Count > 0)
                currentY = checkpoints[0];
            var checkPointNo = 0;
            grassColor = grassColors[startingLevel%2];

            for (int i = 1; i < outerWalls.Count; i++)
            {
               game.SpriteBatch.Draw(
                   game.GetTexture("solid"),
                   new Rectangle(0, (int)(outerWalls[i].Y + yOffset), 
                       (int)(outerWalls[i-1].X), (int)(outerWalls[i-1].Y - outerWalls[i].Y)),grassColor);
               game.SpriteBatch.Draw(
                  game.GetTexture("solid"),
                  new Rectangle((int)(game.GameWidth - outerWalls[i-1].X), (int)(outerWalls[i].Y + yOffset),
                      (int)(outerWalls[i - 1].X), (int)(outerWalls[i - 1].Y - outerWalls[i].Y)), grassColor);
               if (currentY >= outerWalls[i-1].Y && checkPointNo + 1 < checkpoints.Count)
               {
                   currentY = checkpoints[++checkPointNo];
                   grassColor = grassColors[(startingLevel+checkPointNo) % 2];
               }
            }

            ////////////////////
            // Draw Interior
            ////////////////////
            currentY = 0;
            checkPointNo = 0;
            if (checkpoints.Count > 0)
                currentY = checkpoints[0];
            grassColor = grassColors[startingLevel % 2];

            for (int i = 1; i < innerWalls.Count; i++)
            {
                if (currentY >= innerWalls[i - 1].Y && checkPointNo + 1 < checkpoints.Count)
                {
                    currentY = checkpoints[++checkPointNo];
                    grassColor = grassColors[(startingLevel + checkPointNo) % 2];
                }
                game.SpriteBatch.Draw(
                    game.GetTexture("solid"),
                    new Rectangle((int)(half - innerWalls[i - 1].X), (int)(innerWalls[i].Y + yOffset),
                        (int)(2 * innerWalls[i - 1].X), (int)(innerWalls[i - 1].Y - innerWalls[i].Y)), grassColor);
            }


            ////////////////////
            // Draw Entities
            ////////////////////
            for (int i = 0; i < entities.Count; i++)
            {
                var item = entities[i];
                game.SpriteBatch.Draw(game.GetTexture(item.textureName), 
                    GeometryHelper.Vectors2Rectangle(new Vector2(item.position.X, item.position.Y + yOffset), 
                    item.size), null,Color.White,0, Vector2.Zero, item.flip ? SpriteEffects.FlipHorizontally:SpriteEffects.None,0);
            }


            ////////////////////
            // Draw Checkpoints
            ////////////////////
            var road = game.GetTexture("road");
            var bridge = game.GetTexture("bridge");
            for (int i = 0; i < checkpoints.Count; i++)
            {
                game.SpriteBatch.Draw(road, new Rectangle(0, (int)(checkpoints[i]+yOffset),
                    (int)(game.GameWidth - bridge.Width * scale.X) / 2, (int)(road.Height * scale.Y)), Color.White);
                game.SpriteBatch.Draw(road, new Rectangle( (int)(bridge.Width*scale.X) + (int)(game.GameWidth - bridge.Width * scale.X) / 2, (int)(checkpoints[i] + yOffset),
                    (int)(game.GameWidth - bridge.Width * scale.X) / 2, (int)(road.Height * scale.Y)), Color.White);
            }

            ////////////////////
            // Draw Rocket
            ////////////////////
            if (rocketLaunched)
            {
                game.SpriteBatch.Draw(
                    game.GetTexture("solid"),
                    new Rectangle((int)((pSize.X - rocketSize.X) / 2 + pPos.X), (int)(rocketY+yOffset), (int)rocketSize.X, (int)rocketSize.Y), Color.Yellow);
            }

            ////////////////////
            // Draw Player
            ////////////////////
            game.SpriteBatch.Draw(
                (pxVel == 0 ? game.GetTexture("raider") : game.GetTexture("raider_mv")), 
                new Rectangle((int)pPos.X, (int)(pPos.Y + yOffset), (int)(pxVel == 0 ? stillPlayerWidth : movingPlayerWidth), (int)pSize.Y), null, Color.White, 0, Vector2.Zero, 
                (pxVel < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0);

            game.RestartBatch();
            base.Render(gameTime);
        }
        #endregion
    }
}
