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

        public static bool borderedLeft = false;
        public static bool borderedRight = false;
        public static bool borderedTop = false;
        public static bool borderedBottom = false;
        //(ghost)
        private static readonly float ghostDelay = 0.01f;
        private static readonly Vector3 checkPointFall = new Vector3(0, 10f, 0);
        private static Vector3 moveVecGhost = Vector3.Zero;
        public static bool isGhost = false;
        
        //Sprungparameter
        private static readonly float G = 9.81f;
        private static readonly float jumpHeight = 2.5f;

        private static double startFallTime;
        private static double startJumpTime;
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
            }

            //else if (state == Game1.EGameState.Arena)
            //{
            //    previousHealth = currentHealth;
            //    currentHealth = Arena.hud.healthLeft;
            //}

            return previousHealth > currentHealth;
        }

        //update methods

        public static void updateVibration(GameTime gameTime)
        {
            if (isReduced(Game1.currentState)) {
                GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
                vibrTimer = 1.0f;            
            }

            vibrTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (vibrTimer <= 0.0f) {
                GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
            }
          
        }
        
        public static void updateSingleplayer(GameTime gameTime)
        {
            if (isGhost)
            {
                //set ghost = player
                if (Singleplayer.ghost.Position == Vector3.Zero)
                {
                    Singleplayer.ghost.Position = Singleplayer.player.Position;

                    Singleplayer.player.moveTo(Singleplayer.lastCheckpoint.Position + checkPointFall);
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
            }
            else
            {
                #region not dead
                //get currently and previously pressed keys and mouse buttons
                previousKeyboard = currentKeyboard;
                currentKeyboard = Keyboard.GetState();

                previousMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();

                prevGamepad = currGamepad;
                currGamepad = GamePad.GetState(PlayerIndex.One);

                //visible tiles (potential collisions)
                List<Tile> visibles = Tilemap.getVisibleTiles();

                #region Calculate moveVecPlayer
                Vector3 moveVecPlayer = new Vector3(0, 0, 0);

                #region left and right movement
                if ((currentKeyboard.IsKeyDown(Keys.A) || currGamepad.IsButtonDown(Buttons.LeftThumbstickLeft)) && !borderedLeft)
                {
                    moveVecPlayer += new Vector3(-accel, 0, 0);

                    borderedRight = false;

                    //Blickrichtung = links
                    Singleplayer.player.viewingDirection.X = -1;
                }

                if ((currentKeyboard.IsKeyDown(Keys.D) || currGamepad.IsButtonDown(Buttons.LeftThumbstickRight)) && !borderedRight)
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

                    double deltaTime = currentTime - startFallTime;
                    moveVecPlayer += new Vector3(0, (float)-deltaTime * G, 0);
                }
                //calculate gravity influence if airborne by jumping
                if (isJumping && !borderedBottom)
                {
                    currentTime = gameTime.TotalGameTime.TotalSeconds;

                    double deltaTime = currentTime - startJumpTime;
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

                borderedRight = Singleplayer.player.checkRightBorder(visibles);
                borderedLeft = Singleplayer.player.checkLeftBorder(visibles);
                #endregion
            }
        }

        public static void updateMultiplayer(GameTime gameTime)
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            prevGamepad = currGamepad;
            currGamepad = GamePad.GetState(PlayerIndex.One);

            Vector3 moveVecPlayer1 = new Vector3(0, 0, 0);

            if (currentKeyboard.IsKeyDown(Keys.W) || currGamepad.IsButtonDown(Buttons.LeftThumbstickUp)) moveVecPlayer1 += new Vector3(0, 0, -accel);
            if (currentKeyboard.IsKeyDown(Keys.A) || currGamepad.IsButtonDown(Buttons.LeftThumbstickLeft)) moveVecPlayer1 += new Vector3(-accel, 0, 0);
            if (currentKeyboard.IsKeyDown(Keys.S) || currGamepad.IsButtonDown(Buttons.LeftThumbstickDown)) moveVecPlayer1 += new Vector3(0, 0, accel);
            if (currentKeyboard.IsKeyDown(Keys.D) || currGamepad.IsButtonDown(Buttons.LeftThumbstickRight)) moveVecPlayer1 += new Vector3(accel, 0, 0);

            Multiplayer.player1.move(moveVecPlayer1);
        }

        public static void updateArena(GameTime gameTime)
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            prevGamepad = currGamepad;
            currGamepad = GamePad.GetState(PlayerIndex.One);

            if (ringOut)
            {
                currentTime = gameTime.TotalGameTime.TotalSeconds;

                double deltaTime = currentTime - startFallTime;

                fallVecPlayer *= 0.98f;

                Arena.player.move(new Vector3(fallVecPlayer.X, (float) -deltaTime * G/4, fallVecPlayer.Z));
            }
            else
            {
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

                Vector3 moveVecPlayer = new Vector3(0, 0, 0);

                if (currentKeyboard.IsKeyDown(Keys.W) || currGamepad.IsButtonDown(Buttons.LeftThumbstickUp)) moveVecPlayer += new Vector3(0, 0, -accel);
                if (currentKeyboard.IsKeyDown(Keys.A) || currGamepad.IsButtonDown(Buttons.LeftThumbstickLeft)) moveVecPlayer += new Vector3(-accel, 0, 0);
                if (currentKeyboard.IsKeyDown(Keys.S) || currGamepad.IsButtonDown(Buttons.LeftThumbstickDown)) moveVecPlayer += new Vector3(0, 0, accel);
                if (currentKeyboard.IsKeyDown(Keys.D) || currGamepad.IsButtonDown(Buttons.LeftThumbstickRight)) moveVecPlayer += new Vector3(accel, 0, 0);

                if (moveVecPlayer != new Vector3(0, 0, 0))
                {
                    Vector3 recentPlayerPosition = Arena.player.position;
                    Vector3 newPlayerPosition = Arena.player.position + moveVecPlayer;
                    Arena.player.viewingDirection = newPlayerPosition - recentPlayerPosition;
                    Arena.blickWinkel = (float)Math.Atan2(-Arena.player.viewingDirection.Z, Arena.player.viewingDirection.X);
                }

                Arena.player.move(moveVecPlayer);

                if (!Arena.player.Boundary.Intersects(Arena.arenaBound))
                {
                    fallVecPlayer = moveVecPlayer;

                    startFallTime = gameTime.TotalGameTime.TotalSeconds;

                    ringOut = true;
                }
            }            
        }

        public static void updateMainMenu()
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            if (currentKeyboard.IsKeyDown(Keys.Up)) MainMenu.sword.Position += new Vector3(0, 0.5f, 0);
            if (currentKeyboard.IsKeyDown(Keys.Down)) MainMenu.sword.Position += new Vector3(0, -0.5f, 0);
        }
        #endregion
    }
}