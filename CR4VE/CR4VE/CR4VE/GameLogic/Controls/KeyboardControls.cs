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
        private static float speed = 5f;


        public static void update()
        {
            KeyboardState state = Keyboard.GetState();
            Vector2 moveVec = new Vector2(0, 0);

            if (state.IsKeyDown(Keys.W)) moveVec += new Vector2(0, -speed);
            if (state.IsKeyDown(Keys.A)) moveVec += new Vector2(-speed, 0);
            if (state.IsKeyDown(Keys.S)) moveVec += new Vector2(0, speed);
            if (state.IsKeyDown(Keys.D)) moveVec += new Vector2(speed, 0);

            Camera2D.move(moveVec);
        }
    }
}