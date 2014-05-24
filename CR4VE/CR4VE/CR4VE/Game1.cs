using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameLogic.Controls;


namespace CR4VE
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D background;
        Texture2D testTex;

        public Entity player;
        Entity terrain;

        HUD hud;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            //size of Game Window
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.ApplyChanges();

            //initializing Camera2D Class
            Camera2D.WorldRectangle = new Rectangle(0, 0, 1920, 1080);
            Camera2D.ViewPortWidth = 800;
            Camera2D.ViewPortHeight = 600;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load textures
            background = Content.Load<Texture2D>("Assets/Sprites/1397342868251");
            testTex = Content.Load<Texture2D>("Assets/Sprites/doge");

            //load entities
            player = new Entity(new Vector3(0,0,0), "protoSphere", Content);
            terrain = new Entity(new Vector3(0,-5,0), "protoTerrain", Content);
            
            //HUD
            hud = new HUD(Content, graphics);
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            KeyboardControls.update(this);

            //debug
            Console.Clear();
            Console.WriteLine("PlayerPosition: " + player.Position);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            if (false)
            {
                #region drawing background
                spriteBatch.Begin();
                spriteBatch.Draw(background, new Vector2(Camera2D.WorldRectangle.X, Camera2D.WorldRectangle.Y), new Rectangle((int)Camera2D.Position.X, (int)Camera2D.Position.Y, 800, 600), Color.White);
                //spriteBatch.Draw(testTex, new Vector2(200, 200), Color.White);
                spriteBatch.Draw(testTex, Camera2D.transform2D(new Vector2(200, 200)), Color.White);
                spriteBatch.End();

                //GraphicsDevice auf default setzen
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                #endregion

                #region drawing 3D Objects
                Vector3 scale = new Vector3(1, 1, 1);

                player.drawOn2DScreen(scale);
                terrain.drawIn2DWorld(scale);       
                //Matrix worldMatrix2 = Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(Camera2D.transform3D(new Vector3(0, -20, 0)));
                #endregion

                #region drawing HUD
                spriteBatch.Begin();
                hud.Draw(spriteBatch);
                spriteBatch.End();
                #endregion
            }

            if (true)
            {
                Vector3 scale = new Vector3(1, 1, 1);

                terrain.drawInArena(scale);
                player.drawInArena(scale);
            }

            base.Draw(gameTime);
        }
    }
}