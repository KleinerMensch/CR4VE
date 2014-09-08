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
    class Tutorial : GameStateInterface
    {

        #region Attribute 
        public static ContentManager cont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D background;
        public static Effect effect;

        //Terrain
        public static Tilemap[] tileMaps = new Tilemap[]{};

        public static Tilemap activeTileMap1;
        public static Tilemap activeTileMap2;
        public static int activeIndex1 = 0;
        public static int activeIndex2 = 1;

        public static List<Tile> visibles;

        //Player
        public static Character ghost;
        public static Character player;

        //reset point if dead
        public static Checkpoint lastCheckpoint;
        public static Entity ArenaKey;
        
        //Enemies
        public static List<Enemy> enemyList = new List<Enemy>();

        //HUD
        public static HUD hud;
         
        #endregion


        #region Konstruktor

        public Tutorial() {}
        #endregion

        
        #region Init
        public void Initialize(ContentManager content)
    {
        //Zugriff auf Attribute der Game1 Klasse
        spriteBatch = CR4VE.Game1.spriteBatch;
        graphics = CR4VE.Game1.graphics;
        cont = content;

        #region Terrain
        #region layout1
        int[,] layout1 = new int[,] {
                  { 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
                  { 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13},
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13, 0, 0, 0, 0,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13, 0,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                  {13,13,13,13,13,13, 0, 0,13,13,13,13, 0, 0,13, 0, 0,13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,13, 0, 0,92, 0,13, 0, 0},
                  {13,13,13,13,13,14,14,14,13,13,13,13,14,14,13,14,14,13,13,13,14,14,14, 0,13,13,13,13,13,13,13,13,13,13,13,14,14},
                  {13,13,13,13,14,14,14,14,13,13,13,13,14,14,13,14,14,13,13,13,14,14,14,14,13,13,13,13,13,13,13,13,13,13,13,14,14},};
        #endregion

        #region layout2
        int[,] layout2 = new int[,] {
                  {}};
        #endregion

        #region layout3
        int[,] layout3 = new int[,] {
                  {}};
        #endregion

        #region layout4
        int[,] layout4 = new int[,] {
                  {}};
        #endregion

        int boxSize = 10;

        //Array of all generated Tilemaps
        tileMaps = new Tilemap[]
            {
                Tilemap.Generate(layout1, boxSize, new Vector3(0, 0, 0)),
                //Tilemap.Generate(layout2, boxSize, new Vector3(510, 10, 0)),
                //Tilemap.Generate(layout3, boxSize, new Vector3(1030, -60, 0)),
                //Tilemap.Generate(layout4, boxSize, new Vector3(1550, -150, 0)),
              
                //Tilemap.Generate(layout9, boxSize, new Vector3(-80,0,-10)),
            };

        //indices of active Tilemaps
        activeTileMap1 = tileMaps[activeIndex1];
        activeTileMap2 = tileMaps[activeIndex2];

        visibles = new List<Tile>();
        #endregion

        //Camera
        Camera2D.Initialize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

        //SaveGame
        SaveGame.Reset();

        //Textures
        //background = content.Load<Texture2D>("Assets/Sprites/stone");

        //Effects
        effect = content.Load<Effect>("Assets/Effects/DirectLight");

        //Player
        ghost = new Character(Vector3.Zero, "skull", content);
        player = new CharacterOphelia(new Vector3(0, 0, 5), "Ophelia", content, new BoundingBox(new Vector3(-2.5f, -9f, -2.5f), new Vector3(2.5f, 9f, 2.5f)));

        //Checkpoints (default = Startposition)
        lastCheckpoint = new Checkpoint(Vector3.Zero, "checkpoint_hell", content);

        #region Loading AI
        EnemyRedEye redEye;
        EnemySkull skull;

        redEye = new EnemyRedEye(new Vector3(80, 0, 0), "EnemyEye", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
        skull = new EnemySkull(new Vector3(400, 0, 0), "skull", content, new BoundingBox(new Vector3(-3, -3, -3), new Vector3(3, 3, 3)));
        #endregion

        //HUD
        hud = new OpheliaHUD(cont, graphics);
    }
        #endregion


        #region Update
        public Game1.EGameState Update(GameTime gameTime)
    {
       
          //Viewport Culling
            visibles = Tilemap.getVisibleTiles();

            GameControls.updateSingleplayer(gameTime, visibles);

            //check if active Tilemaps have changed
            Tilemap.updateActiveTilemaps();

            #region HUD
            hud.Update();
            
            hud.UpdateMana();
            hud.UpdateHealth();

            hud.UpdateLiquidPositionByResolution();
            
            if (hud.isDead)
            {
                return Game1.EGameState.GameOver;
            }
            #endregion

            //Player
            player.Update(gameTime);

            //Powerups
            foreach (Powerup p in tileMaps[activeIndex1].PowerupList)
            {
                p.Update();
            }
            foreach (Powerup p in tileMaps[activeIndex2].PowerupList)
            {
                p.Update();
            }
            
            //Checkpoints
            foreach (Checkpoint c in tileMaps[activeIndex1].CheckpointList)
            {
                c.Update();
            }
            foreach (Checkpoint c in tileMaps[activeIndex2].CheckpointList)
            {
                c.Update();
            }

            #region Enemies
            foreach (Enemy e in tileMaps[activeIndex1].EnemyList)
            {
                e.UpdateSingleplayer(gameTime);
            }
            foreach (Enemy e in tileMaps[activeIndex2].EnemyList)
            {
                e.UpdateSingleplayer(gameTime);
            }

            //remove dead enemies from active lists
            for (int i = 0; i < tileMaps[activeIndex1].EnemyList.Count; i++)
            {
                if (tileMaps[activeIndex1].EnemyList.ElementAt(i).isDead)
                {
                    tileMaps[activeIndex1].EnemyList.ElementAt(i).Destroy();
                    tileMaps[activeIndex1].EnemyList.Remove(tileMaps[activeIndex1].EnemyList.ElementAt(i));
                }
            }
            for (int i = 0; i < tileMaps[activeIndex2].EnemyList.Count; i++)
            {
                if (tileMaps[activeIndex2].EnemyList.ElementAt(i).isDead)
                {
                    tileMaps[activeIndex2].EnemyList.ElementAt(i).Destroy();
                    tileMaps[activeIndex2].EnemyList.Remove(tileMaps[activeIndex2].EnemyList.ElementAt(i));
                }
            }
            #endregion

            //DEBUG
            //Console.WriteLine(tileMaps[activeIndex1].EnemyList.Count);
            //

            GameControls.updateVibration(gameTime);

            //notwendiger Rueckgabewert
            if (player.Boundary.Intersects(ArenaKey.Boundary))
                return Game1.EGameState.Arena;
            else
                return Game1.EGameState.Singleplayer;
        }
        #endregion



        #region Draw
        public void Draw(GameTime gameTime)
        {
            #region Background
            spriteBatch.Begin();

            graphics.GraphicsDevice.Clear(Color.Gray);

            //width and height for spriteBatch rectangle needed to draw background texture
            Vector2 drawPos = new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y);
            int drawRecWidth = graphics.PreferredBackBufferWidth;
            int drawRecHeight = graphics.PreferredBackBufferHeight;
            
            Rectangle drawRec = new Rectangle((int)Camera2D.Position2D.X, (int)Camera2D.Position2D.Y, drawRecWidth, drawRecHeight);
            
            //spriteBatch.Draw(background, drawPos, drawRec, Color.White);
            
            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion

            #region 3D Objects
            //Terrain (includes Powerups, Checkpoints)
            tileMaps[activeIndex1].Draw(visibles);
            tileMaps[activeIndex2].Draw(visibles);


            //Player or Ghost
            if (GameControls.isGhost && GameControls.moveVecGhost != Vector3.Zero)
                ghost.drawIn2DWorld(new Vector3(0.05f, 0.05f, 0.05f), 0, 0, 0);
            else
            {
                player.DrawModelWithEffect(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) * player.viewingDirection.X, 0);
                player.DrawAttacks();
            }

            #region Enemies
            foreach (Enemy e in tileMaps[activeIndex1].EnemyList)
            {
                e.Draw();
            }
            foreach (Enemy e in tileMaps[activeIndex2].EnemyList)
            {
                e.Draw();
            }
            #endregion

            if (ArenaKey.Position.X - player.Position.X < 100)
                ArenaKey.drawIn2DWorld(new Vector3(0.03f, 0.03f, 0.03f), 0, 0, 0);
            #endregion

            #region HUD
            spriteBatch.Begin();

            hud.Draw(spriteBatch);

            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion
        }
        #endregion
    
        
        // loeschen aller grafischen Elemente
        public void Unload()
        {
            foreach (Tilemap tm in tileMaps)
            {
                tm.CheckpointList.Clear();
                tm.EnemyList.Clear();
                tm.PowerupList.Clear();
                tm.TileList.Clear();
            }
        }

        // Freigabe der restlichen Standardressourcen
        // automatischer Aufruf des GarbageCollector beim Beenden des Spiels
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
    



