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
        private static float speed = 0.5f;


        public static void update()
        {
            KeyboardState state = Keyboard.GetState();

            //moving 2D Camera (Singleplayer)
            //Vector2 moveVec2D = new Vector2(0, 0);

            //if (state.IsKeyDown(Keys.W)) moveVec2D += new Vector2(0, -speed);
            //if (state.IsKeyDown(Keys.A)) moveVec2D += new Vector2(-speed, 0);
            //if (state.IsKeyDown(Keys.S)) moveVec2D += new Vector2(0, speed);
            //if (state.IsKeyDown(Keys.D)) moveVec2D += new Vector2(speed, 0);

            //Camera2D.move(moveVec2D);

            //moving player in Arena (Multiplayer)
            Vector3 moveVec3D = new Vector3(0, 0, 0);

            if (state.IsKeyDown(Keys.W)) moveVec3D += new Vector3(0, 0, -speed);
            if (state.IsKeyDown(Keys.A)) moveVec3D += new Vector3(-speed, 0, 0);
            if (state.IsKeyDown(Keys.S)) moveVec3D += new Vector3(0, 0, speed);
            if (state.IsKeyDown(Keys.D)) moveVec3D += new Vector3(speed, 0, 0);

            CR4VE.Game1.playerPos += moveVec3D;
        }
    }
}