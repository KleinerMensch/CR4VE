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
using CR4VE.GameLogic.Controls;

namespace CR4VE.GameLogic.GameStates
{
    class Singleplayer : GameStateInterface
    {
        #region Attribute
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D background;
        Texture2D testTex;

        Model player;
        Model protoTerrain;

        HUD hud;
        #endregion

        #region Konstruktor
        // im Konstruktor braucht nichts veraendert werden
        public Singleplayer() { }
        #endregion

        // Contents werden hier reingeladen
        #region Init
        public void Initialize(ContentManager content)
        {
            // Zugriff auf Attribute der Game1 Klasse
            spriteBatch = CR4VE.Game1.spriteBatch;
            graphics = CR4VE.Game1.graphics;

            //initializing Camera Class
            Camera2D.WorldRectangle = new Rectangle(0, 0, 1920, 1080);
            Camera2D.ViewPortWidth = 800;
            Camera2D.ViewPortHeight = 600;

            //load textures
            background = content.Load<Texture2D>("Assets/Sprites/1397342868251");
            testTex = content.Load<Texture2D>("Assets/Sprites/doge");

            //load models
            player = content.Load<Model>("Assets/Models/protoSphere");
            protoTerrain = content.Load<Model>("Assets/Models/protoTerrain");

            //HUD
            hud = new HUD(content, graphics);
        }
        #endregion

        #region Update
        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardControls.update();

            //debug
            Console.Clear();
            Console.WriteLine("CamPosition: " + Camera2D.Position);

            //notwendiger Rueckgabewert
            return Game1.EGameState.Singleplayer;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);
            //spriteBatch.Draw(testTex, new Vector2(200, 200), Color.White);
            spriteBatch.Draw(testTex, Camera2D.transform2D(new Vector2(200, 200)), Color.White);
            spriteBatch.End();

            //GraphicsDevice auf default setzen
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            #region 3D Objects
            //draw 3D objects (player)
            Vector3 scale = new Vector3(1, 1, 1);

            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 1, 10f, 1000);
            Matrix worldMatrix = Matrix.CreateScale(scale) * 1 * Matrix.CreateTranslation(0, 0, 0);

            foreach (ModelMesh mesh in player.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = worldMatrix;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
            //draw 3D objects (terrain)        
            Matrix worldMatrix2 = Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(Camera2D.transform3D(new Vector3(0, -20, 0)));

            foreach (ModelMesh mesh in protoTerrain.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = worldMatrix2;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
            #endregion

            spriteBatch.Begin();
            hud.Draw(spriteBatch);
            spriteBatch.End();

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

