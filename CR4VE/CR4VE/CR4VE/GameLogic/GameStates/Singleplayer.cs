using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameBase.Objects.Terrain;
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.Characters;
using CR4VE.GameBase.HeadUpDisplay;

namespace CR4VE.GameLogic.GameStates
{
    class Singleplayer : GameStateInterface
    {
        #region Attribute
        public static ContentManager cont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D background;

        public static Tilemap terrainMap;
        public static Character player;

        public static Checkpoint c1_hell;
        public static Powerup powerup_health;
        
        public static List<Enemy> enemyList = new List<Enemy>();

        public static HUD hud;
        #endregion

        #region Konstruktor
        // im Konstruktor braucht nichts veraendert werden
        public Singleplayer() { }
        #endregion

        // Contents werden hier reingeladen
        #region Init
        public void Initialize(ContentManager content)
        {
            #region Terrain
            terrainMap = new Tilemap();
            
            Tile.Content = content;

            int[,] layout = new int[,] {
          /*
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,0,1,1,1,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,3,3,1,0,0,0,1,0,0,0,1,0,0,0,0},
           {0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,3,0,0,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0},
           {1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,3,3,3,3,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,3,4,4,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0},
           {3,3,3,3,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,4,4,3,3,4,4,3,3,4,4,3,3,4,4,3,4,4,3,3,3,3,3,4,4,4,4,4,4,4,4,4,0},
           {3,3,3,3,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,4,4,3,3,4,4,3,3,4,4,3,3,4,4,3,4,4,3,3,3,3,3,4,4,4,4,4,4,4,4,4,0}};
        **/             
         /*  { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,0,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,0,0,0,5,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,0,5,5,5,0,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,0,0,0,0,0,0,0,0,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 5,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 5,5,0,0,5,5,5,0,0,5,0,0,5,0,0,5,0,0,5,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,5,0,0,5,0,0,5,0,0,0,5,5,5,5},
            { 7,7,7,7,7,7,5,8,8,5,8,8,5,8,8,5,8,8,5,5,7,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,5,8,8,8,8,5,5,8,8,8,8,5,5,5,5},
            { 7,7,7,7,7,7,5,8,8,5,8,8,5,8,8,5,8,8,5,7,7,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8}};
       **/
        
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           { 0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            { 0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,9,0,0,0,0,0,0,0,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,0,0,0,0,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,9,0,0,0,0,0,0,0,0,9,0,0,9,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,9,0,0,0,0,0,0,0,9,0,0,0,9,9,0,0,9,0,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,0,0,9,0,0,9,9,9,9,9,9,9,0,0,9,9,0,0,0,0,0,0,0,0},
           { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,0,0,0,0,0}};
            
         



          
                
            int boxSize = 10;

            terrainMap.Generate(layout, boxSize);

        /*    int[,] layout2 = new int[,] {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,5,0,0,5,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,7,0,0,0,0,7,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,5,7,0,0,7,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,6,5,7,7,7,7,7,0,0,7,0,0,7,0,0,7,0,0,7,7,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {4,6,6,6,6,6,6,6,4,4,5,4,4,5,4,4,5,4,4,5,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {4,6,6,6,6,6,6,6,4,4,5,4,4,5,4,4,6,4,4,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
            

                    terrainMap.Generate(layout2, boxSize);
**/
            #endregion

            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            //initialize Camera Class
            Camera2D.Initialize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Console.WriteLine(graphics.PreferredBackBufferWidth);

            //load textures
            background = content.Load<Texture2D>("Assets/Sprites/stone");

            //load models
            player = new CharacterSeraphin(new Vector3(0, 0, 0), "sphereD5", content, new BoundingBox(new Vector3(-2.5f, -2.5f, -2.5f), new Vector3(2.5f, 2.5f, 2.5f)));
            
            powerup_health = new Powerup(new Vector3(10,0,0), "powerup_hell_health", content, new BoundingBox(Vector3.Zero, Vector3.One), "energy", 50);
            
            c1_hell = new Checkpoint(new Vector3(10,0,0), "checkpoint_hell", content, new BoundingBox(new Vector3(-3,-3,-3), new Vector3(3,3,3)));
            
            #region Loading AI
            EnemyRedEye redEye;
            EnemySkull skull;
            EnemySpinningCrystal spinningCrystal;

            redEye = new EnemyRedEye(new Vector3(60, 0, 0),"EnemyEye",content,new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            skull = new EnemySkull(new Vector3(260, 0, 0), "skull", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
            spinningCrystal = new EnemySpinningCrystal(new Vector3(120, 0, 0), "enemySpinningNoAnim", content,new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));

            //fill list with enemies
            enemyList.Add(redEye);
            enemyList.Add(spinningCrystal);
            enemyList.Add(skull);
            #endregion

            //HUD
            hud = new OpheliaHUD(content, graphics);
            cont = content;
        }
        #endregion

        #region Update
        public Game1.EGameState Update(GameTime gameTime)
        {
            GameControls.updateSingleplayer(gameTime);

            #region Updating HUD
            hud.Update();
            /*hud.UpdateMana();
            if (hud.isDead)
            {
                return Game1.EGameState.GameOver;
            }*/
            #endregion

            //updating Characters
            player.Update(gameTime);

            //updating Powerups
            powerup_health.Update();

            //updating Enemies
            foreach (Enemy enemy in enemyList)
            {
                enemy.UpdateSingleplayer(gameTime);
            }
            //aktualisieren der lebenden Gegner
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList.ElementAt(i).health <= 0)
                {
                    enemyList.ElementAt(i).Destroy();
                    enemyList.Remove(enemyList.ElementAt(i));
                }
            }

            //notwendiger Rueckgabewert
            return Game1.EGameState.Singleplayer;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            #region Background
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position2D.X, (int)Camera2D.Position2D.Y, 800, 600), Color.White);
            
            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            terrainMap.Draw(Vector3.One, 0, 0, 0);

            powerup_health.drawIn2DWorld(new Vector3(1,1,1), 0, powerup_health.rotatedDegree, 0);

            player.drawIn2DWorldWithoutBones(Vector3.One, 0, MathHelper.ToRadians(90) * player.viewingDirection.X, 0);

            //animatedEnemy.Draw(gameTime, new Vector3(0.5f,0.5f,0.5f),0,MathHelper.ToRadians(180),0);

            //enemies
            foreach (AIInterface enemy in enemyList)
            {
                enemy.Draw(gameTime);
            }

            //minions etc.
            foreach (Entity laser in CR4VE.GameLogic.AI.EnemyRedEye.laserList)
            {
                laser.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, MathHelper.ToRadians(-90) * laser.viewingDirection.X);
            }
            foreach (Entity minion in CR4VE.GameLogic.Characters.CharacterSeraphin.minionList)
            {
                minion.drawIn2DWorld(new Vector3(0.5f,0.5f,0.5f), 0, 0, 0);
            }
            foreach (Entity crystal in CR4VE.GameLogic.Characters.CharacterFractus.crystalList)
            {
                crystal.drawIn2DWorld(new Vector3(0.1f, 0.1f, 0.1f), 0, 0, 0);
            }
            #endregion

            #region HUD
            spriteBatch.Begin();

            hud.DrawGenerals(spriteBatch);
            hud.Draw(spriteBatch);

            spriteBatch.End();
            #endregion
        }
        #endregion

        // loeschen aller grafischen Elemente
        public void Unload()
        {
            //throw new NotImplementedException();
        }

        // Freigabe der restlichen Standardressourcen
        // automatischer Aufruf des GarbageCollector beim Beenden des Spiels
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
