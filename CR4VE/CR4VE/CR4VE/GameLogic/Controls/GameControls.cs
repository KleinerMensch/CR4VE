using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CR4VE.GameBase.Camera;
using CR4VE.GameBase.Objects;
using CR4VE.GameBase.Objects.Terrain;
using CR4VE.GameLogic.GameStates;
using CR4VE.GameLogic.Characters;

namespace CR4VE.GameLogic.Controls
{
    public static class GameControls
    {
        #region Attributes
        //Keyboard-, Maus- und Gamepadparameter
        static KeyboardState currentKeyboard;
        static KeyboardState previousKeyboard;

        static MouseState currentMouseState;
        static MouseState previousMouseState;

        static GamePadState currGamepad;
        static GamePadState prevGamepad;

        //Bewegungsparameter
        //(alive)
        private static readonly float accel = 1f;

        private static bool borderedLeft = false;
        private static bool borderedRight = false;
        private static bool borderedTop = false;
        private static bool borderedBottom = false;
        //(ghost)
        private static readonly float ghostDelay = 0.01f;
        private static readonly Vector3 checkPointFall = new Vector3(0, 10f, 0);

        public static Vector3 moveVecGhost = Vector3.Zero;
        public static bool isGhost = false;
        
        //Sprungparameter
        private static readonly float G = 9.81f;
        private static readonly float jumpHeight = 2.5f;

        private static double startFallTime;
        private static double startJumpTime;
        private static double startPauseTime;
        private static double endPauseTime;
        private static double currentTime;
        
        public static bool isJumping = false;
        public static bool isAirborne = true;
        public static bool isFalling = false;

        //Arena
        static Vector3 fallVecPlayer;
        static bool ringOut = false;

        //Vibration
        static double vibrTimer;
        static float currentHealth;
        static float previousHealth;

        //MainMenu
        private static Vector3 moveVecSword;
        public static bool isMenuMoving = false;
        public static bool fullscreenPossible = false;
        public static int menuPosIndex = 0;
        

        private static readonly Vector3[] menuPositions = new Vector3[]
        {
            new Vector3(0, -238, -10), //Start
            new Vector3(0, -84, -10), //Singleplayer
            new Vector3(0, 0, -10), //Multiplayer
            new Vector3(0, 96, -10), //Options
            new Vector3(0, 150, -10), //detailed Options
            new Vector3(0, 235, -10) //more detailed Options
        };
        #endregion

        #region Methods

        #region Help Methods
        //help methods for Keyboard
        public static bool isPressed(Keys key)
        {
            return currentKeyboard.IsKeyDown(key);
        }

        public static bool isClicked(Keys key)
        {
            return currentKeyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }

        public static bool isReleased(Keys key)
        {
            return currentKeyboard.IsKeyUp(key) && previousKeyboard.IsKeyDown(key);
        }

        //help methods for Mouse
        public static bool leftClick(MouseState current, MouseState previous)
        {
            return (current.LeftButton == ButtonState.Pressed) && (previous.LeftButton == ButtonState.Released);
        }
        public static bool rightClick(MouseState current, MouseState previous)
        {
            return (current.RightButton == ButtonState.Pressed) && (previous.RightButton == ButtonState.Released);
        }
        public static bool middleClick(MouseState current, MouseState previous)
        {
            return (current.MiddleButton == ButtonState.Pressed) && (previous.MiddleButton == ButtonState.Released);
        }

        //help methods for Gamepad
        public static bool isPressed(Buttons button)
        {
            return currGamepad.IsButtonDown(button);
        }
        public static bool isClicked(Buttons button)
        {
            return currGamepad.IsButtonDown(button) && prevGamepad.IsButtonUp(button);
        }
        public static bool isReleased(Buttons button)
        {
            return currGamepad.IsButtonUp(button) && prevGamepad.IsButtonDown(button);
        }
        #endregion

