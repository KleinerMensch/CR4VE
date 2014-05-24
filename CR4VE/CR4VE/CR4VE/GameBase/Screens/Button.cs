using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CR4VE.GameBase.Screens
{
    class Button
    {
        Texture2D texture;
        Rectangle buttonRectangle;
        Rectangle mouseRectangle;
        Vector2 position;
        Vector2 size;
        Color color;

        public bool isClicked;
        bool alphaZero;
        
        public Button(Texture2D texture, GraphicsDevice device)
        {
            this.texture = texture;
            // screenwidth & screenheight in Abhaengigkeit von imagewidth & imageheight
            // 800 x 600
            size = new Vector2(device.Viewport.Width / 8, device.Viewport.Height / 30);
            color = new Color(255, 255, 255, 255); // 0% durchsichtiges Weiß
        }

        public void Update(MouseState mouse)
        {
            buttonRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRectangle.Intersects(buttonRectangle))
            {
                // A: Alphakomponente -> Fadingeffekte
                if (color.A == 0) alphaZero = true;
                if (color.A == 255) alphaZero = false;
                if (alphaZero) color.A += 2;
                else color.A -= 2;
                if (mouse.LeftButton == ButtonState.Pressed) isClicked = true;
            }
            else if (color.A < 255)
            {
                color.A += 2;
                isClicked = false;
            }
        }

        public void setPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, buttonRectangle, color);
        }
    }
}
