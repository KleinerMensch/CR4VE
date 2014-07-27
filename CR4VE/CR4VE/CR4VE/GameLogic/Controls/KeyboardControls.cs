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
using CR4VE.GameLogic.GameStates;
using CR4VE.GameBase.Objects;


namespace CR4VE.GameLogic.Controls
{
    public static class KeyboardControls
    {
        #region Attributes
        //Keyboardparameter
        static KeyboardState currentKeyboard;
        static KeyboardState previousKeyboard;

        //Bewegungsparameter
        private static float accel = 1;

        //Sprungparameter
        private static readonly float G = 9.81f;
        private static double startJumpTime;
        private static double currentTime;
        private static bool isJumping = false;
        private static float maxVelocity = 2.5f;
        private static float velocityGain = 0.2f;
        private static float velocityLeft = 0;
        private static float velocityRight = 0;

        //Grenzen fuer Kamerabewegung
        //(spaeter noch durch prozentuale Angaben zu ersetzen)
        private static int topLimit = 20;
        private static int botLimit = -20;
        private static int leftLimit = -20;
        private static int rightLimit = 20;
        #endregion

        #region Methods
        //help methods
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

        //update methods
        public static void updateSingleplayer(GameTime gameTime)
        {

            //get currently and previously pressed buttons
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            //initialize move vectors
            Vector2 moveVecCam = new Vector2(0, 0);
            Vector3 moveVecPlayer = new Vector3(0, 0, 0);

            //calculate moveVecPlayer
            if (currentKeyboard.IsKeyDown(Keys.A))
            {
                if (velocityLeft < maxVelocity) velocityLeft += velocityGain;
                velocityRight = 0;
                moveVecPlayer += new Vector3(-accel, 0, 0);
            }
            else
                velocityLeft = MathHelper.Clamp(velocityLeft -= velocityGain, 0, maxVelocity);

            if (currentKeyboard.IsKeyDown(Keys.D))
            {
                if (velocityRight < maxVelocity) velocityRight += velocityGain;
                velocityLeft = 0;
                moveVecPlayer += new Vector3(accel, 0, 0);
            }
            else
                velocityRight = MathHelper.Clamp(velocityRight -= velocityGain, 0, maxVelocity);

            #region jump
            //initialize jump if space was pressed
            if (isClicked(Keys.Space) && !isJumping)
            {
                isJumping = true;
                startJumpTime = gameTime.TotalGameTime.TotalSeconds;
            }

            //calculate moveVecPlayer influenced by jumping
            if (isJumping)
            {
                currentTime = gameTime.TotalGameTime.TotalSeconds;
                double deltaTime = currentTime - startJumpTime;
                moveVecPlayer += new Vector3(0, Math.Max(velocityRight, velocityLeft) - (float)deltaTime * G, 0);
            }
            #endregion
            
            Singleplayer.player.move(moveVecPlayer);

            //calculate moveVecCam if player reaches screen limit
            //(using absolute values because of reversed Y movement for 2D objects)
            if (Camera2D.transform3D(Singleplayer.player.Position).X > rightLimit)
                moveVecCam += new Vector2(Math.Abs(moveVecPlayer.X), 0);

            if (Camera2D.transform3D(Singleplayer.player.Position).X < leftLimit)
                moveVecCam -= new Vector2(Math.Abs(moveVecPlayer.X), 0);

            if (Camera2D.transform3D(Singleplayer.player.Position).Y > topLimit)
                moveVecCam -= new Vector2(0, Math.Abs(moveVecPlayer.Y));

            if (Camera2D.transform3D(Singleplayer.player.Position).Y < botLimit)
                moveVecCam += new Vector2(0, Math.Abs(moveVecPlayer.Y));
            
            Camera2D.move(moveVecCam);

            //reset jump parameters
            //(Positionsabfrage spaeter noch durch Kollision ersetzen)
            // Positions abfrage Y= -35 fuer Meilenstein3
            if (Singleplayer.player.Position.Y < -45 && isJumping)
            {
                isJumping = false;
                Singleplayer.player.Position = new Vector3(Singleplayer.player.Position.X, -45 , 0);                
            }
            
        }
        
        public static void updateMultiplayer(GameTime gameTime)
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            Vector3 moveVec3D = new Vector3(0, 0, 0);

            if (currentKeyboard.IsKeyDown(Keys.W)) moveVec3D += new Vector3(0, 0, -accel);
            if (currentKeyboard.IsKeyDown(Keys.A)) moveVec3D += new Vector3(-accel, 0, 0);
            if (currentKeyboard.IsKeyDown(Keys.S)) moveVec3D += new Vector3(0, 0, accel);
            if (currentKeyboard.IsKeyDown(Keys.D)) moveVec3D += new Vector3(accel, 0, 0);

            Multiplayer.player.move(moveVec3D);
        }
        #endregion
    }
}