        public static bool isReduced(Game1.EGameState state)
        {
            if (state == Game1.EGameState.Singleplayer)
            {
                previousHealth = currentHealth;
                currentHealth = Singleplayer.hud.healthLeft;

                return previousHealth > currentHealth;
            }
            /*else if (state == Game1.EGameState.Arena)
            {
                previousHealth = currentHealth;
                currentHealth = Arena.opheliaHud.healthLeft;

                return previousHealth > currentHealth;
            }*/

            return false;
        }

        //update methods
        public static void updateVibration(GameTime gameTime)
        {
            if (isReduced(Game1.currentState))
            {
                GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
                vibrTimer = 1.0f;            
            }

            vibrTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (vibrTimer <= 0.0f)
            {
                GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
            }          
        }

        public static void initializeSingleplayer()
        {
            moveVecGhost = Vector3.Zero;
            isGhost = false;

            startJumpTime = 0;
            startFallTime = 0;
            startPauseTime = 0;
            endPauseTime = 0;

            borderedLeft = false;
            borderedRight = false;
            borderedTop = false;
            borderedBottom = false;

            isJumping = false;
            isAirborne = true;
            isFalling = false;
        }
        public static void updateSingleplayer(GameTime gameTime, List<Tile> visibles)
        {
            //get currently and previously pressed keys and mouse buttons
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            prevGamepad = currGamepad;
            currGamepad = GamePad.GetState(PlayerIndex.One);


            if (isGhost)
            {
                #region ghost movement
                //reset damage parameters
                Singleplayer.hud.isBurning = false;
                Singleplayer.hud.isSwimming = false;

                startJumpTime = 0;
                startFallTime = 0;
                startPauseTime = 0;
                endPauseTime = 0;

                borderedLeft = false;
                borderedRight = false;
                borderedTop = false;
                borderedBottom = false;

                isJumping = false;
                isAirborne = true;
                isFalling = false;

                //set ghost = player
                if (Singleplayer.ghost.Position == Vector3.Zero)
                {
                    Singleplayer.ghost.Position = Singleplayer.player.Position;

                    Singleplayer.player.moveTo(Singleplayer.lastCheckpoint.Position + checkPointFall + new Vector3(0,0,5));
                }

                //save moveVecGhost and calculate moveVec for ghost
                if (moveVecGhost == Vector3.Zero)
                {
                    moveVecGhost = Singleplayer.lastCheckpoint.Position + checkPointFall - Singleplayer.ghost.Position;
                    Singleplayer.ghost.viewingDirection = moveVecGhost;
                }

                Vector3 moveVec = moveVecGhost * ghostDelay;

                //update Playerposition
                Singleplayer.ghost.move(moveVec);

                //check if still a ghost and reanimate player if not
                if (Math.Abs(Singleplayer.ghost.Position.Length() - (Singleplayer.lastCheckpoint.Position + checkPointFall).Length()) < 0.25f)
                {
                    moveVecGhost = Vector3.Zero;
                    
                    Singleplayer.ghost.Position = Vector3.Zero;
                    
                    isGhost = false;
                }

                //move camera and realign BoundingFrustum
                Camera2D.realign(moveVec, Singleplayer.ghost.Position);
                #endregion
            }
            else
            {
                #region player movement
                #region Calculate moveVecPlayer
                Vector3 moveVecPlayer = new Vector3(0, 0, 0);

                #region left and right movement
                if ((currentKeyboard.IsKeyDown(Keys.A) || currGamepad.IsButtonDown(Buttons.DPadLeft) || currGamepad.IsButtonDown(Buttons.LeftThumbstickLeft)) && !borderedLeft)
                {
                    moveVecPlayer += new Vector3(-accel, 0, 0);

                    borderedRight = false;

                    //Blickrichtung = links
                    Singleplayer.player.viewingDirection.X = -1;
                }

                if ((currentKeyboard.IsKeyDown(Keys.D) || currGamepad.IsButtonDown(Buttons.DPadRight) || currGamepad.IsButtonDown(Buttons.LeftThumbstickRight)) && !borderedRight)
                {
                    moveVecPlayer += new Vector3(accel, 0, 0);

                    borderedLeft = false;

                    //Blickrichtung = rechts
                    Singleplayer.player.viewingDirection.X = 1;
                }
                #endregion

                #region airborne influence
                //being airborne by jumping
                if ((isClicked(Keys.Space) || isClicked(Buttons.A)) && !isJumping && !isFalling)
                {
                    isJumping = true;
                    borderedBottom = false;

                    startJumpTime = gameTime.TotalGameTime.TotalSeconds;
                }

                //being airborne by falling
                if (!isFalling && !isJumping && !Singleplayer.player.checkFooting(visibles))
                {
                    isFalling = true;
                    borderedBottom = false;

                    startFallTime = gameTime.TotalGameTime.TotalSeconds;
                }

                //calculate gravity influence if airborne by falling
                if (isFalling && !borderedBottom)
                {
                    currentTime = gameTime.TotalGameTime.TotalSeconds;

                    double deltaTime = currentTime - startFallTime - (endPauseTime - startPauseTime);
                    moveVecPlayer += new Vector3(0, (float)-deltaTime * G, 0);
                }
                //calculate gravity influence if airborne by jumping
                if (isJumping && !borderedBottom)
                {
                    currentTime = gameTime.TotalGameTime.TotalSeconds;

                    double deltaTime = currentTime - startJumpTime - (endPauseTime - startPauseTime);
                    moveVecPlayer += new Vector3(0, jumpHeight - (float)deltaTime * G, 0);
                }
                #endregion

                #region collision
                Vector3 temp = moveVecPlayer;

                if (Singleplayer.player.handleTerrainCollisionInDirection("left", moveVecPlayer, visibles))
                {
                    if (temp.X < 0) temp.X = 0;
                    borderedLeft = true;
                }
                if (Singleplayer.player.handleTerrainCollisionInDirection("right", moveVecPlayer, visibles))
                {
                    if (temp.X > 0) temp.X = 0;
                    borderedRight = true;
                }
                if (Singleplayer.player.handleTerrainCollisionInDirection("up", moveVecPlayer, visibles))
                {
                    if (temp.Y > 0) temp.Y = 0;
                    borderedTop = true;
                }
                if (Singleplayer.player.handleTerrainCollisionInDirection("down", moveVecPlayer, visibles))
                {
                    if (temp.Y < 0) temp.Y = 0;
                    isJumping = false;
                    borderedBottom = true;
                }

                if (moveVecPlayer.Y < 0) borderedTop = false;

                if (moveVecPlayer.Y < 0 && temp.Y == 0)
                {
                    isAirborne = false;
                    isFalling = false;
                }

                moveVecPlayer = temp;
                #endregion
                #endregion

                //update Playerposition
                Singleplayer.player.move(moveVecPlayer);

                //move camera and realign BoundingFrustum
                Camera2D.realign(moveVecPlayer, Singleplayer.player.Position);

                #region Updating attacks
                if (leftClick(currentMouseState, previousMouseState) || isClicked(Buttons.X))
                {
                    //Nahangriff
                    Singleplayer.player.MeleeAttack(gameTime);
                }
                else if (rightClick(currentMouseState, previousMouseState) || isClicked(Buttons.B))
                {
                    //Fernangriff
                    Singleplayer.player.RangedAttack(gameTime);
                }
                else if (middleClick(currentMouseState, previousMouseState) || isClicked(Buttons.Y))
                {
                    //Spezialangriff
                    Singleplayer.player.SpecialAttack(gameTime);
                }
                #endregion
                
                //check parameters
                borderedRight = Singleplayer.player.checkRightBorder(visibles);
                borderedLeft = Singleplayer.player.checkLeftBorder(visibles);

                if (!borderedBottom)
                {
                    Singleplayer.hud.isBurning = false;
                    Singleplayer.hud.isSwimming = false;
                }
                else
                {
                    startPauseTime = 0;
                    endPauseTime = 0;
                }
                #endregion

                //Utilities
                if ((isClicked(Keys.Escape) || isClicked(Buttons.Start)) && !Singleplayer.isPopup)
                {
                    startPauseTime = gameTime.TotalGameTime.TotalSeconds;

                    Singleplayer.isPaused = true;
                }            
            }
        }
        public static Game1.EGameState updateSingleplayerPaused(GameTime gameTime, List<Tile> visibles)
        {
            //get currently and previously pressed keys and mouse buttons
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            prevGamepad = currGamepad;
            currGamepad = GamePad.GetState(PlayerIndex.One);

            //Continue
            if ((isClicked(Keys.Escape) || isClicked(Buttons.Start)) && !Singleplayer.isPopup)
            {
                endPauseTime = gameTime.TotalGameTime.TotalSeconds;

                Singleplayer.isPaused = false;
            }

            //Main Menu
            else if (isClicked(Keys.Back) || isClicked(Buttons.Back))
                return Game1.currentState = Game1.EGameState.MainMenu;

            //Quit Game
            else if (isClicked(Keys.End) || isClicked(Buttons.RightStick))
                return Game1.currentState = Game1.EGameState.Nothing;

            return Game1.EGameState.Singleplayer;
        }
        public static void updateTutorial()
        {
            //check for index change
            if (!Singleplayer.isPopup)
            {
                switch (Singleplayer.tutIndex)
                {
                    case -1:
                        if (Math.Abs(Singleplayer.player.Position.X - 0) < 1)
                        {
                            Singleplayer.isPopup = true;
                            Singleplayer.tutIndex += 1;
                        }
                        break;

                    case 0:
                        if (Math.Abs(Singleplayer.player.Position.X - 100) < 1)
                        {
                            Singleplayer.isPopup = true;
                            Singleplayer.tutIndex += 1;
                        }
                        break;

                    case 1:
                        if (Math.Abs(Singleplayer.player.Position.X - 300) < 1)
                        {
                            Singleplayer.isPopup = true;
                            Singleplayer.tutIndex += 1;
                        }
                        break;

                    case 2:
                        if (Math.Abs(Singleplayer.player.Position.X - 420) < 1)
                        {
                            Singleplayer.isPopup = true;
                            Singleplayer.tutIndex += 1;
                        }
                        break;

                    case 3:
                        if (Math.Abs(Singleplayer.player.Position.X - 530) < 1 && !Singleplayer.tutStop)
                        {
                            Singleplayer.isPopup = true;
                            Singleplayer.tutIndex += 1;
                            Singleplayer.tutStop = true;
                        }
                        break;
                }
            }
            //controls while popup is open
            else if (isClicked(Keys.Enter) || isClicked(Buttons.Start))
            {
                Singleplayer.isPopup = false;
            }
        }

