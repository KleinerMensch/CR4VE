using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR4VE.GameBase.Camera;

namespace CR4VE.GameLogic.GameStates
{
    class StartScreen : GameStateInterface
    {
        #region Attribute
        GraphicsDeviceManager gDevice;
        SpriteBatch spriteBatch;
        Texture2D background;

        Color cr4veColor = Color.White;
        int countForBlink = 0;

        // zum Test
        bool firstupdate;
        double starttime;
        #endregion

        #region Konstruktor
        public StartScreen() { }
        #endregion

        #region Init
        public void Initialize(ContentManager content)
        {
            firstupdate = true;
            spriteBatch = CR4VE.Game1.spriteBatch;
            background = content.Load<Texture2D>("Assets/Sprites/CR4VE_Ambigramm");
            gDevice = CR4VE.Game1.graphics;
        }
        #endregion

        #region Update
        public Game1.EGameState Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (firstupdate)
            {
                // einmalige Initialisierung zum Test
                starttime = gameTime.TotalGameTime.TotalMilliseconds;
                firstupdate = false;
            }

            //Fadingeffekte
            //Alphawert 255 => komplett transparent
            //Alphawert 0 => nicht transparent
            if (cr4veColor.A < 180)
            {
                cr4veColor.A += 1;
            }
            else if (cr4veColor.A >= 180)
            {
                cr4veColor = new Color(255, 255, 255, 0);
            }
            if (cr4veColor.A == 0)
                countForBlink++;

            if(countForBlink == 3)
                return Game1.EGameState.MainMenu;

            //// nach 3 Sek Wechsel in das Hauptmenue
            //if ((gameTime.TotalGameTime.TotalMilliseconds - starttime) > 3000)
            //{
            //    return Game1.EGameState.MainMenu;
            //}
            return Game1.EGameState.StartScreen;
        }
        #endregion

        #region Draw
        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin();
            gDevice.GraphicsDevice.Clear(Color.White);
            spriteBatch.Draw(background, gDevice.GraphicsDevice.Viewport.Bounds,cr4veColor);
            spriteBatch.End();
        }
        #endregion

        public void Unload()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

