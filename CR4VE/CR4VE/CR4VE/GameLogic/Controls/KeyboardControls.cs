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
    public static class KeyboardControls
    {
        #region Attributes
        //Keyboardparameter
        static KeyboardState currentKeyboard;
        static KeyboardState previousKeyboard;
        static MouseState currentMouseState;
        static MouseState previousMouseState;

        //Bewegungsparameter
        private static readonly float accel = 1;

        public static bool borderedLeft = false;
        public static bool borderedRight = false;
        public static bool borderedTop = false;
        public static bool borderedBottom = false;

        //Sprungparameter
        private static readonly float G = 9.81f;
        private static readonly float velocityGain = 0.2f;
        private static readonly float maxVelocity = 2.5f;
        private static readonly float maxFallSpeed = 3f;

        private static double startJumpTime;
        private static double currentTime;
        
        public static bool isJumping = false;
        public static bool isFalling = true;
        
        private static float velocityLeft = 0;
        private static float velocityRight = 0;
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
        #endregion

        //update methods
        public static void updateSingleplayer(GameTime gameTime)
        {
            //get currently and previously pressed buttons
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            Vector3 moveVecPlayer = new Vector3(0, 0, 0);

            //Entity.getTerrainCollisions(Singleplayer.player);

            #region calculate moveVecPlayer
            if (currentKeyboard.IsKeyDown(Keys.A) && !borderedLeft)
            {
                borderedRight = false;

                if (velocityLeft < maxVelocity) velocityLeft += velocityGain;
                velocityRight = 0;
                moveVecPlayer += new Vector3(-accel, 0, 0);

                //richtung = links
                Singleplayer.player.viewingDirection.X = -1;
            }
            else
                velocityLeft = MathHelper.Clamp(velocityLeft -= velocityGain, 0, maxVelocity);

            if (currentKeyboard.IsKeyDown(Keys.D) && !borderedRight)
            {
                borderedLeft = false;

                if (velocityRight < maxVelocity) velocityRight += velocityGain;
                velocityLeft = 0;
                moveVecPlayer += new Vector3(accel, 0, 0);

                //richtung = rechts
                Singleplayer.player.viewingDirection.X = 1;
            }
            else
                velocityRight = MathHelper.Clamp(velocityRight -= velocityGain, 0, maxVelocity);
            #endregion

            #region jump
            //initialize airborne influence
            if (isClicked(Keys.Space) && !isJumping)
            {
                isJumping = true;
                isFalling = true;

                //minimum jump
                if (velocityLeft < 0.5f && velocityRight < 0.5f && isClicked(Keys.Space))
                    moveVecPlayer += new Vector3(0, 2, 0);

                startJumpTime = gameTime.TotalGameTime.TotalSeconds;

                borderedBottom = false;
            }

            //calculate moveVecPlayer influenced by jumping
            if (isJumping)
            {
                currentTime = gameTime.TotalGameTime.TotalSeconds;

                double deltaTime = currentTime - startJumpTime;
                moveVecPlayer += new Vector3(0, Math.Max(velocityRight, velocityLeft) - (float)deltaTime * G, 0);
            }
            #endregion

            #endregion

            //updating Playerposition
            Singleplayer.player.move(moveVecPlayer);

            //move camera and realign BoundingFrustum
            Camera2D.realign(moveVecPlayer, Singleplayer.player.Position);
        

            #region Updating attacks
            if (leftClick(currentMouseState, previousMouseState))
            {
                //Nahangriff
                Character playerCastedToCharacter = (Character)Singleplayer.player;
                playerCastedToCharacter.MeleeAttack(gameTime);
            }
            else if (rightClick(currentMouseState, previousMouseState))
            {
                //Fernangriff
                Character playerCastedToCharacter = (Character)Singleplayer.player;
                playerCastedToCharacter.RangedAttack(gameTime);
            }
            else if (middleClick(currentMouseState, previousMouseState))
            {
                //Spezialangriff
                Character playerCastedToCharacter = (Character)Singleplayer.player;
                playerCastedToCharacter.SpecialAttack(gameTime);
            }
            #endregion
}

        public static void updateMultiplayer(GameTime gameTime)
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            Vector3 moveVecPlayer = new Vector3(0, 0, 0);

            if (currentKeyboard.IsKeyDown(Keys.W)) moveVecPlayer += new Vector3(0, 0, -accel);
            if (currentKeyboard.IsKeyDown(Keys.A)) moveVecPlayer += new Vector3(-accel, 0, 0);
            if (currentKeyboard.IsKeyDown(Keys.S)) moveVecPlayer += new Vector3(0, 0, accel);
            if (currentKeyboard.IsKeyDown(Keys.D)) moveVecPlayer += new Vector3(accel, 0, 0);

            //noch nicht richtig
            Vector3 recentPlayerPosition = Arena.player.position;
            Vector3 newPlayerPosition = Arena.player.position + moveVecPlayer;
            Arena.player.viewingDirection = recentPlayerPosition - newPlayerPosition;
            Arena.player.move(moveVecPlayer);
        }
        
    }
}