        public static void updateMultiplayer(GameTime gameTime)
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            prevGamepad = currGamepad;
            currGamepad = GamePad.GetState(PlayerIndex.One);

            Vector3 moveVecPlayer1 = new Vector3(0, 0, 0);

            if (currentKeyboard.IsKeyDown(Keys.W) || currGamepad.IsButtonDown(Buttons.LeftThumbstickUp) || currGamepad.IsButtonDown(Buttons.DPadUp)) moveVecPlayer1 += new Vector3(0, 0, -accel);
            if (currentKeyboard.IsKeyDown(Keys.A) || currGamepad.IsButtonDown(Buttons.LeftThumbstickLeft) || currGamepad.IsButtonDown(Buttons.DPadLeft)) moveVecPlayer1 += new Vector3(-accel, 0, 0);
            if (currentKeyboard.IsKeyDown(Keys.S) || currGamepad.IsButtonDown(Buttons.LeftThumbstickDown) || currGamepad.IsButtonDown(Buttons.DPadDown)) moveVecPlayer1 += new Vector3(0, 0, accel);
            if (currentKeyboard.IsKeyDown(Keys.D) || currGamepad.IsButtonDown(Buttons.LeftThumbstickRight) || currGamepad.IsButtonDown(Buttons.DPadRight)) moveVecPlayer1 += new Vector3(accel, 0, 0);

