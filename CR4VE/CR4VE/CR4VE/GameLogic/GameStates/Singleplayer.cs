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

        //Player
        public static Character ghost;
        public static Character player;

        //Checkpoints
        public static Checkpoint lastCheckpoint;
        public static Checkpoint c1_hell;

        //Powerups
        public static Powerup powerup_health;
        public static Powerup powerup_mana;
        
        //Enemies
        public static List<Enemy> enemyList = new List<Enemy>();

        //HUD
        public static HUD hud;
        #endregion

        #region Konstruktor
        // im Konstruktor braucht nichts veraendert werden
        public Singleplayer() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            #region Terrain
            terrainMap = new Tilemap();
            
            Tile.Content = content;

            int[,] layout = new int[,] {
        
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,0,1,1,1,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,3,3,1,0,0,0,1,0,0,0,1,0,0,0,0},
           {0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,3,0,0,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0},
           {1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,3,3,3,3,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,3,4,4,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0},
           {3,3,3,3,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,4,4,3,3,4,4,3,3,4,4,3,3,4,4,3,4,4,3,3,3,3,3,4,4,4,4,4,4,4,4,4,0},
           {3,3,3,3,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,4,4,3,3,4,4,3,3,4,4,3,3,4,4,3,4,4,3,3,3,3,3,4,4,4,4,4,4,4,4,4,0}};
           
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
        
           /*{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,9,0,0,0,0,0,0,0,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,9,0,0,0,0,0,0,0,0,9,0,0,9,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,0,0,0,9,0,0,0,0,0,0,0,9,0,0,0,9,9,0,0,9,0,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,0,0,9,0,0,9,9,9,9,9,9,9,0,0,9,9,0,0,0,0,0,0,0,0},
           {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,0,0,0,0,0}};*/
            
         



          
                
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

            //Camera
            Camera2D.Initialize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Console.WriteLine(Camera2D.Position2D);
            Console.WriteLine(Tilemap.getVisibleTiles().Count);
            //SaveGame
            SaveGame.Reset();

            //Textures
            background = content.Load<Texture2D>("Assets/Sprites/stone");

            //Player
            ghost = new Character(Vector3.Zero, "skull", content);
            player = new CharacterSeraphin(Vector3.Zero, "sphereD5", content, new BoundingBox(new Vector3(-2.5f, -2.5f, -2.5f), new Vector3(2.5f, 2.5f, 2.5f)));
            
            //Powerups
            powerup_health = new Powerup(new Vector3(10,0,0), "powerup_hell_health", content, new BoundingBox(Vector3.Zero, Vector3.One), "health", 50);
            powerup_mana = new Powerup(new Vector3(50, -20, 0), "powerup_hell_mana", content, new BoundingBox(Vector3.Zero, Vector3.One), "mana", 1);
            
            //Checkpoints
            lastCheckpoint = new Checkpoint(Vector3.Zero, "checkpoint_hell", content);
            c1_hell = new Checkpoint(new Vector3(110, 4f, 0), "checkpoint_hell", content);
            
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

            #region HUD
            hud.Update();

            hud.UpdateMana();

            if (hud.isDead)
            {
                return Game1.EGameState.GameOver;
            }
            #endregion

            //Player
            player.Update(gameTime);

            //Powerups
            powerup_health.Update();
            powerup_mana.Update();

            //Checkpoints
            c1_hell.Update();

            #region Enemies
            foreach (Enemy enemy in enemyList)
            {
                enemy.UpdateSingleplayer(gameTime);
            }
            
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList.ElementAt(i).health <= 0)
                {
                    enemyList.ElementAt(i).Destroy();
                    enemyList.Remove(enemyList.ElementAt(i));
                }
            }
            #endregion

            //notwendiger Rueckgabewert
            return Game1.EGameState.Singleplayer;
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
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
            //Terrain
            terrainMap.Draw(Vector3.One, 0, 0, 0);

            //Powerups
            powerup_health.drawIn2DWorld(new Vector3(1,1,1), 0, powerup_health.rotatedDegree, 0);
            powerup_mana.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, MathHelper.ToRadians(-90), 0);

            //Player or Ghost
            if (GameControls.isGhost)
                ghost.drawIn2DWorld(new Vector3(0.05f, 0.05f, 0.05f), 0, 0, 0);
            else
                player.drawIn2DWorldWithoutBones(Vector3.One, 0, MathHelper.ToRadians(90) * player.viewingDirection.X, 0);

            //Checkpoints
            c1_hell.drawIn2DWorld(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, 0);

            #region Enemies
            foreach (AIInterface enemy in enemyList)
            {
                enemy.Draw(gameTime);
            }

            //Minions etc.
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
