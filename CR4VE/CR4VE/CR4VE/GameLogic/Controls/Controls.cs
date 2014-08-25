﻿using System;
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
using CR4VE.GameLogic.Controls;

namespace CR4VE.GameLogic.Controls
{
    public static class Controls
    {
        #region Attributes
        //Keyboard-, Maus- und Gamepadstates
        static KeyboardState currentKeyboard;
        static KeyboardState previousKeyboard;

        static MouseState currentMouseState;
        static MouseState previousMouseState;

        static GamePadState currentGamepad;
        static GamePadState previousGamepad;

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

        private static double startFallTime;
        private static double startJumpTime;
        private static double currentTime;
        
        public static bool isJumping = false;
        public static bool isAirborne = true;
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

            previousGamepad = currentGamepad;
            currentGamepad = GamePad.GetState(PlayerIndex.One);


            #region Calculate moveVecPlayer
            Vector3 moveVecPlayer = new Vector3(0, 0, 0);

            #region left and right movement
            if ((currentKeyboard.IsKeyDown(Keys.A) || currentGamepad.IsButtonDown(Buttons.DPadLeft)) && !borderedLeft)
            {
                if (velocityLeft < maxVelocity) velocityLeft += velocityGain;
                velocityRight = 0;
                moveVecPlayer += new Vector3(-accel, 0, 0);

                borderedRight = false;

                //Blickrichtung = links
                Singleplayer.player.viewingDirection.X = -1;
            }
            else
                velocityLeft = MathHelper.Clamp(velocityLeft -= velocityGain, 0, maxVelocity);

            if (currentKeyboard.IsKeyDown(Keys.D) && !borderedRight)
            {
                if (velocityRight < maxVelocity) velocityRight += velocityGain;
                velocityLeft = 0;
                moveVecPlayer += new Vector3(accel, 0, 0);

                borderedLeft = false;

                //Blickrichtung = rechts
                Singleplayer.player.viewingDirection.X = 1;
            }
            else
                velocityRight = MathHelper.Clamp(velocityRight -= velocityGain, 0, maxVelocity);
            #endregion

            #region airborne influence
            //being airborne by jumping
            if (isClicked(Keys.Space) && !isJumping && !isFalling)
            {
                isJumping = true;
                borderedBottom = false;

                //minimum jump
                if (velocityLeft < 0.5f && velocityRight < 0.5f)
                    moveVecPlayer += new Vector3(0, 2.5f, 0);

                startJumpTime = gameTime.TotalGameTime.TotalSeconds;
            }

            //being airborne by falling
            if (!isFalling && !isJumping && !Singleplayer.player.checkFooting())
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
                moveVecPlayer += new Vector3(0, (float) -deltaTime * G, 0);
            }
            //calculate gravity influence if airborne by jumping
            if (isJumping && !borderedBottom)
            {
                currentTime = gameTime.TotalGameTime.TotalSeconds;

                double deltaTime = currentTime - startJumpTime;
                moveVecPlayer += new Vector3(0, Math.Max(velocityRight, velocityLeft) - (float)deltaTime * G, 0);
            }
            #endregion
            
            #region collision
            Vector3 temp = moveVecPlayer;
            
            if (Singleplayer.player.handleTerrainCollisionInDirection("left", moveVecPlayer))
            {
                if (temp.X < 0) temp.X = 0;
                borderedLeft = true;
            }
            if (Singleplayer.player.handleTerrainCollisionInDirection("right", moveVecPlayer))
            {
                if (temp.X > 0) temp.X = 0;
                borderedRight = true;
            }
            if (Singleplayer.player.handleTerrainCollisionInDirection("up", moveVecPlayer))
            {
                if (temp.Y > 0) temp.Y = 0;
                borderedTop = true;
            }
            if (Singleplayer.player.handleTerrainCollisionInDirection("down", moveVecPlayer))
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

            //updating Playerposition
            Singleplayer.player.move(moveVecPlayer);

            //move camera and realign BoundingFrustum
            Camera2D.realign(moveVecPlayer, Singleplayer.player.Position);
        

            #region Updating attacks
            if (leftClick(currentMouseState, previousMouseState))
            {
                //Nahangriff
                Singleplayer.player.MeleeAttack(gameTime);
            }
            else if (rightClick(currentMouseState, previousMouseState))
            {
                //Fernangriff
                Singleplayer.player.RangedAttack(gameTime);
            }
            else if (middleClick(currentMouseState, previousMouseState))
            {
                //Spezialangriff
                Singleplayer.player.SpecialAttack(gameTime);
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

            Multiplayer.player1.move(moveVecPlayer);

            //Arena Model Debug
            if (currentKeyboard.IsKeyDown(Keys.Up)) Multiplayer.lava.position.Z += 0.5f;
            if (currentKeyboard.IsKeyDown(Keys.Down)) Multiplayer.lava.position.Z -= 0.5f;
            if (currentKeyboard.IsKeyDown(Keys.Left)) Multiplayer.lava.position.X += 0.5f;
            if (currentKeyboard.IsKeyDown(Keys.Right)) Multiplayer.lava.position.X -= 0.5f;

            Console.WriteLine(Multiplayer.lava.Position);
        }

        public static void updateArena(GameTime gameTime)
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            #region Updating attacks
            if (leftClick(currentMouseState, previousMouseState))
            {
                //Nahangriff
                Arena.player.MeleeAttack(gameTime);
            }
            else if (rightClick(currentMouseState, previousMouseState))
            {
                //Fernangriff
                Arena.player.RangedAttack(gameTime);
            }
            else if (middleClick(currentMouseState, previousMouseState))
            {
                //Spezialangriff
                Arena.player.SpecialAttack(gameTime);
            }
            #endregion

            Vector3 moveVecPlayer = new Vector3(0, 0, 0);

            if (currentKeyboard.IsKeyDown(Keys.W)) moveVecPlayer += new Vector3(0, 0, -accel);
            if (currentKeyboard.IsKeyDown(Keys.A)) moveVecPlayer += new Vector3(-accel, 0, 0);
            if (currentKeyboard.IsKeyDown(Keys.S)) moveVecPlayer += new Vector3(0, 0, accel);
            if (currentKeyboard.IsKeyDown(Keys.D)) moveVecPlayer += new Vector3(accel, 0, 0);

            if (moveVecPlayer != new Vector3(0, 0, 0))
            {
                Vector3 recentPlayerPosition = Arena.player.position;
                Vector3 newPlayerPosition = Arena.player.position + moveVecPlayer;
                Arena.player.viewingDirection = newPlayerPosition - recentPlayerPosition;
                Arena.blickWinkel = (float)Math.Atan2(-Arena.player.viewingDirection.Z, Arena.player.viewingDirection.X);
            }
            
            Arena.player.move(moveVecPlayer);
        }
        #endregion
    }
}