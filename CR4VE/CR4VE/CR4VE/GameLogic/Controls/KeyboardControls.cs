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


namespace CR4VE.GameLogic.Controls
{
    public static class KeyboardControls
    {
        #region Attributes
        static KeyboardState currentKeyboard;
        static KeyboardState previousKeyboard;

        private static float accel = 1;
        private static float maxVelocity = 2.5f;
        private static float velocityLeft = 0;
        private static float velocityRight = 0;

        //Sprungparameter
        private static readonly float G = 9.81f;
        private static double startJumpTime;
        private static double currentTime;
        private static float limitJumpHeight = 25;
        private static float limitCheck = 0;
        private static bool isJumping = false;
        private static bool grounded = false;

        //Grenzen fuer Kamerabewegung
        private static int topLimit = 20;
        private static int botLimit = -20;
        private static int leftLimit = -20;
        private static int rightLimit = 20;
        private static Vector3 deltaPosition;
        #endregion

        #region Methods
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

        public static void updateSingleplayer(GameTime gameTime)
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
            currentTime = gameTime.TotalGameTime.TotalSeconds;
             
            Vector2 moveVecCam = new Vector2(0, 0);
            Vector3 moveVecPlayer = new Vector3(0, 0, 0);

            //calculate moveVecPlayer
            if (currentKeyboard.IsKeyDown(Keys.A))
            {
                if (velocityLeft < maxVelocity) velocityLeft += 0.5f;
                moveVecPlayer += new Vector3(-accel, 0, 0);
            }
            if (currentKeyboard.IsKeyDown(Keys.D))
            {
                if (velocityRight < maxVelocity) velocityRight += 0.5f;
                moveVecPlayer += new Vector3(accel, 0, 0);
            }

            #region jump
            //initialize jump if space was pressed
            if (isClicked(Keys.Space) && !isJumping)
            {
                isJumping = true;
                startJumpTime = gameTime.TotalGameTime.TotalSeconds;
            }

            //calculate moveVecPlayer with influence of jump
            if (isJumping)
            {
                double deltaTime = currentTime - startJumpTime;
                moveVecPlayer += new Vector3(0, velocityRight - (float)deltaTime * 5, 0);
            }
            else
            { 
                
            }
            #endregion

            //debug
            Console.Clear();
            Console.WriteLine(moveVecPlayer);
            Console.WriteLine(moveVecCam);

            Vector3 temp = Singleplayer.player.Position;
            
            Singleplayer.player.move(moveVecPlayer);

            //calculate moveVecCam if player reaches screen limit
            //if (Camera2D.transform3D(Singleplayer.player.Position).X > rightLimit) moveVecCam += new Vector2(accel, 0);
            //if (Camera2D.transform3D(Singleplayer.player.Position).X < leftLimit) moveVecCam += new Vector2(-accel, 0);
            //if (Camera2D.transform3D(Singleplayer.player.Position).Y > topLimit) moveVecCam += new Vector2(0, -accel);
            //if (Camera2D.transform3D(Singleplayer.player.Position).Y < botLimit) moveVecCam += new Vector2(0, accel);
            if (Camera2D.transform3D(Singleplayer.player.Position).X >= rightLimit)
            {
                Camera2D.move(new Vector2(moveVecPlayer.X, 0));
            }
            if (Camera2D.transform3D(Singleplayer.player.Position).X < leftLimit) moveVecCam += new Vector2(-accel, 0);
            if (Camera2D.transform3D(Singleplayer.player.Position).Y > topLimit) moveVecCam += new Vector2(0, -accel);
            if (Camera2D.transform3D(Singleplayer.player.Position).Y < botLimit) moveVecCam += new Vector2(0, accel);
            
            Camera2D.move(moveVecCam);

            //reset jump parameters
            //(Positionsabfrage spaeter noch durch Kollision ersetzen)
            if (Singleplayer.player.Position.Y < 0 && isJumping)
            {
                Singleplayer.player.Position = new Vector3(Singleplayer.player.Position.X, 0, 0);
                isJumping = false;
            }
        }
        
        public static void updateMultiplayer()
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