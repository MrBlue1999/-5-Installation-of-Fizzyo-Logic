using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace GameFile
{
    class Enemies
    {
        Stopwatch stopwatch = new Stopwatch();
        public Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;
        public bool isVisible = true;
        public int speedLimit;
        public Enemies(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
            stopwatch.Start();
            speedLimit = -10;
            velocity = new Vector2(speedLimiter(speedLimit), 0);
        }

        public void Update(GraphicsDevice graphics)
        {
            position += velocity;

            if (position.X < 0 - texture.Width)
            {
                isVisible = false;
            }

        }

        public int speedLimiter(int speedLimit)
        {
            long checkTime = stopwatch.ElapsedMilliseconds;
            if (checkTime > 10)
            {
                if (speedLimit > -20)
                {
                    speedLimit -= 1;
                    stopwatch.Reset();
                }
                stopwatch.Reset();
            }
           return (speedLimit);
        }
        public int GetSpeedLimit()
        {
            return speedLimit;
        }

        public int RoundPositionX()
        {
            return ((int)position.X);
        }
        public int RoundPositionY()
        {
            return ((int)position.Y);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,position,Color.White);
        }
    }
}
