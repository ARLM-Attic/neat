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
namespace Tree
{
    public enum Directions { Left, Right, Straight }

    public partial class Massacre : RedBulb.RedBulbGame 
    {
        #region Fields
        bool gameRunning = true;
        Random ran = new Random();

        bool god = false;
        int playableAreaWidth = 900;
        int playableAreaOffset = 000;
        int gameCycles = 2;
        const int recyclePeriod = 40000;
        int maxTrees = 30;
        int minmaxTrees = 3;
        int minTreeSpeed = 3;
        int maxTreeSpeed = 13;
        int treeSpeed;
        int playerSpeed = 4;
        int highScore = 0;
        int score = 0;
        int secondScore = 1;
        int avoidScore = 10;
        int seconds = 0;
        int maxtreeDistance = 30;
        int treeDistance = 30;
        int minTreeDistance = 10;
        float backgroundColor = 1f;
        float backgroundColorSpeed = 0.01f;
        Rectangle zapperBar;
        bool zapperHold = true;
        List<Rectangle> trees;
        Rectangle gameArea;
        Rectangle player;
        Rectangle tree;
        Point offsetPoint;
        int zappers;
        const int maxZappers = 3;
        const int minZappers = 1;
        Directions playerDirection = Directions.Straight;

        void InitializeGame()
        {
            gameArea = new Rectangle(playableAreaOffset, 0, playableAreaWidth, gameHeight);
            tree = new Rectangle(0, 0, 42, 45);
            offsetPoint = new Point(playableAreaOffset, 0);
           // InitializeLogics();
        }
        void InitializeLogics()
        {

            zapperBar = new Rectangle(playableAreaOffset, gameArea.Height - 32, playableAreaWidth, 32);
            maxTrees = minmaxTrees;
            trees = new List<Rectangle>();
            treeDistance = maxtreeDistance;
            player = new Rectangle(playableAreaWidth / 2, gameHeight+50, 32, 37);
            score = 0;
            seconds = 0;
            treeSpeed = minTreeSpeed;
            playingState = PlayingState.Cinematics;
            playerDirection = Directions.Straight;
            gameRunning = false;
             ts = gamesTime.TotalGameTime.Duration();
             zappers = minZappers;
             gameCycles = 0;
            
        }
        #endregion

        #region Input
        void HandleGameInput()
        {
            if (IsTapped(Keys.Escape,Buttons.Back)) gameState = GameState.InGameMenu;
                
            if (IsTapped(Keys.Space, Buttons.A)) gameRunning = !gameRunning;

            if (gameRunning)
            {
                // Player Controls
                if (IsTapped (Keys.Z,Buttons.BigButton )) UseZapper();
                if (IsPressed(Keys.Left,Buttons.DPadLeft) || IsPressed(Buttons.LeftThumbstickLeft)) playerDirection = Directions.Left;
                if (IsPressed(Keys.Right,Buttons.DPadRight) || IsPressed(Buttons.LeftThumbstickRight)) playerDirection = Directions.Right;
                if (IsPressed(Keys.Up,Buttons.DPadUp) || IsPressed(Buttons.LeftThumbstickUp)) playerDirection = Directions.Straight;
                
                if (IsTapped(Keys.OemTilde,Buttons.A)) god = !god;
            }
        }
        #endregion

        #region Update
        TimeSpan ts;

