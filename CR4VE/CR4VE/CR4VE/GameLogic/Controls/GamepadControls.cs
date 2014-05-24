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
    public static class GamepadControls
    {
        static GamePadState currGamepad;
        static GamePadState prevGamepad;

        private static float speed = 5f;
        private static float limit = 25;
        private static float test = 0;
        private static bool isJumping = false;
        private static bool limitReached = false;

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

        public static void update()
        {
            prevGamepad = currGamepad;
            currGamepad = GamePad.GetState(PlayerIndex.One); 

            Vector2 moveVec = new Vector2(0, 0);

            if (currGamepad.IsButtonDown(Buttons.DPadUp)) moveVec += new Vector2(0, -speed);
            if (currGamepad.IsButtonDown(Buttons.DPadLeft)) moveVec += new Vector2(-speed, 0);
            if (currGamepad.IsButtonDown(Buttons.DPadDown)) moveVec += new Vector2(0, speed);
            if (currGamepad.IsButtonDown(Buttons.DPadRight)) moveVec += new Vector2(speed, 0);

            if (isClicked(Buttons.A) && !isJumping)
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

            //if (state.IsButtonDown(Buttons.DPadUp)) moveVec3D += new Vector3(0, 0, -speed);
            //if (state.IsButtonDown(Buttons.DPadLeft)) moveVec3D += new Vector3(-speed, 0, 0);
            //if (state.IsButtonDown(Buttons.DPadDown)) moveVec3D += new Vector3(0, 0, speed);
            //if (state.IsButtonDown(Buttons.DPadRight)) moveVec3D += new Vector3(speed, 0, 0);

            //CR4VE.Game1.playerPos += moveVec3D;
        }
    }
}