            Multiplayer.player1.move(moveVecPlayer1);
        }

        public static void updateArena(GameTime gameTime)
        {
            if (ringOut)
            {
                currentTime = gameTime.TotalGameTime.TotalSeconds;

                double deltaTime = currentTime - startFallTime;

                fallVecPlayer *= 0.98f;

                Arena.player.move(new Vector3(fallVecPlayer.X, (float) -deltaTime * G/4, fallVecPlayer.Z));

                if (Arena.player.Position.Y < -100)
                {
                    ringOut = false;

                    Arena.opheliaHud.healthLeft = 0;

                    Arena.player.moveTo(Arena.startPos);                    
                }
            }
            else
            {
                previousKeyboard = currentKeyboard;
                currentKeyboard = Keyboard.GetState();

                previousMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();

                prevGamepad = currGamepad;
                currGamepad = GamePad.GetState(PlayerIndex.One);

                #region Updating attacks
                if (leftClick(currentMouseState, previousMouseState) || isClicked(Buttons.X))
                {
                    //Nahangriff
                    Arena.player.MeleeAttack(gameTime);
                }
                else if (rightClick(currentMouseState, previousMouseState) || isClicked(Buttons.B))
                {
                    //Fernangriff
                    Arena.player.RangedAttack(gameTime);
                }
                else if (middleClick(currentMouseState, previousMouseState) || isClicked(Buttons.Y))
                {
                    //Spezialangriff
                    Arena.player.SpecialAttack(gameTime);
                }
                #endregion

                //Left ThumbStick control
                Vector3 moveVecPad = new Vector3(currGamepad.ThumbSticks.Left.X, 0, -currGamepad.ThumbSticks.Left.Y);

                //WASD controls
                Vector3 moveVecBoard = new Vector3(0, 0, 0);
                if (currentKeyboard.IsKeyDown(Keys.W)) moveVecBoard += new Vector3(0, 0, -accel);
                if (currentKeyboard.IsKeyDown(Keys.A)) moveVecBoard += new Vector3(-accel, 0, 0);
                if (currentKeyboard.IsKeyDown(Keys.S)) moveVecBoard += new Vector3(0, 0, accel);
                if (currentKeyboard.IsKeyDown(Keys.D)) moveVecBoard += new Vector3(accel, 0, 0);

                if (moveVecPad != new Vector3(0, 0, 0))
                {
                    Vector3 recentPlayerPosition = Arena.player.Position;

                    Arena.player.move(moveVecPad);

                    Arena.player.viewingDirection = Arena.player.Position - recentPlayerPosition;
                    Arena.blickWinkel = (float)Math.Atan2(-Arena.player.viewingDirection.Z, Arena.player.viewingDirection.X);
                }
                else if (moveVecBoard != Vector3.Zero)
                {
                    Vector3 recentPlayerPosition = Arena.player.Position;

                    if (moveVecBoard.Length() > 1) moveVecBoard.Normalize();
                    Arena.player.move(moveVecBoard);

                    Arena.player.viewingDirection = Arena.player.Position - recentPlayerPosition;
                    Arena.blickWinkel = (float)Math.Atan2(-Arena.player.viewingDirection.Z, Arena.player.viewingDirection.X);
                }

                //check for ring out
                if (!Arena.player.Boundary.Intersects(Arena.arenaBound))
                {
                    fallVecPlayer = moveVecPad;

                    startFallTime = gameTime.TotalGameTime.TotalSeconds;

                    ringOut = true;
                }
            }            
        }