        enum PlayingState { Playing, Cinematics }
        PlayingState playingState = PlayingState.Cinematics;
        void UpdateCinematics(GameTime gametime)
        {
            player.Y-=1 ;
            if (player.Y < gameHeight - 100)
            {
                player.Y = gameHeight - 100;
                playingState = PlayingState.Playing;
                gameRunning = true;
            }
        }
        void UpdateGame(GameTime gameTime)
        {
            
            HandleGameInput();
            if (playingState == PlayingState.Cinematics)
                UpdateCinematics(gameTime);
            else 
            if (gameRunning)
            {
                // TODO: Add your update logic here
                // 1: Move Player
                switch (playerDirection)
                {
                    case (Directions.Left):
                        player.Offset(-playerSpeed, 0);
                        break;
                    case (Directions.Right):
                        player.Offset(playerSpeed, 0);
                        break;
                }
                // 2: Clip Player
                if (player.Left < 0) player.X = 0;
                if (player.Right > playableAreaWidth) player.X = playableAreaWidth - player.Width;
                // 3: Delete Trees
                for (int i = 0; i < trees.Count; i++)
                    if (trees[i].Intersects(zapperBar))
                    {
                        trees.RemoveAt(i);
                        score += avoidScore;
                    }
                if (!zapperHold)
                {
                    zapperBar.Offset(0, -20);
                    if (zapperBar.Top <= 0)
                    {
                        zapperBar.Y = gameArea.Height - 20;
                        zapperHold = true;
                    }
                }
                // 4: Generate Trees
                if (gameCycles % treeDistance == 0 && (trees.Count < maxTrees))
                    trees.Add(
                       new Rectangle(ran.Next(0, playableAreaWidth - tree.Width), 0, tree.Width, tree.Height));

                if (gameCycles % 500 == 0 && treeDistance > minTreeDistance) treeDistance--;
                if (gameTime.TotalGameTime.Subtract(ts).TotalMilliseconds > 30000)
                    ts = gameTime.TotalGameTime.Duration();

                if (gameCycles % 2000 == 0) AddZapper();

                if (gameCycles % 600 == 0)
                {
                    maxTrees++;
                    if (backgroundColor > 0)
                    {
                        backgroundColor -= backgroundColorSpeed;
                        if (backgroundColor > 1) { backgroundColorSpeed *= -1; backgroundColor = 0.99f; }
                    }
                    else { backgroundColorSpeed *= -1; backgroundColor = 0.1f; }
                }
                if (gameCycles % 300 == 0)
                    player.Y--;
                // 5: Move Trees
                for (int i = 0; i < trees.Count; i++)
                {
                    Rectangle r = trees[i];
                    r.Offset(0, treeSpeed);
                    trees[i] = r;
                }
                // 6: Collision Detection
                if (!god)
                    foreach (Rectangle t in trees)
                    {
                        if (t.Intersects(player))
                        {
                            GameOver();
                            break;
                        }
                    }
                // 7: Score & Stuff
                if (gameCycles % 5 == 0) score += secondScore;
                if (gamesTime.TotalGameTime.Subtract(ts).Milliseconds == 0) seconds++;
                if (gameCycles % 1200 == 0 && treeSpeed < maxTreeSpeed) treeSpeed++;
                if (score > highScore) highScore = score;
                if (gameCycles > recyclePeriod) gameCycles = 0;
                gameCycles++;
            }
        }
        void AddZapper()
        {
            if (zappers < maxZappers) zappers++;
        }
        void UseZapper()
        {
            if (zappers < 1) return;
            zappers--;
            zapperHold = false;
        }
        int GetMs()
        {
            return (int)(gamesTime.TotalGameTime.Subtract(ts).TotalMilliseconds);
        }
        void GameOver()
        {
            console.Run("g_saymessage BLAMMO!");
            console.Run("g_saymessage Score: " + score.ToString());
            /*
            networkHelper.MessageBox(
                "BLAMMO!",
                "" +
                (score >= highScore ? "New High Score!\n" : "") +
                "Score: " + score.ToString() + "\n" +
                "Skiing time: " + seconds.ToString() + " seconds!",
                MessageBoxIcon.Warning);*/
            InitializeLogics();
        }

        #endregion

        #region Draw
        void DrawGame(GameTime gameTime)
        {
            //gamesTime = gameTime;
            
            spriteBatch.Draw(getTexture("gameBG"), gameArea, (god ? Color.Gold : new Color(new Vector3(backgroundColor,backgroundColor ,backgroundColor ))));
            spriteBatch.Draw(getTexture("bars"), new Rectangle(0, 0, gameArea.X, gameArea.Height), Color.White);
            spriteBatch.Draw(getTexture("bars"), new Rectangle(gameArea.Right, 0, gameWidth-gameArea.Right, gameArea.Height), Color.White);
            spriteBatch.Draw(getTexture(
                ((playerDirection == Directions.Right ? "playerRight" :
                (playerDirection == Directions.Left ? "playerLeft" : "playerStraight")))),
                Geometry2D.MoveRectangle(player, offsetPoint), Color.White);

            foreach (Rectangle t in trees)
            {
                spriteBatch.Draw(getTexture("Tree"), Geometry2D.MoveRectangle(t, offsetPoint), Color.White);
                spriteBatch.Draw(getTexture("Tree"), Geometry2D.MoveRectangle(t, new Point(offsetPoint.X+2,offsetPoint.Y+2)), Color.White);
            }
            spriteBatch.Draw(getTexture("transparent"), zapperBar, Color.White);
            DrawShadowedString(GetFont("gamefont"), "Inventory", new Vector2(gameArea.Right + 35, 60), Color.White);
    
            for (int i = 0; i < zappers ; i++)
            {
                spriteBatch.Draw(getTexture("zapper"),
                    new Rectangle(gameArea.Right + 35 + 72 * i, 100, 64, 64),
                    Color.White);
            }
            
            DrawShadowedString(GetFont("gamefont"), "Score", new Vector2(gameArea.Right + 35, 200), Color.White);
            DrawShadowedString(GetFont("gamefont"), score.ToString(), new Vector2(gameArea.Right + 60, 232), Color.Yellow);

            DrawShadowedString(GetFont("gamefont"), "High Score", new Vector2(gameArea.Right + 35, 300), Color.White);
            DrawShadowedString(GetFont("gamefont"), highScore.ToString(), new Vector2(gameArea.Right + 60, 332), Color.Tomato);

            spriteBatch.Draw(getTexture("keys"), new Vector2(gameArea.Right + 35, 500), Color.White);
        }
        
        #endregion
    }
}