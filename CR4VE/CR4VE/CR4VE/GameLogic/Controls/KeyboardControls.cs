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

        private static float speed = 1;

        //Sprungparameter
        private static float limit = 25;
        private static float limitCheck = 0;
        private static bool isJumping = false;
        private static bool limitReached = false;

        //Grenzen fuer Kamerabewegung
        private static int topLimit = 20;
        private static int botLimit = -20;
        private static int leftLimit = -20;
        private static int rightLimit = 20;
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

        public static void updateSingleplayer()
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
             
            Vector2 moveVecCam = new Vector2(0, 0);
            Vector2 moveVecPlayer = new Vector2(0, 0);

            //moveVecCam berechnen, falls Spieler nahe am Bildschirmrand
            if (Camera2D.transform3D(Singleplayer.player.Position).X > rightLimit) moveVecCam += new Vector2(speed, 0);
            if (Camera2D.transform3D(Singleplayer.player.Position).X < leftLimit) moveVecCam += new Vector2(-speed, 0);
            if (Camera2D.transform3D(Singleplayer.player.Position).Y > topLimit) moveVecCam += new Vector2(0, -speed);
            if (Camera2D.transform3D(Singleplayer.player.Position).Y < botLimit) moveVecCam += new Vector2(0, speed);

            //moveVecPlayer berechnen
            if (currentKeyboard.IsKeyDown(Keys.A)) moveVecPlayer += new Vector2(-speed, 0);
            if (currentKeyboard.IsKeyDown(Keys.D)) moveVecPlayer += new Vector2(speed, 0);

            #region jump
            if (isClicked(Keys.Space) && !isJumping) isJumping = true;

            if (isJumping && !limitReached)
            {
                moveVecPlayer += new Vector2(0, speed);
                limitCheck += speed;
                if (limitCheck == limit)
                    limitReached = true;
            }
            else if (limitReached)
            {
                moveVecPlayer -= new Vector2(0, speed);
                limitCheck -= speed;
                if (limitCheck == 0)
                {
                    limitReached = false;
                    isJumping = false;
                }
            }            
            #endregion

            Camera2D.move(moveVecCam);
            Singleplayer.player.move(moveVecPlayer);
        }
        
        public static void updateMultiplayer()
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            Vector3 moveVec3D = new Vector3(0, 0, 0);

            if (currentKeyboard.IsKeyDown(Keys.W)) moveVec3D += new Vector3(0, 0, -speed);
            if (currentKeyboard.IsKeyDown(Keys.A)) moveVec3D += new Vector3(-speed, 0, 0);
            if (currentKeyboard.IsKeyDown(Keys.S)) moveVec3D += new Vector3(0, 0, speed);
            if (currentKeyboard.IsKeyDown(Keys.D)) moveVec3D += new Vector3(speed, 0, 0);

            Multiplayer.player.move(moveVec3D);
        }
        #endregion
    }
}