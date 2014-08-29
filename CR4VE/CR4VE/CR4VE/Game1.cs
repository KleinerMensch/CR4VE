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
using CR4VE.GameLogic.Controls;
using CR4VE.GameLogic.GameStates;
using CR4VE.GameLogic;

namespace CR4VE
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Game1 game = new Game1();
        
        private EGameState gameState;
        public static EGameState currentState;
        
        // Erstellung der Objekte aus den GameState Klassen
        private Arena arena = null;
        private MainMenu menu = null;
        private Continue cont = null;
        private Credits credits = null;
        private GameOver gameOver = null;
        private Multiplayer multiPlayer = null;
        private Singleplayer singlePlayer = null;
        private StartScreen startScreen = null;

        public enum EGameState
        {
            Arena,
            Continue,
            Credits,
            GameOver,
            MainMenu,
            Multiplayer,
            Nothing,
            Singleplayer,
            StartScreen,
        }
        #endregion

        #region GameState
        public EGameState GameState
        {
            // aufzurufen mit EGameState.Gamestatename 
            get { return this.gameState; }
            set
            {
                #region Unload & Load Content of GameState
                //unload old content
                switch (this.gameState)
                {
                    case EGameState.Nothing:
                        //Unload... nothing... Razz
                        break;
                    case EGameState.StartScreen:
                        this.startScreen.Unload();
                        break;
                    case EGameState.MainMenu:
                        this.menu.Unload();
                        break;
                    case EGameState.Singleplayer:
                        this.singlePlayer.Unload();
                        break;
                    case EGameState.Multiplayer:
                        this.multiPlayer.Unload();
                        break;
                    case EGameState.Continue:
                        this.cont.Unload();
                        break;
                    case EGameState.GameOver:
                        this.gameOver.Unload();
                        break;
                    case EGameState.Credits:
                        this.credits.Unload();
                        break;
                    case EGameState.Arena:
                        this.arena.Unload();
                        break;
                }

                //load new content
                this.gameState = value;
                switch (this.gameState)
                {
                    case EGameState.Nothing:
                        this.Exit();    //Quit Game!
                        break;
                    case EGameState.StartScreen:
                        this.startScreen.Initialize(Content);
                        break;
                    case EGameState.MainMenu:
                        this.menu.Initialize(Content);
                        break;
                    case EGameState.Singleplayer:
                        this.singlePlayer.Initialize(Content);
                        break;
                    case EGameState.Multiplayer:
                        this.multiPlayer.Initialize(Content);
                        break;
                    case EGameState.Continue:
                        this.cont.Initialize(Content);
                        break;
                    case EGameState.GameOver:
                        this.gameOver.Initialize(Content);
                        break;
                    case EGameState.Credits:
                        this.credits.Initialize(Content);
                        break;
                    case EGameState.Arena:
                        this.arena.Initialize(Content);
                        break;
                }
                #endregion
            }
        }
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            this.cont = new Continue();
            this.credits = new Credits();
            this.gameOver = new GameOver();
            this.menu = new MainMenu();
            this.multiPlayer = new Multiplayer();
            this.singlePlayer = new Singleplayer();
            this.startScreen = new StartScreen();
            this.arena = new Arena();
        }


        protected override void Initialize()
        {
            IsMouseVisible = false;

            //size of Game Window
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //graphics.IsFullScreen = true;

            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Gamestate am Anfang
            // zum Testen jeweiligen GameState einsetzen
            this.GameState = EGameState.Arena;
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            #region Update of current GameState
            switch (this.gameState)
            {
                case EGameState.Nothing:
                    // do nothing
                    break;

                case EGameState.StartScreen:
                    currentState = this.startScreen.Update(gameTime);
                    if (currentState != EGameState.StartScreen)
                        this.GameState = currentState;
                    break;

                case EGameState.MainMenu:
                    currentState = this.menu.Update(gameTime);
                    if (currentState != EGameState.MainMenu)
                        this.GameState = currentState;
                    break;

                case EGameState.Singleplayer:
                    currentState = this.singlePlayer.Update(gameTime);
                    if (currentState != EGameState.Singleplayer)
                        this.GameState = currentState;
                    break;

                case EGameState.Multiplayer:
                    currentState = this.multiPlayer.Update(gameTime);
                    if (currentState != EGameState.Multiplayer)
                        this.GameState = currentState;
                    break;

                case EGameState.Continue:
                    currentState = this.cont.Update(gameTime);
                    if (currentState != EGameState.Continue)
                        this.GameState = currentState;
                    break;

                case EGameState.GameOver:
                    currentState = this.gameOver.Update(gameTime);
                    if (currentState != EGameState.GameOver)
                        this.GameState = currentState;
                    break;

                case EGameState.Credits:
                    currentState = this.credits.Update(gameTime);
                    if (currentState != EGameState.Credits)
                        this.GameState = currentState;
                    break;

                case EGameState.Arena:
                    currentState = this.arena.Update(gameTime);
                    if (currentState != EGameState.Arena)
                        this.GameState = currentState;
                    break;
            }
            #endregion

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            #region Draw of current GameState
            switch (this.gameState)
            {
                case EGameState.Nothing:
                    break;
                case EGameState.StartScreen:
                    this.startScreen.Draw(gameTime);
                    break;
                case EGameState.MainMenu:
                    this.menu.Draw(gameTime);
                    break;
                case EGameState.Singleplayer:
                    this.singlePlayer.Draw(gameTime);
                    break;
                case EGameState.Multiplayer:
                    this.multiPlayer.Draw(gameTime);
                    break;
                case EGameState.Continue:
                    this.cont.Draw(gameTime);
                    break;
                case EGameState.GameOver:
                    this.gameOver.Draw(gameTime);
                    break;
                case EGameState.Credits:
                    this.credits.Draw(gameTime);
                    break;
                case EGameState.Arena:
                    this.arena.Draw(gameTime);
                    break;
            }
            #endregion

            base.Draw(gameTime);
        }
    }
}