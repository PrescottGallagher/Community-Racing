using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Village_Racing_3
{
    class Player
    {
        #region Declarations

        Texture2D SpriteSheet;
        public const float speed = 0.8f;
        public const float gravity = 1f;
        public Vector2 Position;
        public Vector2 Velocity;
        public SpriteEffects Direction;
        KeyboardState now;
        KeyboardState keyState;

        #endregion

        #region Helper Functions

        private void handleInput()
        {
            now = Keyboard.GetState();

            if (now.IsKeyDown(Keys.Up))
                Velocity.Y -= 5;

            if (now.IsKeyDown(Keys.Left))
                Velocity.X -= speed;

            if (now.IsKeyDown(Keys.Right))
                Velocity.X += speed;


            keyState = Keyboard.GetState();
        }

        private void handleGravity()
        {
            Velocity.Y += gravity;
        }

        private void handleCollisions()
        {
            //Temporary Testing Collisions
            if (Position.Y > 500)
            {
                Position.Y = 500;
                Velocity.Y = 0;
            }

        }

        #endregion

        #region Update

        public void Update(GameTime gt)
        {
            handleInput();

            handleGravity();

            handleCollisions();
        }

        #endregion

        #region Draw



        #endregion
    }
}
