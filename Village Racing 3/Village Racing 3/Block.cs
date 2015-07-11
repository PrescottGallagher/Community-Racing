using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Village_Racing_3
{
    class Block
    {
        #region Declarations

        public const int BLOCKHEIGHT = 25;
        public const int BLOCKWIDTH = 25;
        public const int LARGEBLOCKSCALE = 64;

        Rectangle myRect;
        Vector2 Position;
        Texture2D spriteSheet;
        Vector2 spritePosition;

        #endregion

        #region Constructor

        public Block(Texture2D sprites, Vector2 SSP, Vector2 BlockPosition)
        {
            spriteSheet = sprites;
            Position = BlockPosition;
            spritePosition = SSP;
            myRect = new Rectangle((int)Position.X, (int)Position.Y, BLOCKWIDTH, BLOCKHEIGHT);
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(spriteSheet, new Rectangle((int)Position.X, (int)Position.Y, BLOCKWIDTH, BLOCKHEIGHT), new Rectangle((int)spritePosition.X, (int)spritePosition.Y, 65, 65), Color.White);
        }

        public void DrawLarge(SpriteBatch sb)
        {
            sb.Draw(spriteSheet, new Rectangle((int)Position.X, (int)Position.Y, LARGEBLOCKSCALE, LARGEBLOCKSCALE), new Rectangle((int)spritePosition.X, (int)spritePosition.Y, 65, 65), Color.White);
        }

        #endregion
    }
}
