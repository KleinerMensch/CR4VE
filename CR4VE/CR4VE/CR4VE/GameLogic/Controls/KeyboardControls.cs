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


namespace CR4VE.GameLogic.Controls
{
    public static class KeyboardControls
    {
        static KeyboardState currentKeyboard;
        static KeyboardState previousKeyboard;

        private static float speed = 5f;
        private static float limit = 25;
        private static float test = 0;
        private static bool isJumping = false;
        private static bool limitReached = false;


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

        public static void update()
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
             
            Vector2 moveVec = new Vector2(0, 0);

            if (currentKeyboard.IsKeyDown(Keys.W)) moveVec += new Vector2(0, -speed);
            if (currentKeyboard.IsKeyDown(Keys.A)) moveVec += new Vector2(-speed, 0);
            if (currentKeyboard.IsKeyDown(Keys.S)) moveVec += new Vector2(0, speed);
            if (currentKeyboard.IsKeyDown(Keys.D)) moveVec += new Vector2(speed, 0);

            if (isClicked(Keys.Space) && !isJumping)
            {
                Console.WriteLine("asdasdas");
                isJumping = true;                
            }

            if (isJumping && !limitReached)
            {
                moveVec += new Vector2(0, -speed);
                test += speed;
                if (test == limit)
                    limitReached = true;
            }
            else if (limitReached)
            {
                moveVec -= new Vector2(0, -speed);
                test -= speed;
                if (test == 0)
                {
                    limitReached = false;
                    isJumping = false;
                }
            }
            Camera2D.move(moveVec);

            //multiplayer
            //Vector3 moveVec3D = new Vector3(0, 0, 0);

            //if (state.IsKeyDown(Keys.W)) moveVec3D += new Vector3(0, 0, -speed);
            //if (state.IsKeyDown(Keys.A)) moveVec3D += new Vector3(-speed, 0, 0);
            //if (state.IsKeyDown(Keys.S)) moveVec3D += new Vector3(0, 0, speed);
            //if (state.IsKeyDown(Keys.D)) moveVec3D += new Vector3(speed, 0, 0);

            //CR4VE.Game1.playerPos += moveVec3D;
        }


    }
}