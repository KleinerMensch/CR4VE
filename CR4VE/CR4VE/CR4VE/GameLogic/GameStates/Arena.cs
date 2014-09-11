using CR4VE.GameBase.Camera;
using CR4VE.GameBase.HeadUpDisplay;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.AI;
using CR4VE.GameLogic.Characters;
using CR4VE.GameLogic.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CR4VE.GameLogic.GameStates
{
    class Arena : GameStateInterface
    {
        #region Attribute
        public static ContentManager cont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static HUD opheliaHud, seraphinBossHUD, fractusBossHUD;

        //Terrain
        static Entity terrain;
        public static Entity lava;

        //Arena Boundaries
        public static readonly BoundingBox arenaFloorBox = new BoundingBox(new Vector3(-54, -25, -65), new Vector3(63, -15, 53));
        public static readonly BoundingSphere arenaBound = new BoundingSphere(new Vector3(5, -20, -8), 60f);        

        //moveable Entities
        public static Character player;
        public static Character boss;

        public static readonly Vector3 startPos = new Vector3(0, -12.5f, 0);

        public static float blickWinkel;
        public static float blickwinkelBoss;

        public static BoundingSphere sphere;
        public static BoundingSphere rangeOfMeleeFromBoss;
        #endregion

        #region Konstruktor
        public Arena() { }
        #endregion

        #region Methoden
        public void Initialize(ContentManager content)
        {
            //Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            //Camera
            CameraArena.Initialize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //Terrain
            terrain = new Entity(new Vector3(5, -20, -10), "Terrain/arena_hell", content);

            BoundingBox lavaBound = new BoundingBox(new Vector3(), new Vector3());
            lava = new Entity(new Vector3(0, -110, -30), "Terrain/lavafloor", content);

            //moveable Entities
            player = new CharacterOphelia(startPos, "Ophelia", content);
            BoundingBox playerBound = new BoundingBox(player.Position + new Vector3(-2.5f, -9, -2.5f), player.Position + new Vector3(2.5f, 9, 2.5f));
            player.Boundary = playerBound;

            sphere = new BoundingSphere(player.position, 30);
            rangeOfMeleeFromBoss = new BoundingSphere(player.position, 6);
            //testBox = new Entity(boss.position, "Terrain10x10x10Box1", content);

            #region Loading AI
            //Boss je nachdem, wer der Player ist
            boss = new BossCrystal(new Vector3(60, 0, 0), "Ophelia", content);
            boss.boundary = new BoundingBox(boss.position + new Vector3(-3, -3, -3), boss.position + new Vector3(3, 3, 3));
            #endregion

            //HUD
            opheliaHud = new OpheliaHUD(content, graphics);
            seraphinBossHUD = new BossHellHUD(content, graphics);
            fractusBossHUD = new BossCrystalHUD(content, graphics);
            cont = content;
        }

        public Game1.EGameState Update(GameTime gameTime)
        {
            GameControls.updateArena(gameTime);
            GameControls.updateVibration(gameTime);

            player.Update(gameTime);
            sphere.Center = player.position;
            rangeOfMeleeFromBoss.Center = player.position;

            boss.Update(gameTime);
            
            #region HUD
            opheliaHud.Update();
            opheliaHud.UpdateLiquidPositionByResolution();
            opheliaHud.UpdateMana();
            opheliaHud.UpdateHealth();

            seraphinBossHUD.Update();
            seraphinBossHUD.UpdateLiquidPositionByResolution();

            fractusBossHUD.Update();
            fractusBossHUD.UpdateLiquidPositionByResolution();

            if (opheliaHud.isDead)
            {
                return Game1.EGameState.GameOver;
            }
            #endregion

            return Game1.EGameState.Arena;
        }

        public void Draw(GameTime gameTime)
        {
            //Terrain
            lava.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);

            //DEBUG boundings
            //arenaFloor.drawInArenaWithoutBones(new Vector3(12, 2, 12), 0, 0, 0);
            //arenaBoundSphere.drawInArenaWithoutBones(new Vector3(24, 24, 24), 0, 0, 0);
            
            terrain.drawInArena(new Vector3(0.5f, 0.5f, 0.5f), 0, MathHelper.ToRadians(30), 0);

            //Player
            player.drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + blickWinkel, 0);
            player.DrawAttacks();

            #region Enemies
            boss.drawInArena(/*new Vector3(0.75f, 0.75f, 0.75f)*/new Vector3(0.03f, 0.03f, 0.03f)/*boss.scaleForDrawMethod*/, 0, MathHelper.ToRadians(90) + blickwinkelBoss, 0);
            boss.DrawAttacks();
            #endregion

            #region HUD
            spriteBatch.Begin();

            //seraphinBossHUD.Draw(spriteBatch);
            fractusBossHUD.Draw(spriteBatch);
            opheliaHud.Draw(spriteBatch);

            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            #endregion
        }

        public void Unload()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
