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

        public static HUD opheliaHud, seraphinBossHUD, kazumiHud, fractusBossHUD;

        //Terrain
        static Entity terrain;
        public static Entity lava;
        public static Effect lava_effect;
        public static Texture2D texture;

        //Arena Boundaries
        public static readonly BoundingBox arenaFloorBox = new BoundingBox(new Vector3(-54, -25, -65), new Vector3(63, -15, 53));
        public static readonly BoundingSphere arenaBound = new BoundingSphere(new Vector3(5, -20, -8), 60f);

        //moveable Entities
        public static Character player;
        public static Character boss;

        public Vector3 scaleForBossFractus = new Vector3(0.03f, 0.03f, 0.03f);
        public Vector3 scaleForBossSeraphin = new Vector3(0.75f, 0.75f, 0.75f);

        //da bounding box vom player 18einheiten hoch ist und arenafloor.y.max bei -15 liegt
        public static readonly Vector3 startPos = new Vector3(0, -6f, 0);

        public static float blickWinkel;
        public static float blickwinkelBoss;

        public static BoundingSphere sphere;
        public static BoundingSphere rangeOfMeleeFromBoss;
        Entity testBoundingBox;
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
            if (Singleplayer.isCrystal)
            {
                player = new CharacterKazumi(startPos, "Kazumi", content); //Kazumi
                CharacterKazumi.manaLeft = 3;
            }
            else
            {
                player = new CharacterSeraphin(startPos, "Ophelia", content); //Ophelia
                CharacterOphelia.manaLeft = 3;
            }

            BoundingBox playerBound = new BoundingBox(player.Position + new Vector3(-2.5f, -9, -2.5f), player.Position + new Vector3(2.5f, 9, 2.5f));
            player.Boundary = playerBound;
            sphere = new BoundingSphere(player.position, 30);

            #region Loading AI
            //Boss je nachdem, wer der Player ist
            boss = new BossCrystal(new Vector3(60, 0, 0), "Kazumi", content);
            boss.boundary = new BoundingBox(boss.position + new Vector3(-4f, -12, -4f), boss.position + new Vector3(4f, 12, 4f));
            rangeOfMeleeFromBoss = new BoundingSphere(player.position, 4);
            #endregion

            testBoundingBox = new Entity(boss.position, "5x5x5Box1", content);
            
            #region Initialize and Reset HUD
            opheliaHud = new OpheliaHUD(content, graphics);
            opheliaHud.healthLeft = opheliaHud.fullHealth;
            seraphinBossHUD = new BossHellHUD(content, graphics);
            seraphinBossHUD.healthLeft = seraphinBossHUD.fullHealth;

            kazumiHud = new KazumiHUD(content, graphics);
            kazumiHud.healthLeft = kazumiHud.fullHealth;
            fractusBossHUD = new BossCrystalHUD(content, graphics);
            fractusBossHUD.healthLeft = fractusBossHUD.fullHealth;
            #endregion

            Sounds.Initialize(content);

            cont = content;
        }

        public Game1.EGameState Update(GameTime gameTime)
        {
            GameControls.updateArena(gameTime);
            GameControls.updateVibration(gameTime);

            player.Update(gameTime);
            sphere.Center = player.position;
            rangeOfMeleeFromBoss.Center = boss.position;

            boss.Update(gameTime);

            testBoundingBox.moveTo(boss.position);
            
            #region HUD
            opheliaHud.Update();
            opheliaHud.UpdateLiquidPositionByResolution();
            opheliaHud.UpdateMana();
            opheliaHud.UpdateHealth();

            kazumiHud.Update();
            kazumiHud.UpdateLiquidPositionByResolution();
            kazumiHud.UpdateMana();
            kazumiHud.UpdateHealth();

            seraphinBossHUD.Update();
            seraphinBossHUD.UpdateLiquidPositionByResolution();

            fractusBossHUD.Update();
            fractusBossHUD.UpdateLiquidPositionByResolution();

            if (opheliaHud.isDead || kazumiHud.isDead)
            {
                return Game1.EGameState.GameOver;
            }
            #endregion

            return Game1.EGameState.Arena;
        }

        public void Draw(GameTime gameTime)
        {
            #region 3D Objects
            //Terrain
            lava.drawInArena(new Vector3(1, 1, 1), 0, 0, 0);
            terrain.drawInArena(new Vector3(0.4f, 0.4f, 0.4f), 0, MathHelper.ToRadians(30), 0);            

            //Player
            player.drawInArena(new Vector3(0.02f, 0.02f, 0.02f), 0, MathHelper.ToRadians(90) + blickWinkel, 0);
            player.DrawAttacks();

            //Boss
            boss.drawInArena(scaleForBossFractus, 0, MathHelper.ToRadians(90) + blickwinkelBoss, 0);
            boss.DrawAttacks();

            
            //DEBUG boundings------------------------------------------------------------
            //arenaFloor.drawInArenaWithoutBones(new Vector3(12, 2, 12), 0, 0, 0);
            //arenaBoundSphere.drawInArenaWithoutBones(new Vector3(24, 24, 24), 0, 0, 0);  
            //testBoundingBox.drawInArena(Vector3.One, 0, MathHelper.ToRadians(90) + blickwinkelBoss, 0);
            //---------------------------------------------------------------------------
            #endregion

            #region HUD
            spriteBatch.Begin();

            if (Singleplayer.isCrystal)
            {
                kazumiHud.Draw(spriteBatch);
                fractusBossHUD.Draw(spriteBatch);
            }
            else
            {
                opheliaHud.Draw(spriteBatch);
                seraphinBossHUD.Draw(spriteBatch);
            }

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