        public static Game1.EGameState updateMainMenu()
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            prevGamepad = currGamepad;
            currGamepad = GamePad.GetState(PlayerIndex.One);

            
            //Up- and Down movement
            if (menuPosIndex != 0 && menuPosIndex != 4)
            {
                if ((currentKeyboard.IsKeyDown(Keys.Down) || currGamepad.IsButtonDown(Buttons.DPadDown)) && !isMenuMoving)
                {
                    menuPosIndex = (int)MathHelper.Clamp((float)(menuPosIndex + 1), 1f, 3f);

                    isMenuMoving = true;

                    moveVecSword = menuPositions[menuPosIndex] - MainMenu.sword.position;
                }
                if ((currentKeyboard.IsKeyDown(Keys.Up) || currGamepad.IsButtonDown(Buttons.DPadUp)) && !isMenuMoving)
                {
                    menuPosIndex = (int)MathHelper.Clamp((float)(menuPosIndex - 1), 1f, 3f);

                    isMenuMoving = true;

                    moveVecSword = menuPositions[menuPosIndex] - MainMenu.sword.position;
                }
            }

            if (isMenuMoving)
                if ((MainMenu.sword.Position - menuPositions[menuPosIndex]).Length() >= 0.01f)
                    MainMenu.sword.move(moveVecSword * 0.025f);
                else
                    isMenuMoving = false;

