using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFile
{
    class Player
    {

        public int screenWidth;
        public int screenHeight;

        public Texture2D texture;
        protected Rectangle rectangle;

        public float xPosition;
        public float yPosition;

        protected float xResetPosition;
        protected float yResetPosition;

        public Player(int inScreenWidth, int inScreenHeight, Texture2D inSpriteTexture, int inDrawWidth, float inResetX, float inResetY)
        {

            screenWidth = inScreenWidth;
            screenHeight = inScreenHeight;
            texture = inSpriteTexture;
            xResetPosition = inResetX;
            yResetPosition = inResetY;

            float aspect = inSpriteTexture.Width / inSpriteTexture.Height;
            int height = (int)Math.Round(inDrawWidth * aspect);
            rectangle = new Rectangle(0, 0, inDrawWidth, height);

            Reset();
        }
        public void SetPosition(float x, float y)
        {
            xPosition = (int)Math.Round(x);
            yPosition = (int)Math.Round(y);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            rectangle.X = (int)Math.Round(xPosition);
            rectangle.Y = (int)Math.Round(yPosition);
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
        public Vector2 GetCentre()
        {
            float x = xPosition + rectangle.Width / 2;
            float y = yPosition + rectangle.Height / 2;
            return new Vector2(x, y);
        }

        public virtual void Update(float deltaTime)
        {

        }

        public void SetResetPosition(float x, float y)
        {
            xResetPosition = x;
            yResetPosition = y;
        }
        public virtual void Reset()
        {
            SetPosition(xResetPosition, yResetPosition);
        }
        public int RoundPositionX()
        {
            return ((int)xPosition);
        }
        public int RoundPositionY()
        {
            return ((int)yPosition);
        }
        
    }
}