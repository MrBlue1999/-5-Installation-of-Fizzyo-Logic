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
    class PlayerMover : Player
    {
        Stopwatch stopwatch = new Stopwatch();

        public void StartMovingUp()
        {
            PlayerMovingUp = true;
        }
        public void StartMovingDown()
        {
            PlayerMovingDown = true;
        }
        public void BadBreath(float startRightMovement)
        {
            BadBreathSlowRight = true;
            BreathLimiter = startRightMovement;
        }
        public void GoodBreath(float startRightMovement)
        {
            GoodBreathQuickRight = true;
            BreathLimiter = startRightMovement;
        }
        public void StopMovingUp()
        {
            PlayerMovingUp = false;
        }
        public void StopMovingDown()
        {
            PlayerMovingDown = false;
        }
        public bool PlayerMovingUp;
        public bool PlayerMovingDown;
        protected bool BadBreathSlowRight;
        protected bool GoodBreathQuickRight;
        public bool DiedViaLackOfGoodBreaths = false;
        public bool PositionUp;
        public bool PositionDown;
        public int ChangePlayerTexture;
        public float BreathLimiter;
        protected float resetXSpeed;
        protected float resetYSpeed;
        protected float xSpeed;
        protected float ySpeed;


        public PlayerMover(int inScreenWidth, int inScreenHeight, Texture2D inSpriteTexture, int inDrawWidth, float inResetX, float inResetY, float inResetXSpeed, float inResetYSpeed) :
            base(inScreenWidth, inScreenHeight, inSpriteTexture, inDrawWidth, inResetX, inResetY)
        {
            resetXSpeed = inResetXSpeed;
            resetYSpeed = inResetYSpeed;
            Reset();
        }

        public override void Reset()
        {
            PlayerMovingDown = false;
            PlayerMovingUp = false;
            BadBreathSlowRight = false;
            GoodBreathQuickRight = false;
            SetSpeed(resetXSpeed, resetYSpeed);
            base.Reset();
        }

        public int ChangeTexture()
        {
            return ChangePlayerTexture;
        }

        public void SetSpeed(float inXSpeed, float inYSpeed)
        {
            xSpeed = inXSpeed;
            ySpeed = inYSpeed;
        }

        public override void Update(float deltaTime)
        {
            /////////////////////////
            /*  XPOSITION MOVEMENT */
            /////////////////////////
            // The X position of the player will be controlled by the number of lives that they have should this reach zero the player will lose should the player use a good breath a life will be restored
            // The player will have a minimum of 3 lives with the possiblle addition of a shop system to provide minor boosts to the players abilities ie durability and starting speed.
            // Should the player purchase more lives they will be moved closer to the direction of incoming enemies to a limit of 5 increments opposed to the origional 3 increments. This will mean although they have more lives
            // Their reaction time will have to be faster

            if (GoodBreathQuickRight || BadBreathSlowRight)
            {
                if (GoodBreathQuickRight)
                {
                    if (xPosition < screenWidth / 4 && xPosition - BreathLimiter < 100)
                    {
                        xPosition = xPosition + 50 * deltaTime;
                    }
                    else
                    {
                        GoodBreathQuickRight = false;
                    }
                }
                else if (BadBreathSlowRight)
                {
                    if (xPosition < screenWidth / 4 && xPosition - BreathLimiter < 50)
                    {
                        xPosition = xPosition + 30 * deltaTime;
                    }
                    else
                    {
                        BadBreathSlowRight = false;
                    }
                }

            }
            else if (!GoodBreathQuickRight && !BadBreathSlowRight)
            {
                xPosition = xPosition - 10 * deltaTime;
            }


            /////////////////////////
            /*  YPOSITION MOVEMENT */
            /////////////////////////
            //The player movement exclusivly vertically controls height and is used to dodge incomming propjectiles it is limited as going too high and too low doesnt fit the aesthetic i am aiming for and I believe that purely from this percpective it should be limited.
            // I need to add the "leap movement i am aiming for this means the players movement will not be as precise increasing difficulty and will also make the player look more natural" 
            //These events that happen should the player move
            if (PlayerMovingUp == true)
            {
                PositionUp = true;
                yPosition -= (ySpeed * deltaTime);
                ChangePlayerTexture = 2;
            }
            else if (PlayerMovingDown == true)
            {
                PositionDown = true;
                yPosition += (ySpeed * deltaTime);
                ChangePlayerTexture = 1;
            }
            else
            {
                PositionDown = false;
                PositionUp = false;
                ChangePlayerTexture = 0;
                if (yPosition - screenHeight / 2 < 20 && yPosition - screenHeight / 2 > -20)
                {
                    yPosition = screenHeight / 2;
                }
                if (yPosition < (screenHeight / 2))
                {
                    yPosition += 500 * (3 * deltaTime);
                }
                else if (yPosition > (screenHeight / 2))
                {
                    yPosition -= 500 * (3 * deltaTime);
                }
            }


            //These are the screen height limits
            if (yPosition < screenHeight / 4)
            {
                yPosition = (screenHeight / 4);
            }
            if (yPosition > screenHeight - (screenHeight / 4))
            {
                yPosition = (screenHeight - (screenHeight / 4));
            }

            base.Update(deltaTime);
        }
    }
}   