            //special cases (Start, Option, Optiondetails)
            if (!isMenuMoving)
            {
                switch (menuPosIndex)
                {
                    //Press Start
                    case 0:
                        if ((isClicked(Keys.Enter) || isClicked(Buttons.Start)) && !isMenuMoving)
                        {
                            menuPosIndex += 1;

                            isMenuMoving = true;

                            moveVecSword = (menuPositions[menuPosIndex] - MainMenu.sword.position) * 0.5f;
                        }
                        break;
                    
                    //Singleplayer
                    case 1:
                        //toggle tutorial
                        if ((isClicked(Keys.Left) || isClicked(Keys.Right) || 
                                isClicked(Buttons.DPadLeft) || isClicked(Buttons.DPadRight)) && !isMenuMoving)
                            Singleplayer.isTutorial = !Singleplayer.isTutorial;

                        if ((isClicked(Keys.Enter) || isClicked(Buttons.A)) && !isMenuMoving)
                            return Game1.EGameState.Singleplayer;
                        break;

                    //Multiplayer
                    case 2:
                        /*if ((isClicked(Keys.Enter) || isClicked(Buttons.A)) && !isMoving)
                            return Game1.EGameState.Arena;*/
                        break;

                    //Options
                    case 3:
                        if ((isClicked(Keys.Enter) || isClicked(Buttons.A)) && !isMenuMoving)
                        {
                            menuPosIndex = (int)MathHelper.Clamp((float)(menuPosIndex + 1), 0f, 4f);

                            isMenuMoving = true;

                            moveVecSword = menuPositions[menuPosIndex] - MainMenu.sword.position;
                        }
                        break;

                    //Options Details
                    case 4:
                        //move routine
                        if ((isClicked(Keys.Escape) || isClicked(Buttons.B)) && !isMenuMoving)
                        {
                            menuPosIndex = (int)MathHelper.Clamp((float)(menuPosIndex - 1), 0f, 3f);

                            isMenuMoving = true;

                            moveVecSword = menuPositions[menuPosIndex] - MainMenu.sword.position;
                        }

                        //change resolution
                        if (isClicked(Keys.Left) || isClicked(Buttons.DPadLeft))
                        {
                            Game1.resolutionIndex = Math.Abs((Game1.resolutionIndex - 1)) % Game1.resolutions.Length;

                            CameraMenu.updateResolution();
                        }

                        //toggle fullscreen
                        if ((isClicked(Keys.Right) || isClicked(Buttons.DPadRight)) && fullscreenPossible)
                        {
                            Game1.graphics.IsFullScreen = !Game1.graphics.IsFullScreen;

                            CameraMenu.updateResolution();
                        }
                        break;
                }
            }
            
            return Game1.EGameState.MainMenu;
        }

        public static Game1.EGameState updateGameOver()
        {
            prevGamepad = currGamepad;
            currGamepad = GamePad.GetState(PlayerIndex.One);

            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            //Rage Quit
            if (isClicked(Buttons.RightStick) || isClicked(Keys.End))
                return Game1.EGameState.Nothing;

            //back to MainMenu
            if (isClicked(Buttons.Back) || isClicked(Keys.Back))
                return Game1.EGameState.MainMenu;

            return Game1.EGameState.GameOver;
        }
        #endregion
    }
}